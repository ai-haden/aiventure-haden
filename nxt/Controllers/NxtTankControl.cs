using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Haden.NxtSharp.Controllers
{
    /// <summary>
    /// The class encapsulating the controls available to the motor array.
    /// </summary>
	public partial class NxtTankControl : UserControl
    {
        int _power = 75;
        /// <summary>
        /// Initializes a new instance of the <see cref="NxtTankControl"/> class.
        /// </summary>
		public NxtTankControl()
        {
			InitializeComponent();
		}
        /// <summary>
        /// Should the motor put in brake mode when stopped?
        /// </summary>
        [Category("MotorControl"), Description("Should the motor put in brake mode when stopped?")]
        public bool Brake { get; set; }
        /// <summary>
		/// Power of the motor between 0 and 100.
		/// </summary>
        [Category("MotorControl"), Description("Power of the motor between 0 and 100.")]
        public int Power
        {
            get
            {
                return _power;
            }
            set
            {
                if (value < 0 || value > 100)
                {
                    throw new ArgumentException("Power must be between 0 and 100.");
                }
                _power = value;
            }
        }
        /// <summary>
        /// The tank drive this control is assigned to
        /// </summary>
        [Category("MotorControl"), Description("The tank drive this control is assigned to.")]
        public NxtTankDrive TankDrive { get; set; }
        /// <summary>
        /// Stops the motor.
        /// </summary>
        private void Stop()
        {
            if (TankDrive != null)
            {
                if (Brake)
                {
                    TankDrive.Brake();
                }
            }
            else
            {
                if (TankDrive != null) TankDrive.Coast();
            }
        }

		#region Event handlers
        /// <summary>
        /// Handles the MouseDown event of the buttonForward control.
        /// </summary>
		private void buttonForward_MouseDown(object sender, MouseEventArgs e)
        {
            if(TankDrive == null)
            {
				throw new InvalidOperationException("Can't drive forward - no NxtTankDrive assigned to this control.");
			}
            TankDrive.MoveForward(Power, 0);
        }
        /// <summary>
        /// Handles the MouseUp event of the buttonForward control.
        /// </summary>
		private void buttonForward_MouseUp(object sender, MouseEventArgs e)
        {
			Stop();
		}
        /// <summary>
        /// Handles the MouseDown event of the buttonBack control.
        /// </summary>
		private void buttonBack_MouseDown(object sender, MouseEventArgs e)
        {
            if(TankDrive == null)
            {
				throw new InvalidOperationException("Can't drive back - no NxtTankDrive assigned to this control.");
			}
            TankDrive.MoveBack(Power, 0);
        }
        /// <summary>
        /// Handles the MouseUp event of the buttonBack control.
        /// </summary>
		private void buttonBack_MouseUp(object sender, MouseEventArgs e)
        {
			Stop();
		}
        /// <summary>
        /// Handles the MouseDown event of the buttonTurnLeft control.
        /// </summary>
		private void buttonTurnLeft_MouseDown(object sender, MouseEventArgs e)
        {
            if(TankDrive == null)
            {
				throw new InvalidOperationException("Can't turn left - no NxtTankDrive assigned to this control.");
			}
            TankDrive.TurnLeft(Power, 0);
        }
        /// <summary>
        /// Handles the MouseUp event of the buttonTurnLeft control.
        /// </summary>
		private void buttonTurnLeft_MouseUp(object sender, MouseEventArgs e)
        {
			Stop();
		}
        /// <summary>
        /// Handles the MouseDown event of the buttonTurnRight control.
        /// </summary>
		private void buttonTurnRight_MouseDown(object sender, MouseEventArgs e)
        {
            if(TankDrive == null)
            {
				throw new InvalidOperationException("Can't turn right - no NxtTankDrive assigned to this control.");
			}
            TankDrive.TurnRight(Power, 0);
        }
        /// <summary>
        /// Handles the MouseUp event of the buttonTurnRight control.
        /// </summary>
		private void buttonTurnRight_MouseUp(object sender, MouseEventArgs e)
        {
			Stop();
		}
		#endregion

	}
}
