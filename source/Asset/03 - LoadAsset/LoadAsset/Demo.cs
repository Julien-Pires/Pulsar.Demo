using Common.Asset;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Common;
#if WINDOWS
using Common.Windows;
#endif

using Pulsar.Assets;

namespace LoadAsset
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Demo : Game
    {
        #region Fields

        private const string StorageName = "MyGameStorage";
        private const string MeshFolder = "Common.Content/Mesh";
        private const string CrateMesh = "Crate/Crate";
        private const string CrateName = "MyCrate";

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
            _gameStorage = _assetEngineService.AssetEngine.CreateStorage(StorageName);
            Output.WriteLine(AssetMessages.StorageCreated, StorageName);

            _gameStorage.AddFolder(MeshFolder);
            Output.WriteLine(AssetMessages.FolderCreated, MeshFolder);

            base.Initialize();
        }

        /// <summary>
        /// Releases resources
        /// </summary>
        /// <param name="disposing"></param>
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
            /*
             * To load an asset you need to do it from a folder with the load method
             * You can get a folder from a storage in different way :
             * 
             * AssetFolder meshFolder = _gameStorage[MeshFolder];
             * or
             * AssetFolder meshFolder = _gameStorage.GetFolder(MeshFolder);
             */
            AssetFolder meshFolder = _gameStorage[MeshFolder];

            // The load method works exactly like the load method of ContentManager class :
            Model crate = meshFolder.Load<Model>(CrateMesh);
            Output.WriteLine(string.Format("Mesh {0} loaded : {1} Mesh - {2} Bones", CrateMesh, 
                crate.Meshes.Count, crate.Bones.Count));

            /*
             * Different signature exists for the load method
             * One of them allow you to use a custom name for the mesh instead of the path :
             */
            Model namedCrate = meshFolder.Load<Model>(CrateName, CrateMesh);
            Output.WriteLine(string.Format("Mesh {0} loaded : {1} Mesh - {2} Bones", CrateName, 
                namedCrate.Meshes.Count, namedCrate.Bones.Count));

            Output.WriteLine(string.Format("Total loaded asset : {0}", meshFolder.Count));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            AssetFolder meshFolder = _gameStorage[MeshFolder];

            // Assets can be unload individually :
            meshFolder.Unload(CrateMesh);
            meshFolder.Unload(CrateName);

            // Or you can unload all assets at once
            meshFolder.UnloadAll();

            /*
             * If you are done with the storage you can destroy it
             * Destroying the storage will unload every assets in each folder, this will avoid to do it manually
             */
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

        public GraphicsDeviceManager Graphics { get; private set; }

        #endregion
    }
}
