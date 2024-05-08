using System;

//using UnityEditor.PackageManager.UI;
using UnityEngine;

namespace script
{
    public struct SampleResult
    {
        public bool IsOverSample; //是否过采样 ，也就是是说采样x轴是否大于1
        public float SampleSpeed; //采样出的速度结果
    }

    public interface ISampleStrategy
    {
        SampleResult Sample(float sampling);
    }
}