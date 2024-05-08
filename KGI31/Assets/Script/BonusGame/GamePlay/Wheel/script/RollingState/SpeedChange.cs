using System;
using UnityEngine;

namespace script.RollingState
{
    public sealed class SpeedChange : RollingState
    {
        private readonly ISampleStrategy _sampleStrategy;

        private int _target;


        /// <summary>
        /// 指定时间和速度上下限，在规定时间到达目标速度， 加速减速及目标速度取决于曲线方向,即可加速也可减速
        /// </summary>
        /// <param name="curve">速度采样曲线，曲线方向决定了使用上限或是下限作为目标速度</param>
        /// <param name="minSpeed">速度下限</param>
        /// <param name="maxSpeed">速度上限</param>
        /// <param name="during">速度变化持续时间</param>
        /// <param name="onComplete">完成回调，当采样超过1（也就是持续时间刚好超过目标时间）时回调</param>
        public SpeedChange(AnimationCurve curve, float minSpeed, float maxSpeed, float during, Action onComplete)
        {
            if (minSpeed > maxSpeed)
            {
                (minSpeed, maxSpeed) = (maxSpeed, minSpeed);
            }

            _sampleStrategy = new SampleSpeedByTime(curve, minSpeed, maxSpeed, during);
            OnComplete = onComplete;
        }


        /// <summary>
        /// 指定目标角度，在到达目标角度时保证滚轮转速等于目标速度，目标速度取决于曲线方向 。即可加速也可减速
        /// 通常用于获得结果，此时应当使用递减曲线，maxSpeed设置为当前滚轮速度，而minSpeed应**大于0且小于当前速**
        /// </summary>
        /// <param name="curve">速度采样曲线，曲线方向决定了使用上限或是下限作为目标速度</param>
        /// <param name="minSpeed">速度的下限</param>
        /// <param name="maxSpeed">速度上限</param>
        /// <param name="currentDeg">当前的角度</param>
        /// <param name="targetDeg">目标角度</param>
        /// <param name="revers">是否进行了反转</param>
        /// <param name="onComplete">完成回调，当采样超过1（也就是持续时间刚好超过目标点）时回调</param>
        public SpeedChange(AnimationCurve curve, float minSpeed, float maxSpeed, float currentDeg,
            float targetDeg, bool revers, Action onComplete)
        {
            if (minSpeed > maxSpeed)
            {
                (minSpeed, maxSpeed) = (maxSpeed, minSpeed);
            }


            var deltaDeg = targetDeg - currentDeg;

            if (revers)
            {
                var normalize = targetDeg % 360;
                var round = (int)targetDeg / 360;
                deltaDeg = 360 - normalize + currentDeg;
                deltaDeg += round * 360;
            }


            _sampleStrategy = new SampleSpeedByDistance(curve, deltaDeg, minSpeed, maxSpeed);
            OnComplete = onComplete;
        }


        /// <summary>
        /// 采样
        /// </summary>
        /// <param name="deltaTime">通常是Time.deltaTime</param>
        /// <returns></returns>
        public override SampleResult Sample(float deltaTime)
        {
            var ret = _sampleStrategy.Sample(deltaTime);
            if (ret.IsOverSample)
            {
                OnComplete?.Invoke();
            }

            return ret;
        }

        public override event Action OnComplete;
    }
}
