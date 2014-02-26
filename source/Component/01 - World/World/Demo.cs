using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace World
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Demo : Game
    {
        #region Fields

        private Pulsar.Components.World _myWorld;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of Demo class
        /// </summary>
        public Demo()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        #endregion

        #region Methods

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            // Create a new World instance
            _myWorld = new Pulsar.Components.World();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            _myWorld = null;
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();

            base.Update(gameTime);

            // Update the world
            // This will update all systems associated to this world
            _myWorld.Update(gameTime);
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

        public GraphicsDeviceManager Graphics
        {
            get; 
            private set;
        }

        public SpriteBatch SpriteBatch
        {
            get;
            private set;
        }

        #endregion
    }
}
