using System;
using Haden.NxtSharp.Sensors;

namespace Haden.NxtSharp.Brick
{
    /// <summary>
    /// Get the input value(s) of the NXT.
    /// </summary>
    public struct NxtGetInputValues
    {
        /// <summary>
        /// Valid input value.
        /// </summary>
        public bool Valid;
        /// <summary>
        /// Calibrated input value.
        /// </summary>
        public bool Calibrated;
        /// <summary>
        /// Sensor type.
        /// </summary>
        public NxtSensorType Type;
        /// <summary>
        /// Sensor mode.
        /// </summary>
        public NxtSensorMode Mode;
        /// <summary>
        /// Raw data value.
        /// </summary>
        public UInt16 RawAd;
        /// <summary>
        /// Normalized data value.
        /// </summary>
        public UInt16 NormalizedAd;
        /// <summary>
        /// Scaled value.
        /// </summary>
        public Int16 ScaledValue;
        /// <summary>
        /// Calibrated value.
        /// </summary>
        public Int16 CalibratedValue;
    }
}
