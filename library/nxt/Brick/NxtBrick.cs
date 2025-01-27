using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using Haden.NxtSharp.Motors;
using Haden.NxtSharp.Sensors;
using Haden.NxtSharp.Utilties;

namespace Haden.NxtSharp.Brick
{
	/// <inheritdoc />
    /// <summary>
    /// This class encapsulates core lego NXT functionality.
    /// </summary>
	public class NxtBrick
    {
		private System.Timers.Timer keepAliveTimer;
        delegate void NxtMotorEvent(NxtMotor motor);
        private Thread _pollThread;
        private List<NxtSensor> _pollList;
        private NxtMotor _motorA;
        private NxtMotor _motorB;
        private NxtMotor _motorC;
        private NxtSensor _sensor1;
        private NxtSensor _sensor2;
        private NxtSensor _sensor3;
        private NxtSensor _sensor4;
        /// <summary>
        /// Is the communicator object connected to the brick?
        /// </summary>
        [Browsable(false)]
        public bool IsConnected { get; private set; }
        /// <summary>
        /// The communicator property.
        /// </summary>
        [Browsable(false)]
        public BrickCommunicator Communicator { get; private set; }
		/// <summary>
		/// Initializes a new instance of the <see cref="T:Haden.NxtSharp.Brick.NxtBrick" /> class.
		/// </summary>
		public NxtBrick(string comPortName = "COM40")
		{
			ComPortName = comPortName;
			keepAliveTimer = new System.Timers.Timer(1000);
			keepAliveTimer.Elapsed += KeepAliveTimerTick;
		}

		#region Connection
	    /// <summary>
		/// Connects to the Lego NXT Brick.
		/// </summary>
		public void Connect()
        {
            OpenSerialPorts();
            InitializeSettings();
		    Communicator.KeepAlive();

			OnConnected(this);
			if(AutoPoll) {
				StartPolling();
			}

			keepAliveTimer.Enabled = true;
		}
		/// <summary>
		/// Disconnects the Lego NXT Brick.
		/// </summary>
		public void Disconnect()
        {
			keepAliveTimer.Enabled = false;

			if(AutoPoll)
            {
				StopPolling();
			}

			CloseSerialPorts();
			OnDisconnected(this);
		}
		#endregion 

		#region Serial Communication
		/// <summary>
		/// Sets the COM port to use for communication.
		/// </summary>
		[Category("Lego NXT"), Description("Sets the COM port to use for communication. Look for outgoing 'Dev B' ports in your Bluetooth manager to see which virtual COM Port is assigned to your NXT.")]
		public string ComPortName { get; set; }
	    
	    /// <summary>
        /// Opens the serial ports.
        /// </summary>
        private void OpenSerialPorts()
        {
            Communicator = new BrickCommunicator(ComPortName);
            Communicator.Connect();
        }
        /// <summary>
        /// Closes the serial ports.
        /// </summary>
		private void CloseSerialPorts()
        {
            if(Communicator != null)
                Communicator.Disconnect();
		}

		#endregion

		#region Device settings and initialization
        /// <summary>
        /// Initializes the settings of the NXT brick.
        /// </summary>
        private void InitializeSettings()
        {
            foreach (var sensor in ConnectedSensors())
            {
                try
                {
                    sensor.InitializeSensor();
                }
                catch (Exception ex)
                {
                    Logging.WriteLog(ex.Message, Logging.LogType.Error, Logging.LogCaller.Brick);
                    throw;
                }
                
            }
        }

		#endregion

		#region NXT Ports

		#region Motor ports
		/// <summary>
		/// The motor that is connected to Port A.
		/// </summary>
		[Category("Lego NXT"), Description("The motor that is connected to Port A.")]
		public NxtMotor MotorA
        {
			get {
				return _motorA;
			}
			set {
				AttachMotor(NxtMotorPort.PortA, value);
			}
		}
		/// <summary>
		/// The motor that is connected to Port B.
		/// </summary>
		[Category("Lego NXT"), Description("The motor that is connected to Port B.")]
		public NxtMotor MotorB
        {
			get {
				return _motorB;
			}
			set {
				AttachMotor(NxtMotorPort.PortB, value);
			}
		}
		/// <summary>
		/// The motor that is connected to Port C.
		/// </summary>
		[Category("Lego NXT"), Description("The motor that is connected to Port C.")]
		public NxtMotor MotorC
        {
			get {
				return _motorC;
			}
			set {
				AttachMotor(NxtMotorPort.PortC, value);
			}
		}
        /// <summary>
        /// Attaches the specified motor to the specified port.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="motor">The motor.</param>
		public void AttachMotor(NxtMotorPort port, NxtMotor motor)
        {
			foreach(var p in MotorPorts())
            {
				if(GetMotor(p) == motor)
                {
					SetMotor(p, null);
				}
			}
			if(port != NxtMotorPort.None)
            {
				SetMotor(port, motor);
			}
		}
        /// <summary>
        /// Returns the motor connected to the specified port.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <returns></returns>
		public NxtMotor GetMotor(NxtMotorPort port)
        {
			if(port == NxtMotorPort.PortA)
            {
				return MotorA;
			}
			if(port == NxtMotorPort.PortB)
            {
				return MotorB;
			}
			if(port == NxtMotorPort.PortC)
            {
				return MotorC;
			}
			return null;
		}
        /// <summary>
        /// Sets the port for the specified motor without any checks to see if two motors are attached to the same port.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="motor">The motor.</param>
		private void SetMotor(NxtMotorPort port, NxtMotor motor)
        {
			if(port == NxtMotorPort.PortA)
            {
				_motorA = motor;
			}
			if(port == NxtMotorPort.PortB)
            {
				_motorB = motor;
			}
			if(port == NxtMotorPort.PortC)
            {
				_motorC = motor;
			}
		}
        /// <summary>
        /// Enumerates all motor ports connected to the NXT brick.
        /// </summary>
		public IEnumerable<NxtMotorPort> MotorPorts()
        {
			yield return NxtMotorPort.PortA;
			yield return NxtMotorPort.PortB;
			yield return NxtMotorPort.PortC;
		}
		/// <summary>
		/// Enumerates all motors connected to the NXT brick.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<NxtMotor> ConnectedMotors()
        {
			if(MotorA != null)
            {
				yield return MotorA;
			}
			if(MotorB != null)
            {
				yield return MotorB;
			}
			if(MotorC != null)
            {
				yield return MotorC;
			}
		}

		#endregion

		#region Sensor Ports
		/// <summary>
		/// The sensor that is connected to Port 1 (usually the pressure sensor).
		/// </summary>
		[Category("Lego NXT"), Description("The sensor that is connected to Port 1 (usually the pressure sensor).")]
		public NxtSensor Sensor1
        {
			get {
				return _sensor1;
			}
			set {
				AttachSensor(NxtSensorPort.Port1, value);
			}
		}
		/// <summary>
		/// The sensor that is connected to Port 2 (usually the sound sensor).
		/// </summary>
		[Category("Lego NXT"), Description("The sensor that is connected to Port 2 (usually the sound sensor).")]
		public NxtSensor Sensor2
        {
			get {
				return _sensor2;
			}
			set {
				AttachSensor(NxtSensorPort.Port2, value);
			}
		}
		/// <summary>
		/// The sensor that is connected to Port 3 (usually the light sensor).
		/// </summary>
		[Category("Lego NXT"), Description("The sensor that is connected to Port 3 (usually the light sensor).")]
		public NxtSensor Sensor3
        {
			get {
				return _sensor3;
			}
			set {
				AttachSensor(NxtSensorPort.Port3, value);
			}
		}
		/// <summary>
		/// The sensor that is connected to Port 4 (usually the sonar sensor).
		/// </summary>
		[Category("Lego NXT"), Description("The sensor that is connected to Port 4 (usually the sonar sensor).")]
		public NxtSensor Sensor4
        {
			get {
				return _sensor4;
			}
			set {
				AttachSensor(NxtSensorPort.Port4, value);
			}
		}
        /// <summary>
        /// Attaches the specified sensor to the specified port.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="sensor">The sensor.</param>
		public void AttachSensor(NxtSensorPort port, NxtSensor sensor)
        {
			foreach(NxtSensorPort p in SensorPorts())
            {
				if(GetSensor(p) == sensor)
                {
					SetSensor(p, null);
				}
			}
			if(port != NxtSensorPort.None)
            {
				SetSensor(port, sensor);
			}
		}
        /// <summary>
        /// Returns the sensor connected to the specified port.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <returns></returns>
		public NxtSensor GetSensor(NxtSensorPort port)
        {
			if(port == NxtSensorPort.Port1)
            {
				return Sensor1;
			}
			if(port == NxtSensorPort.Port2)
            {
				return Sensor2;
			}
			if(port == NxtSensorPort.Port3)
            {
				return Sensor3;
			}
			if(port == NxtSensorPort.Port4)
            {
				return Sensor4;
			}
			return null;
		}
        /// <summary>
        /// Sets the port for the specified sensor without any checks to see if two sensors are attached to the same port.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="sensor">The sensor.</param>
		private void SetSensor(NxtSensorPort port, NxtSensor sensor)
        {
			if(port == NxtSensorPort.Port1)
            {
				_sensor1 = sensor;
			}
			if(port == NxtSensorPort.Port2)
            {
				_sensor2 = sensor;
			}
			if(port == NxtSensorPort.Port3)
            {
				_sensor3 = sensor;
			}
			if(port == NxtSensorPort.Port4)
            {
				_sensor4 = sensor;
			}
		}
		/// <summary>
		/// Enumerates all sensor ports on the NXT brick.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<NxtSensorPort> SensorPorts()
        {
			yield return NxtSensorPort.Port1;
			yield return NxtSensorPort.Port2;
			yield return NxtSensorPort.Port3;
			yield return NxtSensorPort.Port4;
		}
		/// <summary>
		/// Enumerates all sensors connected to the NXT brick.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<NxtSensor> ConnectedSensors()
        {
			if(Sensor1 != null)
            {
				yield return Sensor1;
			}
			if(Sensor2 != null)
            {
				yield return Sensor2;
			}
			if(Sensor3 != null)
            {
				yield return Sensor3;
			}
			if(Sensor4 != null)
            {
				yield return Sensor4;
			}
		}

		#endregion

		#endregion

		#region Sensor Polling
	    /// <summary>
	    /// When AutoPoll is set, the NxtBrick will continuously poll the state of all sensors which are connected to the NXT brick and have the AutoPoll property set.
	    /// </summary>
	    [Category("Lego NXT"), Description("When AutoPoll is set, the NxtBrick will continuously poll the state of all sensors which are connected to the Brick and have the AutoPoll property set.")]
	    public bool AutoPoll { get; set; }
	    /// <summary>
		/// Starts polling the sensors continuously.
		/// </summary>
		public void StartPolling()
        {
			lock(this)
            {
                if (_pollThread == null)
                {
                    _pollList = new List<NxtSensor>();

                    foreach (var sensor in ConnectedSensors())
                    {
                        if (sensor.AutoPoll)
                        {
                            _pollList.Add(sensor);
                        }
                    }
                    if (_pollList.Count > 0)
                    {
                        _pollThread = new Thread(PollLoop) {Name = "SensorPoller", IsBackground = true};
                        _pollThread.Start();
                    }
                }
			}
		}
		/// <summary>
		/// Terminates the thread poll queue.
		/// </summary>
		public void StopPolling()
        {
			lock(this)
            {
				if(_pollThread != null)
                {
					_pollThread.Abort();
					// Wait until the polling thread is stopped.
					while(_pollThread.IsAlive)
                    {
						Thread.Sleep(100);
					}
                    _pollThread = null;
				}
			}
		}
		/// <summary>
		/// Continuously polls sensors.
		/// </summary>
		private void PollLoop()
        {
			try
            {
				while(true)
                {
					foreach(NxtSensor sensor in _pollList)
                    {
						if(sensor.LastPollTimestamp + sensor.AutoPollDelay <= Functions.MilliSeconds())
                        {
							sensor.Poll();
							Thread.Sleep(0);
						}
					}
					Thread.Sleep(0); // Allow context switch.
				}
			}
			catch(ThreadAbortException) { }
		}

		#endregion
        #region Events
        /// <summary>
        /// Occurs when connected.
        /// </summary>
        public event EventHandler Connected;
        /// <summary>
        /// Occurs when disconnected from the Nxt Brick.
        /// </summary>
        public event EventHandler Disconnected;
        /// <summary>
        /// Raises the OnConnected event.
        /// </summary>
        protected virtual void OnConnected(NxtBrick sender)
        {
            IsConnected = true;
            if (Connected != null)
            {
                Connected(sender, new EventArgs());
            }
        }
        /// <summary>
        /// Raises the OnDisconnected event.
        /// </summary>
        protected virtual void OnDisconnected(NxtBrick sender)
        {
            IsConnected = false;
            if (Disconnected != null)
            {
                Disconnected(sender, new EventArgs());
            }
        }
        /// <summary>
        /// Keep alive timer.
        /// </summary>
        private void KeepAliveTimerTick(object sender, EventArgs e)
        {
            if (IsConnected)
                Communicator.KeepAlive();
        }
        #endregion
    }
}
