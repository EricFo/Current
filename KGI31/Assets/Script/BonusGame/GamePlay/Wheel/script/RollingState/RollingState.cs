using System;

namespace script.RollingState
{
    public abstract class RollingState {
        protected RollingState() { }
        public abstract SampleResult Sample(float deltaTime);
        public abstract event Action OnComplete;
    }
}
