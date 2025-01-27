using Haden.NxtSharp.Motors;

namespace Haden.NxtSharp.Brick
{
    /// <summary>
    /// Get the output state of the NXT.
    /// </summary>
    public struct NxtGetOutputState
    {
        /// <summary>
        /// Motor power.
        /// </summary>
        public sbyte Power;
        /// <summary>
        /// Motor mode.
        /// </summary>
        public NxtMotorMode Mode;
        /// <summary>
        /// Motor regulated mode.
        /// </summary>
        public NxtMotorRegulationMode RegulationMode;
        /// <summary>
        /// Turn ratio.
        /// </summary>
        public sbyte TurnRatio;
        /// <summary>
        /// Motor run state.
        /// </summary>
        public NxtMotorRunState RunState;
        /// <summary>
        /// Tachometer limit.
        /// </summary>
        public uint TachoLimit;
        /// <summary>
        /// Tachometer count.
        /// </summary>
        public int TachoCount;
        /// <summary>
        /// Block tachometer count.
        /// </summary>
        public int BlockTachoCount;
        /// <summary>
        /// Rotation count.
        /// </summary>
        public int RotationCount;
    }
}
