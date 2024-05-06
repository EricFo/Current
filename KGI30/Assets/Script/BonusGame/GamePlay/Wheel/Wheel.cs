using SlotGame.Core;
using System;
using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UniversalModule.AudioSystem;
using Random = UnityEngine.Random;

namespace Script.BonusGame.GamePlay.Wheel
{
    public class Wheel : MonoBehaviour
    {
        [SerializeField] private int cellCount = 12;

        //[SerializeField] private TMP_Text debugTargetText;

        //[FormerlySerializedAs("debugStatus")] [SerializeField]
        //private TMP_Text debugStatusText;


        //在不拍停的情况下滚轮所需旋转转的单元格 但时机旋转的单元格数会在此值周围浮动
        [SerializeField] private int defaultRollingStep = 36;


        [SerializeField] public bool infinity = false;

        //刹车时所需经过的单元格数
        [FormerlySerializedAs("defaultBreakingStep")] [SerializeField]
        private int defaultBreakingRound = 6;


        //到达目标点后开始反弹的 时间-速度 曲线,因为要弹回原位要求曲线图像对于（0.5，0.0）对称
        [SerializeField] private AnimationCurve afterBreakingCurve;

        //加速阶段 时间-速度 曲线
        [SerializeField] private AnimationCurve startCurve;

        [SerializeField] private AnimationCurve breakingSpeedUp;

        //减速阶段 路程-速度 曲线
        [SerializeField] private AnimationCurve breakingSpeedCurve;

        //转轮加速后所能取得的最大速度
        [SerializeField] public Vector3 maxSpeed;
        [SerializeField] private float speedUpTotalTime = 1.5f;
        [SerializeField] public Vector3 bonusIdlingSpeed;
        [SerializeField] public Vector3 idlingSpeed;
        [SerializeField] private bool reverse = false;


        //加速阶段所需时间
        public int startStatusTime = 2;

        //TODO 其实应该设计成点击shutdown后继续加速至最小速度然后减速
        //能够Shutdown的最小速度  
        public int minShutdownSpeed = 0;

        public bool IsUpgrad = false;


        public event Action<object, StatusChangeEventArgs> OnStatusChange;

        private int _targetCell;

        private float _rollingTime;
        private float _unitDeg;
        private float _afterBreakingStartTime;
        private Status _wheelStatus = Status.Stop; //旋转状态


        private Vector3 _startRollingRotate;

        private Vector3 _targetRotate;

        // Start is called before the first frame update
        private Vector3 _breakingSpeed;

        private Vector3 _rotateSpeed;
        private Vector3 _deltaRotate;
        private Vector3 _breakingLastRotate;


        private int _startCellIndex = 0;

        private bool isSpeedUp = false;

        private bool isPlaySpeedUpSound = false;


        public Status WheelStatus
        {
            get => _wheelStatus;
            private set
            {
                var lastStatus = _wheelStatus;
                _wheelStatus = value;
                //状态回调
                if (OnStatusChange != null)
                    OnStatusChange.Invoke(this, new StatusChangeEventArgs(lastStatus, _wheelStatus));

                //if (debugStatusText != null)
                //{
                //    debugStatusText.text = _wheelStatus.ToString();
                //}
            }
        }

        public int TargetCell
        {
            get => _targetCell;
            private set
            {
                //debugTargetText.text = value.ToString();
                _targetCell = CellIndexRevers(value);
            }
        }


        private void Start()
        {
            Init();
        }


        public void Hide()
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
            _wheelStatus = Status.Stop;
            gameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
            //  gameObject.SetActive(false);
        }

        /// <summary>
        ///初始化
        /// </summary>
        public void Init()
        {
            _unitDeg = 360f / cellCount; //单个单元格的度数

            _wheelStatus = Status.Stop;
        }

        private Vector3 _startTargetSpeed;

        /// <summary>
        /// 开始旋转
        /// </summary>
        /// <param name="target">旋转结果</param>
        /// <exception cref="Exception">该滚轮已经在旋转</exception>
        public void StartRolling(float targetSpeed, int target = 0)
        {
            if (_coroutine != null)
            {
                //塑料英文 ，但不怕乱码
                throw new Exception("Already has a rolling coroutine,please waiting for stop.");
            }

            TargetCell = target;
            _startTargetSpeed = new Vector3(0, 0, targetSpeed);

            _coroutine = StartCoroutine(_Internal_Rolling());
        }


        /// <summary>
        /// 可在shutdown被调用时修改结果
        /// </summary>
        /// <param name="newTargetCell">新的旋转结果</param>
        public void Shutdown(int newTargetCell)
        {
            CheckShutdown();
            TargetCell = newTargetCell;

            Shutdown();
        }

        public void Shutdown(bool lessRound = false)
        {
            isPlaySpeedUpSound = false;
            //AudioManager.Stop("FE_WheelHyper");
            if (WheelStatus == Status.AfterBreaking)
            {
                return;
            }

            //重新计算刹车点到目标点的距离
            GenTargetRotate();
            _breakingLastRotate.z = (_targetRotate.z - _deltaRotate.z) % 360;
            if (!lessRound)
            {
                _breakingLastRotate.z += defaultBreakingRound * 360f;
            }
            else
            {
                _breakingLastRotate.z += 360;
            }

            if (_wheelStatus == Status.Breaking)
            {
                _rotateSpeed = _breakingSpeed;
            }

            //去除掉多余的圈内数
            _targetRotate.z = _breakingLastRotate.z + _deltaRotate.z;
            WheelStatus = Status.Breaking;
            BreakingMove();
        }

        void GenTargetRotate()
        {
            int endStep = TotalRollingStep();
            _targetRotate = new Vector3(0, 0, (endStep) * _unitDeg);
        }


        //需要在revers模式下反转数值映射
        int CellIndexRevers(int index)
        {
            if (!reverse)
            {
                return index;
            }

            if (index == 0)
            {
                return index;
            }

            return Math.Abs(index - 12);
        }

        /// <summary>
        /// 旋转的核心函数
        /// </summary>
        /// <returns></returns>
        IEnumerator _Internal_Rolling()
        {
            if (WheelStatus != Status.Stop)
            {
                yield break;
            }

            WheelStatus = Status.Start;
            _rollingTime = 0f;
            _deltaRotate = new Vector3();
            _startRollingRotate = transform.localRotation.eulerAngles;
            _startCellIndex = CellIndexRevers(CurrentCellIndex());
            GenTargetRotate();


            int endStep = TotalRollingStep();
            _targetRotate = new Vector3(0, 0, (endStep) * _unitDeg);

            while (WheelStatus != Status.Stop)
            {
                _rollingTime += Time.deltaTime;

                switch (WheelStatus)
                {
                    case Status.Start:
                        StartMove();
                        isPlaySpeedUpSound = false;
                        break;
                    case Status.Rolling:
                        RollingMove();
                        break;
                    case Status.SpeedUp:
                        SpeedUpMove();
                        if (SpeedUpNextStatus == Status.Breaking && isPlaySpeedUpSound == false)
                        {
                            isSpeedUp = true;
                            isPlaySpeedUpSound = true;
                            if (GlobalObserver.CurrentBonusState == 3)
                            {
                                if (IsUpgrad == false)
                                {
                                    AudioManager.PlayOneShot("FE_WheelHyperSmall");
                                }
                                else
                                {
                                    AudioManager.PlayOneShot("FE_WheelHyper");
                                }
                            }
                            else
                            {
                                AudioManager.PlayOneShot("FE_WheelHyperSmall");
                            }
                        }

                        break;
                    case Status.Breaking:
                        BreakingMove();
                        break;
                    case Status.AfterBreaking:
                        isSpeedUp = false;
                        AfterBreakingMove();
                        break;
                }

                //  Debug.Log(CurrentIndex());
                yield return null;
            }

            _coroutine = null;
        }


        #region SpeedUp

        private float speedUpDeltaTime = 0;
        private Vector3 SpeedUpTargetSpeed;
        private Vector3 SpeedUpStartSpeed;
        private Status SpeedUpNextStatus;
        private int speedUpBreakingTarget = 0;

        private bool isAllowPlaySound = false;

        public void SpeedUp(float targetSpeed, Status nextStatus, int target = -1)
        {
            speedUpDeltaTime = 0;
            SpeedUpStartSpeed = _rotateSpeed;
            SpeedUpNextStatus = nextStatus;
            SpeedUpTargetSpeed = new Vector3(0, 0, targetSpeed);

            speedUpBreakingTarget = target;
            TargetCell = target;

            WheelStatus = Status.SpeedUp;
        }

        void SpeedUpMove()
        {
            speedUpDeltaTime += Time.deltaTime;
            var samp = speedUpDeltaTime / speedUpTotalTime;
            if (samp >= 1)
            {
                if (SpeedUpNextStatus == Status.Breaking)
                {
                    if (speedUpBreakingTarget >= 0)
                    {
                        Shutdown(speedUpBreakingTarget);
                        return;
                    }

                    Shutdown();


                    return;
                }

                WheelStatus = SpeedUpNextStatus;
            }

            var diff = SpeedUpTargetSpeed.z - SpeedUpStartSpeed.z;
            _rotateSpeed.z = breakingSpeedUp.Evaluate(samp) * diff + SpeedUpStartSpeed.z;
            _deltaRotate += _rotateSpeed * Time.deltaTime;
            _Move();
        }

        #endregion


        /// <summary>
        /// 计算达到目标所需的步数（经过的单元格数），
        /// </summary>
        /// <returns></returns>
        int TotalRollingStep()
        {
            return TargetCell - _startCellIndex + defaultRollingStep;
        }


        void CheckShutdown()
        {
            if (WheelStatus == Status.AfterBreaking && WheelStatus == Status.Breaking)
            {
                throw new Exception("Can not shutdown wheel that not start or rolling");
            }
        }


        /// <summary>
        /// 根据角度获取当前的转盘Id
        /// </summary>
        /// <returns></returns>
        public int CurrentCellIndex()
        {
            float baseRotate = transform.localRotation.eulerAngles.z + _unitDeg / 2f;

            baseRotate = NormalizeDeg(baseRotate);
            var ret = (int)((baseRotate + float.Epsilon) / 30) % 12;
            return ret;
        }

        /// <summary>
        /// 简述过程速度采样，通过与剩余总路程的比值计算速度
        /// </summary>
        void BreakingMove()
        {
            //此处Breaking last rotate 就是在开始减速时到转轮停止时剩余的度数,而此处是求从减速开始的路程与上述1的比值归一化后用作在曲线上采样。
            var samp = (_deltaRotate + new Vector3(0, 0, float.Epsilon) - (_targetRotate - _breakingLastRotate)).z /
                       _breakingLastRotate.z;


            _breakingSpeed = breakingSpeedCurve.Evaluate(samp) * _rotateSpeed;


            _deltaRotate += _breakingSpeed * Time.deltaTime;
            _Move();

            if (_deltaRotate.z > _targetRotate.z)
            {
                _breakingSpeed = breakingSpeedCurve.Evaluate(1) * _rotateSpeed;
                _afterBreakingStartTime = _rollingTime;
                WheelStatus = Status.AfterBreaking;
            }
        }

        /// <summary>
        /// 归一化角度至 0-360
        /// </summary>
        /// <param name="deg"></param>
        /// <returns></returns>
        float NormalizeDeg(float deg)
        {
            deg %= 360;
            return deg < 0 ? 360 + deg : deg;
        }

        private Vector3 _currentStatusRollingSpeed;

        /// <summary>
        /// 匀速旋转过程
        /// </summary>
        void RollingMove()
        {
            _deltaRotate += _rotateSpeed * Time.deltaTime;

            if (infinity)
            {
                _deltaRotate.z %= 360f;
            }

            _Move();


            if (_targetRotate.z - _deltaRotate.z <= defaultBreakingRound * _unitDeg)
            {
                //    transform.rotation = Quaternion.Euler(targetRotate - new Vector3(0, 0, breakingStep * UnitDeg));
                Shutdown();
            }
        }

        /// <summary>
        /// 启动状态采用函数
        /// </summary>
        void StartMove()
        {
            float sample = _rollingTime / startStatusTime;
            if (sample > 1)
            {
                _rotateSpeed = _startTargetSpeed;

                _Move();
                WheelStatus = Status.Rolling;
                return;
            }


            _rotateSpeed = startCurve.Evaluate(sample) * _startTargetSpeed;
            _deltaRotate += _rotateSpeed * Time.deltaTime;

            _Move();
        }

        /// <summary>
        /// 移动 应用revers
        /// </summary>
        private void _Move()
        {
            if (GlobalObserver.CurrentBonusState == 3)
            {
                reverse = IsUpgrad;
            }
            else
            {
                reverse = true;
            }

            int i = reverse ? -1 : 1;
            if (isSpeedUp == true)
            {
                var angle = _deltaRotate.z % _unitDeg;
                if (angle >= (_unitDeg / 2f))
                {
                    if (isAllowPlaySound == true)
                    {
                        isAllowPlaySound = false;
                        AudioManager.Playback("FE_WheelHyper Ding");
                    }
                }
                else
                {
                    isAllowPlaySound = true;
                }
            }

            transform.localRotation = Quaternion.Euler(i * _deltaRotate + _startRollingRotate);
        }

        /// <summary>
        /// 在减速完成后的反弹
        /// </summary>
        void AfterBreakingMove()
        {
            var samp = _rollingTime - _afterBreakingStartTime;
            var bounceSpeed = _breakingSpeed * afterBreakingCurve.Evaluate(samp);
            _deltaRotate += bounceSpeed * Time.deltaTime;
            _Move();
            if (samp >= 1)
            {
                _deltaRotate = _targetRotate;
                _Move();
                WheelStatus = Status.Stop;
            }
        }

        public enum Status
        {
            Start,

            Rolling,
            SpeedUp,
            Breaking,
            AfterBreaking,
            Stop
        }

        private Coroutine _coroutine;

        public Wheel(Vector3 currentStatusRollingSpeed)
        {
            this._currentStatusRollingSpeed = currentStatusRollingSpeed;
        }

        private void OnMouseDown()
        {
            if (WheelStatus == Status.Stop)
            {
                StartRolling(idlingSpeed.z);
                return;
            }


            if (_rotateSpeed.z > minShutdownSpeed && _rotateSpeed.z < bonusIdlingSpeed.z)
            {
                SpeedUp(bonusIdlingSpeed.z, Status.Rolling);
                Debug.Log(SpeedUpTargetSpeed);
                return;
            }

            if (_rotateSpeed.z >= bonusIdlingSpeed.z)
            {
                SpeedUp(maxSpeed.z, Status.Breaking, Random.Range(0, 12));
                Debug.Log(SpeedUpTargetSpeed);
            }
        }
    }
}