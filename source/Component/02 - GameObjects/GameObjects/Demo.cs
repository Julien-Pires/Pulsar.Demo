using Common;
#if WINDOWS
using Common.Windows;
#endif

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Pulsar.Components;

namespace GameObjects
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Demo : Game
    {
        #region Fields

        private const string ObjectInWorldBefore = "{0} game object in the world Before Process()";
        private const string ObjectInWorldAfter = "{0} game object in the world after Process()";

        private World _myWorld;
        private GameObject _firstObject;
        private GameObject _secondObject;

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

        protected override void Initialize()
        {
#if WINDOWS
            ConsoleHelper.OpenConsole();
#endif

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            _myWorld = new World();

            _firstObject = new GameObject {Id = 1}; // Create a game object and assign an id
            _secondObject = new GameObject{Id = 2};

            Output.WriteLine("Adding game object...");
            _myWorld.Add(_firstObject); // Add the game object to the world
            _myWorld.Add(_secondObject);

            Output.WriteLine(ObjectInWorldBefore, _myWorld.GameObjectManager.Count);
            _myWorld.Process(); // Process the pending queue to add definitely the game object to the world
            Output.WriteLine(ObjectInWorldAfter, _myWorld.GameObjectManager.Count);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            Output.WriteLine("Removing game object...");
            _myWorld.Remove(_firstObject); // Remove the game object from the world
            _myWorld.Remove(_secondObject);

            Output.WriteLine(ObjectInWorldBefore, _myWorld.GameObjectManager.Count);
            _myWorld.Process(); // Process the pending queue to remove definitely the game object to the world
            Output.WriteLine(ObjectInWorldAfter, _myWorld.GameObjectManager.Count);
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
