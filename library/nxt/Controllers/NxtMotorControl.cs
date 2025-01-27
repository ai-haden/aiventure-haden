using System;
using System.ComponentModel;
using System.Drawing;
using Haden.NxtSharp.Motors;

namespace Haden.NxtSharp.Controllers
{
    /// <summary>
    /// The class containing the NxtMotorControl logic.
    /// </summary>
	public class NxtMotorControl
    {
        private int _power = 75;
        private int _buttonDistance = 4;
        private NxtMotorControlOrientation _orientation = NxtMotorControlOrientation.Vertical;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is running.
        /// </summary>
        public bool IsRunning { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is connected to the robot.
        /// </summary>
        public bool IsConnected { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="NxtMotorControl"/> class.
        /// </summary>
		public NxtMotorControl()
        {
			
		}
        /// <summary>
        /// Should the motor put in brake mode when stopped?
        /// </summary>
        public bool Brake { get; set; }
        /// <summary>
        /// Power of the motor between 0 and 100.
		/// </summary>
		public int Power {
			get {
				return _power;
			}
			set
			{
			    if(value < 0 || value > 100) {
					throw new ArgumentException("Power must be between 0 and 100.");
				}
			    _power = value;
			}
		}
        /// <summary>
        /// The motor this control is assigned to.
        /// </summary>
        public NxtMotor Motor { get; set; }
        /// <summary>
		/// Orientation of the buttons.
		/// 
		/// Vertical: Up/Down.
		/// Horizontal: Left/Right.
		/// </summary>
		public NxtMotorControlOrientation Orientation {
			get {
				return _orientation;
			}
			set {
				_orientation = value;
			}
		}
		/// <summary>
		/// The distance between the buttons.
		/// </summary>
		public int ButtonDistance {
			get {
				return _buttonDistance;
			}
			set {
				_buttonDistance = value;
			}
		}

        #region Motor operations
        /// <summary>
        /// Turns the motor clockwise.
        /// </summary>
        public void TurnClockwise()
        {
			if(Motor == null)
            {
				//MessageBox.Show("Can't turn! There is no motor connected to this control.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
            else
            {
				if(!IsRunning)
                {
					Motor.Turn(-Power, 0);
					IsRunning = true;
				}
			}
		}
        /// <summary>
        /// Turns the motor clockwise.
        /// </summary>
        /// <param name="power">The motor power between 0 and 100..</param>
        /// <param name="degrees">Angle amount to turn, use 0 for infinite.</param>
        public void TurnClockwise(int power, int degrees)
        {
            if (Motor == null)
            {
                //MessageBox.Show("Can't turn! There is no motor connected to this control.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (!IsRunning)
                {
                    Motor.Turn(-power, degrees);
                    IsRunning = true;
                }
            }
        }
        /// <summary>
        /// Turns the motor counterclockwise.
        /// </summary>
		public void TurnCounterClockwise()
        {
			if(Motor == null)
            {
				//throw; //Console.WriteLine("Can't turn! There is no motor connected to this control.");
			}
            else
            {
				if(!IsRunning)
                {
					Motor.Turn(Power, 0);
					IsRunning = true;
				}
			}
		}
        /// <summary>
        /// Turns the motor counterclockwise.
        /// </summary>
        /// <param name="power">The motor power between 0 and 100.</param>
        /// <param name="degrees">Angle amount to turn, use 0 for infinite.</param>
        public void TurnCounterClockwise(int power, int degrees)
        {
            if (Motor == null)
            {
                //MessageBox.Show("Can't turn! There is no motor connected to this control.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (!IsRunning)
                {
                    Motor.Turn(power, degrees);
                    IsRunning = true;
                }
            }
        }
        /// <summary>
        /// Stops the motor.
        /// </summary>
		public void Stop()
        {
			if(Motor != null && IsRunning)
            {
				if(Brake)
                {
					Motor.Brake();
				}
                else
                {
					Motor.Coast();
				}
				IsRunning = false;
			}
		}
		#endregion

    }
}
