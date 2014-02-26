using Pulsar.Components;

namespace Systems.Components
{
    /// <summary>
    /// Represents a shield for a game object
    /// </summary>
    public sealed class ShieldComponent : Component
    {
        #region Fields

        /// <summary>
        /// Default maximum power value
        /// </summary>
        public const int MaxPower = 150;

        /// <summary>
        /// Default minimum power value
        /// </summary>
        public const int MinPower = 0;

        /// <summary>
        /// Default quantity of power that can be regenerated
        /// </summary>
        public const int DefaultRegeneration = 10;

        /// <summary>
        /// Default delay to wait before regeneration can begin
        /// </summary>
        public const double DefaultDelay = 8000.0d;

        /// <summary>
        /// Default tick frequency for regeneration
        /// </summary>
        public const double DefaultFrequency = 4000.0d;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of ShieldComponent class
        /// </summary>
        public ShieldComponent()
        {
            Power = MaxPower;
            Regeneration = DefaultRegeneration;
            RegenerationDelay = DefaultDelay;
            RegenerationFrequency = DefaultFrequency;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value that indicates if the shield is active
        /// </summary>
        public bool Activated { get; set; }

        /// <summary>
        /// Gets or sets the remaining power of the shield
        /// </summary>
        public int Power { get; set; }

        /// <summary>
        /// Gets or sets the quantity of power that a shield can regenerate per tick
        /// </summary>
        public int Regeneration { get; set; }

        /// <summary>
        /// Gets or sets the delay to wait since last damage have been done to a game object in order to begin regeneration
        /// </summary>
        public double RegenerationDelay { get; set; }

        /// <summary>
        /// Gets or sets the regeneration tick frequency
        /// </summary>
        public double RegenerationFrequency { get; set; }

        /// <summary>
        /// Gets or sets the time since the last regeneration
        /// </summary>
        internal double LastRegeneration { get; set; }

        #endregion
    }
}
