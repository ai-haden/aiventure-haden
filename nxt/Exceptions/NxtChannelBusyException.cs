using System;

namespace Haden.NxtSharp.Exceptions
{
    /// <summary>
    /// Throws an exception if the communication channel is busy.
    /// </summary>
    public class NxtChannelBusyException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NxtChannelBusyException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public NxtChannelBusyException(string message)
            : base(message) { }
    }
}
