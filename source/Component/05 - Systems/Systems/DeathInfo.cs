namespace Systems
{
    /// <summary>
    /// Contains information about a dead game object
    /// </summary>
    public sealed class DeathInfo
    {
        #region Constructor

        /// <summary>
        /// Constructor of DeathInfo class
        /// </summary>
        /// <param name="id">Game object id</param>
        public DeathInfo(int id)
        {
            GameObjectId = id;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the game object id
        /// </summary>
        public int GameObjectId { get; private set; }

        #endregion
    }
}
