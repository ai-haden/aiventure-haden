namespace Haden.NxtSharp.Controllers
{
    /// <summary>
    /// The Nxt motor control stop mode.
    /// </summary>
    public enum NxtMotorControlStopMode
    {
        /// <summary>
        /// Coast when the motor is stopped
        /// </summary>
        Coast,
        /// <summary>
        /// Put the motor in brake mode when stopped
        /// </summary>
        Brake
    }
}
