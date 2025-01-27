namespace Haden.NxtSharp.Motors
{
    /// <summary>
    /// The Nxt motor port enumeration.
    /// </summary>
    public enum NxtMotorPort
    {
        /// <summary>
        /// Port A (Auxiliary motor).
        /// </summary>
        PortA = 0x00,
        /// <summary>
        /// Port B (Drive motor 1).
        /// </summary>
        PortB = 0x01,
        /// <summary>
        /// Port C (Drive motor 2).
        /// </summary>
        PortC = 0x02,
        /// <summary>
        /// No port.
        /// </summary>
        None = 0xFE,
        /// <summary>
        /// All motors.
        /// </summary>
        All = 0xFF
    }
}