using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System;

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

        // Audio 
        List<Song> gameSongs;
        Song menuMusic;
        SoundEffect menuSelectSound;

        bool menuFirstFrame = true;
        bool startGameMusic = true;

        //used for randomly picking a song for each playthrough
        Random rn;

        GameState gameState;
        GameState previousGameState;

        bool paused = false; //used to determine which 
                             //game logic is run based on if game is paused or not

        bool bothPlayersDead; //used to determine if game over


        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Testing stuff
        Tile testPlatform = new Tile(false, true, false, false, "ffff");
        Tile secondTestPlatform = new Tile(false, true, false, false, "ffff");
        Tile testWall = new Tile(false, false, false, true, "ffff");
        SpriteFont testFont;

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

        Texture2D playerOneTexture;
        Texture2D playerTwoTexture;
        Texture2D defaultEnemySprite;

        //Gamepad Support
        GamePadCapabilities capabilities1;
        GamePadCapabilities capabilities2;

        GamePadState previousGp1;
        GamePadState gp1;
        bool workingGamepad1;

        GamePadState previousGp2;
        GamePadState gp2;
        bool workingGamepad2;

        //Level & Tile Information
        List<Texture2D> tileTextures = new List<Texture2D>();
        LevelMapCoordinator LvlCoordinator;
        List<string[]> levelMap;
        Texture2D defaultTile;
        List<Tile> tilesOnScreen = new List<Tile>();
        Texture2D tileBrick;
        Texture2D tileDirt;
        Texture2D tileGrass;
        Texture2D tileMoss;

        //Buttons for Menu
        Button playButton;
        Button optionsButton;
        Button exitButton;

        //Menu images
        Texture2D mainMenuTexture;
        Texture2D cursorTexture;

        //Menu navigation using 2d array
        Button[,] menuChoices;
        int menuRow;
        int menuColumn;

        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            //temporary
            testPlatform.Hitbox = new Rectangle(0, 400, 1000, 100);
            secondTestPlatform.Hitbox = new Rectangle(400, 600, 1000, 100);
            testWall.Hitbox = new Rectangle(300, 500, 100, 500);
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            gameSongs = new List<Song>();

            rn = new Random();
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

            levelMap = new List<string[]>();
            levelMap.Add(new string[2]);
            enemyList = new List<Enemy>();  //when you instantiate any enemy, add it to this list

            bothPlayersDead = false;

            tilesOnScreen.Add(testPlatform);
            tilesOnScreen.Add(secondTestPlatform);
            tilesOnScreen.Add(testWall);

            previousGp1 = new GamePadState();
            gp1 = new GamePadState();

            previousGp2 = new GamePadState();
            gp2 = new GamePadState();

            #region Menu stuff
            //Menu textures
            mainMenuTexture = Content.Load<Texture2D>("menuImages\\partyTowerMenuBG");
            cursorTexture = Content.Load<Texture2D>("menuImages\\selector");

            //Menu buttons
            playButton = new Button(Content.Load<Texture2D>("menuImages\\playUnselected"), Content.Load<Texture2D>("menuImages\\playSelected"));
            optionsButton = new Button(Content.Load<Texture2D>("menuImages\\optionsUnselected"), Content.Load<Texture2D>("menuImages\\optionsSelected"));
            exitButton = new Button(Content.Load<Texture2D>("menuImages\\exitUnselected"), Content.Load<Texture2D>("menuImages\\exitSelected"));

            //Menu button locations and areas
            playButton.StartLocation = new Point(graphics.PreferredBackBufferWidth * 4 / 9 + (int)Nudge(true, 1), graphics.PreferredBackBufferHeight * 2 / 3 - (int)Nudge(false, 1));
            optionsButton.StartLocation = new Point(graphics.PreferredBackBufferWidth * 4 / 9 + (int)Nudge(true, 1), graphics.PreferredBackBufferHeight * 7 / 9 - (int)Nudge(false, 1));
            exitButton.StartLocation = new Point(graphics.PreferredBackBufferWidth * 4 / 9 + (int)Nudge(true, 1), graphics.PreferredBackBufferHeight * 8 / 9 + (int)Nudge(false, 1));

            playButton.Area = new Rectangle(playButton.X, playButton.Y, graphics.PreferredBackBufferWidth / 12, graphics.PreferredBackBufferHeight / 10);
            optionsButton.Area = new Rectangle(optionsButton.X, optionsButton.Y, graphics.PreferredBackBufferWidth / 10, graphics.PreferredBackBufferHeight / 10);
            exitButton.Area = new Rectangle(exitButton.X, exitButton.Y, graphics.PreferredBackBufferWidth / 12, graphics.PreferredBackBufferHeight / 12);

            gameState = GameState.Menu;

            //arranging the buttons in the correct order
            menuChoices = new Button[3, 1];
            menuChoices[0, 0] = playButton;
            menuChoices[1, 0] = optionsButton;
            menuChoices[2, 0] = exitButton;
            menuRow = 0;
            menuColumn = 0;
            #endregion

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
            defaultEnemySprite = Content.Load<Texture2D>("enemy");
            testFont = Content.Load<SpriteFont>("DefaultText");
            

            #region Tile Textures
            //################################################
            //########### Add Tile Textures Here #############
            //################################################
            tileBrick = Content.Load<Texture2D>("brick");
            tileDirt = Content.Load<Texture2D>("dirt");
            tileGrass = Content.Load<Texture2D>("grass");
            tileMoss = Content.Load<Texture2D>("moss");
            defaultTile = Content.Load<Texture2D>("default");


            //################################################
            //########## Add to Texture Lists Here ###########
            //################################################
            tileTextures.Add(tileBrick);
            tileTextures.Add(tileDirt);
            tileTextures.Add(tileGrass);
            tileTextures.Add(tileMoss);
            //tileTextures.Add(tileEnemy);
            tileTextures.Add(defaultTile);
            #endregion

            //Instantiate level coordinator and add the starter tiles to the list
            LvlCoordinator = new LevelMapCoordinator("enemyBugtest", tileTextures, defaultEnemySprite);
            foreach (Tile currentTile in LvlCoordinator.CurrentMap)
            {
                tilesOnScreen.Add(currentTile);
            }

            // Test Enemy Manually Made
            enemyList.Add(new Enemy(EnemyType.Stationary, new Rectangle(1200, 500, 64, 64), defaultEnemySprite, 500));

            levelMap[0] = (LvlCoordinator.PathManagerMap);
            

            //had to move this to load content because the textures are null if you try to instantiate a player in Initialize
            #region Player-Initalization
            players = new List<Player>();
            playerOne = new Player(1, 0, playerOneTexture, new Rectangle(300, 300, 64, 64), Color.White, Content, 0);
            playerTwo = new Player(2, 1, playerTwoTexture, new Rectangle(400, 300, 64, 64), Color.Red, Content, 1);

            playerOne.BindableKb.Add("left", Keys.A);
            playerOne.BindableKb.Add("right", Keys.D);
            playerOne.BindableKb.Add("up", Keys.W); //added for menu navigation
            playerOne.BindableKb.Add("jump", Keys.Space);
            playerOne.BindableKb.Add("roll", Keys.LeftShift);
            playerOne.BindableKb.Add("downDash", Keys.S);
            playerOne.BindableKb.Add("pause", Keys.P);
            playerOne.BindableKb.Add("throw", Keys.C);

            //currently only player 1 can navigate menu, might change this if we do rebindable buttons
            playerTwo.BindableKb.Add("left", Keys.Left);
            playerTwo.BindableKb.Add("right", Keys.Right);
            playerTwo.BindableKb.Add("jump", Keys.Up);
            playerTwo.BindableKb.Add("roll", Keys.RightControl);
            playerTwo.BindableKb.Add("downDash", Keys.Down);
            playerTwo.BindableKb.Add("pause", Keys.P);
            playerTwo.BindableKb.Add("throw", Keys.RightShift);

            coopManager = new Coop_Manager(playerOne, playerTwo, Content);
            pathManager = new PathManager(GraphicsDevice.Viewport);
            cameraLimiters = new CameraLimiters(GraphicsDevice.Viewport, playerOne.Hitbox);
            camera = new Dynamic_Camera(GraphicsDevice.Viewport, playerOne.Width, cameraLimiters.MaxWidthDistance, pathManager.WidthConstant);
            camera.SetMapEdge(LvlCoordinator.MapEdge);

            players.Add(playerOne);
            players.Add(playerTwo);
            #endregion Player-Initalization


            //sound stuff
            gameSongs.Add(Content.Load<Song>("sound/gamemusic1"));
            gameSongs.Add(Content.Load<Song>("sound/gamemusic2"));
            gameSongs.Add(Content.Load<Song>("sound/gamemusic3"));


            MediaPlayer.Volume = .5f;
            MediaPlayer.IsRepeating = true;

            menuMusic = Content.Load<Song>("sound/menumusic");
            menuSelectSound = Content.Load<SoundEffect>("sound/menuselect");
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
                        #region UPDATE PLAYER
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

                        //throw gets priority over bounce, so that way a player can be thrown if they roll into a throw

                        //Player bouncing
                        coopManager.CheckForRollAndThenBounce();

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

                        //check collision with each tile for each player
                        foreach (Tile t in tilesOnScreen)
                        {
                            playerOne.CollisionCheck(t, workingGamepad1);
                            playerTwo.CollisionCheck(t, workingGamepad2);
                        }
                        #endregion

                        #region CAMERA / UPDATE A* MAP / ACTIVE GAMEOBJECTS

                        //only adjust players if they are beyond the camera limiter's designated max values
                        if (Math.Abs(playerOne.X - playerTwo.X) > cameraLimiters.MaxWidthDistance || Math.Abs(playerOne.Y - playerTwo.Y) > cameraLimiters.MaxHeightDistance)
                        {
                            tempRects = cameraLimiters.RepositionPlayers(playerOne.Hitbox, playerTwo.Hitbox, playerOne.PreviousHitbox, playerTwo.PreviousHitbox); //get the adjusted rectangles
                            for (int i = 0; i < 2; i++)
                            {
                                players[i].Hitbox = tempRects[i]; //adjust each hitbox accordingly
                            }
                        }

                        // Update A* Map of current players
                        pathManager.UpdatePlayersOnMap(levelMap[0], playerOne.Hitbox, playerTwo.Hitbox);

                        // Update Camera's
                        camera.UpdateCamera(GraphicsDevice.Viewport, playerOne.Hitbox, playerTwo.Hitbox);

                        if (enemyList != null)
                        {
                            foreach (Enemy e in enemyList)
                            {
                                e.IsDrawn = camera.IsDrawn(e.Hitbox);
                                e.IsActive = camera.IsUpdated(e.Hitbox);
                            }
                        }

                        #endregion

                        #region UPDATE ENEMY

                        foreach (Enemy currentEnemy in enemyList)
                        {
                            if (currentEnemy.Type == EnemyType.Alive && currentEnemy.IsActive) //only type of enemy with movement
                            {
                                currentEnemy.UpdateEnemy(playerOne, playerTwo, pathManager.Following(currentEnemy));
                                foreach (Tile t in tilesOnScreen)
                                {
                                    currentEnemy.CollisionCheck(t);
                                }
                            }
                            //there shouldn't be any other type of movement
                        }

                        #endregion

                        #region PLAYER-ENEMY COLLISIONS

                        //might need to optimize this if there are too many enemies
                        //check player colliding with enemy
                        foreach (Player currentPlayer in players)
                        {
                            foreach (Enemy currentEnemy in enemyList)
                            {
                                if (currentEnemy.Hitpoints > 0 && currentPlayer.PlayerState != PlayerState.Die)
                                {
                                    currentPlayer.CheckColliderAgainstEnemy(currentEnemy);
                                }
                            }
                        }

                        #endregion

                        #region PLAYERS DYING

                        //Player(s) dying
                        if (!coopManager.CheckAndRespawnPlayer(gameTime)) //if this call is false, that means both players are dead
                        {
                            bothPlayersDead = true;
                        }
                        else //both players not dead
                        {
                            bothPlayersDead = false;
                        }

                        #endregion

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
                    //only create/fill the array if this is the first frame of the menu state
                    paused = false;
                    if (menuFirstFrame)
                    {
                        menuChoices = new Button[3, 1];
                        menuChoices[0, 0] = playButton;
                        menuChoices[1, 0] = optionsButton;
                        menuChoices[2, 0] = exitButton;

                        MediaPlayer.Play(menuMusic);

                        menuRow = 0;
                        menuColumn = 0;

                        MediaPlayer.Play(menuMusic);
                        menuFirstFrame = false;
                    }
                    //navigate the menu and update game state based on selection
                    NavigateMenu(workingGamepad1, menuRow, menuColumn);

                    //anything else
                    break;

                case GameState.Options:
                    if (SingleKeyPress(Keys.Tab))
                    {
                        if (paused)
                        {
                            gameState = GameState.Game;
                        }
                        else
                        {
                            gameState = GameState.Menu;
                            menuFirstFrame = true;
                        }

                    }
                    //Options stuff
                    break;

                case GameState.Game:
                    if (startGameMusic)
                    {
                        MediaPlayer.Play(gameSongs[rn.Next(0, 3)]);
                        startGameMusic = false;
                    }
                    if (bothPlayersDead) //if both players are dead during overlaping intervals, game over
                    {
                        MediaPlayer.Stop();
                        gameState = GameState.GameOver;
                    }
                    foreach (Player player in players) //any player can do this
                    {
                        if (SingleKeyPress(player.BindableKb["pause"]))
                        {
                            menuSelectSound.Play();
                            if (paused)
                            {
                                paused = false;
                                MediaPlayer.Resume();
                            }
                            else
                            {
                                paused = true;
                                MediaPlayer.Pause();
                            }
                            break;
                        }
                    }

                    if (paused) //navigate when paused
                    {
                        if (SingleKeyPress(Keys.Back)) //press backspace to go back to main menu
                        {
                            menuFirstFrame = true;
                            startGameMusic = true;
                            gameState = GameState.Menu;
                        }
                        if (SingleKeyPress(Keys.Tab)) //press Tab to go to options
                        {
                            gameState = GameState.Options;
                        }
                    }
                    break;

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
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
                    GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;                  //sets interpolation to nearest neighbor
                    //draw the main menu background image
                    spriteBatch.Draw(mainMenuTexture, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);

                    //draw each button
                    foreach (Button currentButton in menuChoices)
                    {
                        spriteBatch.Draw(currentButton.DrawnTexture, currentButton.Area, Color.White);
                        if (currentButton.IsHighlighted)
                        {
                            //draw cursor next to button
                            spriteBatch.Draw(cursorTexture, new Rectangle(currentButton.X - (int)Nudge(true, 3), currentButton.Y + (int)Nudge(false, 3),
                                graphics.PreferredBackBufferWidth / 40, graphics.PreferredBackBufferHeight / 40), Color.White);
                        }
                    }

                    spriteBatch.End();
                    break;

                case GameState.Options:
                    spriteBatch.Begin();
                    // 
                    spriteBatch.End();
                    break;

                case GameState.Game:
                    // (NULL, NULL, NULL, NULL, CAMERA.TRANSFORM IS HOW YOU USE THE CAMERA IN THE GAME! :D)
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, null, null, null, null, camera.Transform); //setup for keeping pixel art nice
                    GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;                                                //sets interpolation to nearest neighbor

                    if (paused)
                    {
                        spriteBatch.DrawString(testFont, "PAUSED", new Vector2(camera.CameraCenter.X - 100, camera.CameraCenter.Y - 20), Color.White);
                    }

                    //Drawing each player
                    foreach (Player currentPlayer in players)
                    {
                        if (currentPlayer.PlayerState != PlayerState.Die)
                        {
                            currentPlayer.Draw(spriteBatch);
                        }

                    }


                    foreach (Enemy e in enemyList)
                    {
                        if (e.Hitpoints > 0)
                        {
                            e.Draw(spriteBatch);
                        }
                    }

                    //a random rectangle, for testing onyl
                    spriteBatch.Draw(playerOneTexture, testPlatform.Hitbox, Color.Black);
                    spriteBatch.Draw(playerTwoTexture, testWall.Hitbox, Color.Red);
                    spriteBatch.Draw(playerOneTexture, secondTestPlatform.Hitbox, Color.Black);

                    //debugging text for bug stomping
                    if (playerOne.IsDebugging)
                    {
                        spriteBatch.DrawString(testFont, "Horizontal Velocity: " + playerOne.HorizontalVelocity, new Vector2(camera.CameraCenter.X - 900, camera.CameraCenter.Y - 500), Color.Cyan);
                        spriteBatch.DrawString(testFont, "Vertical Velocity: " + playerOne.VerticalVelocity, new Vector2(camera.CameraCenter.X - 900, camera.CameraCenter.Y - 465), Color.Cyan);
                        spriteBatch.DrawString(testFont, "Player State: " + playerOne.PlayerState, new Vector2(camera.CameraCenter.X - 900, camera.CameraCenter.Y - 430), Color.Cyan);
                        spriteBatch.DrawString(testFont, "Facing right?: " + playerOne.IsFacingRight, new Vector2(camera.CameraCenter.X - 900, camera.CameraCenter.Y - 395), Color.Cyan);
                    }
                    if (playerTwo.IsDebugging)
                    {
                        spriteBatch.DrawString(testFont, "Horizontal Velocity: " + playerTwo.HorizontalVelocity, new Vector2(camera.CameraCenter.X + 300, camera.CameraCenter.Y - 500), Color.Red);
                        spriteBatch.DrawString(testFont, "Vertical Velocity: " + playerTwo.VerticalVelocity, new Vector2(camera.CameraCenter.X + 300, camera.CameraCenter.Y - 465), Color.Red);
                        spriteBatch.DrawString(testFont, "Player State: " + playerTwo.PlayerState, new Vector2(camera.CameraCenter.X + 300, camera.CameraCenter.Y - 430), Color.Red);
                        spriteBatch.DrawString(testFont, "Facing right?: " + playerTwo.IsFacingRight, new Vector2(camera.CameraCenter.X + 300, camera.CameraCenter.Y - 395), Color.Red);
                    }

                    //drawing out level tiles
                    LvlCoordinator.Draw(spriteBatch);

                    if (playerTwo.IsDebugging)
                    {
                        spriteBatch.DrawString(testFont, "Horizontal Velocity: " + enemyList[0].HorizontalVelocity, new Vector2(camera.CameraCenter.X + 300, camera.CameraCenter.Y + 260), Color.Yellow);
                        spriteBatch.DrawString(testFont, "Vertical Velocity: " + enemyList[0].VerticalVelocity, new Vector2(camera.CameraCenter.X + 300, camera.CameraCenter.Y + 300), Color.Yellow);
                        spriteBatch.DrawString(testFont, "Enemy State: " + enemyList[0].EnemyState, new Vector2(camera.CameraCenter.X + 300, camera.CameraCenter.Y + 340), Color.Yellow);
                        spriteBatch.DrawString(testFont, "Walking State: " + enemyList[0].WalkingState, new Vector2(camera.CameraCenter.X + 300, camera.CameraCenter.Y + 380), Color.Yellow);
                        spriteBatch.DrawString(testFont, "Target: " + enemyList[0].TargetDebug, new Vector2(camera.CameraCenter.X + 300, camera.CameraCenter.Y + 420), Color.Yellow);
                        spriteBatch.DrawString(testFont, "TargetLoc: " + pathManager.TargetLocation, new Vector2(camera.CameraCenter.X + 300, camera.CameraCenter.Y + 460), Color.Yellow);
                        spriteBatch.DrawString(testFont, "C Map: \n" + pathManager.CorrectMap, new Vector2(camera.CameraCenter.X + -900, camera.CameraCenter.Y - 300), Color.Yellow);
                    }

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
        /// <summary>
        /// use to nudge elements on screen around by a single percent of the screen size
        /// </summary>
        /// <param name="horizontal"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public double Nudge(bool horizontal, double amount)
        {
            if (horizontal)
            {
                return graphics.PreferredBackBufferWidth * (amount / 100);
            }
            else
            {
                return graphics.PreferredBackBufferHeight * (amount / 100);
            }
        }

        /// <summary>
        /// uses 2d array variables (row/column) to navigate, then calls helper method for button selection
        /// </summary>
        /// <param name="hasGamepad"></param>
        /// <param name="beginningRow"></param>
        /// <param name="beginningColumn"></param>
        public void NavigateMenu(bool hasGamepad, int beginningRow, int beginningColumn)
        {
            //start menu selection at correct spot
            int currentRow = beginningRow;
            int currentColumn = beginningColumn;

            if (hasGamepad)
            {
                //only set the button highlight to false if there is appropriate player input
                if (SingleKeyPress(playerOne.BindableKb["up"]) || playerOne.SingleButtonPress(Buttons.DPadUp) || playerOne.GamepadUp())
                {
                    menuSelectSound.Play();
                    menuChoices[currentRow, currentColumn].IsHighlighted = false;

                    if (currentRow == 0)
                    {
                        currentRow = menuChoices.GetLength(0) - 1; //loop around
                    }
                    else
                    {
                        currentRow--;
                    }
                }
                else if (SingleKeyPress(playerOne.BindableKb["downDash"]) || playerOne.SingleButtonPress(Buttons.DPadDown) || playerOne.GamepadDown())
                {
                    menuSelectSound.Play();
                    menuChoices[currentRow, currentColumn].IsHighlighted = false;

                    if (currentRow == menuChoices.GetLength(0) - 1)
                    {
                        currentRow = 0; //loop around
                    }
                    else
                    {
                        currentRow++;
                    }
                }
                else if (SingleKeyPress(playerOne.BindableKb["left"]) || playerOne.SingleButtonPress(Buttons.DPadLeft) || playerOne.GamepadLeft())
                {
                    menuSelectSound.Play();
                    menuChoices[currentRow, currentColumn].IsHighlighted = false;
                    if (currentColumn == 0)
                    {
                        currentColumn = menuChoices.GetLength(1) - 1; //loop around
                    }
                    else
                    {
                        currentColumn--;
                    }
                }
                else if (SingleKeyPress(playerOne.BindableKb["right"]) || playerOne.SingleButtonPress(Buttons.DPadRight) || playerOne.GamepadRight())
                {
                    menuSelectSound.Play();
                    menuChoices[currentRow, currentColumn].IsHighlighted = false; 
                    if (currentColumn == menuChoices.GetLength(1) - 1) //loop around
                    {
                        currentColumn = 0;
                    }
                    else
                    {
                        currentColumn++;
                    }
                }
            }
            else
            {
                if (SingleKeyPress(playerOne.BindableKb["up"]))
                {
                    menuSelectSound.Play();
                    menuChoices[currentRow, currentColumn].IsHighlighted = false;
                    if (currentRow == 0)
                    {
                        currentRow = menuChoices.GetLength(0) - 1; //loop around
                    }
                    else
                    {
                        currentRow--;
                    }
                }
                else if (SingleKeyPress(playerOne.BindableKb["downDash"]))
                {
                    menuSelectSound.Play();
                    menuChoices[currentRow, currentColumn].IsHighlighted = false;

                    if (currentRow == menuChoices.GetLength(0) - 1)
                    {
                        currentRow = 0; //loop around
                    }
                    else
                    {
                        currentRow++;
                    }
                }
                else if (SingleKeyPress(playerOne.BindableKb["left"]))
                {
                    menuSelectSound.Play();
                    menuChoices[currentRow, currentColumn].IsHighlighted = false;
                    if (currentColumn == 0)
                    {
                        currentColumn = menuChoices.GetLength(1) - 1; //loop around
                    }
                    else
                    {
                        currentColumn--;
                    }
                }
                else if (SingleKeyPress(playerOne.BindableKb["right"]))
                {
                    menuSelectSound.Play();
                    if (currentColumn == menuChoices.GetLength(1) - 1)
                    {
                        currentColumn = 0; //loop around
                    }
                    else
                    {
                        currentColumn++;
                    }
                    menuChoices[currentRow, currentColumn].IsHighlighted = false;
                }
            }
            //always highlight the correct button
            menuChoices[currentRow, currentColumn].IsHighlighted = true;

            //used so that the array is at the correct indices in the next frame
            menuRow = currentRow;
            menuColumn = currentColumn;

            ButtonSelection(hasGamepad, currentRow, currentColumn);
        }

        /// <summary>
        /// helper method for NavigateMenu that determines which gamestate to switch to based on selected button
        /// </summary>
        /// <param name="hasGamepad"></param>
        /// <param name="currentRow"></param>
        /// <param name="currentColumn"></param>
        public void ButtonSelection(bool hasGamepad, int currentRow, int currentColumn)
        {
            if (hasGamepad)
            {
                if (SingleKeyPress(Keys.Enter) || SingleKeyPress(playerOne.BindableKb["jump"]) || playerOne.SingleButtonPress(Buttons.A) || playerOne.SingleButtonPress(Buttons.Start))
                {
                    menuSelectSound.Play();
                    if (menuChoices[currentRow, currentColumn].Equals(playButton))
                    {
                        gameState = GameState.Game;
                    }
                    else if (menuChoices[currentRow, currentColumn].Equals(optionsButton))
                    {
                        gameState = GameState.Options;
                    }
                    else if (menuChoices[currentRow, currentColumn].Equals(exitButton))
                    {
                        Exit();
                    }
                }
            }
            else
            {
                if (SingleKeyPress(Keys.Enter) || SingleKeyPress(playerOne.BindableKb["jump"]))
                {
                    menuSelectSound.Play();
                    if (menuChoices[currentRow, currentColumn].Equals(playButton))
                    {
                        gameState = GameState.Game;
                    }
                    else if (menuChoices[currentRow, currentColumn].Equals(optionsButton))
                    {
                        gameState = GameState.Options;
                    }
                    else if (menuChoices[currentRow, currentColumn].Equals(exitButton))
                    {
                        Exit();
                    }
                }

            }
        }
    }
}