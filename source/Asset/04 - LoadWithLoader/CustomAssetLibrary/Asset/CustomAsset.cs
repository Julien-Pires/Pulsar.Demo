using Microsoft.Xna.Framework.Content;

namespace CustomAssetLibrary.Asset
{
    /// <summary>
    /// Represents a custom asset
    /// </summary>
    public sealed class CustomAsset
    {
        #region Fields

        [ContentSerializer]
        private string _name;

        [ContentSerializer]
        private string _value;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of CustomAsset class
        /// </summary>
        internal CustomAsset() : this(string.Empty, string.Empty)
        {
        }

        /// <summary>
        /// Constructor of CustomAsset class
        /// </summary>
        /// <param name="name">Name of the asset</param>
        /// <param name="value">Value of the asset</param>
        internal CustomAsset(string name, string value)
        {
            _name = name;
            _value = value;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the asset
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets the value of the asset
        /// </summary>
        public string Value
        {
            get { return _value; }
        }

        #endregion
    }
}
