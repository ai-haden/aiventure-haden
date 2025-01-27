
namespace Haden.Library.Algorithm
{
    public abstract class LearningAlgorithm
    {
        public abstract int GetNextAction(int currentState);

        // Perform things according to which reward was given.
        public abstract void ReceiveReward(int oldState, string action, int nextState, double reward);

        // Do things that need to be done when an episode was finished.
        //public abstract void FinalizeEpisode();
        
    }
}
