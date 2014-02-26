using Common;

#if WINDOWS
using Common.Windows;
#endif

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Components.Components;

using Pulsar.Components;

namespace Components
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Demo : Game
    {
        #region Fields

        private const string GameObjectComponentCount = "{0} has {1} component";

        private World _myWorld;

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

            GameObject simpleSoldier = new GameObject {Id = 1};
            GameObject armoredSoldier = new GameObject {Id = 2};
            simpleSoldier.Add(new HealthComponent()); // Create and add a component to a game object
            armoredSoldier.Add(new HealthComponent());
            armoredSoldier.Add(new ShieldComponent());

            Output.WriteLine("Component added...");
            Output.WriteLine(GameObjectComponentCount, "SimpleSoldier", simpleSoldier.Count);
            Output.WriteLine(GameObjectComponentCount, "ArmoredSoldier", armoredSoldier.Count);

            _myWorld.Add(simpleSoldier);
            _myWorld.Add(armoredSoldier);
            _myWorld.Process();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            GameObject simpleSoldier = _myWorld.GameObjectManager.Get(1);
            GameObject armoredSoldier = _myWorld.GameObjectManager.Get(2);
            simpleSoldier.Remove<HealthComponent>(); // Remove a component from a game object with a specified type
            armoredSoldier.Remove<HealthComponent>();
            armoredSoldier.Remove<ShieldComponent>();

            Output.WriteLine("Component removed...");
            Output.WriteLine(GameObjectComponentCount, "SimpleSoldier", simpleSoldier.Count);
            Output.WriteLine(GameObjectComponentCount, "ArmoredSoldier", armoredSoldier.Count);

            _myWorld.Remove(simpleSoldier);
            _myWorld.Remove(armoredSoldier);
            _myWorld.Process();
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
