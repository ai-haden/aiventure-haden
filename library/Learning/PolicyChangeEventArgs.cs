using System;

namespace Haden.Library.Learning
{
    public class PolicyChangeEventArgs : EventArgs
    {
        public State State { get; set; }
        public Query Query { get; set; }
        public FeatureValuePair Label { get; set; }
    }
}
