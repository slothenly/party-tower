using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;


enum GameState
{
    Menu,
    Options,
    Game,
    GameOver,
    LoadLevel
}

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

        // Audio Stuff
        List<SoundEffect> soundEffects;
        List<Song> gameSongs;

        GameState gameState;
        CameraLimiters cameraLimiters;
        Dynamic_Camera camera;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        KeyboardState kb;
        KeyboardState previousKb;

        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            soundEffects = new List<SoundEffect>();
            gameSongs = new List<Song>();
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

            UpdateGameState();

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
        private void UpdateGameState()
        {
            previousKb = kb;
            kb = Keyboard.GetState();
            switch (gameState)
            {
                case GameState.Menu:
                    if (SingleKeyPress(Keys.Enter))
                    {
                        gameState = GameState.Game;
                    }
                    if (SingleKeyPress(Keys.Tab))
                    {
                        gameState = GameState.Options;
                    }
                    //anything else
                    break;

                case GameState.Options:
                    if (SingleKeyPress(Keys.Tab))
                    {
                        gameState = GameState.Game;
                    }
                    //Options stuff
                    break;

                case GameState.Game:
                    if (SingleKeyPress(Keys.P))
                    {
                        //Pause stuff
                    }
                    break;

                    //Don't need a state for pause

                case GameState.GameOver:
                    if (SingleKeyPress(Keys.Enter))
                    {
                        gameState = GameState.Menu;
                    }
                    break;

                    //Do we need this?
                case GameState.LoadLevel:
                    break;

            }
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
                    // 
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
        public bool SingleKeyPress(Keys pressedKey)
        {
            if (kb.IsKeyDown(pressedKey) && previousKb.IsKeyUp(pressedKey))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
