namespace Systems
{
    /// <summary>
    /// Contains information about a message that must be displayed to the player
    /// </summary>
    public sealed class MessageInfo
    {
        #region Constructor

        /// <summary>
        /// Constructor of MessageInfo class
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="args">Extra parameters</param>
        public MessageInfo(string message, params object[] args)
        {
            Message = message;
            Arguments = args;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the message
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Gets a list of extra parameters
        /// </summary>
        public object[] Arguments { get; private set; }

        #endregion
    }
}
