using System;
using System.Collections.Generic;
using System.Linq;

namespace Haden.Library.Algorithm
{
    public class ReinforcementLearningHaden : LearningAlgorithm
    {
        public StateActionSpace ActionSpace { get; set; }
        public Dictionary<int, string> Policy { get; set; }
        public List<int> States { get; set; }
        public double LearningRate { get; set; }
        public double Gamma { get; set; }
        /// <summary>
        /// Find out which is the next action given the state 'current_state' return next_action : next action according to policy
        /// </summary>
        /// <param name="currentState">State of the current.</param>
        /// <returns></returns>
        public override int GetNextAction(int currentState)
        {
            Console.WriteLine("state", currentState);
            if (rd.random() >= 0.1)
            {
                // print 'deterministic choice', self.policy[self.states.index(currentState)]
                Console.WriteLine("action", Policy[States.IndexOf(currentState)]);
                return Policy[States.IndexOf(currentState)];
            }
            else
            {
                // print 'epsilon-greedy choice'
                var a = rd.choice(ActionSpace.GetEligibleActions(currentState));
                Console.WriteLine("action", a);
                return a;
                // return rd.choice(self.state_action_space.get_eligible_actions(currentState))
            }

        }

        public List<int> ComputeExpectaton(int currentState, Dictionary<string, int> freqDictionary)
        {
            double probabilityCurrentStateActionState;
            var expectedValuesPerActions = (from x in ActionSpace.GetEligibleActions(currentState)
                                          select 0).ToList();
            foreach (var action in ActionSpace.GetEligibleActions(currentState))
            {
                var totalNumberCounts = freqDictionary[action][1];
                var frequencyPerAction = freqDictionary[action][0];
                var expectedCurrentAction = 0;

                foreach (var state in States)
                {
                    if (totalNumberCounts != 0)
                    {
                        probabilityCurrentStateActionState = (frequencyPerAction[States.IndexOf(state)]) / totalNumberCounts;
                    }
                    else
                    {
                        probabilityCurrentStateActionState = 1 / (States.Count);
                    }
                    var exp_component = this.rewardObj.get_rewards(currentState, action, nextState: state, problemType: "capture") + Gamma * this.values[States.IndexOf(state)];
                    expectedCurrentAction += probabilityCurrentStateActionState * exp_component;
                }
                expectedValuesPerActions[ActionSpace.GetEligibleActions(currentState).IndexOf(action)] = expectedCurrentAction;
            }
            // print "curr_state: ", curr_state, "freq_dict: ", freq_dict
            // print "expected Values per action: ",  exp_values_per_actions
            return expectedValuesPerActions;
        }

        // Perform things according to which reward was given
        public override void ReceiveReward(int oldState, string action, int nextState, double reward)
        {
            var old_state_index = this.states.index(oldState);
            var next_state_index = this.states.index(nextState);
            var td_error = reward + this.Gamma * this.values[next_state_index] - this.values[old_state_index];
            this.values[old_state_index] += this.LearningRate * td_error;
            // Update frequencies ~ probabilities
            this.frequencies[old_state_index][action][0][next_state_index] += 1;
            this.frequencies[old_state_index][action][1] += 1;
        }

        // Do things that need to be done when an episode was finished
        public override void FinalizeEpisode()
        {
            // print('finalise episode')
            // Update policy
            // if self.global_counter == self.max_number_iterations:
            foreach (var state in this.states)
            {
                var state_index = this.states.index(state);
                var expected_values = this.compute_expectation(state, this.frequencies[state_index]);
                this.policy[state_index] = this.state_action_space.get_eligible_actions(state)[expected_values.index(max(expected_values))];
                if (!this.state_action_space.get_eligible_actions(state).Contains(this.policy[state_index]))
                {
                    Console.WriteLine("ERROR IN FINALISE EPISODE");
                }
            }
            // print 'policy updated'
            Console.WriteLine(this.policy, this.values);
            // self.plot_results()
        }

        public void PlotResults()
        {
            var max_tup_coord = Math.Max(this.states);
            var min_tup_coord = Math.Min(this.states);
            var min_x = min_tup_coord[0];
            var min_y = min_tup_coord[1];
            var max_x = max_tup_coord[0];
            var max_y = max_tup_coord[1];
            var matr_values = new List<object>();
            foreach (var y in Enumerable.Range(min_y, max_y + 1 - min_y))
            {
                var new_row_values = new List<object>();
                foreach (var x in Enumerable.Range(min_x, max_x + 1 - min_x))
                {
                    new_row_values.append(this.values[this.states.index((x, -y))]);
                }
                matr_values.append(new_row_values);
            }
            // http://matplotlib.org/api/pyplot_api.html#matplotlib.pyplot.matshow
            // http://matplotlib.org/examples/pylab_examples/matshow.html
            //var VALUES_MAT = np.array(matr_values);
            //pyplot.matshow(VALUES_MAT, fignum: this.figure_count, cmap: pyplot.cm.gray);
            //pyplot.show();
            //this.figure_count += 1;
        }
    }
}
