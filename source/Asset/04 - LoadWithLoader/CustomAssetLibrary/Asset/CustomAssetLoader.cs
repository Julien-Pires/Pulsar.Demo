using System;

using Pulsar.Assets;

namespace CustomAssetLibrary.Asset
{
    /// <summary>
    /// Provides load customs mechanism for asset of type CustomAsset
    /// </summary>
    public sealed class CustomAssetLoader : AssetLoader
    {
        #region Fields

        private const string InternName = "CustomAssetLoader";

        private readonly Type[] _supportedTypes = { typeof(CustomAsset) };

        /*
         * _defaultParameter contains an instance of CustomAssetParameter with default value to provides
         * default behavior when no parameters is passed to the loader
         */
        private readonly CustomAssetParameter _defaultParameter;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of CustomAssetLoader class
        /// </summary>
        public CustomAssetLoader()
        {
            _defaultParameter = new CustomAssetParameter
            {
                Source = AssetSource.FromFile,
                Value = string.Empty
            };
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads a CustomAssetInstance
        /// </summary>
        /// <typeparam name="T">Type of asset (here CustomAsset; if the loader supports multiple 
        /// asset type T will be of that type)</typeparam>
        /// <param name="assetName">Name of the asset</param>
        /// <param name="path">Path of the asset</param>
        /// <param name="parameters">Parameters used to create the instance</param>
        /// <param name="assetFolder">Folder in wich the asset will be stored</param>
        /// <returns>Returns the loaded asset</returns>
        public override LoadedAsset Load<T>(string assetName, string path, object parameters, AssetFolder assetFolder)
        {
            CustomAssetParameter customParameter;
            // We can check if parameters has been passed to the loader
            if (parameters != null)
            {
                customParameter = parameters as CustomAssetParameter;
                if(customParameter == null)
                    throw new Exception("Wrong parameters");
            }
            else
                customParameter = _defaultParameter;

            CustomAsset asset;
            switch (customParameter.Source)
            {
                /*
                 * AssetSource can be used to detect how the asset must be created
                 * You can use your own enum or value if AssetSource doesn't provides what you want
                 */
                case AssetSource.NewInstance:
                    asset = new CustomAsset(assetName, customParameter.Value); // There isn't built-in mechanism to create new asset instance, loader has to do it
                    break;

                case AssetSource.FromFile:
                    LoadedAsset fileResult = new LoadedAsset();

                    /*
                     * LoadFromFile is a method provided by AssetLoader abstract class
                     * This method allow you to load data from a file
                     * You need to provide the path, the folder from wich the file is read and LoadedAsset instance
                     * The method will fill the LoadedAsset instance with the asset and all its disposables resources
                     */
                    LoadFromFile<T>(path, assetFolder, fileResult);

                    asset = fileResult.Asset as CustomAsset;
                    if(asset == null)
                        throw new Exception("An occurred when trying to load from file");
                    break;

                default:
                    throw new NotSupportedException("Cannot load CustomAsset from other source than NewInstance");
            }

            /*
             * Load method must return a LoadedAsset instance that contains everything about the asset
             * Asset property contains the asset
             * Disposables property is a collection of IDisposables that can contains disposables resources used by the asset
             */
            LoadedAsset loadedAsset = new LoadedAsset {Asset = asset};
            /*
             * The previous example doesn't use Disposables property because CustomAsset doesn't implement IDisposables or has
             * resources that implements this interface
             * If CustomAsset had implemented IDisposable we can add it this way :
             * 
             * loadedAsset.Disposables.Add(asset);
             */

            return loadedAsset;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the loader
        /// </summary>
        public override string Name
        {
            get { return InternName; }
        }

        /// <summary>
        /// Gets the list of asset supported by this loader
        /// This array is used to detect which loader is used with wich asset
        /// </summary>
        public override Type[] SupportedTypes
        {
            get { return _supportedTypes; }
        }

        #endregion
    }
}
