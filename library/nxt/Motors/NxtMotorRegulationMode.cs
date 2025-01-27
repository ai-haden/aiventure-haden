using System;

namespace Haden.NxtSharp.Motors
{
    /// <summary>
    /// The Nxt motor regulation mode enumeration.
    /// </summary>
    [Flags]
    public enum NxtMotorRegulationMode : byte
    {
        /// <summary>
        /// Use when motor usage is not regulated.
        /// </summary>
        Idle = 0x00,
        /// <summary>
        /// Use this to regulate speed, whatever that may be.
        /// </summary>
        MotorSpeed = 0x01,
        /// <summary>
        /// Use this to synchronize two motors.
        /// </summary>
        MotorSynchronization = 0x02
    }
}