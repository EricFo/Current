using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using script;
using script.RollingState;
using SlotGame.Core;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using UniversalModule.AudioSystem;
using Random = UnityEngine.Random;

public class Wheel : MonoBehaviour
{
    private RollingState _rollingState;
    private RollingState _nextState;
    [SerializeField] private AnimationCurve speedUp;
    [SerializeField] private AnimationCurve breaking;
    [SerializeField] private AnimationCurve bounce;


    private LinkedList<RollingState> _statesPlanning = new LinkedList<RollingState>();

    private readonly UniformMotionState _uniformMotionStateInf = new UniformMotionState(1.0f, true);

    private float _currentSpeed = 0;

    private float _targetDeg;

    //反转
    public bool reverse = false;
    public bool isExtra = false;
    public float lowSpeed;
    public float midSpeed;
    public float highSpeed;
    public float stopSpeed;

    public Action OnWheelStop;

    public Dictionary<int, float> degOfIndex = new Dictionary<int, float>()
    {
        {0,0},{1,30},{2,60},{3,90},{4,120},{5,150},{6,180},{7,210},{8,240},{9,270},{10,300},{11,330}
    };


    float CheckRevers()
    {
        return reverse ? -1f : 1f;
    }

    private void Update()
    {
        if (GlobalObserver.CurrentBonusState == 3)
        {
            reverse = isExtra;
        }
        else
        {
            reverse = true;
        }
        if (_rollingState != null)
        {
            var current = gameObject.transform.localRotation.eulerAngles;
            var ret = _rollingState.Sample(Time.deltaTime);
            _currentSpeed = ret.SampleSpeed;

            if (ret.IsOverSample)
            {
                _rollingState = _nextState;
                _nextState = null;
                return;
            }

            current.z += _currentSpeed * Time.deltaTime * CheckRevers();
            //Debug.Log(_currentSpeed);
            gameObject.transform.localRotation = Quaternion.Euler(current);
        }
    }
    void Start()
    {
        SlowUniForm();
    }


    #region 示例

#if UNITY_EDITOR
    bool _testRolling = false;


    //自动测试结果是否准确
    IEnumerator Test()
    {
        while (true)
        {
            if (_testRolling)
            {
                yield return null;
                continue;
            }

            Debug.Log("=============================");
            _testRolling = true;

            reverse = Random.value >= 0.5F;
            Debug.Log($"Reverse:{reverse}");

            var targetSpeed = Random.Range(100, 1800);
            Debug.Log($"target Speed:{targetSpeed}");

            //加速阶段
            _rollingState = new SpeedChange(speedUp, 10, targetSpeed, 3f, null);
            //匀速状态
            _uniformMotionStateInf.Speed = targetSpeed;
            _nextState = _uniformMotionStateInf;

            //随机切换减速至结果
            var time = Random.Range(0.01f, 5);
            Debug.Log($"Shutdown time:{time}");
            yield return new WaitForSeconds(time);

            //随即目标
            float targetDeg = Random.Range(0, 360);
            Debug.Log($"Shutdown Speed：{_currentSpeed}");
            _nextState = null;


            //切换状态为有目标的减速
            _rollingState = new SpeedChange(breaking, 10, _currentSpeed, gameObject.transform.rotation.eulerAngles.z,
                targetDeg + 3 * 360f, reverse, () =>
                {
                    var curr = gameObject.transform.rotation.eulerAngles.z;
                    Debug.Log($"target:{targetDeg}  result:{curr}  bias:{targetDeg - curr}");
                    Assert.IsTrue(curr > targetDeg - 0.15 && curr < targetDeg + 0.15);
                    _testRolling = false;
                });
        }
    }

    public bool testPlanning = false;

    private void OnMouseDown()
    {
        if (testPlanning)
        {
            SpeedUp(6);
            //PlanningState();
            _rollingState = _statesPlanning.First.Value;
            return;
        }

        StartCoroutine(Test());
    }

    private void PlanningState()
    {
        float[] targetSpeed = { 1200f, 360f, 15 };
        float targetDeg = 75f;
        //1. 10加速到1200
        _statesPlanning.AddLast(_rollingState = new SpeedChange(speedUp, 10, targetSpeed[0], 3f, NextStep));

        //2. 匀速保持1200
        _statesPlanning.AddLast(new UniformMotionState(targetSpeed[0], 2f, NextStep));

        void OnNeedStop()
        {
            //4. 开始停止旋转停止后会到达75° ，同时剩余15的速度 这里是在回调中添加因为需要获取当前速度
            _statesPlanning.AddLast(new SpeedChange(breaking, targetSpeed[2], _currentSpeed,
                transform.rotation.eulerAngles.z, targetDeg + 360F, reverse, NextStep));
            NextStep();
            _statesPlanning.AddLast(new SpeedChange(bounce, 30, -30, 1f, NextStep));
        }

        //3. 1200减速到360
        _statesPlanning.AddLast(new SpeedChange(breaking, targetSpeed[1], targetSpeed[0], 2f, OnNeedStop));
    }


#endif

    #endregion


    public int CurrentCellIndex()
    {
        float baseRotate = transform.localRotation.eulerAngles.z;
        baseRotate = NormalizeDeg(baseRotate);
        for (int i = 0; i < degOfIndex.Count; i++)
        {
            if (baseRotate < degOfIndex[i]+15)
            {
                return i;
            }
        }
        if (baseRotate > degOfIndex.Last().Value + 15)
        {
            return 0;
        }
        Debug.LogError("找不到当前角度索引");
        return 0;
    }
    float NormalizeDeg(float deg)
    {
        deg %= 360;
        return deg < 0 ? 360 + deg : deg;
    }


    public void SlowUniForm()
    {
        _statesPlanning.Clear();
        //_statesPlanning.AddLast(new SpeedChange(speedUp, 0, lowSpeed, 2f, NextStep));
        _statesPlanning.AddLast(new UniformMotionState(lowSpeed, true));
        _rollingState = _statesPlanning.First.Value;
    }
    public void StartRolling(float minSpeed)
    {
        UIController.BottomPanel.StopBtn.interactable = true;
        _statesPlanning.Clear();
        _statesPlanning.AddLast(new SpeedChange(speedUp, minSpeed, midSpeed, 1f, NextStep));
        _statesPlanning.AddLast(new UniformMotionState(midSpeed, true));
        _rollingState = _statesPlanning.First.Value;
    }
    public void SpeedUp(int targetIndex)
    {
        if (GlobalObserver.CurrentBonusState == 3)
        {
            if (isExtra == false)
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
        _targetDeg = degOfIndex[targetIndex];
        _statesPlanning.Clear();
        _statesPlanning.AddLast(new SpeedChange(speedUp, midSpeed, highSpeed, 1f, NextStep));
        _statesPlanning.AddLast(new UniformMotionState(highSpeed, 1f, OnNeedStop));
        void OnNeedStop()
        {
            _statesPlanning.AddLast(new SpeedChange(breaking, stopSpeed, _currentSpeed,
            transform.localRotation.eulerAngles.z, _targetDeg + 360F, reverse, RollingBack));
            NextStep();
            
        }
        _rollingState = _statesPlanning.First.Value;
    }
    void RollingBack()
    {
        _nextState = new SpeedChange(bounce, stopSpeed, -stopSpeed, 1f, () =>
        {
            Vector3 final = transform.localRotation.eulerAngles;
            final.z = _targetDeg;
            transform.localRotation = Quaternion.Euler(final);
            OnWheelStop();
        });
    }

    public void ShutDown()
    {
        if (_currentSpeed < midSpeed)
        {
            return;
        }
        _statesPlanning.Clear();
        _statesPlanning.AddLast(new SpeedChange(breaking, stopSpeed, _currentSpeed,
            transform.localRotation.eulerAngles.z, _targetDeg + 360F, reverse, RollingBack));
        _rollingState = _statesPlanning.First.Value;
    }


    void NextStep()
    {
        _statesPlanning.RemoveFirst();
        if (_statesPlanning.Count <= 0)
        {
            return;
        }

        _nextState = _statesPlanning.First.Value;
        //Debug.LogWarning($"NextStep：{_nextState.GetType().Name}==========================");
    }

}