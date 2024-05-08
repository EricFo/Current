using System;
using UnityEngine;

namespace script
{
    public class SampleSpeedByDistance : ISampleStrategy
    {
        private readonly AnimationCurve _animationCurve;
        private readonly float _deltaDistance;
        private readonly float _baseSpeed;
        private readonly float _deltaSpeed;
        private float _currentDistance = 0;


        /// <summary>
        /// 根据当前已移动的距离和需要移动的距离的比值采样速度。
        /// 此时曲线 x-已移动距离 y-速度
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="deltaDistance"></param>
        /// <param name="minSpeed"></param>
        /// <param name="maxSpeed"></param>
        /// <exception cref="Exception"></exception>
        public SampleSpeedByDistance(AnimationCurve curve, float deltaDistance, float minSpeed, float maxSpeed)
        {
            if (maxSpeed < minSpeed)
            {
                throw new Exception("maxSpeed < minSpeed");
            }

            _deltaSpeed = maxSpeed - minSpeed;
            _baseSpeed = minSpeed;
            _deltaDistance = deltaDistance;
            _animationCurve = curve;
        }

        public SampleResult Sample(float deltaTime)
        {
            bool isOverSample = false;
            var normalizedDistance = _currentDistance / _deltaDistance;
            if (normalizedDistance > 1)
            {
                normalizedDistance = 1;
                isOverSample = true;
            }

            var samp = _animationCurve.Evaluate(normalizedDistance);

            var speed = _baseSpeed + _deltaSpeed * samp;
            _currentDistance += speed * deltaTime;


            return new SampleResult() { SampleSpeed = speed, IsOverSample = isOverSample };
        }
    }
}