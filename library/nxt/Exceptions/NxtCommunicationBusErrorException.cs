using System;

namespace Haden.NxtSharp.Exceptions
{
    /// <summary>
    /// Throws an exception if the communication bus experiences an error.
    /// </summary>
    public class NxtCommunicationBusErrorException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NxtCommunicationBusErrorException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public NxtCommunicationBusErrorException(string message)
            : base(message) { }
    }
}