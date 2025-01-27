using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Haden.Library.Algorithm
{
    public class DummyLearner : LearningAlgorithm
    {
        public StateActionSpace ActionSpace { get; set; }
        public List<int> States { get; set; }
        public List<string> Actions { get; set; }
        public double Gamma { get; set; } 
        public double LearningRate { get; set; }
        public Dictionary<int, int> Values { get; set; }
        public Dictionary<int, string> Policy { get; set; }
        public DummyLearner(StateActionSpace stateActionSpace)
        {
            Debug.Assert(stateActionSpace is StateActionSpace);// Check type.
            ActionSpace = stateActionSpace;
            States = ActionSpace.GetListofStates();
            Actions = ActionSpace.GetListofActions();
            Gamma = 0.5;
            LearningRate = 0.1;
            //random initialisation
            Values = new Dictionary<int, int>();
            Policy = new Dictionary<int, string>();

            foreach (var _tup_1 in this.States.Select((_p_1, _p_2) => Tuple.Create(_p_2, _p_1)))
            {
                var i = _tup_1.Item1;
                var state = _tup_1.Item2;
                Values[state] = 0;
                var eligibleActions = ActionSpace.GetEligibleActions(state);
                Policy[state] = eligibleActions[i % eligibleActions.Count];
            }
            Console.WriteLine(this.Values);
        }

        public override int GetNextAction(int currentState)
        {
            return Policy[currentState][1];
        }

        // Do TD return
        public override void ReceiveReward(int oldState, string action, int nextState, double reward)
        {
            var td_error = reward + Gamma * Values[nextState] - Values[oldState];
            Values[oldState] += Convert.ToInt32(LearningRate * td_error);
        }

        // 
        // Update policy greedily, in the dummy example assuming equal transition probabilities
        //         
        //public override void FinalizeEpisode(int[] currentStates, int[] nextActions)
        //{
        //    var currentState = currentStates[0] + nextActions[0];
        //    var nextAction = currentStates[1] + nextActions[1];
        //    var nextStateDeterministic = currentState + nextAction;//) => (currentStates[0] + nextActions[0], currentStates[1] + nextActions[1]);
        //    try
        //    {
        //        foreach (var state in Policy.Keys)
        //        {
        //            var current_next_value = Values[nextStateDeterministic(state, Policy[state])];
        //            // find best action
        //            foreach (var action in ActionSpace.GetEligibleActions(state))
        //            {
        //                var value_of_next = Values[nextStateDeterministic(state, action)];
        //                if (value_of_next >= current_next_value)
        //                {
        //                    Policy[state] = action;
        //                    current_next_value = value_of_next;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        //<parser-error>
        //    }
        //}
    }
}
