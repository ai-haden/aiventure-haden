using System.ComponentModel;
using Haden.NxtSharp.Brick;

namespace Haden.NxtSharp.Sensors
{
    /// <summary>
    /// The class encapsulating the functionality of the light sensor.
    /// </summary>
	public partial class NxtLightSensor : NxtSensor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NxtLightSensor"/> class.
        /// </summary>
		public NxtLightSensor()
        {
			InitializeComponent();
		}
        /// <summary>
        /// Initializes a new instance of the <see cref="NxtLightSensor"/> class using IContainer.
        /// </summary>
		public NxtLightSensor(IContainer container)
        {
			container.Add(this);
			InitializeComponent();
		}
        /// <summary>
        /// Should the light sensor generate its own light?
        /// </summary>
        [Category("Lego NXT"), Description("Should the light sensor generate its own light?")]
        public bool Active { get; set; }
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
			    if(Active)
                {
					return NxtSensorType.LightActive;
				}
			    return NxtSensorType.LightInactive;
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
		protected override bool IsSensorReadingDifferent(NxtGetInputValues previousValue, NxtGetInputValues newValue)
        {
			return previousValue.ScaledValue != newValue.ScaledValue;
		}
	}
}
