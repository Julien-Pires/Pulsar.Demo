using Pulsar.Components;

namespace Systems.Components
{
    /// <summary>
    /// Represents the health for a game object
    /// </summary>
    public sealed class HealthComponent : Component
    {
        #region Fields

        /// <summary>
        /// Maximum life
        /// </summary>
        public const int MaxLife = 100;

        /// <summary>
        /// Minimum life
        /// </summary>
        public const int MinLife = 0;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of HealthComponent class
        /// </summary>
        public HealthComponent()
        {
            Life = MaxLife;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the life of the game object
        /// </summary>
        public int Life { get; set; }

        #endregion
    }
}
