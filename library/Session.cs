using System;

namespace Haden.Library
{
    /// <summary>
    /// The number of the user such that the system can recall them.
    /// </summary>
    public class Session
    {
        /// <summary>
        /// Gets or sets the session identifier.
        /// </summary>
        public static int SessionID { get; set; }
        /// <summary>
        /// Initializes a new <see cref="Session"/> with a hard-coded seed of 1013 (I made this) and a max-value of 10k.
        /// </summary>
        public Session()
        {
            var seed = 1013;
            var randomNumberSeeded = new Random(seed);
            SessionID = randomNumberSeeded.Next(10000);
        }
    }
}
