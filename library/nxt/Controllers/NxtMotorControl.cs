using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
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
        private Keys _key1 = Keys.None;
        private Keys _key2 = Keys.None;
        private NxtMotorControlOrientation _orientation = NxtMotorControlOrientation.Vertical;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is running.
        /// </summary>
        [Browsable(false)]
        public bool IsRunning { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is connected to the robot.
        /// </summary>
        [Browsable(false)]
        public bool IsConnected { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="NxtMotorControl"/> class.
        /// </summary>
		public NxtMotorControl()
        {
			InitializeComponent();
			UpdateView();
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
        [Category("MotorControl"), Description("The motor this control is assigned to.")]
        public NxtMotor Motor { get; set; }
        /// <summary>
		/// Orientation of the buttons.
		/// 
		/// Vertical: Up/Down.
		/// Horizontal: Left/Right.
		/// </summary>
		[Category("MotorControl"), Description("Orientation of the buttons.\r\nVertical: Up/Down.\r\nHorizontal: Left/Right.")]
		public NxtMotorControlOrientation Orientation {
			get {
				return _orientation;
			}
			set {
				_orientation = value;
				UpdateView();
			}
		}
		/// <summary>
		/// The distance between the buttons.
		/// </summary>
		[Category("MotorControl"), Description("The distance between the buttons.")]
		public int ButtonDistance {
			get {
				return _buttonDistance;
			}
			set {
				_buttonDistance = value;
				UpdateView();
			}
		}
		/// <summary>
		/// The up/left key.
		/// </summary>
		[Category("MotorControl"), Description("The up/left key.")]
		public Keys Key1 { get { return _key1; } set { _key1 = value; } }
		/// <summary>
		/// The down/right key.
		/// </summary>
		[Category("MotorControl"), Description("The down/right key.")]
		public Keys Key2 { get { return _key2; } set { _key2 = value; } }

        #region Motor operations
        /// <summary>
        /// Turns the motor clockwise.
        /// </summary>
        public void TurnClockwise()
        {
			if(Motor == null)
            {
				MessageBox.Show("Can't turn! There is no motor connected to this control.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("Can't turn! There is no motor connected to this control.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
				MessageBox.Show("Can't turn! There is no motor connected to this control.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("Can't turn! There is no motor connected to this control.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        #region Control layout
        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MarginChanged"/> event.
        /// </summary>
        protected override void OnMarginChanged(EventArgs e)
        {
            base.OnMarginChanged(e);
            UpdateView();
        }
        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.Resize"/> event.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateView();
        }
        /// <summary>
        /// Updates the view containing the buttons.
        /// </summary>
        private void UpdateView()
        {
            int hMargin = Margin.Left + Margin.Right;
            int vMargin = Margin.Top + Margin.Bottom;

            if (Orientation == NxtMotorControlOrientation.Vertical)
            {
                // Set the minimum size.
                MinimumSize = new Size(36 + hMargin, 48 + vMargin + ButtonDistance);

                // Orient the buttons.
                int width = Width - hMargin;
                int height = (Height - (vMargin + ButtonDistance)) / 2;

                buttonTurnClockwise.Width = width;
                buttonTurnClockwise.Height = height;
                buttonTurnClockwise.Left = Margin.Left;
                buttonTurnClockwise.Top = Margin.Top;
                buttonTurnClockwise.Text = "5"; // Up arrow in Marlett font.

                buttonTurnCounterClockwise.Width = width;
                buttonTurnCounterClockwise.Height = height;
                buttonTurnCounterClockwise.Left = Margin.Left;
                buttonTurnCounterClockwise.Top = Height - (Margin.Bottom + height);
                buttonTurnCounterClockwise.Text = "6"; // Down arrow in Marlett font.				
            }
            else
            {
                // Set the minimum size.
                MinimumSize = new Size(48 + hMargin + ButtonDistance, 36 + vMargin);

                int width = (Width - (hMargin + ButtonDistance)) / 2;
                int height = Height - vMargin;

                // Orient the buttons.
                buttonTurnClockwise.Width = width;
                buttonTurnClockwise.Height = height;
                buttonTurnClockwise.Left = Margin.Left;
                buttonTurnClockwise.Top = Margin.Top;
                buttonTurnClockwise.Text = "3"; // Left arrow in Marlett font.

                buttonTurnCounterClockwise.Width = width;
                buttonTurnCounterClockwise.Height = height;
                buttonTurnCounterClockwise.Left = Width - (Margin.Right + width);
                buttonTurnCounterClockwise.Top = Margin.Top;
                buttonTurnCounterClockwise.Text = "4"; // Right arrow in Marlett font.			
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Raises the <see cref="OnLayout" /> event.
        /// </summary>
        /// <param name="e">The <see cref="LayoutEventArgs"/> instance containing the event data.</param>
        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);
            Form f = FindForm();
            if (f != null)
            {
                f.KeyDown += f_KeyDown;
                f.KeyUp += f_KeyUp;
            }
        }
        /// <summary>
        /// Handles the KeyDown event of the control.
        /// </summary>
        void f_KeyDown(object sender, KeyEventArgs e)
        {
            HandleKeyDown(e.KeyCode);
        }
        /// <summary>
        /// Handles the KeyUp event of the control.
        /// </summary>
        void f_KeyUp(object sender, KeyEventArgs e)
        {
            HandleKeyUp(e.KeyCode);
        }
        /// <summary>
        /// Handles the key down event.
        /// </summary>
        private void HandleKeyDown(Keys key)
        {
            if (key == Key1 || key == Key2)
            {
                if (key == Key1)
                {
                    TurnClockwise();
                }
                else if (key == Key2)
                {
                    TurnCounterClockwise();
                }
            }
        }
        /// <summary>
        /// Handles the key up event.
        /// </summary>
        private void HandleKeyUp(Keys key)
        {
            if (key == Key1 || key == Key2)
            {
                Stop();
            }
        }
        /// <summary>
        /// Handles the MouseDown event of the buttonTurnClockwise control.
        /// </summary>
        private void buttonTurnClockwise_MouseDown(object sender, MouseEventArgs e)
        {
            if (Motor.Brick.Communicator != null)
            {
                TurnClockwise();
            }
        }
        /// <summary>
        /// Handles the MouseUp event of the buttonTurnClockwise control.
        /// </summary>
        private void buttonTurnClockwise_MouseUp(object sender, MouseEventArgs e)
        {
            if (Motor.Brick.Communicator != null)
            {
                Stop();
            }
        }
        /// <summary>
        /// Handles the MouseDown event of the buttonTurnCounterClockwise control.
        /// </summary>
        private void buttonTurnCounterClockwise_MouseDown(object sender, MouseEventArgs e)
        {
            if (Motor.Brick.Communicator != null)
            {
                TurnCounterClockwise();
            }
        }
        /// <summary>
        /// Handles the MouseUp event of the buttonTurnCounterClockwise control.
        /// </summary>
        private void buttonTurnCounterClockwise_MouseUp(object sender, MouseEventArgs e)
        {
            if (Motor.Brick.Communicator != null)
            {
                Stop();
            }
        }
        /// <summary>
        /// Handles the MouseHover event of the buttonTurnCounterClockwise control.
        /// </summary>
        private void buttonTurnCounterClockwise_MouseHover(object sender, EventArgs e)
        {
            if (Motor.Brick.Communicator != null)
            {
                Motor.Coast();
            }
        }

        #endregion
	}
}
