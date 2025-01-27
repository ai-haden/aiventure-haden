using System.ComponentModel;

namespace Haden.NxtSharp.Sensors
{
    /// <summary>
    /// The class encapsulating the functionality of the pressure sensor.
    /// </summary>
	public partial class NxtPressureSensor : NxtSensor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NxtPressureSensor"/> class.
        /// </summary>
		public NxtPressureSensor()
        {
			InitializeComponent();
		}
        /// <summary>
        /// Initializes a new instance of the <see cref="NxtPressureSensor"/> class using IContainer.
        /// </summary>
		public NxtPressureSensor(IContainer container)
        {
			container.Add(this);
			InitializeComponent();
		}
		/// <summary>
		/// Use the boolean mode.
		/// </summary>
		protected override NxtSensorMode Mode { get { return NxtSensorMode.Boolean; } }
		/// <summary>
		/// The type of this sensor.
		/// </summary>
        protected override NxtSensorType Type { get { return NxtSensorType.Switch; } }
		/// <summary>
		/// Was the button pressed at the last poll?
		/// </summary>
		public bool IsPressed { get { return LastResult.ScaledValue == 1; } }
	}
}
