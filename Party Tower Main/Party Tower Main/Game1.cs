using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Party_Tower_Main
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        #region GameStates

        enum GameState
        {
            Menu,
            Options,
            Game,
            LoadLevel,
            Pause,
            GameOver
        }

        #endregion

        #region Fields

        GameState gameState;
        CameraLimiters cameraLimiters;
        Dynamic_Camera camera;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            gameState = GameState.Menu;
            cameraLimiters = new CameraLimiters(GraphicsDevice.Viewport, new Rectangle(0, 0, 64, 64));
            camera = new Dynamic_Camera(GraphicsDevice.Viewport, 32, cameraLimiters.MaxWidthDistance);      // Dummy Values that need changed
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            gameState = UpdateGameState();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            DrawRightGameState();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Helper Method that determines the New Game State after properly updating the current state of the game
        /// </summary>
        /// <returns></returns>
        private GameState UpdateGameState()
        {
            switch (gameState)
            {
                case GameState.Menu:
                    // gameState = Menu.Update();
                    return gameState;

                case GameState.Options:
                    // gameState = ??
                    return gameState;

                case GameState.Game:
                    // gameState = GameLoop.Update();
                    return gameState;

                case GameState.Pause:
                    // gameState = ?
                    return gameState;

                case GameState.GameOver:
                    // gameState = ?
                    return gameState;

                case GameState.LoadLevel:
                    // GameLoop.NextLevel();
                    // gameState = ?
                    return gameState;
            }

            return gameState;
        }

        /// <summary>
        /// Helper Method that determines what to draw to the screen based on this new GameState
        /// </summary>
        private void DrawRightGameState()
        {
            switch (gameState)
            {
                case GameState.Menu:
                    spriteBatch.Begin();
                    // Menu.DrawElements();
                    spriteBatch.End();
                    break;

                case GameState.Options:
                    spriteBatch.Begin();
                    // ?
                    spriteBatch.End();
                    break;

                case GameState.Game:
                    spriteBatch.Begin();
                    // GameLoop.DrawElements;
                    spriteBatch.End();
                    break;

                case GameState.Pause:
                    spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, 
                        null, null, null, null, camera.Transform);
                    // GameLoop.DrawElements();
                    spriteBatch.End();
                    spriteBatch.Begin();
                    // GameLoop.DrawPauseOverlay();
                    spriteBatch.End();
                    break;

                case GameState.GameOver:
                    spriteBatch.Begin();
                    // ?
                    spriteBatch.End();
                    break;

                case GameState.LoadLevel:
                    spriteBatch.Begin();
                    // ?
                    spriteBatch.End();
                    break;
            }
        }
    }
}
