using System;
using System.ComponentModel;
using Haden.NxtSharp.Brick;
using Haden.NxtSharp.Motors;

namespace Haden.NxtSharp.Controllers
{
    /// <summary>
    /// The class encapsulating the functionality of the Nxt tank drive.
    /// </summary>
	public partial class NxtTankDrive : Component
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NxtTankDrive"/> class.
        /// </summary>
		public NxtTankDrive()
        {
			InitializeComponent();
		}
        /// <summary>
        /// Initializes a new instance of the <see cref="NxtTankDrive"/> class using IContainer.
        /// </summary>
		public NxtTankDrive(IContainer container)
        {
			container.Add(this);
			InitializeComponent();
		}
        /// <summary>
        /// The Nxt brick this sensor is connected to.
        /// </summary>
        [Category("Lego NXT"), Description("The brick this sensor is connected to.")]
        public NxtBrick Brick { get; set; }
        /// <summary>
        /// The first motor of the tank drive.
        /// </summary>
        [Category("Lego NXT"), Description("The first motor of the tank drive.")]
        public NxtMotor Motor1 { get; set; }
        /// <summary>
        /// The second motor of the tank drive.
        /// </summary>
        [Category("Lego NXT"), Description("The second motor of the tank drive.")]
        public NxtMotor Motor2 { get; set; }
        // Commands used by this control.
		/// <summary>
		/// Moves the tank drive forward while synchronizing movement.
		/// </summary>
		/// <param name="power">The motor power between 0 and 100.</param>
        /// <param name="tachoLimit">Distance to move, use 0 for infinite.</param>
		public void MoveForward(int power, int tachoLimit)
        {
			Move(Math.Abs(power), tachoLimit);
		}
		/// <summary>
		/// Moves the tank drive back while synchronizing movement.
		/// </summary>
        /// <param name="power">The motor power between 0 and 100.</param>
        /// <param name="tachoLimit">Distance to move, use 0 for infinite.</param>
		public void MoveBack(int power, int tachoLimit)
        {
			Move(-Math.Abs(power), tachoLimit);
		}
		/// <summary>
		/// Moves the tank while synchronizing movement.
		/// </summary>
        /// <param name="power">The motor power between 0 and 100.</param>
        /// <param name="tachoLimit">Distance to move, use 0 for infinite.</param>
		public void Move(int power, int tachoLimit)
        {
			Reset(true);
			Motor1.SetOutputState(power, NxtMotorMode.Brake | NxtMotorMode.MotorOn | NxtMotorMode.Regulated, NxtMotorRegulationMode.MotorSynchronization, 0, NxtMotorRunState.Running, tachoLimit);
			Motor2.SetOutputState(power, NxtMotorMode.Brake | NxtMotorMode.MotorOn | NxtMotorMode.Regulated, NxtMotorRegulationMode.MotorSynchronization, 0, NxtMotorRunState.Running, tachoLimit);

		}
        /// <summary>
        /// Turns the robot to the left.
        /// </summary>
        /// <param name="power">The motor power between 0 and 100.</param>
        /// <param name="tachoLimit">Distance to move, use 0 for infinite.</param>
        public void TurnLeft(int power, int tachoLimit)
        {
            Turn(power, tachoLimit, -100);
        }
        /// <summary>
        /// Turns the robot to the left.
        /// </summary>
        /// <param name="power">The motor power between 0 and 100.</param>
        /// <param name="tachoLimit">Distance to move, use 0 for infinite.</param>
        /// <param name="singleMotor">Is this for a single motor?</param>
		public void TurnLeft(int power, int tachoLimit, bool singleMotor)
        {
            if (singleMotor)
            {
                Turn(power, tachoLimit, -100, true);
            }
		}
        /// <summary>
        /// Turns the robot to the right.
        /// </summary>
        /// <param name="power">The motor power between 0 and 100.</param>
        /// <param name="tachoLimit">Distance to move, use 0 for infinite.</param>
        public void TurnRight(int power, int tachoLimit)
        {
            Turn(power, tachoLimit, 100);
        }
        /// <summary>
        /// Turns the robot to the right.
        /// </summary>
        /// <param name="power">The motor power between 0 and 100.</param>
        /// <param name="tachoLimit">Distance to move, use 0 for infinite.</param>
        /// <param name="singleMotor">Is this for a single motor?</param>
		public void TurnRight(int power, int tachoLimit, bool singleMotor)
        {
            if (singleMotor)
            {
                Turn(power, tachoLimit, 100, true);
            }
		}
		/// <summary>
		/// Turns the tank in its place
		/// </summary>
        /// <param name="power">The motor power between 0 and 100.</param>
        /// <param name="tachoLimit">Distance to move, use 0 for infinite.</param>
		/// <param name="turnRate">The rate at which the turn should be made.</param>
		public void Turn(int power, int tachoLimit, int turnRate)
        {
			Reset(true);
			Motor1.SetOutputState(power, NxtMotorMode.Brake | NxtMotorMode.MotorOn | NxtMotorMode.Regulated, NxtMotorRegulationMode.MotorSynchronization, turnRate, NxtMotorRunState.Running, tachoLimit);
			Motor2.SetOutputState(power, NxtMotorMode.Brake | NxtMotorMode.MotorOn | NxtMotorMode.Regulated, NxtMotorRegulationMode.MotorSynchronization, turnRate, NxtMotorRunState.Running, tachoLimit);
		}
        /// <summary>
        /// Turns a single motor like a tank drive.
        /// </summary>
        /// <param name="power">The power.</param>
        /// <param name="tachoLimit">The tacho limit.</param>
        /// <param name="turnRate">The turn rate.</param>
        /// <param name="singleMotor">if set to <c>true</c> [single motor].</param>
        public void Turn(int power, int tachoLimit, int turnRate, bool singleMotor)
        {
            if (singleMotor)
            {
                Reset(true);
                Motor1.SetOutputState(power, NxtMotorMode.Brake | NxtMotorMode.MotorOn | NxtMotorMode.Regulated, NxtMotorRegulationMode.MotorSynchronization, turnRate, NxtMotorRunState.Running, tachoLimit);
            }
            
        }
		/// <summary>
		/// Puts the drive into coast mode.
		/// </summary>
		public void Coast()
        {
			Motor1.Coast();
			Motor2.Coast();
		}
		/// <summary>
		/// Puts the drive in brake mode.
		/// </summary>
		public void Brake()
        {
			Motor1.Brake();
			Motor2.Brake();
		}
		/// <summary>
		/// Reset the motor position counters.
		/// </summary>
		public void Reset(bool relative)
        {
		    if (Motor2 != null)
		    {
		        Motor1.ResetPosition(relative);
		        Motor2.ResetPosition(relative);
		    }
            Motor1.ResetPosition(relative);
        }

	}
}
