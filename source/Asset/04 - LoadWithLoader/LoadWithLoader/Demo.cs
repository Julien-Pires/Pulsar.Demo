using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Pulsar.Assets;
using CustomAssetLibrary.Asset;

using Common;
using Common.Asset;
#if WINDOWS
using Common.Windows;
#endif

namespace LoadWithLoader
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Demo : Game
    {
        #region Fields

        private const string StorageName = "MyGameStorage";
        private const string CustomAssetFolder = "Content/CustomAssetData";

        private AssetEngineService _assetEngineService;
        private Storage _gameStorage;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of Demo class
        /// </summary>
        public Demo()
        {
            Graphics = new GraphicsDeviceManager(this);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
#if WINDOWS
            ConsoleHelper.OpenConsole();
#endif
            _assetEngineService = new AssetEngineService(this);

            // Create storage
            _gameStorage = _assetEngineService.AssetEngine.CreateStorage(StorageName);
            Output.WriteLine(AssetMessages.StorageCreated, StorageName);

            // Create folder in storage
            _gameStorage.AddFolder(CustomAssetFolder);
            Output.WriteLine(AssetMessages.FolderCreated, CustomAssetFolder);

            /*
             * A loader must implement the IAssetLoader interface
             * When a loader is added to the engine, it is available for all storage
             * To add a new loader you have to call the AddLoader method of the AssetEngine :
             */
            _assetEngineService.AssetEngine.AddLoader(new CustomAssetLoader());
            Output.WriteLine(AssetMessages.AssetLoaderAdded, typeof(CustomAssetLoader));

            base.Initialize();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

#if WINDOWS
            ConsoleHelper.CloseConsole();
#endif
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Following two examples will try to load CustomAsset instances from file
            CustomAsset myFirstAsset = _gameStorage[CustomAssetFolder].Load<CustomAsset>("MyFirstCustomAsset");
            Output.WriteLine(AssetMessages.AssetLoaded, "MyFirstCustomAsset");
            Output.WriteLine(myFirstAsset.Value);

            CustomAsset mySecondAsset = _gameStorage[CustomAssetFolder].Load<CustomAsset>("MySecondCustomAsset");
            Output.WriteLine(AssetMessages.AssetLoaded, "MySecondCustomAsset");
            Output.WriteLine(mySecondAsset.Value);

            /*
             * Following example will try to load a CustomAsset instance by passing additional parameter to Load
             * For example, we tell the loader to create a new instance instead of loading data from a file :
             */
            CustomAssetParameter assetParameter = new CustomAssetParameter
            {
                Source = AssetSource.NewInstance,
                Value = "A value manually loaded"
            };
            CustomAsset myThirdAsset = _gameStorage[CustomAssetFolder].Load<CustomAsset>("MyThirdCustomAsset",
                assetParameter);
            Output.WriteLine(AssetMessages.AssetLoaded, "MyThirdCustomAsset");
            Output.WriteLine(myThirdAsset.Value);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // Unload assets and destroy the storage
            _assetEngineService.AssetEngine.DestroyStorage(StorageName);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the graphics device manager
        /// </summary>
        public GraphicsDeviceManager Graphics { get; private set; }

        #endregion
    }
}
