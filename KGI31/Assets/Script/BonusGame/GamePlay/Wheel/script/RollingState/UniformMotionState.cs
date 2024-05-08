using System;

namespace script.RollingState
{
    public sealed class UniformMotionState : RollingState
    {
        private readonly float _during;
        private float _current;
        private readonly bool _infinity;

        public float Speed
        {
            get => _result.SampleSpeed;
            set => _result.SampleSpeed = value;
        }

        private SampleResult _result = new SampleResult() { SampleSpeed = 0 };

        /// <summary>
        /// 持续时间的匀速运动
        /// </summary>
        /// <param name="speed"></param>
        /// <param name="during"></param>
        /// <param name="onComplete"></param>
        public UniformMotionState(float speed, float during, Action onComplete)
        {
            Speed = speed;
            _during = during;
            OnComplete += onComplete;
        }

        /// <summary>
        /// 无限匀速运动
        /// </summary>
        /// <param name="speed"></param>
        /// <param name="infinity">这个参数只是让使用者明白自己再用无限的匀速运动</param>
        public UniformMotionState(float speed, bool infinity)
        {
            Speed = speed;
            _infinity = infinity;
        }


        public override SampleResult Sample(float deltaTime)
        {
            if (_infinity) return _result;

            _current += deltaTime;
            if (_current > _during)
            {
                _result.IsOverSample = true;
                OnComplete?.Invoke();
            }

            return _result;
        }

        public override event Action OnComplete;
    }
}