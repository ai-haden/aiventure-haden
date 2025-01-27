using System;
using System.ComponentModel;
using Haden.NxtSharp.Brick;
using Haden.NxtSharp.Utilties;

namespace Haden.NxtSharp.Sensors
{
    public partial class NxtSensor : Component
    {
        int _autoPollDelay = 100;
        /// <summary>
        /// Initializes a new instance of the <see cref="NxtSensor"/> class.
        /// </summary>
        public NxtSensor()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="NxtSensor" /> class using IContainer.
        /// </summary>
        /// <param name="container">The container.</param>
        public NxtSensor(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }

        /// <summary>
        /// The brick this sensor is connected to.
        /// </summary>
        [Category("Lego NXT"), Description("The brick this sensor is connected to.")]
        public NxtBrick Brick { get; set; }
        /// <summary>
        /// The port this sensor is connected to
        /// </summary>
        [Category("Lego NXT"), Description("The port this sensor is connected to.")]
        public NxtSensorPort Port
        {
            get
            {
                if (Brick != null)
                {
                    foreach (var p in Brick.SensorPorts())
                    {
                        if (Brick.GetSensor(p) == this)
                        {
                            return p;
                        }
                    }
                }
                return NxtSensorPort.None;
            }
            set
            {
                if (Brick != null)
                {
                    Brick.AttachSensor(value, this);
                }
            }
        }
        /// <summary>
        /// When AutoPoll is set, the NxtBrick will continuously poll the state of the sensor. How often this is done is controlled by the AutoPollDelay property.
        /// 
        /// NOTE: The AutoPoll property must also be set on the brick Component.
        /// </summary>
        [Category("Lego NXT"), Description("When AutoPoll is set, the NxtBrick will continuously poll the state of the sensor. How often this is done is controlled by the AutoPollDelay property.\r\n\r\nNOTE: The AutoPoll property must also be set on the brick Component.")]
        public bool AutoPoll { get; set; }
        /// <summary>
        /// Delay between sensor polls. 0 = as often as possible.
        /// 
        /// NOTE: The AutoPoll property must be set for this property to have any effect
        /// </summary>
        [Category("Lego NXT"), Description("Delay between sensor polls. 0 = as often as possible.\r\n\r\nNOTE: The AutoPoll property must be set for this property to have any effect.")]
        public int AutoPollDelay { get { return _autoPollDelay; } set { _autoPollDelay = value; } }

        /// <summary>
        /// The last read raw value.
        /// </summary>
        [Browsable(false)]
        public virtual int RawValue { get { return LastResult.RawAd; } }
        /// <summary>
        /// The last input values result.
        /// </summary>
        [Browsable(false)]
        public NxtGetInputValues LastResult { get; private set; }
        /// <summary>
        /// The last time the sensor was polled (using Haden.Utilities.MilliSeconds()).
        /// </summary>
        [Browsable(false)]
        public long LastPollTimestamp { get; private set; }
        /// <summary>
        /// Polls the sensor value.
        /// </summary>
        public virtual void Poll()
        {
            if (Brick != null)
            {
                LastPollTimestamp = Functions.MilliSeconds();
                var previous = LastResult;
                LastResult = Brick.Communicator.GetInputValues(Port);
                OnPolled();
                if (IsSensorReadingDifferent(previous, LastResult))
                {
                    OnValueChanged();
                }
            }
        }
        /// <summary>
        /// Initializes the sensor.
        /// </summary>
        public virtual void InitializeSensor()
        {
            if (Brick == null) return;
            try
            {
                Brick.Communicator.SetInputMode(Port, Type, Mode);
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.Message, Logging.LogType.Error, Logging.LogCaller.Sensor);
                throw;
            }
        }
        /// <summary>
        /// Override this to set the sensor type.
        /// </summary>
        [Browsable(false)]
        protected virtual NxtSensorType Type
        {
            get
            {
                return NxtSensorType.None;
            }
        }
        /// <summary>
        /// Override this to set the sensor mode.
        /// </summary>
        [Browsable(false)]
        protected virtual NxtSensorMode Mode { get { return NxtSensorMode.Raw; } }
        /// <summary>
        /// Override this to define the sensor threshold.
        /// </summary>
        /// <param name="previousValue">previous value</param>
        /// <param name="newValue">new value</param>
        protected virtual bool IsSensorReadingDifferent(NxtGetInputValues previousValue, NxtGetInputValues newValue)
        {
            if (previousValue.RawAd != newValue.RawAd)
            {
                return true;
            }
            if (previousValue.CalibratedValue != newValue.CalibratedValue)
            {
                return true;
            }
            if (previousValue.NormalizedAd != newValue.NormalizedAd)
            {
                return true;
            }
            if (previousValue.ScaledValue != newValue.ScaledValue)
            {
                return true;
            }
            if (previousValue.CalibratedValue != newValue.CalibratedValue)
            {
                return true;
            }
            if (previousValue.Mode != newValue.Mode)
            {
                return true;
            }
            if (previousValue.Type != newValue.Type)
            {
                return true;
            }
            return false;
        }

        #region Events
        /// <summary>
        /// Is called whenever a poll has occurred.
        /// </summary>
        public event SensorEvent Polled;
        /// <summary>
        /// Is called whenever the polled value has changed.
        /// </summary>
        public event SensorEvent ValueChanged;
        /// <summary>
        /// Raises the OnPolled event.
        /// </summary>
        protected virtual void OnPolled()
        {
            if (Polled != null)
            {
                Polled(this);
            }
        }
        /// <summary>
        /// Raises the OnValueChanged event.
        /// </summary>
        protected virtual void OnValueChanged()
        {
            if (ValueChanged != null)
            {
                ValueChanged(this);
            }
        }
        #endregion

    }
}
