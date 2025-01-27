using System;
using System.ComponentModel;
using Haden.NxtSharp.Brick;

namespace Haden.NxtSharp.Motors
{
    /// <summary>
    /// The NXT motor componet.
    /// </summary>
	public class NxtMotor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NxtMotor"/> class.
        /// </summary>
		public NxtMotor()
        {
			
		}
        /// <summary>
        /// The brick this motor is connected to.
        /// </summary>
        public NxtBrick Brick { get; set; }
        /// <summary>
        /// The port this motor is connected to.
        /// </summary>
		public NxtMotorPort Port
        {
			get {
				if(Brick != null) {
					foreach(NxtMotorPort p in Brick.MotorPorts()) {
						if(Brick.GetMotor(p) == this) {
							return p;
						}
					}
				}
				return NxtMotorPort.None;
			}
			set {
				if(Brick != null) {
					Brick.AttachMotor(value, this);
				}
			}
		}
        /// <summary>
        /// Flip the motor direction.
        /// </summary>
        /// <value>
        ///   <c>true</c> if flip; otherwise, <c>false</c>.
        /// </value>
        public bool Flip { get; set; }

        /// <summary>
        /// Gets the flip factor (either 1 or -1).
        /// </summary>
		public int FlipFactor {
			get {
				return Flip ? -1 : 1;
			}
		}
		/// <summary>
		/// Turns the motor.
		/// </summary>
		/// <param name="speed">Speed [-100,100], positive values being clockwise, negative values counterclockwise</param>
		/// <param name="degrees">+ number of degrees to turn, 0 being infinite. 
		/// NOTE: After the command is finished, the motor will coast about 10-30 degrees so don't rely on this command for precise positioning.
		/// </param>
		public void Turn(int speed, int degrees)
		{
		    if(Brick == null)
            {
				throw new InvalidOperationException("This motor must be connected to a brick first.");
			}
		    SetOutputState(speed, NxtMotorMode.MotorOn | NxtMotorMode.Regulated, NxtMotorRegulationMode.MotorSpeed, 0, NxtMotorRunState.Running, degrees);
		}
        /// <summary>
		/// Puts the motor into coast mode.
		/// </summary>
		public void Coast()
        {
            if(Brick == null)
            {
				throw new InvalidOperationException("This motor must be connected to a brick first.");
			}
            SetOutputState(
                0,
                NxtMotorMode.None,
                NxtMotorRegulationMode.Idle,
                0,
                NxtMotorRunState.Idle,
                0
                );
        }
        /// <summary>
		/// Puts the motor into brake mode (the motor will move to retain current position).
		/// </summary>
		public void Brake()
        {
            if(Brick == null)
            {
				throw new InvalidOperationException("This motor must be connected to a brick first.");
			}
            SetOutputState(
                0,
                NxtMotorMode.MotorOn | NxtMotorMode.Brake | NxtMotorMode.Regulated,
                NxtMotorRegulationMode.MotorSpeed,
                0,
                NxtMotorRunState.Running,
                0
                );
        }
        /// <summary>
		/// Resets the motor position.
		/// </summary>
		public void ResetPosition(bool relative)
        {
            if(Brick == null) {
				throw new InvalidOperationException("This motor must be connected to a brick first.");
			}
            Brick.Communicator.ResetMotorPosition(Port, relative);
        }
        /// <summary>
		/// Sets the output state of this motor.
		/// </summary>
		public void SetOutputState(int power, NxtMotorMode mode, NxtMotorRegulationMode regulationMode, int turnRate, NxtMotorRunState runState, int tachoLimit)
        {
            if (Brick.Communicator != null)
            {
                Brick.Communicator.SetOutputState(
                    Port,
                    (sbyte)(power * FlipFactor),
                    mode,
                    regulationMode,
                    (sbyte)turnRate,
                    runState,
                    (uint)tachoLimit
                );
            }
            else
            {
                throw new InvalidOperationException("This must be connected to a brick first. Please check that a Bluetooth connection has been made.");
            }
		}
	}
}
