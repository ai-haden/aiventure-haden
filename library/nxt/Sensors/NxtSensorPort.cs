namespace Haden.NxtSharp.Sensors
{
    /// <summary>
    /// The Nxt sensor port enumeration.
    /// </summary>
    public enum NxtSensorPort
    {
        /// <summary>
        /// Port 1: Pressure sensor.
        /// </summary>
        Port1 = 0x00,
        /// <summary>
        /// Port 2: Sound sensor.
        /// </summary>
        Port2 = 0x01,
        /// <summary>
        /// 
        /// </summary>
        Port3 = 0x02,
        /// <summary>
        /// 
        /// </summary>
        Port4 = 0x03,
        /// <summary>
        /// No sensor.
        /// </summary>
        None = 0xFE
    }
}
