namespace Haden.NxtSharp
{
    public static class RueTheWhirl
    {
        static int NumberOfWhirls = 0;
        public enum States
        {
            Zero, One, Two, Three, Four
        }
        /// <summary>
        /// Gets or sets a value indicating whether [connected to haden].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [connected to haden]; otherwise, <c>false</c>.
        /// </value>
        public static bool ConnectedHaden { get; set; }
        /// <summary>
        /// The central notifier of the current state of the whirl.
        /// </summary>
        /// <value>
        /// The state of the current.
        /// </value>
        public static string CurrentState { get; set; }
        /// <summary>
        /// The actions to perform, given the ratio between form and function.
        /// </summary>
        /// <returns></returns>
        public static string ActionStateZero()
        {
            // The zeroth-rest state.
            if (NumberOfWhirls == 0 && !ConnectedHaden)
            {
                return "Beginning my whirl. Not connected to haden.";
            }
            else
            {
                return "Resting in my zeroth state. Connected.";
            }
        }
        public static string ActionStateOne()
        {
            // TASK: Discover the peak value of a light sensor
            return "Discover the peak value detected at a light sensor.";
        }
        public static string ActionStateTwo()
        {
            // TASK: Check if connected to the correct hardware.
            if (!ConnectedHaden)
            {
                return "Not connected to haden.";
            }
            else
            {
                return "I am now in state two on the whirl. Connected.";
            }
        }
        public static string ActionStateThree()
        {
            return "Discover the peak value detected at a light sensor.";
        }
        public static string ActionStateFour()
        {
            NumberOfWhirls++;
            // TASK: Check if connected to the correct hardware.
            if (!ConnectedHaden)
            {
                return "Not connected to haden.";
            }
            else
            {
                //return "Discover the peak value detected at a light sensor.";
                return "I am now in state four on the whirl. Connected.";
            }
        }
        /// <summary>
        /// The global controller.
        /// </summary>
        /// <returns>Actions</returns>
        public static string ActionController()
        {
            if (CurrentState == "Zero")
            {
                return ActionStateZero();
            }
            if (CurrentState == "One")
            {
                return ActionStateOne();
            }
            if (CurrentState == "Two")
            {
                return ActionStateTwo();
            }
            if (CurrentState == "Three")
            {
                return ActionStateThree();
            }
            if (CurrentState == "Four")
            {
               return ActionStateFour();
            }
            else { return ActionStateZero(); }
        }
    }
}
