using System;

namespace UniversalModule.Initialize {
    [AttributeUsage(AttributeTargets.Method)]
    public class AutoLoadAttribute : Attribute
    {
        public int order { get; private set; }
        public AutoLoadAttribute(int order = 100)
        {
            this.order = order;
        }
    }
}