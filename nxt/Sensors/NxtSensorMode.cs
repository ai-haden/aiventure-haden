namespace Haden.NxtSharp.Sensors
{
    /// <summary>
    /// The Nxt sensor modes.
    /// </summary>
    public enum NxtSensorMode
    {
        /// <summary>
        /// I2C raw mode.
        /// </summary>
        Raw = 0x00,
        /// <summary>
        /// Boolean mode.
        /// </summary>
        Boolean = 0x20,
        /// <summary>
        /// Transition measuring mode.
        /// </summary>
        TransitionCounter = 0x40,
        /// <summary>
        /// Period measuring mode.
        /// </summary>
        PeriodCounter = 0x60,
        /// <summary>
        /// Percentage mode.
        /// </summary>
        Percentage = 0x80,
        /// <summary>
        /// Celsius reading mode.
        /// </summary>
        Celsius = 0xA0,
        /// <summary>
        /// Fahrenheit reading mode.
        /// </summary>
        Fahrenheit = 0xC0,
        /// <summary>
        /// Step angle mode for greater precision in movement.
        /// </summary>
        AngleStep = 0xE0
    }
}
