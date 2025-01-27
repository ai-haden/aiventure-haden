using System;

namespace Haden.Library
{
    /// <summary>
    /// Session event arguments.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class SessionEventArgs : EventArgs
    {
        private readonly string _sessionid;
        /// <summary>
        /// Initializes a new instance of the <see cref="SessionEventArgs"/> class.
        /// </summary>
        /// <param name="sessionID">The session identifier.</param>
        public SessionEventArgs(string sessionID)
        {
            _sessionid = sessionID;
        }
        /// <summary>
        /// Gets the session ID. Accessible by the listener.
        /// </summary>
        public string SessionID { get { return _sessionid; } }

    }
}
