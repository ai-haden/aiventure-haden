using System.ComponentModel;
using Haden.NxtSharp.Brick;

namespace Haden.NxtSharp.Sensors
{
    /// <summary>
    /// The class encapsulating the functionality of the sound sensor.
    /// </summary>
	public class NxtSoundSensor : NxtSensor
    {
        bool _adjustForHumanEar = true;
        /// <summary>
        /// Initializes a new instance of the <see cref="NxtSoundSensor"/> class.
        /// </summary>
		public NxtSoundSensor()
        {
			
		}
        /// <summary>
        /// Should the sensor adjust for the sensitivity of the human ear?
        /// </summary>
		public bool AdjustForHumanEar {
			get {
				return _adjustForHumanEar;
			}
			set {
				_adjustForHumanEar = value;
			}
		}
		/// <summary>
		/// Use the boolean mode.
		/// </summary>
		protected override NxtSensorMode Mode { get { return NxtSensorMode.Percentage; } }
		/// <summary>
		/// The type of this sensor.
		/// </summary>
		protected override NxtSensorType Type
        {
			get
			{
			    if(AdjustForHumanEar)
                {
					return NxtSensorType.SoundDba;
				}
			    return NxtSensorType.SoundDb;
			}
        }
        /// <summary>
        /// Returns the value in percent.
        /// </summary>
		[Browsable(false)]
		public int Value { get { return LastResult.ScaledValue; } }
        /// <summary>
        /// Override this to define the sensor threshold.
        /// </summary>
        /// <param name="previousValue">previous value</param>
        /// <param name="newValue">new value</param>
		protected override bool IsSensorReadingDifferent(NxtGetInputValues previousValue, NxtGetInputValues newValue) {
			return previousValue.ScaledValue != newValue.ScaledValue;
		}
	}
}
