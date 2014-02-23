using Common;

#if WINDOWS
using Common.Windows;
#endif

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using GameObjectsManager.Components;

using Pulsar.Components;

namespace GameObjectsManager
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Demo : Game
    {
        #region Fields

        private const string OddEventCountBefore = "{0} even game object / {1} odd game object before Process()";
        private const string OddEventCountAfter = "{0} even game object / {1} odd game object after Process()";

        private World _myWorld;
        private MyGameObjectManager _myObjectManager;

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

        protected override void Initialize()
        {
#if WINDOWS
            ConsoleHelper.OpenConsole();
#endif

            base.Initialize();
        }

        protected override void Dispose(bool disposing)
        {
#if WINDOWS
            ConsoleHelper.CloseConsole();
#endif

            base.Dispose(disposing);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            _myWorld = new World();
            _myObjectManager = new MyGameObjectManager(); // Create the custom game object manager
            _myWorld.AddManager(_myObjectManager); // Add it to the world

            for (int i = 0; i < 5; i++)
            {
                GameObject obj = new GameObject {Id = i};
                _myWorld.Add(obj);
            }

            Output.WriteLine(OddEventCountBefore, _myObjectManager.EventCount, _myObjectManager.OddCount);
            _myWorld.Process();
            Output.WriteLine(OddEventCountAfter, _myObjectManager.EventCount, _myObjectManager.OddCount);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            for (int i = 0; i < 5; i++)
            {
                GameObject obj = _myWorld.GameObjectManager.Get(i);
                if(obj != null)
                    _myWorld.Remove(obj);
            }

            Output.WriteLine(OddEventCountBefore, _myObjectManager.EventCount, _myObjectManager.OddCount);
            _myWorld.Process();
            Output.WriteLine(OddEventCountAfter, _myObjectManager.EventCount, _myObjectManager.OddCount);

            _myWorld.RemoveManager(_myObjectManager); // Remove the custom game object manager
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
