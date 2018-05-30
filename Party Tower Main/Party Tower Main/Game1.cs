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

        bool paused = false; //used to determine which 
                             //game logic is run based on if game is paused or not

        bool bothPlayersDead; //used to determine if game over


        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Shared keyboard
        KeyboardState kb;
        KeyboardState previousKb;

        //enemy list
        List<Enemy> enemyList;

        //Player fields
        Player playerOne;
        Player playerTwo;
        List<Player> players;
        Coop_Manager coopManager;

        //Enemy Fields
        PathManager pathManager;

        //Shared Fields
        CameraLimiters cameraLimiters;
        Dynamic_Camera camera;
        Rectangle[] tempRects;
        Viewport view; //used to update camera

        Texture2D playerOneTexture;
        Texture2D playerTwoTexture;

        //Gamepad Support
        GamePadCapabilities capabilities1;
        GamePadCapabilities capabilities2;

        GamePadState previousGp1;
        GamePadState gp1;
        bool workingGamepad1;

        GamePadState previousGp2;
        GamePadState gp2;
        bool workingGamepad2;

        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            
            //temporary
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.ApplyChanges();
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

            view = new Viewport();
            view.X = 0;
            view.Y = 0;
            view.Width = graphics.PreferredBackBufferWidth;
            view.Height = graphics.PreferredBackBufferHeight;

            gameState = GameState.Menu;
            enemyList = new List<Enemy>(); //when you instantiate any enemy, add it to this list

            bothPlayersDead = false;



            previousGp1 = new GamePadState();
            gp1 = new GamePadState();

            previousGp2 = new GamePadState();
            gp2 = new GamePadState();

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

            //Placeholder textures for now
            playerOneTexture = Content.Load<Texture2D>("white");
            playerTwoTexture = Content.Load<Texture2D>("white");

            //had to move this to load content because the textures are null if you try to instantiate a player in Initialize
            #region Player-Initalization
            players = new List<Player>();
            playerOne = new Player(1, 0, playerOneTexture, new Rectangle(300, 300, 64, 64), Color.White, Content, 0);
            playerTwo = new Player(2, 1, playerTwoTexture, new Rectangle(400, 300, 64, 64), Color.Red, Content, 1);

            playerOne.BindableKb.Add("left", Keys.A);
            playerOne.BindableKb.Add("right", Keys.D);
            playerOne.BindableKb.Add("jump", Keys.Space);
            playerOne.BindableKb.Add("roll", Keys.LeftShift);
            playerOne.BindableKb.Add("downDash", Keys.S);
            playerOne.BindableKb.Add("pause", Keys.P);
            playerOne.BindableKb.Add("throw", Keys.C);

            playerTwo.BindableKb.Add("left", Keys.Left);
            playerTwo.BindableKb.Add("right", Keys.Right);
            playerTwo.BindableKb.Add("jump", Keys.Up);
            playerTwo.BindableKb.Add("roll", Keys.RightControl);
            playerTwo.BindableKb.Add("downDash", Keys.Down);
            playerTwo.BindableKb.Add("pause", Keys.P);
            playerTwo.BindableKb.Add("throw", Keys.RightShift);

            coopManager = new Coop_Manager(playerOne, playerTwo);
            pathManager = new PathManager(GraphicsDevice.Viewport);
            cameraLimiters = new CameraLimiters(GraphicsDevice.Viewport, playerOne.Hitbox);
            camera = new Dynamic_Camera(GraphicsDevice.Viewport, playerOne.Width, cameraLimiters.MaxWidthDistance, pathManager.WidthConstant);

            players.Add(playerOne);
            players.Add(playerTwo);
            #endregion Player-Initalization

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

            previousKb = kb;
            kb = Keyboard.GetState();

            capabilities1 = GamePad.GetCapabilities(PlayerIndex.One);
            capabilities2 = GamePad.GetCapabilities(PlayerIndex.Two);

            //check if there is player one controller
            if (capabilities1.IsConnected)
            {
                if (!capabilities2.IsConnected) //if there is only 1 controller connected
                {
                    workingGamepad2 = false;
                }
                workingGamepad1 = true;
                previousGp1 = gp1;
                gp1 = GamePad.GetState(PlayerIndex.One);
            }

            //check if there is a player two controller
            if (capabilities2.IsConnected) //finish trying to get controller support to not crash, keep in mind possibility of only one controller
            {
                if (!capabilities1.IsConnected) //if there is only 1 controller connected
                {
                    workingGamepad1 = false;
                }
                workingGamepad2 = true;
                previousGp2 = gp2;
                gp2 = GamePad.GetState(PlayerIndex.Two);
            }



            UpdateGameState();

            //Write logic for each gameState in here
            switch (gameState)
            {
                case GameState.Game:
                    if (!paused) //do normal stuff
                    {
                        //ENEMY MOVEMENT
                        foreach (Enemy currentEnemy in enemyList)
                        {
                            if (currentEnemy.Type == EnemyType.Alive) //only type of enemy with movement
                            {
                                //currentEnemy.FiniteStateFollowing(pathManager.Target);
                            }
                            //there shouldn't be any other type of movement
                        }

                        //PLAYER MOVEMENT
                        //first adjust the needed coop manager states, then adjust states and movement of both players

                        //One Player Carrying another
                        if (playerOne.DownDashOn(playerTwo) || playerOne.InCarry) //Player one on top
                        {
                            coopManager.PlayerCarry(playerOne, playerTwo);
                        }
                        else if (playerTwo.DownDashOn(playerOne) || playerTwo.InCarry) //Player two on top
                        {
                            coopManager.PlayerCarry(playerTwo, playerOne);
                        }
                        else //if neither player is on top, then no player should be carrying
                        {
                            foreach (Player currentPlayer in players)
                            {
                                currentPlayer.Carrying = false;
                            }
                        }

                        //Player throwing
                        coopManager.CheckForThrowAndThenThrow();

                        if (workingGamepad1) //check if working gamepad, and call corresponding finitestate
                        {
                            playerOne.FiniteState(true);
                        }
                        else //no controller (so check keyboard input)
                        {
                            playerOne.FiniteState(false);
                        }
                        if (workingGamepad2) //check if working gamepad, and call corresponding finitestate
                        {
                            playerTwo.FiniteState(true);
                        }
                        else //no controller (so check keyboard input)
                        {
                            playerTwo.FiniteState(false);
                        }


                        //CAMERA LIMITERS
                        //DO CAMERA LIMITER STUFF HERE, BEFORE COLLISION, THIS WILL JUST OVERRITE MOVEMENT ALREADY DONE
                        tempRects = cameraLimiters.RepositionPlayers(playerOne.Hitbox, playerTwo.Hitbox, playerOne.PreviousHitbox, playerTwo.PreviousHitbox); //get the adjusted rectangles
                        for (int i = 0; i < 2; i++) 
                        {
                            players[i].Hitbox = tempRects[i]; //adjust each hitbox accordingly
                        }

                        //PLAYER/ENEMY COLLISION

                        //might need to optimize this if there are too many enemies
                        //check player colliding with enemy
                        foreach (Player currentPlayer in players)
                        {
                            foreach (Enemy currentEnemy in enemyList)
                            {
                                currentPlayer.CheckColliderAgainstEnemy(currentEnemy);
                            }
                        }

                        //check enemy colliding with player
                        foreach (Enemy currentEnemy in enemyList)
                        {
                            foreach (Player currentPlayer in players)
                            {
                                currentEnemy.CheckColliderAgainstPlayer(currentPlayer);
                            }

                        }

                        //Player(s) dying
                        if (!coopManager.CheckAndRespawnPlayer(gameTime)) //if this call is false, that means both players are dead
                        {
                            bothPlayersDead = true;
                        }
                        else //both players not dead
                        {
                            bothPlayersDead = false;
                        }

                        //UPDATE ENEMY
                        //UPDATE PLAYER not sure what to exactly update here

                        //DYNAMIC CAMERA / UPDATE CAMERA
                        camera.UpdateCamera(view, playerOne.Hitbox, playerTwo.Hitbox);


                    }
                    else //paused
                    {
                        //do stuff when paused
                    }
                    break;
            }

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
                    if (bothPlayersDead) //if both players are dead during overlaping intervals, game over
                    {
                        gameState = GameState.GameOver;
                    }
                    foreach (Player player in players) //any player can do this
                    {
                        if (SingleKeyPress(player.BindableKb["pause"]))
                        {
                            paused = !paused; //pause / unpause the game
                        }
                    }

                    if (paused) //navigate when paused
                    {
                        if (SingleKeyPress(Keys.Back)) //press backspace to go back to main menu
                        {
                            gameState = GameState.Menu;
                        }
                        if (SingleKeyPress(Keys.Tab)) //press Tab to go to options
                        {
                            gameState = GameState.Options;
                        }
                    }
                    break;

                    //Don't need a state for pause

                case GameState.GameOver:
                    if (SingleKeyPress(Keys.Enter))
                    {
                        gameState = GameState.Menu;
                    }
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
                    //Drawing each player
                    foreach(Player currentPlayer in players)
                    {
                        currentPlayer.Draw(spriteBatch);
                    }
                    //a random rectangle, for testing onyl
                    spriteBatch.Draw(playerOneTexture, new Rectangle(500, 500, 500, 500), Color.Black);
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
        /// <summary>
        /// Check if a key has been pressed for this frame only
        /// </summary>
        /// <param name="pressedKey"></param>
        /// <returns></returns>
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
        /// <summary>
        /// check if a button has been pressed for this frame only
        /// </summary>
        /// <param name="pressedButton"></param>
        /// <returns></returns>
        public bool SingleButtonPress(Buttons pressedButton)
        {
            if (gp1.IsButtonDown(pressedButton) && previousGp1.IsButtonUp(pressedButton))
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
