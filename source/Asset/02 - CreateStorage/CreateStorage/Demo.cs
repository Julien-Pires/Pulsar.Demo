using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Pulsar.Assets;

namespace CreateStorage
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Demo : Game
    {
        #region Fields

        private AssetEngineService _assetEngineService;

        #endregion

        #region Constructors

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
            _assetEngineService = new AssetEngineService(this);

            // Creates a storage by calling CreateStorage method and give any name you want
            Storage storage = _assetEngineService.AssetEngine.CreateStorage("MyGameStorage");

            /* 
             * Previous example store the storage in a variable when it is created 
             * But there are other way to get it from AssetEngine :
             * 
             * Storage storage = _assetEngineService.AssetEngine["MyGameStorage"];
             * 
             * or
             * 
             * Storage storage = _assetEngineService.AssetEngine.GetStorage("MyGameStorage");
             */

            /*
             * Each storage is organized by folders, one folder represents one path in a content project
             * In one storage you can have multiple folders that target different content project
             * The following example will create folders that use the root of two different content project :
             */
            storage.AddFolder("Content");
            storage.AddFolder("OtherContent");

            /*
             * You can also go deeper with folder path, this will allow you to organize asset as you want :
             * 
             * storage.AddFolder("Content/Mesh");
             * storage.AddFolder("Content/Textures");
             * storage.AddFolder("OtherContent/Sounds");
             */

            base.Initialize();
        }

        /// <summary>
        /// Loads assets from AssetEngine
        /// </summary>
        protected override void LoadContent()
        {
            // TODO: Use the AssetEngine to load assets
        }

        /// <summary>
        /// Unloads any assets
        /// </summary>
        protected override void UnloadContent()
        {
            // Once your done with storage you can destroy it by calling Dispose()
            _assetEngineService.AssetEngine["MyGameStorage"].Dispose();
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
