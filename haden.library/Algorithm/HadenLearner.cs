

namespace Haden.Library.Algorithm
{
    public class HadenLearner
    {
        public Policy HadenPolicy { get; set; }
        public HadenLearner() 
        {
            HadenPolicy = new Policy();
        }
    }
}
