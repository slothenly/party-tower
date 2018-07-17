using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System;
using System.IO;

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
        //Saving
        StreamReader textReader;
        StreamWriter textWriter;

        // Audio 
        List<Song> gameSongs;
        Song menuMusic;
        SoundEffect menuSelectSound;

        //first frames used to fill Button Array when the user goes to these gameStates
        bool menuFirstFrame = true;
        bool optionsFirstFrame = false;
        bool escapeFirstFrame = false;

        //used to prevent music from starting over and over again
        bool startGameMusic = true;
        bool startMenuMusic = true;

        //used for randomly picking a song for each playthrough
        Random rn;

        GameState gameState;

        //Mouse
        MouseState ms;
        MouseState previousMs;
        Rectangle mouseRect; //used to check for clicking on certain elements

        bool paused = false; //used to determine which 
                             //game logic is run based on if game is paused or not
        bool menuPaused = false;

        //used to determine which buttons/screens to draw/instantiate
        bool tryingToQuit = false;
        bool quitFirstFrame = false;

        bool bothPlayersDead; //used to determine if game over


        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Testing stuff
        Tile testPlatform = new Tile(false, true, false, false, null);
        Tile secondTestPlatform = new Tile(false, true, false, false, null);
        Tile testWall = new Tile(false, false, false, true, null);
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

        //Cake fields
        CakeManager cakeManager;
        Cake cake;

        //Ladder fields
        Ladder topladder;
        Ladder bottomLadder;
        Ladder normalLadder1;
        Ladder normalLadder2;
        Ladder normalLadder3;

        List<Ladder> ladders;

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
        Texture2D mainTileSheet;
        Texture2D tileBrick;
        Texture2D tileDirt;
        Texture2D tileGrass;
        Texture2D tileMoss;
        Level testLevel1;
        Level testLevel2;

        Map LevelMapCurrent;
        Map LevelMapOld;

        //Ladder textures
        Texture2D topLadderTexture;
        Texture2D bottomLadderTexture;
        Texture2D normalLadderTexture;

        //Buttons for Menu
        Button playButton;
        Button menuOptionsButton;
        Button menuExitButton;    

        //Menu images
        Texture2D mainMenuTexture;
        Texture2D cursorTexture;

        //Buttons for Options
        Button fullscreenButton;
        Button returnButton;
        Slider masterVolumeSlider;
        Slider soundEffectSlider;
        Slider musicSlider;

        //Buttons for Game
        Button resumeButton;
        Button gameOptionsButton;
        Button gameExitButton;

        //Exit buttons
        Button yesButton;
        Button noButton;

        //Game Over stuff
        Timer gameOverTimer = new Timer(3);


        //Menu navigation using 2d array
        Button[,] menuChoices;
        int menuRow;
        int menuColumn;
        bool lockedSelection = false; //lock selection when using a slider
        bool mouseScrollLock = false; //prevent mouse from selecting multiple elements at a time (by holding down the mouse button)

        int buttonHoldingCounter = 0; //used for delay for fast navigation
        int sliderHoldingCounter = 0;

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
            IsMouseVisible = true;
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
            playButton = new Button(Content.Load<Texture2D>("menuImages\\playNeutral"), Content.Load<Texture2D>("menuImages\\playHovered"));
            menuOptionsButton = new Button(Content.Load<Texture2D>("menuImages\\optionsNeutral"), Content.Load<Texture2D>("menuImages\\optionsHovered"));
            menuExitButton = new Button(Content.Load<Texture2D>("menuImages\\exitNeutral"), Content.Load<Texture2D>("menuImages\\exitHovered"));

            //Menu button locations and areas
            playButton.StartLocation = new Point(graphics.PreferredBackBufferWidth * 4 / 9 + Nudge(true, 1), graphics.PreferredBackBufferHeight * 2 / 3 - Nudge(false, 1));
            menuOptionsButton.StartLocation = new Point(graphics.PreferredBackBufferWidth * 4 / 9 + Nudge(true, 1), graphics.PreferredBackBufferHeight * 7 / 9 - Nudge(false, 1));
            menuExitButton.StartLocation = new Point(graphics.PreferredBackBufferWidth * 4 / 9 + Nudge(true, 1), graphics.PreferredBackBufferHeight * 8 / 9 + Nudge(false, 1));

            playButton.Area = new Rectangle(playButton.StartX, playButton.StartY, graphics.PreferredBackBufferWidth / 12, graphics.PreferredBackBufferHeight / 10);
            menuOptionsButton.Area = new Rectangle(menuOptionsButton.StartX, menuOptionsButton.StartY, graphics.PreferredBackBufferWidth / 10, graphics.PreferredBackBufferHeight / 10);
            menuExitButton.Area = new Rectangle(menuExitButton.StartX, menuExitButton.StartY, graphics.PreferredBackBufferWidth / 12, graphics.PreferredBackBufferHeight / 12);

            //Options buttons/sliders
            returnButton = new Button(Content.Load<Texture2D>("menuImages\\playNeutral"), Content.Load<Texture2D>("menuImages\\playHovered"));
            fullscreenButton = new Button(Content.Load<Texture2D>("menuImages\\optionsNeutral"), Content.Load<Texture2D>("menuImages\\optionsHovered"));
            masterVolumeSlider = new Slider(Content.Load<Texture2D>("menuImages\\exitNeutral"), Content.Load<Texture2D>("menuImages\\exitHovered"), 100);
            musicSlider = new Slider(Content.Load<Texture2D>("menuImages\\playNeutral"), Content.Load<Texture2D>("menuImages\\playHovered"), 100);
            soundEffectSlider = new Slider(Content.Load<Texture2D>("menuImages\\optionsNeutral"), Content.Load<Texture2D>("menuImages\\optionsHovered"), 100);

            //options buttons/sliders start locations
            returnButton.StartLocation = new Point(graphics.PreferredBackBufferWidth * 4 / 9 + Nudge(true, 1), graphics.PreferredBackBufferHeight / 3 - Nudge(false, 1));
            fullscreenButton.StartLocation = new Point(graphics.PreferredBackBufferWidth * 4 / 9 + Nudge(true, 1), graphics.PreferredBackBufferHeight * 4 / 9 - Nudge(false, 1));
            masterVolumeSlider.StartLocation = new Point(graphics.PreferredBackBufferWidth * 4 / 9 + Nudge(true, 1), graphics.PreferredBackBufferHeight * 5 / 9 + Nudge(false, 5));
            musicSlider.StartLocation = new Point(graphics.PreferredBackBufferWidth * 4 / 9 + Nudge(true, 1), graphics.PreferredBackBufferHeight * 5 / 9 + Nudge(false, 20));
            soundEffectSlider.StartLocation = new Point(graphics.PreferredBackBufferWidth * 4 / 9 + Nudge(true, 1), graphics.PreferredBackBufferHeight * 5 / 9 + Nudge(false, 30));

            //area of buttons in options
            returnButton.Area = new Rectangle(returnButton.StartX, returnButton.StartY, graphics.PreferredBackBufferWidth / 11, graphics.PreferredBackBufferHeight / 10);
            fullscreenButton.Area = new Rectangle(fullscreenButton.StartX, fullscreenButton.StartY, graphics.PreferredBackBufferWidth / 11, graphics.PreferredBackBufferHeight / 10);
            masterVolumeSlider.Area = new Rectangle(masterVolumeSlider.StartX, masterVolumeSlider.StartY, graphics.PreferredBackBufferWidth / 5, graphics.PreferredBackBufferHeight / 10);
            musicSlider.Area = new Rectangle(musicSlider.StartX, musicSlider.StartY, graphics.PreferredBackBufferWidth / 5, graphics.PreferredBackBufferHeight / 10);
            soundEffectSlider.Area = new Rectangle(soundEffectSlider.StartX, soundEffectSlider.StartY, graphics.PreferredBackBufferWidth / 5, graphics.PreferredBackBufferHeight / 10);

            //loading from save file
            textReader = new StreamReader("save.txt");

            //default for now, will use text files to save settings
            masterVolumeSlider.ReturnedValue = float.Parse(textReader.ReadLine());
            musicSlider.ReturnedValue = float.Parse(textReader.ReadLine());
            soundEffectSlider.ReturnedValue = float.Parse(textReader.ReadLine());

            textReader.Close();

            //set the positions of the sliderButtons on the actual sliders
            masterVolumeSlider.SetSliderButtonArea();
            musicSlider.SetSliderButtonArea();
            soundEffectSlider.SetSliderButtonArea();

            //game buttons
            resumeButton = new Button(Content.Load<Texture2D>("menuImages\\playNeutral"), Content.Load<Texture2D>("menuImages\\playHovered"));
            gameOptionsButton = new Button(Content.Load<Texture2D>("menuImages\\optionsNeutral"), Content.Load<Texture2D>("menuImages\\optionsHovered"));
            gameExitButton = new Button(Content.Load<Texture2D>("menuImages\\exitNeutral"), Content.Load<Texture2D>("menuImages\\exitHovered"));

            //game buttons start locations
            resumeButton.StartLocation = new Point(graphics.PreferredBackBufferWidth / 2 - Nudge(true, 5), graphics.PreferredBackBufferHeight / 3);
            gameOptionsButton.StartLocation = new Point(graphics.PreferredBackBufferWidth / 2 - Nudge(true, 5), graphics.PreferredBackBufferHeight / 3 + Nudge(false, 15));
            gameExitButton.StartLocation = new Point(graphics.PreferredBackBufferWidth / 2 - Nudge(true, 5), graphics.PreferredBackBufferHeight / 3 + Nudge(false, 30));

            //area of buttons in game escape screen
            resumeButton.Area = new Rectangle(resumeButton.StartX, resumeButton.StartY, graphics.PreferredBackBufferWidth / 10, graphics.PreferredBackBufferHeight / 10);
            gameOptionsButton.Area = new Rectangle(gameOptionsButton.StartX, gameOptionsButton.StartY, graphics.PreferredBackBufferWidth / 10, graphics.PreferredBackBufferHeight / 10);
            gameExitButton.Area = new Rectangle(gameExitButton.StartX, gameExitButton.StartY, graphics.PreferredBackBufferWidth / 10, graphics.PreferredBackBufferHeight / 10);

            //Exit Buttons
            yesButton = new Button(Content.Load<Texture2D>("menuImages\\exitNeutral"), Content.Load<Texture2D>("menuImages\\exitHovered"));
            noButton = new Button(Content.Load<Texture2D>("menuImages\\playNeutral"), Content.Load<Texture2D>("menuImages\\playHovered"));

            //exit buttons start locations
            noButton.StartLocation = new Point(graphics.PreferredBackBufferWidth / 3 - Nudge(true, 5), graphics.PreferredBackBufferHeight / 2 - Nudge(false, 3));
            yesButton.StartLocation = new Point(graphics.PreferredBackBufferWidth * 2 / 3 - Nudge(true, 5), graphics.PreferredBackBufferHeight / 2 - Nudge(false, 3));

            //exit buttons area
            noButton.Area = new Rectangle(noButton.StartX, noButton.StartY, graphics.PreferredBackBufferWidth / 10, graphics.PreferredBackBufferHeight / 10);
            yesButton.Area = new Rectangle(yesButton.StartX, yesButton.StartY, graphics.PreferredBackBufferWidth / 10, graphics.PreferredBackBufferHeight / 10);


            gameState = GameState.Menu;

            //arranging the buttons in the correct order
            menuChoices = new Button[3, 1];
            menuChoices[0, 0] = playButton;
            menuChoices[1, 0] = menuOptionsButton;
            menuChoices[2, 0] = menuExitButton;
            menuRow = 0;
            menuColumn = 0;

            //adjust volumes using formula based on sliders
            MediaPlayer.Volume = (masterVolumeSlider.ReturnedValue / 100) * (musicSlider.ReturnedValue / 100);
            SoundEffect.MasterVolume = (masterVolumeSlider.ReturnedValue / 100) * (musicSlider.ReturnedValue / 100);
            MediaPlayer.IsRepeating = true;

            //Ladder testing
            bottomLadder = new Ladder(false, true, 1200, 500);
            normalLadder1 = new Ladder(false, false, 1200, 500 - 1920 / 16);
            normalLadder2 = new Ladder(false, false, 1200, 500 - ((1920 / 16) * 2));
            normalLadder3 = new Ladder(false, false, 1200, 500 - ((1920 / 16) * 3));
            topladder = new Ladder(true, false, 1200, 500 - ((1920 / 16) * 4));


            ladders = new List<Ladder>();

            ladders.Add(bottomLadder);
            ladders.Add(normalLadder1);
            ladders.Add(normalLadder2);
            ladders.Add(normalLadder3);
            ladders.Add(topladder);
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

            topLadderTexture = playerOneTexture;
            bottomLadderTexture = playerTwoTexture;
            normalLadderTexture = playerOneTexture;

            #region Tile Textures
            //########### Add Tile Textures Here #############
            tileBrick = Content.Load<Texture2D>("brick");
            tileDirt = Content.Load<Texture2D>("dirt");
            tileGrass = Content.Load<Texture2D>("grass");
            tileMoss = Content.Load<Texture2D>("moss");
            defaultTile = Content.Load<Texture2D>("default");
            mainTileSheet = Content.Load<Texture2D>(@"textures\basicTileSheet");

            //########## Add to Texture Lists Here ###########
            tileTextures.Add(tileBrick);
            tileTextures.Add(tileDirt);
            tileTextures.Add(tileGrass);
            tileTextures.Add(tileMoss);
            tileTextures.Add(defaultTile);
            #endregion

            //Instantiate level coordinator and add the starter tiles to the list
            LvlCoordinator = new LevelMapCoordinator("tileOrientationTest", tileTextures, defaultEnemySprite, mainTileSheet);

            //Actually add in levels and connect them on the map here
            Tile tempMeasuringStick = new Tile(false, false, false, false, mainTileSheet);
            LevelMapCurrent = new Map(tempMeasuringStick);
            Tile[,] tempHolder = new Tile[9, 16];
            tempHolder = LvlCoordinator.UpdateMapFromPath("tileOrientationTest");
            testLevel1 = new Level(tempHolder);
            testLevel2 = new Level(LvlCoordinator.UpdateMapFromPath("tileOrientationTest"));
            LevelMapCurrent.AddLevel(testLevel1);
            LevelMapCurrent.AddLevel(testLevel2);
            LevelMapCurrent.PlaceRight(LevelMapCurrent.Root);

            LevelMapOld = null;


            // Test Enemy Manually Made
            enemyList.Add(new Enemy(EnemyType.Stationary, new Rectangle(1200, 500, 64, 64), defaultEnemySprite, 500));

            levelMap[0] = (LvlCoordinator.PathManagerMap);
            testPlatform.TileSheet = mainTileSheet;
            secondTestPlatform.TileSheet = mainTileSheet;
            testWall.TileSheet = mainTileSheet;

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
            playerTwo.BindableKb.Add("throw", Keys.OemQuestion);
            playerTwo.BindableKb.Add("up", Keys.RightShift);

            coopManager = new Coop_Manager(playerOne, playerTwo, Content);
            pathManager = new PathManager(GraphicsDevice.Viewport);
            cameraLimiters = new CameraLimiters(GraphicsDevice.Viewport, playerOne.Hitbox);
            camera = new Dynamic_Camera(GraphicsDevice.Viewport, playerOne.Width, cameraLimiters.MaxWidthDistance, pathManager.WidthConstant);
            //camera.SetMapEdge(LvlCoordinator.MapEdge); <- Correct
            camera.SetMapEdge(new Vector2(5000,5000)); //<- Correct

            //adjust first two values to set spawn point for cake
            cake = new Cake(100, 100, playerOneTexture);
            cakeManager = new CakeManager(players, cake, Content);

            players.Add(playerOne);
            players.Add(playerTwo);
            #endregion Player-Initalization


            //sound stuff
            gameSongs.Add(Content.Load<Song>("sound/gamemusic1"));
            gameSongs.Add(Content.Load<Song>("sound/gamemusic2"));
            gameSongs.Add(Content.Load<Song>("sound/gamemusic3"));

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

            previousKb = kb;
            kb = Keyboard.GetState();

            //mouse instantiation
            previousMs = ms;
            ms = Mouse.GetState();
            mouseRect = new Rectangle(ms.X, ms.Y, 5, 5); //change for smaller/larger mouse footprint on screen

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

            UpdateGameState(gameTime);

            //Write logic for each gameState in here
            switch (gameState)
            {
                case GameState.Game:
                    if (!paused && !menuPaused) //do normal stuff
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

                        //check for ladder interaction
                        foreach (Player player in players)
                        {
                            foreach (Ladder ladder in ladders)
                            {
                                //check if the player is in the position that they can climb a ladder
                                if (player.CheckLadderCollision(ladder))
                                {
                                    player.CanClimb = true;
                                    break; //this will only break out of ladder list
                                }
                                else
                                {
                                    player.CanClimb = false;
                                }
                            }

                        }


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
                        #endregion

                        #region UPDATE CAKE
                        //for the cake, first check if someone is trying to pick it up
                        cakeManager.CheckCakePickUp(gameTime);

                        //then make sure the player is carrying it and adjust the collision checker for putting it down
                        foreach (Player playerCarryingCake in players)
                        {
                            //player carrying the cake
                            if (playerCarryingCake.CakeCarrying)
                            {
                                //adjust the cake for carrying
                                cakeManager.CakeCarry(playerCarryingCake);

                                //adjust the putting down checker
                                if (playerCarryingCake.IsFacingRight)
                                {
                                    cakeManager.PuttingDownChecker = new Rectangle(playerCarryingCake.X + playerCarryingCake.Width, playerCarryingCake.Y - cake.Height,
                                        playerCarryingCake.Width, playerCarryingCake.Height + cake.Height);
                                }
                                else
                                {
                                    cakeManager.PuttingDownChecker = new Rectangle(playerCarryingCake.X - (playerCarryingCake.Width), playerCarryingCake.Y - cake.Height, 
                                        playerCarryingCake.Width, playerCarryingCake.Height + cake.Height);
                                }
                                break; //no need to keep looping through
                            }
                        }

                        //false by default
                        cakeManager.CakeBlockedByTile = false;

                        //check collision with each tile for each player AND CAKE
                        foreach (Tile t in tilesOnScreen)
                        {
                            //player collision with tiles
                            playerOne.CollisionCheck(t, workingGamepad1);
                            playerTwo.CollisionCheck(t, workingGamepad2);

                            //cake collision with tiles
                            cake.CheckTileCollision(t);

                            if (t != null)
                            {
                                //don't let the player put the cake down if it is touching a tile
                                if (cakeManager.PuttingDownChecker.Intersects(t.Hitbox))
                                {
                                    cakeManager.CakeBlockedByTile = true;
                                }
                            }
                        }


                        //then check if each player is dying while carrying the cake or dropping it intentionally

                        //PLAYER ONE carrying cake
                        if (playerOne.CakeCarrying)
                        {
                            //has a working gamepad
                            if (workingGamepad1)
                            {
                                //player dead / drop cake straight down
                                if (playerOne.PlayerState == PlayerState.Die)
                                {
                                    cakeManager.DropCake(playerOne, false);
                                }
                                //presses throw button
                                else if (gp1.IsButtonDown(Buttons.RightTrigger) || playerOne.SingleKeyPress(playerOne.BindableKb["throw"]))
                                {
                                    cakeManager.DropCake(playerOne, true);
                                }
                            }
                            //only keyboard
                            else
                            {
                                //player dead / drop cake straight down
                                if (playerOne.PlayerState == PlayerState.Die)
                                {
                                    cakeManager.DropCake(playerOne, false);
                                }
                                //put down the cake next to player
                                else if (playerOne.SingleKeyPress(playerOne.BindableKb["throw"]))
                                {
                                    cakeManager.DropCake(playerOne, true);
                                }

                            }
                        }
                        //PLAYER TWO
                        else if (playerTwo.CakeCarrying)
                        {
                            //has a working gamepad
                            if (workingGamepad2)
                            {
                                //player dead / drop cake straight down
                                if (playerTwo.PlayerState == PlayerState.Die)
                                {
                                    cakeManager.DropCake(playerTwo, false);
                                }
                                //presses throw button
                                else if (gp1.IsButtonDown(Buttons.RightTrigger) || playerTwo.SingleKeyPress(playerTwo.BindableKb["throw"]))
                                {
                                    cakeManager.DropCake(playerTwo, true);
                                }
                            }
                            //only keyboard
                            else
                            {
                                //player dead / drop cake straight down
                                if (playerTwo.PlayerState == PlayerState.Die)
                                {
                                    cakeManager.DropCake(playerTwo, false);
                                }
                                //put down the cake next to player
                                else if (playerTwo.SingleKeyPress(playerTwo.BindableKb["throw"]))
                                {
                                    cakeManager.DropCake(playerTwo, true);
                                }

                            }
                        }

                        //Finally, check to emulate the cake physics then emulate it if needed
                        cake.Movement();


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

                        #region PLAYER-ENEMY AND CAKE-ENEMY COLLISIONS

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

                        foreach (Enemy currentEnemy in enemyList)
                        {
                            cake.CheckCollisionWithEnemy(currentEnemy);
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

                        #region UPDATE TILES

                        if (LevelMapOld != LevelMapCurrent)
                        {
                            tilesOnScreen.Clear();

                            //Update test tiles through the map system
                            foreach (Level map in LevelMapCurrent.Levels)
                            {
                                foreach (Tile t in map.Tiles)
                                {
                                    if (t != null)
                                    {
                                        tilesOnScreen.Add(t);
                                    }
                                }
                            }

                            //Add temporary test tiles here
                            tilesOnScreen.Add(testPlatform);
                            tilesOnScreen.Add(secondTestPlatform);
                            tilesOnScreen.Add(testWall);

                            LevelMapOld = LevelMapCurrent;
                        }

                        #endregion

                    }
                    else if (menuPaused) //paused using escape
                    {
                        //fill with correct buttons
                        if (escapeFirstFrame)
                        {
                            menuChoices = new Button[3, 1];
                            menuChoices[0, 0] = resumeButton;
                            menuChoices[1, 0] = gameOptionsButton;
                            menuChoices[2, 0] = gameExitButton;

                            menuRow = 0;
                            menuColumn = 0;
                            escapeFirstFrame = false;
                        }
                        //yes/no prompt fill with correct buttons
                        if (quitFirstFrame)
                        {
                            menuChoices = new Button[1, 2];
                            menuChoices[0, 0] = noButton;
                            menuChoices[0, 1] = yesButton;

                            menuRow = 0;
                            menuColumn = 0;

                            quitFirstFrame = false;
                            tryingToQuit = true;
                        }

                        //traverse all buttons
                        for (int row = 0; row < menuChoices.GetLength(0); row++)
                        {
                            for (int column = 0; column < menuChoices.GetLength(1); column++)
                            {
                                //mouse selects button
                                if (LeftMouseSinglePress(ButtonState.Pressed) && mouseRect.Intersects(menuChoices[row, column].Area))
                                {
                                    //highlight the correct button and adjust array indices
                                    lockedSelection = false;
                                    menuRow = row;
                                    menuColumn = column;
                                    menuChoices[menuRow, menuColumn].IsHighlighted = true;
                                    //unhighlight all buttons since the mouse has selected a button
                                    foreach (Button button in menuChoices)
                                    {
                                        if (!button.Equals(menuChoices[menuRow, menuColumn]))
                                        {
                                            button.IsHighlighted = false;
                                        }

                                    }
                                }
                            }
                        }

                        NavigateMenu(workingGamepad1, menuRow, menuColumn);

                    }
                    break;
                case GameState.Options:
                    //navigate the menu
                    NavigateMenu(workingGamepad1, menuRow, menuColumn);

                    //set the settings of the sound in options
                    MediaPlayer.Volume = (masterVolumeSlider.ReturnedValue / 100) * (musicSlider.ReturnedValue / 100);
                    SoundEffect.MasterVolume = (masterVolumeSlider.ReturnedValue / 100) * (soundEffectSlider.ReturnedValue / 100);


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
        private void UpdateGameState(GameTime gameTime)
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
                        menuChoices[1, 0] = menuOptionsButton;
                        menuChoices[2, 0] = menuExitButton;

                        //play the music first frame of the menu
                        if (startMenuMusic)
                        {
                            MediaPlayer.Play(menuMusic);
                            startMenuMusic = false;
                        }

                        menuRow = 0;
                        menuColumn = 0;


                        menuFirstFrame = false;
                    }
                    //fill with correct buttons
                    if (quitFirstFrame)
                    {
                        menuChoices = new Button[1, 2];
                        menuChoices[0, 0] = noButton;
                        menuChoices[0, 1] = yesButton;

                        menuRow = 0;
                        menuColumn = 0;

                        quitFirstFrame = false;
                        tryingToQuit = true;
                    }
                    //search through all the buttons
                    for (int row = 0; row < menuChoices.GetLength(0); row++)
                    {
                        for (int column = 0; column < menuChoices.GetLength(1); column++)
                        {
                            //mouse clicks on button
                            if (LeftMouseSinglePress(ButtonState.Pressed) && mouseRect.Intersects(menuChoices[row, column].Area))
                            {
                                //highlight the correct button and adjust array indices
                                menuRow = row;
                                menuColumn = column;
                                menuChoices[menuRow, menuColumn].IsHighlighted = true;

                                //unhighlight all buttons since the mouse has selected a button
                                foreach (Button button in menuChoices)
                                {
                                    if (!button.Equals(menuChoices[menuRow, menuColumn]))
                                    {
                                        button.IsHighlighted = false;
                                    }

                                }
                            }
                        }
                    }
                    //navigate the menu and update game state based on selection
                    NavigateMenu(workingGamepad1, menuRow, menuColumn);

                    //anything else
                    break;

                case GameState.Options:
                    //initialize all buttons and reset traversal indices on first frame
                    if (optionsFirstFrame)
                    {
                        menuChoices = new Button[5, 1];
                        menuChoices[0, 0] = returnButton;
                        menuChoices[1, 0] = fullscreenButton;
                        menuChoices[2, 0] = masterVolumeSlider;
                        menuChoices[3, 0] = musicSlider;
                        menuChoices[4, 0] = soundEffectSlider;
                        optionsFirstFrame = false;
                        menuRow = 0;
                        menuColumn = 0;

                    }
                    //traverse all buttons
                    for (int row = 0; row < menuChoices.GetLength(0); row++)
                    {
                        for (int column = 0; column < menuChoices.GetLength(1); column++)
                        {
                            //mouse selects button
                            if (LeftMouseSinglePress(ButtonState.Pressed) && mouseRect.Intersects(menuChoices[row, column].Area))
                            {
                                //highlight the correct button and adjust array indices
                                lockedSelection = false;
                                menuRow = row;
                                menuColumn = column;
                                menuChoices[menuRow, menuColumn].IsHighlighted = true;
                                //unhighlight all buttons since the mouse has selected a button
                                foreach (Button button in menuChoices)
                                {
                                    if (!button.Equals(menuChoices[menuRow, menuColumn]))
                                    {
                                        button.IsHighlighted = false;
                                    }

                                }
                            }
                        }
                    }
                    //use escape to leave options
                    if (kb.IsKeyDown(Keys.Escape))
                    {
                        menuSelectSound.Play();
                        if (menuPaused)
                        {
                            gameState = GameState.Game;
                            escapeFirstFrame = true;
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
                        //player 1 & 2 controller can pause
                        if (workingGamepad1)
                        {
                            if (SingleButtonPress(Buttons.Back))
                            {
                                menuSelectSound.Play();
                                if (menuPaused)
                                {
                                    menuPaused = false;
                                    MediaPlayer.Resume();
                                }
                                else
                                {
                                    menuPaused = true;
                                    escapeFirstFrame = true;
                                    MediaPlayer.Pause();
                                }
                                break;
                            }
                            else if (SingleButtonPress(Buttons.Start))
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
                        if (SingleKeyPress(player.BindableKb["pause"]) && !menuPaused)
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
                        //escape to menu
                        if (SingleKeyPress(Keys.Escape))
                        {
                            menuSelectSound.Play();
                            if (menuPaused)
                            {
                                menuPaused = false;
                                MediaPlayer.Resume();
                            }
                            else
                            {
                                menuPaused = true;
                                escapeFirstFrame = true;
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
                    }
                    break;

                case GameState.GameOver:
                    //any key is pressed after a certain delay
                    if (gameOverTimer.UpdateTimer(gameTime))
                    {
                        if (kb.GetPressedKeys().Length > previousKb.GetPressedKeys().Length)
                        {
                            gameState = GameState.Menu;
                            menuFirstFrame = true;
                            startMenuMusic = true;
                        }
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
                    spriteBatch.Begin(SpriteSortMode.Immediate);
                    GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;                  //sets interpolation to nearest neighbor
                    //draw the main menu background image
                    spriteBatch.Draw(mainMenuTexture, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);

                    //draw yes/no window
                    if (tryingToQuit)
                    {
                        spriteBatch.Draw(mainMenuTexture, new Rectangle(graphics.PreferredBackBufferWidth / 3, graphics.PreferredBackBufferHeight / 3,
                            graphics.PreferredBackBufferWidth / 3, graphics.PreferredBackBufferHeight / 3), Color.White);
                    }

                    //draw each button
                    foreach (Button currentButton in menuChoices)
                    {
                        spriteBatch.Draw(currentButton.DrawnTexture, currentButton.Area, Color.White);
                        if (currentButton.IsHighlighted)
                        {
                            //draw cursor next to button
                            spriteBatch.Draw(cursorTexture, new Rectangle(currentButton.StartX - Nudge(true, 3), currentButton.StartY + Nudge(false, 3),
                                graphics.PreferredBackBufferWidth / 40, graphics.PreferredBackBufferHeight / 40), Color.White);
                        }
                        if (currentButton is Slider)
                        {
                            //draw the SliderButton of the slider
                            spriteBatch.DrawString(testFont, currentButton.ReturnedValue.ToString(), new Vector2(currentButton.StartX + Nudge(true, 10),
                                currentButton.StartY + Nudge(false, 3)), Color.Black);
                        }
                    }

                    spriteBatch.End();
                    break;

                case GameState.Options:
                    spriteBatch.Begin();
                    spriteBatch.Draw(mainMenuTexture, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);
                    //draw each button
                    foreach (Button currentButton in menuChoices)
                    {
                        spriteBatch.Draw(currentButton.DrawnTexture, currentButton.Area, Color.White);
                        if (currentButton.IsHighlighted)
                        {
                            //draw cursor next to button
                            spriteBatch.Draw(cursorTexture, new Rectangle(currentButton.StartX - Nudge(true, 3), currentButton.StartY + Nudge(false, 3),
                                graphics.PreferredBackBufferWidth / 40, graphics.PreferredBackBufferHeight / 40), Color.White);
                        }
                        if (currentButton is Slider)
                        {
                            //draw the sliderButton of the slider
                            spriteBatch.Draw(currentButton.SliderButton.DrawnTexture, currentButton.SliderButton.Area, Color.White);
                            spriteBatch.DrawString(testFont, currentButton.ReturnedValue.ToString(), new Vector2(currentButton.StartX + currentButton.Area.Width + Nudge(true, 5),
                                currentButton.StartY + Nudge(false, 5)), Color.Black);
                        }
                    }
                    spriteBatch.End();
                    break;

                case GameState.Game:
                    // (NULL, NULL, NULL, NULL, CAMERA.TRANSFORM IS HOW YOU USE THE CAMERA IN THE GAME! :D)
                    spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, camera.Transform); //setup for keeping pixel art nice
                    GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;                                                //sets interpolation to nearest neighbor


                    foreach (Ladder ladder in ladders)
                    {
                        if (ladder.IsTop)
                        {
                            spriteBatch.Draw(topLadderTexture, ladder.Hitbox, Color.Violet);
                        }
                        else if (ladder.IsBottom)
                        {
                            spriteBatch.Draw(bottomLadderTexture, ladder.Hitbox, Color.Purple);
                        }
                        else
                        {
                            spriteBatch.Draw(normalLadderTexture, ladder.Hitbox, Color.Black);
                        }
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


                    cake.Draw(spriteBatch); //draw cake here (after players)

                    //draw the tiles
                    foreach (Tile t in tilesOnScreen)
                    {
                        t.Draw(spriteBatch);
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
                        spriteBatch.Draw(playerTwoTexture, cakeManager.PuttingDownChecker, Color.Blue); //cake collision checker
                    }
                    if (playerTwo.IsDebugging)
                    {
                        spriteBatch.DrawString(testFont, "Horizontal Velocity: " + playerTwo.HorizontalVelocity, new Vector2(camera.CameraCenter.X + 300, camera.CameraCenter.Y - 500), Color.Red);
                        spriteBatch.DrawString(testFont, "Vertical Velocity: " + playerTwo.VerticalVelocity, new Vector2(camera.CameraCenter.X + 300, camera.CameraCenter.Y - 465), Color.Red);
                        spriteBatch.DrawString(testFont, "Player State: " + playerTwo.PlayerState, new Vector2(camera.CameraCenter.X + 300, camera.CameraCenter.Y - 430), Color.Red);
                        spriteBatch.DrawString(testFont, "Facing right?: " + playerTwo.IsFacingRight, new Vector2(camera.CameraCenter.X + 300, camera.CameraCenter.Y - 395), Color.Red);
                        spriteBatch.Draw(playerTwoTexture, cakeManager.PuttingDownChecker, Color.Blue); //cake collision checker
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

                    //moved this stuff to the bottom so that it draws last (hence on top)
                    if (paused)
                    {
                        spriteBatch.DrawString(testFont, "PAUSED", new Vector2(camera.CameraCenter.X - 100, camera.CameraCenter.Y - 20), Color.White);
                    }

                    if (menuPaused)
                    {
                        spriteBatch.Draw(mainMenuTexture, new Rectangle(graphics.PreferredBackBufferWidth / 4, graphics.PreferredBackBufferHeight / 6,
                            graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight * 3 / 5), Color.White);

                        if (tryingToQuit)
                        {
                            spriteBatch.Draw(mainMenuTexture, new Rectangle(graphics.PreferredBackBufferWidth / 3, graphics.PreferredBackBufferHeight / 3,
                                graphics.PreferredBackBufferWidth / 3, graphics.PreferredBackBufferHeight / 3), Color.White);
                        }

                        //draw each button
                        foreach (Button currentButton in menuChoices)
                        {
                            spriteBatch.Draw(currentButton.DrawnTexture, currentButton.Area, Color.White);
                            if (currentButton.IsHighlighted)
                            {
                                //draw cursor next to button
                                spriteBatch.Draw(cursorTexture, new Rectangle(currentButton.StartX - Nudge(true, 3), currentButton.StartY + Nudge(false, 3),
                                    graphics.PreferredBackBufferWidth / 40, graphics.PreferredBackBufferHeight / 40), Color.White);
                            }
                            if (currentButton is Slider)
                            {
                                //draw the SliderButton of the slider
                                spriteBatch.DrawString(testFont, currentButton.ReturnedValue.ToString(), new Vector2(currentButton.StartX + Nudge(true, 10),
                                    currentButton.StartY + Nudge(false, 3)), Color.Black);
                            }
                        }
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
                //slider
                if (menuChoices[currentRow,currentColumn] is Slider)
                {
                    //slider selected/unselected
                    if (SingleKeyPress(Keys.Enter) || SingleButtonPress(Buttons.A) || 
                        (LeftMouseSinglePress(ButtonState.Pressed) && mouseRect.Intersects(menuChoices[currentRow, currentColumn].Area)))
                    {
                        //toggle selection
                        lockedSelection = !lockedSelection;
                    }
                    //mouse input
                    if ((ms.LeftButton == ButtonState.Pressed && mouseRect.Intersects(menuChoices[currentRow, currentColumn].SliderButton.Area)) || mouseScrollLock)
                    {
                        mouseScrollLock = true;
                        menuChoices[currentRow, currentColumn].CheckAndAlterSlider(ms, previousMs);
                    }
                    if (ms.LeftButton == ButtonState.Released)
                    {
                        mouseScrollLock = false;
                    }
                    //scroll the slider if it's selected
                    if (menuChoices[currentRow, currentColumn].IsHighlighted && lockedSelection)
                    {
                        //scroll right
                        if (SingleKeyPress(playerOne.BindableKb["right"]) || SingleButtonPress(Buttons.DPadRight))               
{
                            sliderHoldingCounter = 0;
                            menuChoices[currentRow, currentColumn].CheckAndAlterSlider(true);
                        }
                        //player holding down a key
                        else if (HoldKey(playerOne.BindableKb["right"]) || HoldButton(Buttons.DPadRight) || GamepadRight())
                        {
                            sliderHoldingCounter++;
                            //delay
                            if (sliderHoldingCounter > 50 && sliderHoldingCounter % 1 == 0)
                            {
                                menuChoices[currentRow, currentColumn].CheckAndAlterSlider(true);
                            }
                        }
                        else if (SingleKeyPress(playerOne.BindableKb["left"]) || SingleButtonPress(Buttons.DPadLeft))
                        {
                            sliderHoldingCounter = 0;
                            menuChoices[currentRow, currentColumn].CheckAndAlterSlider(false);
                        }
                        //holding down a key
                        else if (HoldKey(playerOne.BindableKb["left"]) || HoldButton(Buttons.DPadLeft) || GamepadLeft())
                        {
                            sliderHoldingCounter++;
                            //delay
                            if (sliderHoldingCounter > 50 && sliderHoldingCounter % 1 == 0)
                            {
                                menuChoices[currentRow, currentColumn].CheckAndAlterSlider(false);
                            }
                        }
                        else
                        {
                            //reset the delay counter if nothing is happening
                            sliderHoldingCounter = 0;
                        }
                    }
                    else
                    {
                        lockedSelection = false;
                    }

                }
                //only set the button highlight to false if there is appropriate player input
                if ((SingleKeyPress(playerOne.BindableKb["up"]) || SingleButtonPress(Buttons.DPadUp)) && !lockedSelection)
                {
                    buttonHoldingCounter = 0;
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
                //holding down a key
                else if ((HoldKey(playerOne.BindableKb["up"]) || HoldButton(Buttons.DPadUp) || GamepadUp()) && !lockedSelection)
                {
                    buttonHoldingCounter++;

                    if (buttonHoldingCounter > 30 && buttonHoldingCounter % 7 == 0)
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
                }
                else if ((SingleKeyPress(playerOne.BindableKb["downDash"]) || SingleButtonPress(Buttons.DPadDown)) && !lockedSelection)
                {
                    buttonHoldingCounter = 0;
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
                else if ((HoldKey(playerOne.BindableKb["downDash"]) || HoldButton(Buttons.DPadDown) || GamepadDown()) && !lockedSelection)
                {
                    buttonHoldingCounter++;

                    if (buttonHoldingCounter > 30 && buttonHoldingCounter % 7 == 0)
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
                }
                else if ((SingleKeyPress(playerOne.BindableKb["left"]) || SingleButtonPress(Buttons.DPadLeft)) && !lockedSelection)
                {
                    buttonHoldingCounter = 0;
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
                else if ((HoldKey(playerOne.BindableKb["left"]) || HoldButton(Buttons.DPadLeft) || GamepadLeft()) && !lockedSelection)
                {
                    buttonHoldingCounter++;

                    if (buttonHoldingCounter > 30 && buttonHoldingCounter % 7 == 0)
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
                }
                else if ((SingleKeyPress(playerOne.BindableKb["right"]) || SingleButtonPress(Buttons.DPadRight)) && !lockedSelection)
                {
                    buttonHoldingCounter = 0;
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
                else if ((HoldKey(playerOne.BindableKb["right"]) || HoldButton(Buttons.DPadRight) || GamepadRight()) && !lockedSelection)
                {
                    buttonHoldingCounter++;

                    if (buttonHoldingCounter > 30 && buttonHoldingCounter % 7 == 0)
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
                    buttonHoldingCounter = 0;
                }
            }
            else
            {
                //slider
                if (menuChoices[currentRow, currentColumn] is Slider)
                {
                    //slider selected/unselected
                    if (SingleKeyPress(Keys.Enter) || 
                        (LeftMouseSinglePress(ButtonState.Pressed) && mouseRect.Intersects(menuChoices[currentRow, currentColumn].Area)))
                    {
                        //toggle selection
                        lockedSelection = !lockedSelection;
                    }
                    //mouse input
                    if ((ms.LeftButton == ButtonState.Pressed && mouseRect.Intersects(menuChoices[currentRow, currentColumn].SliderButton.Area)) || mouseScrollLock)
                    {
                        mouseScrollLock = true;
                        menuChoices[currentRow, currentColumn].CheckAndAlterSlider(ms, previousMs);
                    }
                    if (ms.LeftButton == ButtonState.Released)
                    {
                        mouseScrollLock = false;
                    }
                    //scroll the slider if it's selected
                    if (menuChoices[currentRow, currentColumn].IsHighlighted && lockedSelection)
                    {
                        //scroll right
                        if (SingleKeyPress(playerOne.BindableKb["right"]))
                        {
                            sliderHoldingCounter = 0;
                            menuChoices[currentRow, currentColumn].CheckAndAlterSlider(true);
                        }
                        //holding down a key
                        else if (HoldKey(playerOne.BindableKb["right"]))
                        {
                            //delay
                            sliderHoldingCounter++;
                            if (sliderHoldingCounter > 50 && sliderHoldingCounter % 1 == 0)
                            {
                                menuChoices[currentRow, currentColumn].CheckAndAlterSlider(true);
                            }
                        }
                        //scroll left
                        else if (SingleKeyPress(playerOne.BindableKb["left"]))
                        {
                            sliderHoldingCounter = 0;
                            menuChoices[currentRow, currentColumn].CheckAndAlterSlider(false);
                        }
                        //holding down a key
                        else if (HoldKey(playerOne.BindableKb["left"]))
                        {
                            //delay
                            sliderHoldingCounter++;
                            if (sliderHoldingCounter > 50 && sliderHoldingCounter % 1 == 0)
                            {
                                menuChoices[currentRow, currentColumn].CheckAndAlterSlider(false);
                            }
                        }
                        else
                        {
                            //reset the delay counter if nothing is happening
                            sliderHoldingCounter = 0;
                        }
                    }
                    else
                    {
                        lockedSelection = false;
                    }

                }
                //only set the button highlight to false if there is appropriate player input
                if (SingleKeyPress(playerOne.BindableKb["up"]) && !lockedSelection)
                {
                    buttonHoldingCounter = 0;
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
                //holding down a key
                else if (HoldKey(playerOne.BindableKb["up"]) && !lockedSelection)
                {
                    buttonHoldingCounter++;

                    if (buttonHoldingCounter > 30 && buttonHoldingCounter % 7 == 0)
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
                }
                else if (SingleKeyPress(playerOne.BindableKb["downDash"]) && !lockedSelection)
                {
                    buttonHoldingCounter = 0;
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
                else if (HoldKey(playerOne.BindableKb["downDash"]) && !lockedSelection)
                {
                    buttonHoldingCounter++;

                    if (buttonHoldingCounter > 30 && buttonHoldingCounter % 7 == 0)
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
                }
                else if (SingleKeyPress(playerOne.BindableKb["left"]) && !lockedSelection)
                {
                    buttonHoldingCounter = 0;
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
                else if (HoldKey(playerOne.BindableKb["left"]) && !lockedSelection)
                {
                    buttonHoldingCounter++;

                    if (buttonHoldingCounter > 30 && buttonHoldingCounter % 7 == 0)
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
                }
                else if (SingleKeyPress(playerOne.BindableKb["right"]) && !lockedSelection)
                {
                    buttonHoldingCounter = 0;
                    menuSelectSound.Play();
                    menuChoices[currentRow, currentColumn].IsHighlighted = false;
                    if (currentColumn == menuChoices.GetLength(1) - 1)
                    {
                        currentColumn = 0; //loop around
                    }
                    else
                    {
                        currentColumn++;
                    }

                }
                else if (HoldKey(playerOne.BindableKb["right"]) && !lockedSelection)
                {
                    buttonHoldingCounter++;

                    if (buttonHoldingCounter > 30 && buttonHoldingCounter % 7 == 0)
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
                    buttonHoldingCounter = 0;
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
                if (SingleKeyPress(Keys.Enter) || SingleKeyPress(playerOne.BindableKb["jump"]) || SingleButtonPress(Buttons.A) || SingleButtonPress(Buttons.Start) 
                    || (LeftMouseSinglePress(ButtonState.Pressed) && mouseRect.Intersects(menuChoices[currentRow,currentColumn].Area)))
                {
                    menuSelectSound.Play();
                    if (menuChoices[currentRow, currentColumn].Equals(playButton))
                    {
                        gameState = GameState.Game;
                        menuChoices[currentRow, currentColumn].IsHighlighted = false;
                    }
                    else if (menuChoices[currentRow, currentColumn].Equals(menuOptionsButton))
                    {
                        gameState = GameState.Options;
                        optionsFirstFrame = true;
                        menuChoices[currentRow, currentColumn].IsHighlighted = false;
                    }
                    else if (menuChoices[currentRow, currentColumn].Equals(menuExitButton))
                    {
                        menuChoices[currentRow, currentColumn].IsHighlighted = false;
                        quitFirstFrame = true;
                    }
                    else if (menuChoices[currentRow, currentColumn].Equals(returnButton))
                    {
                        //go to correct state depending on what menu player is using
                        if (menuPaused)
                        {
                            gameState = GameState.Game;
                            escapeFirstFrame = true;
                        }
                        else
                        {
                            gameState = GameState.Menu;
                            menuFirstFrame = true;
                        }
                        menuChoices[currentRow, currentColumn].IsHighlighted = false;
                    }
                    else if (menuChoices[currentRow, currentColumn].Equals(fullscreenButton))
                    {
                        graphics.ToggleFullScreen();
                    }
                    else if (menuChoices[currentRow, currentColumn].Equals(resumeButton))
                    {
                        menuPaused = false;
                        menuChoices[currentRow, currentColumn].IsHighlighted = false;
                        MediaPlayer.Resume();
                    }
                    else if (menuChoices[currentRow, currentColumn].Equals(gameOptionsButton))
                    {
                        gameState = GameState.Options;
                        optionsFirstFrame = true;
                        menuChoices[currentRow, currentColumn].IsHighlighted = false;
                    }
                    else if (menuChoices[currentRow, currentColumn].Equals(gameExitButton))
                    {
                        menuChoices[currentRow, currentColumn].IsHighlighted = false;
                        //change to yes/no prompt when that is implemented
                        quitFirstFrame = true;
                    }
                    else if (menuChoices[currentRow, currentColumn].Equals(noButton))
                    {
                        tryingToQuit = false;
                        //escape menu
                        if (menuPaused)
                        {
                            menuRow = 2;
                            menuColumn = 0;

                            menuChoices = new Button[3, 1];
                            menuChoices[0, 0] = resumeButton;
                            menuChoices[1, 0] = gameOptionsButton;
                            menuChoices[2, 0] = gameExitButton;
                        }
                        //normal menu
                        else
                        {
                            menuChoices = new Button[3, 1];
                            menuChoices[0, 0] = playButton;
                            menuChoices[1, 0] = menuOptionsButton;
                            menuChoices[2, 0] = menuExitButton;

                            menuRow = 2;
                            menuColumn = 0;
                        }
                    }
                    else if (menuChoices[currentRow, currentColumn].Equals(yesButton))
                    {
                        Exit();
                    }
                }
            }
            else
            {
                if (SingleKeyPress(Keys.Enter) || SingleKeyPress(playerOne.BindableKb["jump"]) || 
                    (LeftMouseSinglePress(ButtonState.Pressed) && mouseRect.Intersects(menuChoices[currentRow, currentColumn].Area)))
                {
                    menuSelectSound.Play();
                    if (menuChoices[currentRow, currentColumn].Equals(playButton))
                    {
                        menuChoices[currentRow, currentColumn].IsHighlighted = false;
                        gameState = GameState.Game;
                    }
                    else if (menuChoices[currentRow, currentColumn].Equals(menuOptionsButton))
                    {
                        menuChoices[currentRow, currentColumn].IsHighlighted = false;
                        gameState = GameState.Options;
                        optionsFirstFrame = true;
                    }
                    else if (menuChoices[currentRow, currentColumn].Equals(menuExitButton))
                    {
                        menuChoices[currentRow, currentColumn].IsHighlighted = false;
                        //change to yes/no prompt when that is implemented
                        quitFirstFrame = true;
                    }
                    else if (menuChoices[currentRow, currentColumn].Equals(returnButton))
                    {
                        //go to correct state depending on what menu the player is in
                        if (menuPaused)
                        {
                            gameState = GameState.Game;
                            escapeFirstFrame = true;
                        }
                        else
                        {
                            gameState = GameState.Menu;
                            menuFirstFrame = true;
                        }
                    }
                    else if (menuChoices[currentRow, currentColumn].Equals(fullscreenButton))
                    {
                        graphics.ToggleFullScreen();
                    }
                    else if (menuChoices[currentRow, currentColumn].Equals(resumeButton))
                    {
                        menuPaused = false;
                        menuChoices[currentRow, currentColumn].IsHighlighted = false;
                        MediaPlayer.Resume();
                    }
                    else if (menuChoices[currentRow, currentColumn].Equals(gameOptionsButton))
                    {
                        gameState = GameState.Options;
                        optionsFirstFrame = true;
                        menuChoices[currentRow, currentColumn].IsHighlighted = false;
                    }
                    else if (menuChoices[currentRow, currentColumn].Equals(gameExitButton))
                    {
                        menuChoices[currentRow, currentColumn].IsHighlighted = false;
                        //change to yes/no prompt when that is implemented
                        quitFirstFrame = true;
                    }
                    else if (menuChoices[currentRow, currentColumn].Equals(noButton))
                    {
                        tryingToQuit = false;
                        //escape menu
                        if (menuPaused)
                        {
                            menuRow = 2;
                            menuColumn = 0;

                            menuChoices = new Button[3, 1];
                            menuChoices[0, 0] = resumeButton;
                            menuChoices[1, 0] = gameOptionsButton;
                            menuChoices[2, 0] = gameExitButton;
                        }
                        //normal menu
                        else
                        {
                            menuChoices = new Button[3, 1];
                            menuChoices[0, 0] = playButton;
                            menuChoices[1, 0] = menuOptionsButton;
                            menuChoices[2, 0] = menuExitButton;

                            menuRow = 2;
                            menuColumn = 0;
                        }
                    }
                    else if (menuChoices[currentRow, currentColumn].Equals(yesButton))
                    {
                        //save data
                        textWriter = new StreamWriter("save.txt");

                        textWriter.WriteLine(masterVolumeSlider.ReturnedValue);
                        textWriter.WriteLine(musicSlider.ReturnedValue);
                        textWriter.WriteLine(soundEffectSlider.ReturnedValue);
                        textWriter.Close();
                        Exit();
                    }
                }

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
        /// check if a key has been held down for multiple frames
        /// </summary>
        /// <param name="pressedKey"></param>
        /// <returns></returns>
        public bool HoldKey(Keys pressedKey)
        {
            if (kb.IsKeyDown(pressedKey) && previousKb.IsKeyDown(pressedKey))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// check if a button has been pressed for this frame only, also checks if a second controller is working
        /// </summary>
        /// <param name="pressedButton"></param>
        /// <returns></returns>
        public bool SingleButtonPress(Buttons pressedButton)
        {
            //gamepad2 also working
            if (workingGamepad2)
            {
                if ((gp1.IsButtonDown(pressedButton) && previousGp1.IsButtonUp(pressedButton))
                    || (gp2.IsButtonDown(pressedButton) && previousGp2.IsButtonUp(pressedButton)))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else //only gamepad1 working
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

        /// <summary>
        /// check if a button has been held down for multiple frames
        /// </summary>
        /// <param name="pressedButton"></param>
        /// <returns></returns>
        public bool HoldButton(Buttons pressedButton)
        {
            //gamepad2 also working
            if (workingGamepad2)
            {
                if ((gp1.IsButtonDown(pressedButton) && previousGp1.IsButtonDown(pressedButton))
                    || (gp2.IsButtonDown(pressedButton) && previousGp2.IsButtonDown(pressedButton)))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else //only gamepad1 working
            {
                if (gp1.IsButtonDown(pressedButton) && previousGp1.IsButtonDown(pressedButton))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        /// <summary>
        /// use to nudge elements on screen around by a single percent of the screen size
        /// </summary>
        /// <param name="horizontal"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public int Nudge(bool horizontal, double amount)
        {
            if (horizontal)
            {
                return (int)(graphics.PreferredBackBufferWidth * (amount / 100));
            }
            else
            {
                return (int)(graphics.PreferredBackBufferHeight * (amount / 100));
            }
        }
        //check if the mouse is not being held down
        public bool LeftMouseSinglePress(ButtonState pressedButton)
        {
            if (ms.LeftButton == pressedButton && previousMs.LeftButton != pressedButton)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //this code will run whenever the game is closed
        protected override void OnExiting(Object sender, EventArgs args)
        {
            base.OnExiting(sender, args);
            
            //save data
            textWriter = new StreamWriter("save.txt");

            textWriter.WriteLine(masterVolumeSlider.ReturnedValue);
            textWriter.WriteLine(musicSlider.ReturnedValue);
            textWriter.WriteLine(soundEffectSlider.ReturnedValue);
            textWriter.Close();

        }
        #region GAMEPAD STICK CONTROL
        /// <summary>
        /// player trying to move left with dpad/leftThumbstick
        /// </summary>
        /// <returns></returns>
        public bool GamepadLeft()
        {
            if (gp1.ThumbSticks.Left.X < -0.5f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// player trying to move right with dpad/leftThumbstick
        /// </summary>
        /// <returns></returns>
        public bool GamepadRight()
        {
            if (gp1.ThumbSticks.Left.X > 0.5f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// player trying to move Up with dpad/leftThumbstick, used for menu navigation
        /// </summary>
        /// <returns></returns>
        public bool GamepadUp()
        {
            if (gp1.ThumbSticks.Left.Y > 0.5f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// player trying to move Down with dpad/leftThumbstick, used for menu navigation
        /// </summary>
        /// <returns></returns>
        public bool GamepadDown()
        {
            if (gp1.ThumbSticks.Left.Y < -0.5f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}