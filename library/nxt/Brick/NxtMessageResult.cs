namespace Haden.NxtSharp.Brick
{
    /// <summary>
    /// The Nxt message results enumeration.
    /// </summary>
    public enum NxtMessageResult : byte
    {
        /// <summary>
        /// OK.
        /// </summary>
        Ok = 0x00,
        /// <summary>
        /// Communication in progress.
        /// </summary>
        PendingCommunicationInProgress = 0x20,
        /// <summary>
        /// The specified mailbox is empty.
        /// </summary>
        SpecifiedMailBoxEmpty = 0x40,
        /// <summary>
        /// The request failed.
        /// </summary>
        RequestFailed = 0xBD,
        /// <summary>
        /// The command is unknown.
        /// </summary>
        UnknownCommand = 0xBE,
        /// <summary>
        /// The packet is dangerously configured.
        /// </summary>
        InsanePacket = 0xBF,
        /// <summary>
        /// The range data.
        /// </summary>
        PublicOfRangeData = 0xC0,
        /// <summary>
        /// The communcation bus error.
        /// </summary>
        CommBusError = 0xDD,
        /// <summary>
        /// The memory in the communications buffer.
        /// </summary>
        PublicOfMemoryInCommunicationBuffer = 0xDE,
        /// <summary>
        /// The channel is invalid.
        /// </summary>
        ChannelInvalid = 0xDF,
        /// <summary>
        /// The channel is busy.
        /// </summary>
        ChannelBusy = 0xE0,
        /// <summary>
        /// There is no active program.
        /// </summary>
        NoActiveProgram = 0xEC,
        /// <summary>
        /// The specified size is illegal.
        /// </summary>
        IllegalSizeSpecified = 0xED,
        /// <summary>
        /// The mailbox ID is illegal.
        /// </summary>
        IllegalMailboxId = 0xEE,
        /// <summary>
        /// The accessible field is invalid.
        /// </summary>
        InvalidFieldAccess = 0xEF,
        /// <summary>
        /// Bad data in stream.
        /// </summary>
        BadData = 0xF0,
        /// <summary>
        /// The memory buffer.
        /// </summary>
        PublicOfMemory = 0xFB,
        /// <summary>
        /// Bad arguments in stream.
        /// </summary>
        BadArguments = 0xFF
    }
}