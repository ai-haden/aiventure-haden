namespace Haden.Runtime
{
    class Program
    {
        private System.Timers.Timer _whirlTimer;
        private Controller _controller;

        public Session Session { get; }

        /// <summary>
        /// An instance of haden's voice.
        /// </summary>
        private readonly SpVoice _voice = new SpVoice();
        private bool WhirlActive {  get; set; } 
        private bool VoiceSpoken { get; set; }
        /// <summary>
        /// The peak light sensor value, called np.
        /// </summary>
        public object BoxedPeakValue;
        /// <summary>
        /// The current light sensor value, called nc.
        /// </summary>
        public object BoxedCurrentValue;
        /// <summary>
        /// The stored value for difference between current and optimal light sensor values, called d.
        /// </summary>
        public object BoxedDifferenceValue;
        /// <summary>
        /// The stored value for the last turn. The depth of primitive memory is now n-1.
        /// </summary>
        public string RememberedLastTurn;
        /// <summary>
        /// The number of degrees to turn a motor.
        /// </summary>
        /// <value>
        /// The last turn granularity.
        /// </value>
        public int MotorTurnGranularity { get; set; }
        /// <summary>
        /// The stored value for an interation to define the number of autonomous iterations of a function before pausing.
        /// </summary>
        public int Iteration;
        /// <summary>
        /// The iteration limit.
        /// </summary>
        public int InterationLimit;
        /// <summary>
        /// The n - 1 value.
        /// </summary>
        public double Then { get; set; }
        /// <summary>
        /// The n, n + 1, n + 2, ..., np value.
        /// </summary>
        public double Now { get; set; }
        public bool GreaterNow { get; set; }
        public bool LessNow { get; set; }
        public List<double> LightValuesSeen { get; set; }
        public List<string> TurnsMade{ get; set; }
        // Velocity control variables.
        public const int PowerDrive = 30;
        public const int PowerSeek = 20;
        public const int Degrees = 30;

        static void Main(string[] args)
        {
            System.Console.WriteLine("Hello, World!");
        }
    }
}
