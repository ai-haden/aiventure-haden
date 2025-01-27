using System.Collections.Generic;
using Haden.NxtSharp.Sensors;
using Haden.Library;
using System.Xml;

namespace Haden.Runtime
{
    class Program
    {
        private System.Timers.Timer _whirlTimer;
        private Controller _controller;
        private bool WhirlActive {  get; set; } 
        private bool VoiceSpoken { get; set; }

        public Session Session { get; }

        /// <summary>
        /// The peak light sensor value, called np.
        /// </summary>
        public object BoxedPeakValue;
        /// <summary>
        /// The current light sensor value, called nc.
        /// </summary>
        public object BoxedCurrentValue;
        /// <summary>
        /// The stored value for difference between current and optimal light sensor values, called d.
        /// </summary>
        public object BoxedDifferenceValue;
        /// <summary>
        /// The stored value for the last turn. The depth of primitive memory is now n-1.
        /// </summary>
        public string RememberedLastTurn;
        /// <summary>
        /// The number of degrees to turn a motor.
        /// </summary>
        /// <value>
        /// The last turn granularity.
        /// </value>
        public int MotorTurnGranularity { get; set; }
        /// <summary>
        /// The stored value for an interation to define the number of autonomous iterations of a function before pausing.
        /// </summary>
        public int Iteration;
        /// <summary>
        /// The iteration limit.
        /// </summary>
        public int InterationLimit;
        /// <summary>
        /// The n - 1 value.
        /// </summary>
        public double Then { get; set; }
        /// <summary>
        /// The n, n + 1, n + 2, ..., np value.
        /// </summary>
        public double Now { get; set; }
        public bool GreaterNow { get; set; }
        public bool LessNow { get; set; }
        public List<double> LightValuesSeen { get; set; }
        public List<string> TurnsMade{ get; set; }
        // Velocity control variables.
        public const int PowerDrive = 30;
        public const int PowerSeek = 20;
        public const int Degrees = 30;

        static void Main(string[] args)
        {
            System.Console.WriteLine("Initializing a new haden connection...");
            BoxedPeakValue = 0;
            BoxedDifferenceValue = 0;
            BoxedCurrentValue = 0;
            RememberedLastTurn = "";
            Iteration = 0;
            InterationLimit = 10;
            MotorTurnGranularity = 30;
            disconnectBrickButton.Enabled = false;
            KeyPreview = true;
            comPortSelectionBox.Text = "COM7";
            LightValuesSeen = new List<double>();
            TurnsMade = new List<string>();
            _controller = new Controller();
            Session = new Session() { };
            sessionLabel.Text = "Session: " + Session.SessionID;
            BeginToRueTheWhirl(10000);
        }
        /// <summary>
        /// Parses an Xml document.
        /// </summary>
        protected void ParseXmlDocument(XmlDocument input)
        {
            XmlNodeList nodeList = input.SelectNodes("GRAMMAR/RULE/L/P");

            if (nodeList != null)
                foreach (XmlNode speechTexts in nodeList)
                {
                    //speechListBox.Items.Add(speechTexts.LastChild.InnerText);
                }
        }

#region The Novel Whirl
        protected void BeginToRueTheWhirl(double duration)
        {
            _whirlTimer = new System.Timers.Timer
            {
                Interval = duration // Duration of time in each state (in ms)
            };
            _whirlTimer.Elapsed += WhirlTimerElapsed;
            _whirlTimer.Start(); // Start timer.
            WhirlActive = true;
            RueTheWhirl.CurrentState = "Zero"; // Start the whirl.
            reportLabel.Text = RueTheWhirl.ActionController(); // Set the action for the whirl.
        }
        protected void WhirlTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            switch (RueTheWhirl.CurrentState)
            {
                case "Zero":
                    RueTheWhirl.CurrentState = "One";
                    reportLabel.Text = RueTheWhirl.ActionController();
                    break;
                case "One":
                    RueTheWhirl.CurrentState = "Two";
                    reportLabel.Text = RueTheWhirl.ActionController();
                    break;
                case "Two":
                    RueTheWhirl.CurrentState = "Three";
                    reportLabel.Text = RueTheWhirl.ActionController();
                    break;
                case "Three":
                    RueTheWhirl.CurrentState = "Four";
                    reportLabel.Text = RueTheWhirl.ActionController();
                    break;
                case "Four":
                    RueTheWhirl.CurrentState = "Zero";
                    reportLabel.Text = RueTheWhirl.ActionController();
                    break;
                default:
                    break;
            }

            if (RueTheWhirl.ActionController().Contains("Discover") | RueTheWhirl.ActionController() == "Discover the peak value detected at a light sensor.")
            {
                // At those points in the whirl-cycle, we need a routine that brings life to this old toy robot.
                // Based on the form of two wheel motors, one motor turning a light sensor through a range of 150-degrees, and a bump sensor.
                //
                // Tune to the changing value of light intensity.
                // Better-yet: Follow a lamp left or right, as it is the brightest source.
                SensorTemporalValueDifference();
                // Process taxonomy.
                //ProcessTaxonomy();
                // Start as a find of a peek.
                PeekValue();
                // Disturb equilibrium.
                //IncrementLeft();
                // Check the connection to the robot.
                //QueryConnection();
            }
        }

#endregion

#region Sanity Checks

        protected void CheckMotorMovements()
        {
            //motorDelegate = BeginInvoke(new MethodInvoker(() => lightSeekingMotor.TurnCounterClockwise(25, 100)));
            //motorDelegate = BeginInvoke(new MethodInvoker(() => nxtMotorControl1.TurnClockwise(PowerSeek, 400)));
            nxtBrick.MotorA.Turn(25, 100);
            Thread.Sleep(1000);
            nxtBrick.MotorA.Flip = true;
            nxtBrick.MotorA.Turn(25, 100);
            lightSeekingMotor.TurnCounterClockwise(25, 100);
            Thread.Sleep(1000);
            //nxtBrick.MotorB.Turn(75, 1800);
            //nxtBrick.MotorC.Turn(75, 1800);
        }

        protected void CheckDataSystem()
        {
            Controller.ReadData();
            _controller.StoreData("left", 3, Iteration);
            Controller.ReadData();
        }

#endregion

#region Actions
        protected void QueryConnection()
        {
            if (IsConnectedBrickAutonomous == false)
                notifyLabel.Text = "No hardware detected in autonomous mode.";
            if (IsConnectedBrickAutonomous == true)
                notifyLabel.Text = "Interaction mode -- no notifications.";
            //if (IsConnectedBrickDummy == false)
            // notifyLabel.Text = "No hardware detected in dummy mode.";
        }
        /// <summary>
        /// Peek the value of the sensor when iteration is equal to zero.
        /// </summary>
        /// <returns></returns>
        protected bool PeekValue()
        {
            // Store the trace of where this begins.
            StoreTrace("Initial", BoxedCurrentValue.ToString());
            // Set iteration to coincide with the starting temporal value of n.
            if (Iteration == 0)
            {
                StoreTrace("Iteration", Iteration.ToString()); 
                // Poll the n value.
                Now = Convert.ToDouble(BoxedCurrentValue);
                StoreTrace("Now", Now.ToString());
                StoreTrace("Then", Then.ToString());
                // The n - 1 value.
                if (peakLightSensorValue.Text == "np")
                {
                    // Purposefully distrub this first equilibrium to set the system into motion.
                    Then = 1;
                    // Update the form.
                    BeginInvoke(new MethodInvoker(() => peakLightSensorValue.Text = Then.ToString())); // np
                    Compare();
                }
                
                // Increment the iteration value (for data-parity with count).
                Iteration++;
                StoreTrace("Then set equal to now", " x");
                return true;
            }
            // Increment the iteration value (for data-parity with count).
            Iteration++;
            StoreTrace("Iteration", Iteration.ToString());
            // Poll the n + 1 value.
            Now = Convert.ToDouble(BoxedCurrentValue);
            // Poll the n value.
            Then = Convert.ToDouble(BoxedPeakValue);
            // Report to trace.
            StoreTrace("Now", Now.ToString());
            StoreTrace("Then", Then.ToString());
            // GO!
            // What is the theory again? // U can code it here...

            // Check if a value of n + 1 can be found increasing in the direction the (brighter) light was discovered.
            // Most primitive method without knowing. Not storing the value correctly!
            if (Then > Now)
            {
                IncrementLeft(MotorTurnGranularity);
                StoreTrace("last turn", RememberedLastTurn);
            }
            if (Then < Now)
            {
                IncrementRight(MotorTurnGranularity);
                StoreTrace("last turn", RememberedLastTurn);
            } 
            if (Then == Now)
                StoreTrace("Stable", Then.ToString());

            // Run a comparison and report.
            Compare();
            
            // Increment the iteration value (for data-parity with count).
            Iteration++;
            // Poll the n + 1 value. Add it to the list.
            Now = Convert.ToDouble(BoxedCurrentValue);
            //LightValuesSeen.Add(Now);
            StoreTrace("Now", Now.ToString());
            StoreTrace("Then", Then.ToString());
            StoreTrace("Iteration", Iteration.ToString());
            // Run a comparison.
            Compare();

            return true;
        }
        /// <summary>
        /// Compares temporal quantities and sets the properties for booleans GreaterNow and LessNow.
        /// </summary>
        void Compare()
        {
            StoreTrace("Process", "Compare if 'n + 1' is greater or less than 'n'");
            if (Then.IsGreaterThan(Now))
            {
                GreaterNow = true;
                StoreTrace("Greater now?", GreaterNow.ToString());
                //Act("aposteriori");
                BeginInvoke(new MethodInvoker(() => peakLightSensorValue.Text = Then.ToString()));
            }
            else if (Then.IsLessThan(Now)) 
            { 
                LessNow = true;
                StoreTrace("Less now?", LessNow.ToString());
                //Act("apriori");
                BeginInvoke(new MethodInvoker(() => peakLightSensorValue.Text = Now.ToString()));
            }
            // Compute and display the difference.
            var diff = Math.Abs(Then - Now);
            BeginInvoke(new MethodInvoker(() => differenceLightSensorValue.Text = diff.ToString())); // d
            BeginInvoke(new MethodInvoker(() => Refresh()));

            // d
        }
        void Act(string action)
        {
            StoreTrace("Action", "Set for subsequent following Compare");
            if (action == "apriori")
            {
                StoreTrace("Action", "An apriori statement discovered.");
                // U can code it here...
                if (RememberedLastTurn == "left")
                {
                    IncrementLeft(30);
                    // What was the algorithm again? You're looking at it, buddy.
                }
            }
            if (action == "aposteriori")
            {
                StoreTrace("Action", "An aposteriori statement discovered.");
                // U can code it here...
            }
            else
                StoreTrace("Action", "Meaningless statement applied.");

        }

        protected void Sample()
        {
            // The n + x value. Add it to the list.
            Now = Convert.ToDouble(BoxedCurrentValue);
            LightValuesSeen.Add(Now);
            Iteration++;
            Compare();
        }

        /// <summary>
        /// On retrospection: How effective was the increment-turn to finding the peak light source direction?
        /// </summary>
        /// <returns></returns>
        protected bool DeterminePeakValue()
        {
            // Increment to seek of n.
            if (Iteration == 0)
            {
                PeekValue();
            }
            // Compute from the values reported on the form.

            // Increment to seek of n + 1.
            if (Iteration != 1)
            {
                IncrementLeft(MotorTurnGranularity);
            }
            // Is the n + 1 value greater to less than the than the n - 1 value?
            GreaterNow = Then.IsGreaterThan(Now);
            LessNow = Then.IsLessThan(Now);

            if (GreaterNow)
            {
                // Check in the same direction, since it was a positive result.
                Iteration++;
                IncrementLeft(MotorTurnGranularity);
                // Check now.
                // The n + 2 value. Add it to the list.
                Now = Convert.ToDouble(BoxedCurrentValue);
                LightValuesSeen.Add(Now);
                GreaterNow = Then.IsGreaterThan(Now);
                LessNow = Then.IsLessThan(Now);
                // Is it still increasing? Increment again.
                if (GreaterNow)
                {
                    Iteration++;
                    IncrementLeft(MotorTurnGranularity);
                    Recompute(true);
                }
                if (LessNow)
                {
                    Iteration++;
                    IncrementRight(MotorTurnGranularity);
                    Recompute(true);
                }
            }
            if (LessNow)
            {
                Iteration++;
                IncrementRight(MotorTurnGranularity);
                Recompute(true);
            }
            if (GreaterNow == false & LessNow == false)
            {
                Recompute(true);
            }
            return true;
        }
        protected bool Recompute(bool finalCheck)
        {
            if (finalCheck)
            {
                // The n + 3...np value. Add it to the list.
                Now = Convert.ToDouble(BoxedCurrentValue);
                LightValuesSeen.Add(Now);
                // Check now.
                GreaterNow = Then.IsGreaterThan(Now);
                LessNow = Then.IsLessThan(Now);
                if (GreaterNow == false && LessNow == false)
                {
                    if (Then.Equals(Now))
                    {
                        // You are finished.
                        notifyLabel.Text = "Peak has been determined and sensor points toward the lightsource.";
                        // But maybe this is not true. Let's disturb the equilibirum point just discovered.
                        IncrementLeft(MotorTurnGranularity);
                    }
                }
                else
                {
                    // Re-enter the loop.
                    DeterminePeakValue();
                }
                return true;
            }
            return false;
        }

        protected void ProcessTaxonomy()
        {
            if (SensorTemporalValueDifference() > 0)
            {
                BoxedPeakValue = BoxedCurrentValue; // nc
                BeginInvoke(new MethodInvoker(() => peakLightSensorValue.Text = BoxedPeakValue.ToString())); // np
                BoxedDifferenceValue = SensorTemporalValueDifference().ToString(CultureInfo.InvariantCulture);
                BeginInvoke(new MethodInvoker(() => differenceLightSensorValue.Text = BoxedDifferenceValue.ToString())); // d
                // Turn in the same direction.
                if (RememberedLastTurn.ToString() == "left")
                {
                    IncrementLeft(MotorTurnGranularity);
                }
                if (RememberedLastTurn.ToString() == "right")
                {
                    IncrementRight(MotorTurnGranularity);
                }
            }
            else if (SensorTemporalValueDifference() < 0)
            {
                BeginInvoke(new MethodInvoker(() => differenceLightSensorValue.Text = SensorTemporalValueDifference().ToString(CultureInfo.InvariantCulture)));
                // Turn in the opposite direction.
                if (RememberedLastTurn.ToString() == "left")
                {
                    IncrementRight(MotorTurnGranularity);
                }
                if (RememberedLastTurn.ToString() == "right")
                {
                    IncrementLeft(MotorTurnGranularity);
                }
            }
            else if (SensorTemporalValueDifference() == 0)
            {
                BeginInvoke(new MethodInvoker(() => differenceLightSensorValue.Text = SensorTemporalValueDifference().ToString(CultureInfo.InvariantCulture)));
                
                // Display a successful end to the discovery routine. Q: What is the response gesture?
            }
        }

#endregion

#region Autonomy test routines

        protected int SensorTemporalValueDifference()
        {
            BoxedDifferenceValue = (int)BoxedCurrentValue - (int)BoxedPeakValue;
            Compare();
            return (int)BoxedDifferenceValue;
        }
        /// <summary>
        /// Increments the motor to the left.
        /// </summary>
        protected void IncrementLeft(int degrees)
        {
            try
            {
                nxtBrick.MotorA.Flip = false;
                nxtBrick.MotorA.Turn(25, degrees);
                RememberedLastTurn = "left";
                TurnsMade.Add(RememberedLastTurn);
                _controller.StoreData(RememberedLastTurn, TurnsMade.Count, Iteration);
                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.Message, Logging.LogType.Error, Logging.LogCaller.Motor);
            }
        }
        /// <summary>
        /// Increments the motor to the right.
        /// </summary>
        protected void IncrementRight(int degrees)
        {
            try
            {
                //nxtBrick.MotorA.Flip = true;
                nxtBrick.MotorA.Turn(25, degrees);
                RememberedLastTurn = "right";
                TurnsMade.Add(RememberedLastTurn);
                _controller.StoreData(RememberedLastTurn, TurnsMade.Count, Iteration);
                Thread.Sleep(1000);
                nxtBrick.MotorA.Flip = false;
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.Message, Logging.LogType.Error, Logging.LogCaller.Motor);
            }
        }
        /// <summary>
        /// Turns a motor one-tick autonomously.
        /// </summary>
        /// <param name="direction">The direction to turn.</param>
        /// <remarks>Can be extended to include other motor controls.</remarks>
        protected void TurnOneClick(string direction)
        {
            switch (direction)
            {
                case "left":
                    BeginInvoke(new MethodInvoker(() => lightSeekingMotor.buttonTurnCounterClockwise.PerformClick()));
                    RememberedLastTurn = "left";
                    break;
                case "right":
                    BeginInvoke(new MethodInvoker(() => lightSeekingMotor.buttonTurnClockwise.PerformClick()));
                    RememberedLastTurn = "right";
                    break;
            }
        }

        private void NxtMotorPositionChanged(NxtSensor sensor)
        {
            Invoke((MethodInvoker)delegate
            {
                BoxedCurrentValue = (int)Functions.Clamp(nxtLightSensor.Value, 0, 100);
                valLight.Value = (int)BoxedCurrentValue;
                lightSensorValueOut.Text = nxtLightSensor.Value.ToString(CultureInfo.InvariantCulture);
                SensorTemporalValueDifference();
            });
            if (WhirlActive)
            {
                // Add logic when haden responds to a light event and is intelligent.

                var hold = 0;
            }
        }

        private void NxtLightSensorValueChanged(NxtSensor sensor)
        {
            Invoke((MethodInvoker)delegate
            {
                BoxedCurrentValue = (int)Functions.Clamp(nxtLightSensor.Value, 0, 100);
                valLight.Value = (int)BoxedCurrentValue;
                lightSensorValueOut.Text = nxtLightSensor.Value.ToString(CultureInfo.InvariantCulture);
                SensorTemporalValueDifference();
            });
            if (WhirlActive)
            {
                // Add logic when haden responds to a light event and is intelligent.

                var hold = 0;
            }
        }
        private void NxtPressureSensorPolled(NxtSensor sensor)
        {
            if (nxtPressureSensor.IsPressed)
            {
                nxtTankControl1.TankDrive.Brake();
                lightSeekingMotor.Stop();
                if (VoiceSpoken == false)
                    _voice.Speak("The robot has reached its goal.");
                VoiceSpoken = true;
            }
        }

#endregion

#region Coin flip, decider, and session generator

        /// <summary>
        /// Decides to seek to the left or to the right using a (static) random number generator.
        /// </summary>
        public void DecideLeftOrRight()
        {
            var leftRight = StaticRandom.Next(1, 3);
            lastRememberedTextBox.Text = leftRight.ToString();
            Thread.Sleep(1000);
            if (leftRight.Equals(1))
            {
                RememberedLastTurn = "none";
                lastRememberedTextBox.Text = RememberedLastTurn.ToString();
                statusStrip.Text = "I remember not turning before. A random decision is that I turn to the left. Is this okay?";
                CallDecisionForm(statusStrip.Text);
                //Thread.Sleep(10000);
                //AutonomousTurn("left");
            }
            if (leftRight.Equals(2))
            {
                RememberedLastTurn = "none";
                lastRememberedTextBox.Text = RememberedLastTurn.ToString();
                statusStrip.Text = "I remember not turning before. A random decision is that I turn to the right. Is this okay?";
                CallDecisionForm(statusStrip.Text);
                //Thread.Sleep(10000);
                //AutonomousTurn("right");
            }
        }
        public void CallDecisionForm(string message)
        {
            if (YesNo.Instance == false)
            {
                YesNo form = new YesNo(this, message);
                form.Show(this);
                YesNo.Instance = true;
                statusStrip.Text = "Called the decision form.";
            }
            else if (YesNo.Instance)
            {
                // Do nothing.
            }
        }

#endregion
    }
}
