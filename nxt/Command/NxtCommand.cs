namespace Haden.NxtSharp.Command
{
    /// <summary>
    /// The Nxt command enumeration.
    /// </summary>
    public enum NxtCommand : byte
    {
        /// <summary>
        /// Start the program.
        /// </summary>
        StartProgram = 0x00,
        /// <summary>
        /// Stop the program
        /// </summary>
        StopProgram = 0x01,
        /// <summary>
        /// Play a sound file.
        /// </summary>
        PlaySoundFile = 0x02,
        /// <summary>
        /// Play a tone.
        /// </summary>
        PlayTone = 0x03,
        /// <summary>
        /// Set the output state.
        /// </summary>
        SetOutputState = 0x04,
        /// <summary>
        /// Set the input mode.
        /// </summary>
        SetInputMode = 0x05,
        /// <summary>
        /// Get the output state.
        /// </summary>
        GetOutputState = 0x06,
        /// <summary>
        /// Get the input values.
        /// </summary>
        GetInputValues = 0x07,
        /// <summary>
        /// Write a message.
        /// </summary>
        MessageWrite = 0x09,
        /// <summary>
        /// Reset the motor position.
        /// </summary>
        ResetMotorPosition = 0x0A,
        /// <summary>
        /// Get the battery charge level.
        /// </summary>
        GetBatteryLevel = 0x0B,
        /// <summary>
        /// Keep alive.
        /// </summary>
        KeepAlive = 0x0D,
        /// <summary>
        /// Get the low-speed status.
        /// </summary>
        LsGetStatus = 0x0E,
        /// <summary>
        /// Write in low-speed format.
        /// </summary>
        LsWrite = 0x0F,
        /// <summary>
        /// Read in low-speed format.
        /// </summary>
        LsRead = 0x10,
        /// <summary>
        /// Read a message.
        /// </summary>
        MessageRead = 0x13,
        /// <summary>
        /// Open a file for reading.
        /// </summary>
        OpenRead = 0x80,
        /// <summary>
        /// Open a file for writing.
        /// </summary>
        OpenWrite = 0x81,
        /// <summary>
        /// Read the data.
        /// </summary>
        Read = 0x82,
        /// <summary>
        /// Write the data.
        /// </summary>
        Write = 0x83,
        /// <summary>
        /// Set the name of the Nxt brick.
        /// </summary>
        SetBrickName = 0x98
    }
}