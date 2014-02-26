using Pulsar;

namespace Systems
{
    /// <summary>
    /// Contains EventType instance that can be access globally
    /// </summary>
    public static class EventTypeConstant
    {
        #region Fields

        private const string DamageEventKey = "Damage";
        private const string DeathEventKey = "Death";
        private const string ShowMessageKey = "ShowMessage";

        #endregion

        #region Constructor

        /// <summary>
        /// Static constructor of EventTypeConstant class
        /// </summary>
        static EventTypeConstant()
        {
            DamageEvent = EventType.CreateEvent(DamageEventKey);
            DeathEvent = EventType.CreateEvent(DeathEventKey);
            ShowMessageEvent = EventType.CreateEvent(ShowMessageKey);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the EventType instance for the event DamageEvent
        /// </summary>
        public static EventType DamageEvent { get; private set; }

        /// <summary>
        /// Gets the EventType instance for the event DeathEvent
        /// </summary>
        public static EventType DeathEvent { get; private set; }

        /// <summary>
        /// Gets the EventType instance for the event ShowMessageEvent
        /// </summary>
        public static EventType ShowMessageEvent { get; private set; }

        #endregion
    }
}
