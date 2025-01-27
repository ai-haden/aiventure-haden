using System;

namespace Haden.NxtSharp.Motors
{
    /// <summary>
    /// The Nxt motor mode enumeration.
    /// </summary>
    [Flags]
    public enum NxtMotorMode : byte
    {
        /// <summary>
        /// No motor mode.
        /// </summary>
        None = 0x00,
        /// <summary>
        /// Should the motor be turned on?
        /// </summary>
        MotorOn = 0x01,
        /// <summary>
        /// Should the motor brake after the action is completed?
        /// </summary>
        Brake = 0x02,
        /// <summary>
        /// Should motor regulation be used? (use NxtMotorRegulationMode to specify which one).
        /// </summary>
        Regulated = 0x04
    }
}