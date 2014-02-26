using System.Collections.Generic;

using Pulsar.Components;

namespace GameObjectsManager.Components
{
    /// <summary>
    /// Custom game object manager that manages game object
    /// Game objects are stored separately depending on their id (even or odd)
    /// </summary>
    public sealed class MyGameObjectManager : IGameObjectManager
    {
        #region Fields

        private readonly List<GameObject> _oddGameObjects = new List<GameObject>();
        private readonly List<GameObject> _evenGameObjects = new List<GameObject>();

        #endregion

        #region Methods

        /// <summary>
        /// Called when a game object is added to a world
        /// </summary>
        /// <param name="gameObject"></param>
        public void Added(GameObject gameObject)
        {
            if((gameObject.Id % 2) == 0)
                _evenGameObjects.Add(gameObject);
            else
                _oddGameObjects.Add(gameObject);
        }

        /// <summary>
        /// Called when a game object is removed from a world
        /// </summary>
        /// <param name="gameObject"></param>
        public void Removed(GameObject gameObject)
        {
            if ((gameObject.Id % 2) == 0)
                _evenGameObjects.Remove(gameObject);
            else
                _oddGameObjects.Remove(gameObject);
        }

        #endregion

        #region Properties

        public int EventCount
        {
            get { return _evenGameObjects.Count; }
        }

        public int OddCount
        {
            get { return _oddGameObjects.Count; }
        }

        #endregion
    }
}
