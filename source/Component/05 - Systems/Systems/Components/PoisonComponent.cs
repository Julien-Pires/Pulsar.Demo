using Pulsar.Components;

namespace Systems.Components
{
    /// <summary>
    /// Represents a poison state for a game object
    /// </summary>
    public sealed class PoisonComponent : Component
    {
        #region Fields

        /// <summary>
        /// Default minimum damage value
        /// </summary>
        public const int DefaultMinDamage = 1;

        /// <summary>
        /// Default maximum damage value
        /// </summary>
        public const int DefaultMaxDamage = 10;

        /// <summary>
        /// Default tick frequency for damage
        /// </summary>
        public const double DefaultFrequency = 4000.0d;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of PoisonComponent class
        /// </summary>
        public PoisonComponent()
        {
            MinDamage = DefaultMinDamage;
            MaxDamage = DefaultMaxDamage;
            Frequency = DefaultFrequency;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the minimum damage
        /// </summary>
        public int MinDamage { get; set; }

        /// <summary>
        /// Gets or sets the maximum damage
        /// </summary>
        public int MaxDamage { get; set; }

        /// <summary>
        /// Gets or sets the tick frequency
        /// </summary>
        public double Frequency { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates the last time the poison deals damage
        /// </summary>
        internal double LastDamage { get; set; }

        #endregion
    }
}
