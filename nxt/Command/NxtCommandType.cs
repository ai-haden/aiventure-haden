namespace Haden.NxtSharp.Command
{
    /// <summary>
    /// The Nxt command type enumeration.
    /// </summary>
    public enum NxtCommandType
    {
        /// <summary>
        /// Direct command, the NXT will send a response.
        /// </summary>
        DirectCommandWithResponse = 0x00,
        /// <summary>
        /// System command, the NXT will send a response.
        /// </summary>
        SystemCommandWithResponse = 0x01,
        /// <summary>
        /// Reply.
        /// </summary>
        Reply,
        /// <summary>
        /// Direct command, the NXT will NOT send a response.
        /// </summary>
        DirectCommandNoResponse = 0x80,
        /// <summary>
        /// System command, the NXT will NOT send a response.
        /// </summary>
        SystemCommandNoResponse = 0x81
    }
}
