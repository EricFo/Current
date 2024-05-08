using System;
using UnityEngine;

namespace script
{
    public class SampleSpeedByTime : ISampleStrategy
    {
        private readonly AnimationCurve _curve;
        private readonly float _baseSpeed;
        private readonly float _deltaSpeed;
        private readonly float _during;

        private float _currentTime;

        /// <summary>
        /// 采样策略，通过时间采样速度，此时对应曲线上 x-时间 y-速度
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="minSpeed"></param>
        /// <param name="maxSpeed"></param>
        /// <param name="during"></param>
        public SampleSpeedByTime(AnimationCurve curve, float minSpeed, float maxSpeed, float during
        )
        {
            _curve = curve;
            _deltaSpeed = maxSpeed - minSpeed;
            _during = during;
            _baseSpeed = minSpeed;
        }


        /// <summary>
        /// 采样
        /// </summary>
        /// <returns></returns>
        public SampleResult Sample(float deltaTime)
        {
            _currentTime += deltaTime;
            var normalized = _currentTime / _during;
            if (normalized >= 1)
            {
                return new SampleResult() { SampleSpeed = _baseSpeed + _deltaSpeed, IsOverSample = true };
            }

            var samp = _curve.Evaluate(normalized);
            var sampSpeed = _baseSpeed + samp * _deltaSpeed;
            return new SampleResult() { SampleSpeed = sampSpeed, IsOverSample = false };
        }
    }
}