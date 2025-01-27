namespace Haden.NxtSharp.Sensors
{
    /// <summary>
    /// The Nxt sensor types enumeration.
    /// </summary>
    public enum NxtSensorType
    {
        /// <summary>
        /// No sensor.
        /// </summary>
        None = 0x00,
        /// <summary>
        /// Pressure switch, like the one supplied in the Mindstorms NXT set.
        /// </summary>
        Switch = 0x01,
        /// <summary>
        /// Temperature sensor.
        /// </summary>
        Temperature = 0x02,
        /// <summary>
        /// Sonar sensor.
        /// </summary>
        Reflection = 0x03,
        /// <summary>
        /// Compass sensor.
        /// </summary>
        Angle = 0x04,
        /// <summary>
        /// Light sensor generating its own light.
        /// </summary>
        LightActive = 0x05,
        /// <summary>
        /// Light sensor relying on external light sources.
        /// </summary>
        LightInactive = 0x06,
        /// <summary>
        /// Sound (Decibel)
        /// </summary>
        SoundDb = 0x07,
        /// <summary>
        /// Sound (Decibel, adjusted for the human ear).
        /// </summary>
        SoundDba = 0x08,
        /// <summary>
        /// Custom setting.
        /// </summary>
        Custom = 0x09,
        /// <summary>
        /// Low speed.
        /// </summary>
        LowSpeed = 0x0A,
        /// <summary>
        /// I2C communication mode (low speed).
        /// </summary>
        LowSpeed_9V = 0x0B
    }
}
