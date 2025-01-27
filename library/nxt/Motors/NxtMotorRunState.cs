using System;

namespace Haden.NxtSharp.Motors
{
    /// <summary>
    /// The Nxt motor run state.
    /// </summary>
    [Flags]
    public enum NxtMotorRunState : byte
    {
        /// <summary>
        /// Idle motor.
        /// </summary>
        Idle = 0x00,
        /// <summary>
        /// Motor state is ramping up.
        /// </summary>
        RampUp = 0x10,
        /// <summary>
        /// Motor is running.
        /// </summary>
        Running = 0x20,
        /// <summary>
        /// Motor state is ramping down.
        /// </summary>
        Rampdown = 0x40
    }
}
