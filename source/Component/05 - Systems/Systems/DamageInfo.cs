namespace Systems
{
    /// <summary>
    /// Contains information about damage that has been done to a game object
    /// </summary>
    public sealed class DamageInfo
    {
        #region Constructor

        /// <summary>
        /// Constructor of DamageInfo class
        /// </summary>
        /// <param name="id">Game object id</param>
        /// <param name="damage">Quantity of damage</param>
        public DamageInfo(int id, int damage)
        {
            GameObjectId = id;
            Damage = damage;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the game object id
        /// </summary>
        public int GameObjectId { get; private set; }

        /// <summary>
        /// Gets the quantity of damage
        /// </summary>
        public int Damage { get; private set; }

        #endregion
    }
}
