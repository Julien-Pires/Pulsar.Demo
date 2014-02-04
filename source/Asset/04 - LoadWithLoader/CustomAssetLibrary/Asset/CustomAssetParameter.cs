using Pulsar.Assets;

namespace CustomAssetLibrary.Asset
{
    /// <summary>
    /// Represents parameter for loading a CustomAsset instance
    /// </summary>
    public sealed class CustomAssetParameter
    {
        #region Properties

        /// <summary>
        /// Gets or sets the source used to load the asset
        /// </summary>
        public AssetSource Source { get; set; }

        /// <summary>
        /// Gets or sets the value for the asset
        /// </summary>
        public string Value { get; set; }

        #endregion
    }
}
