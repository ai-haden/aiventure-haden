using System;
using System.IO.Ports;
using System.Text;
using System.Windows.Forms;
using Haden.NxtSharp.Brick;
using Haden.NxtSharp.Command;
using Haden.NxtSharp.Exceptions;
using Haden.NxtSharp.Motors;
using Haden.NxtSharp.Sensors;
using Haden.NxtSharp.Utilties;

namespace Haden.NxtSharp
{
    /// <summary>
    /// This class allows communication with a single Nxt brick.
    /// </summary>
    public class BrickCommunicator
    {
        private readonly string _portName;
        private SerialPort _port;

        public bool IsConnectedToSerialPort { get; private set; }

        public BrickCommunicator(string portname)
        {
            _portName = portname;
        }

        #region Connection handling

        /// <summary>
        /// Connects to the COM port associated with this communicator.
        /// </summary>
        public void Connect()
        {
            if (_port != null)
            {
                Disconnect();
            }
            _port = new SerialPort(_portName);
            try
            {
                _port.Open();
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.Message, Logging.LogType.Error, Logging.LogCaller.Communicator, "Connect");
                throw;
            }

            IsConnectedToSerialPort = true;// Connected to serial port is NOT connected to brick!
        }
        /// <summary>
        /// Disconnects from the COM port associated with this communicator.
        /// </summary>
        public void Disconnect()
        {
            IsConnectedToSerialPort = false;
            if (_port.IsOpen)
            {
                _port.Close();
                _port.Dispose();
            }
            _port = null;
        }

        #endregion

        #region Communication primitives

        private byte[] SendMessage(byte[] message)
        {
            if (!IsConnectedToSerialPort)
            {
                Logging.WriteLog("Can't send message - not connected to the brick.", Logging.LogType.Error);
            }
            lock (this)
            {
                var length = message.Length;
                var btMessage = new byte[message.Length + 2];
                var errorMessage = "";
                btMessage[0] = (byte)(length & 0xFF);
                btMessage[1] = (byte)((length & 0xFF00) >> 8);
                message.CopyTo(btMessage, 2);
                _port.Write(btMessage, 0, btMessage.Length);
                _port.ReadTimeout = 5000;

                if (message[0] < 0x80)
                {
                    try
                    {
                        // Crashes here if not connected first to device over Bluetooth. Also aborts the thread on the last pass.
                        var lsb = _port.ReadByte(); 
                        var msb = _port.ReadByte();
                        var size = lsb + msb * 256;
                        var reply = new byte[size];
                        
                        _port.Read(reply, 0, size);

                        if (reply[0] != 0x02)
                        {
                            throw new Exception("Unexpected return message type: " + Functions.Hex(reply[0]) + ".");
                        }

                        if (reply[1] != message[1])
                        {
                            throw new Exception("Unexpected return command: " + Functions.Hex(reply[2]) + ".");
                        }
                        if (reply[2] > 0)
                        {
                            var error = (NxtMessageResult)reply[2];
                            errorMessage = "Brick error: " + error + " in reply to command " + Functions.Hex(message) + ".";

                            switch (error)
                            {
                                case NxtMessageResult.ChannelBusy:
                                    throw new NxtChannelBusyException(errorMessage);
                                case NxtMessageResult.CommBusError:
                                    throw new NxtCommunicationBusErrorException(errorMessage);
                                default:
                                    throw new ApplicationException(errorMessage);
                            }
                        }
                        return reply;
                    }
                    catch (Exception ex)
                    {
                        Logging.WriteLog(ex.Message + " -- " + errorMessage, Logging.LogType.Error, Logging.LogCaller.Communicator, "SendMessage");
                        throw;
                    }
                }
                return null;
            }
        }

        #endregion

        #region System Commands
        /// <summary>
        /// Sets the name of the Nxt brick.
        /// </summary>
        public void SetBrickName(string name)
        {
            if (name.Length > 14)
            {
                name = name.Substring(0, 14);
            }
            var nameBytes = Encoding.ASCII.GetBytes(name);
            var message = new byte[18];
            message[0] = 0x01;	// Do not expect an answer.
            message[1] = (byte)NxtCommand.SetBrickName;	// SetBrickName Command ID.
            nameBytes.CopyTo(message, 2);
            SendMessage(message);
        }

        #endregion

        #region Direct Commands

        #region Motor Commands
        /// <summary>
        /// Sends a drive command to any one of the motors.
        /// </summary>
        public void SetOutputState(NxtMotorPort port, sbyte power, NxtMotorMode mode, NxtMotorRegulationMode regulationMode, sbyte turnRatio, NxtMotorRunState runState, uint tachoLimit)
        {
            var message = new byte[12];
            message[0] = 0x80;
            message[1] = (byte)NxtCommand.SetOutputState;
            message[2] = (byte)port;
            message[3] = (byte)power;
            message[4] = (byte)mode;
            message[5] = (byte)regulationMode;
            message[6] = (byte)turnRatio;
            message[7] = (byte)runState;
            Functions.SetUInt32(message, 8, tachoLimit);
            SendMessage(message);
        }
        /// <summary>
        /// Gets the state of any one of the motors.
        /// </summary>
        public NxtGetOutputState GetOutputState(NxtMotorPort port)
        {
            var message = new byte[3];
            message[0] = 0x00;
            message[1] = (byte)NxtCommand.GetOutputState;
            message[2] = (byte)port;
            var reply = SendMessage(message);
            var result = new NxtGetOutputState
            {
                Power = (sbyte)reply[4],
                Mode = (NxtMotorMode)reply[5],
                RegulationMode = (NxtMotorRegulationMode)reply[6],
                TurnRatio = (sbyte)reply[7],
                RunState = (NxtMotorRunState)reply[8],
                TachoLimit = Functions.GetUInt32(reply, 9),
                TachoCount = Functions.GetInt32(reply, 13),
                BlockTachoCount = Functions.GetInt32(reply, 17),
                RotationCount = Functions.GetInt32(reply, 21)
            };
            return result;
        }
        /// <summary>
        /// Resets the motor position.
        /// </summary>
        public void ResetMotorPosition(NxtMotorPort port, bool relative)
        {
            var message = new byte[4];
            message[0] = 0x80; // Do not expect an answer.
            message[1] = (byte)NxtCommand.ResetMotorPosition;
            message[2] = (byte)port;
            if (relative)
            {
                message[3] = 1;
            }
            else
            {
                message[3] = 0;
            }
            SendMessage(message);
        }

        #endregion

        #region Sensor commands
        /// <summary>
        /// Set the input mode of the sensor.
        /// </summary>
        public void SetInputMode(NxtSensorPort port, NxtSensorType type, NxtSensorMode mode)
        {
            var message = new byte[5];
            message[0] = (byte)NxtCommandType.DirectCommandWithResponse;
            message[1] = (byte)NxtCommand.SetInputMode;
            message[2] = (byte)port;
            message[3] = (byte)type;
            message[4] = (byte)mode;
            SendMessage(message);
        }
        /// <summary>
        /// Get the input values of the sensor.
        /// </summary>
        public NxtGetInputValues GetInputValues(NxtSensorPort port)
        {
            var result = new NxtGetInputValues();
            var message = new byte[3];
            message[0] = 0x00; // Expect an answer.
            message[1] = (byte)NxtCommand.GetInputValues;
            message[2] = (byte)port;
            var reply = SendMessage(message);
            result.Valid = (reply[4] == 1);
            result.Calibrated = (reply[5] == 1);
            result.Type = (NxtSensorType)reply[6];
            result.Mode = (NxtSensorMode)reply[7];
            result.RawAd = Functions.GetUInt16(reply, 8);
            result.NormalizedAd = Functions.GetUInt16(reply, 10);
            result.ScaledValue = Functions.GetInt16(reply, 12);
            result.CalibratedValue = Functions.GetInt16(reply, 14);
            return result;
        }

        #endregion

        #region I2C (Low speed bus) Commands
        /// <summary>
        /// Reads the amount of bytes ready at a port on the I2C (low speed).
        /// </summary>
        public int LsGetStatus(NxtSensorPort port)
        {
            var message = new byte[3];
            message[0] = (byte)NxtCommandType.DirectCommandWithResponse;
            message[1] = (byte)NxtCommand.LsGetStatus;
            message[2] = (byte)port;
            var reply = SendMessage(message);
            return reply[3];
        }
        /// <summary>
        /// Writes a message to a port on the I2C (low speed).
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="data">The data.</param>
        /// <param name="returnMessageLength">the length of the return message (you must specify this).</param>
        public void LsWrite(NxtSensorPort port, byte[] data, int returnMessageLength)
        {
            if (data.Length < 1 || data.Length > 16)
            {
                throw new InvalidOperationException("The data length must be between 1 and 16 bytes.");
            }

            var message = new byte[5 + data.Length];
            message[0] = (byte)NxtCommandType.DirectCommandWithResponse;
            message[1] = (byte)NxtCommand.LsWrite;
            message[2] = (byte)port;
            message[3] = (byte)data.Length;
            message[4] = (byte)returnMessageLength;
            Array.Copy(data, 0, message, 5, data.Length);
            SendMessage(message);
        }
        /// <summary>
        /// Reads all bytes from a sensor port using the I2C (low speed) bus.
        /// </summary>
        public byte[] LsRead(NxtSensorPort port)
        {
            var message = new byte[3];
            message[0] = (byte)NxtCommandType.DirectCommandWithResponse;
            message[1] = (byte)NxtCommand.LsRead;
            message[2] = (byte)port;
            var reply = SendMessage(message);
            int length = reply[3];
            var result = new byte[length];
            Array.Copy(reply, 4, result, 0, length);
            return result;
        }

        #endregion

        #region Misc Commands
        /// <summary>
        /// Send keep-alive signal to the Nxt brick.
        /// </summary>
        public void KeepAlive()
        {
            var message = new byte[2];
            message[0] = 0x80;	// Do not expect an answer.
            message[1] = (byte)NxtCommand.KeepAlive;	// KeepAlive Command ID.
            SendMessage(message);
        }
        /// <summary>
        /// Retrieves the battery level.
        /// </summary>
        /// <returns>the battery level, in millivolts.</returns>
        public int GetBatteryLevel()
        {
            var message = new byte[2];
            message[0] = 0x00;	// Expect an answer.
            message[1] = (byte)NxtCommand.GetBatteryLevel;	// GetBatteryLevel Command ID.
            var reply = SendMessage(message);
            return Functions.GetUInt16(reply, 3);
        }

        #endregion

        #region Bluetooth messages

        #region Sending
        /// <summary>
        /// Sends a message to a bluetooth mailbox.
        /// </summary>
        /// <param name="mailbox">The mailbox number on the Nxt brick [0-9].</param>
        /// <param name="data">The data.</param>
        public void MessageWrite(byte mailbox, byte[] data)
        {
            if (data.Length > 57)
            {
                throw new ArgumentException("Data size must be less than 58 bytes.");
            }
            var message = new byte[5 + data.Length];
            message[0] = 0x80; // Do not expect an answer.
            message[1] = (byte)NxtCommand.MessageWrite;
            message[2] = mailbox;
            message[3] = (byte)(data.Length + 1);
            for (int i = 0; i < data.Length; i++)
            {
                message[i + 4] = data[i];
            }
            SendMessage(message);
        }
        /// <summary>
        /// Sends a string to a bluetooth mailbox.
        /// </summary>
        public void MessageWrite(byte mailbox, string value)
        {
            MessageWrite(mailbox, Encoding.ASCII.GetBytes(value));
        }
        /// <summary>
        /// Sends a integer to a bluetooth mailbox.
        /// </summary>
        public void MessageWrite(byte mailbox, int value)
        {
            var data = new byte[4];
            Functions.SetInt32(data, 0, value);
            MessageWrite(mailbox, data);
        }
        /// <summary>
        /// Sends a boolean to a bluetooth mailbox.
        /// </summary>
        public void MessageWrite(byte mailbox, bool value)
        {
            var data = new byte[1];
            data[0] = (byte)(value ? 0x01 : 0x00);
            MessageWrite(mailbox, data);
        }

        #endregion

        #region Receiving
        /// <summary>
        /// Reads data from the specified mailbox.
        /// </summary>
        /// <param name="mailbox">The mailbox number on the Nxt brick [0-9].</param>
        /// <returns>A byte array.</returns>
        public byte[] MessageRead(byte mailbox)
        {
            var message = new byte[5];
            message[0] = 0x00; // Expect an answer.
            message[1] = (byte)NxtCommand.MessageRead;
            message[2] = (byte)(mailbox + 10);
            message[3] = (byte)(mailbox + 10);
            message[4] = 0x01;
            byte[] reply = SendMessage(message);
            int size = reply[4];
            var result = new byte[size];
            Array.Copy(reply, 5, result, 0, size);
            return result;
        }
        /// <summary>
        /// Reads a string from the specified mailbox.
        /// </summary>
        /// <param name="mailbox">The mailbox number on the Nxt brick [0-9].</param>
        /// <returns></returns>
        public string MessageReadString(byte mailbox)
        {
            byte[] data = MessageRead(mailbox);
            return Encoding.ASCII.GetString(data, 0, data.Length - 1);
        }
        /// <summary>
        /// Reads an integer from the specified mailbox.
        /// </summary>
        /// <param name="mailbox">The mailbox number on the Nxt brick [0-9].</param>
        /// <returns></returns>
        public int MessageReadInt(byte mailbox)
        {
            byte[] data = MessageRead(mailbox);
            return Functions.GetInt32(data, 0);
        }
        /// <summary>
        /// Reads a boolean from the specified mailbox.
        /// </summary>
        /// <param name="mailbox">The mailbox number on the Nxt brick [0-9].</param>
        /// <returns></returns>
        public bool MessageReadBool(byte mailbox)
        {
            byte[] data = MessageRead(mailbox);
            return data[0] != 0;
        }

        #endregion

        #endregion

        #endregion

        #region I2C Command Helpers
        /// <summary>
        /// Writes a byte on the I2C (Low Speed) interface.
        /// </summary>
        /// <param name="port"></param>
        /// <param name="address"></param>
        /// <param name="value"></param>
        public void I2CSetByte(NxtSensorPort port, byte address, byte value)
        {
            var i2CCmd = new byte[3];
            i2CCmd[0] = 0x02;
            i2CCmd[1] = address;
            i2CCmd[2] = value;
            LsWrite(port, i2CCmd, 0);
        }
        /// <summary>
        /// Reads a byte from the I2C (low speed) interface.
        /// </summary>
        /// <param name="port">The sensor port.</param>
        /// <param name="address">The byte address of the port.</param>
        public byte I2CGetByte(NxtSensorPort port, byte address)
        {
            var i2CCmd = new byte[2];
            i2CCmd[0] = 0x02;
            i2CCmd[1] = address;

            int bytesRead;
            do
            {
                LsWrite(port, i2CCmd, 1);
                try
                {
                    bytesRead = LsGetStatus(port);
                }
                catch (NxtCommunicationBusErrorException)
                {
                    bytesRead = 0;
                }
            } while (bytesRead < 1);

            return LsRead(port)[0];
        }

        #endregion

    }
}
