using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Pulsar.Assets;

namespace InitializeEngine
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Demo : Game
    {
        #region Fields

        AssetEngineService _assetEngineService;

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
            // Creates a new instance of AssetEngineService
            // This instance has a reference to the AssetEngine
            // The AssetEngineService instance is automatically added to the services provider of the game
            _assetEngineService = new AssetEngineService(this);

            /*
             * You can also create directly an AssetEngine instance, but it will not be added to the services provider :
             * AssetEngine assetEngine = new AssetEngine(Services);
             * 
             * Otherwise you can create your own AssetEngine service by implementing IAssetEngineService interface
             */

            base.Initialize();
        }

        /// <summary>
        /// Releases resources
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            /* 
             * AssetEngine can be disposed either by calling Dispose of the engine or of the service : 
             * 
             * _assetEngineService.AssetEngine.Dispose();
             */
            _assetEngineService.Dispose();
        }

        /// <summary>
        /// Loads game content
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();
            
            // Gets the AssetEngine from the service
            AssetEngine assetEngine = _assetEngineService.AssetEngine;

            /*
             * Or you can gets it from the service provider :
             * IAssetEngineService assetEngineService = Services.GetService(typeof(IAssetEngineService)) as IAssetEngineService; 
             * AssetEngine assetEngine = assetEngineService.AssetEngine;
             */

            // TODO: Use the AssetEngine to load assets
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
