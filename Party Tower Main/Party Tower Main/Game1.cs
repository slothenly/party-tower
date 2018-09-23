using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System;
using System.IO;

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
            LevelSelect,
            LoadScreen,
            Credits,
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
        SoundEffect errorSound;

        //first frames used to fill Button Array when the user goes to these gameStates
        bool menuFirstFrame = true;
        bool optionsFirstFrame = false;
        bool levelSelectFirstFrame = false;
        bool escapeFirstFrame = false;

        //used to prevent music from starting over and over again
        bool startGameMusic = true;
        bool startMenuMusic = true;

        //used for randomly picking a song for each playthrough
        Random rn;

        GameState gameState;
        GameState secondaryGameState; //used to prevent player from jumping when transitioning from menus to game with controller (A button both jumps and selects)

        //Resolution
        int width;
        int height;

        //Mouse
        MouseState ms;
        MouseState previousMs;
        Rectangle mouseRect; //used to check for clicking on certain elements

        Vector2 resumeDifference;
        Vector2 optionsDifference;
        Vector2 exitDifference;
        Vector2 yesDifference;
        Vector2 noDifference;

         //used to determine which 
         //game logic is run based on if game is paused or not
        bool menuPaused = false;

        //used to determine which buttons/screens to draw/instantiate
        bool tryingToQuit = false;
        bool quitFirstFrame = false;
        bool rebindFirstFrame = false;

        bool displayRebindWindow = false;

        bool bothPlayersDead; //used to determine if game over


        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Testing stuff
        Tile testPlatform = new Tile(false, true, false, false, null);
        Tile secondTestPlatform = new Tile(false, true, false, false, null);
        Tile testWall = new Tile(false, false, false, true, null);
        SpriteFont textFont;

        //Shared keyboard
        KeyboardState kb;
        KeyboardState previousKb;

        //enemy list
        List<Enemy> currentEnemyList;

        //Player fields
        Player playerOne;
        Player playerTwo;
        List<Player> players;
        Coop_Manager coopManager;

        //Cake fields
        CakeManager cakeManager;
        Cake cake;

        //Cake Table
        Table testTable;

        //Ladder fields
        Ladder topladder;
        Ladder bottomLadder;
        Ladder normalLadder1;
        Ladder normalLadder2;
        Ladder normalLadder3;
        List<Ladder> ladders;

        //Background field
        Rectangle backgroundRect;
        Texture2D backgroundTexture;

        //Enemy Fields
        PathManager pathManager;

        //Important GameObjects
        List<GameObject> importantObjects;

        //Fader
        Fader fader;
        float fading_screen_alpha_percent = 0.0f;
        Rectangle fading_screen_rect;
        bool fader_exists;
        bool fader_last_frame;
        Texture2D fading_screen_texture;

        //Shared Fields
        CameraLimiters cameraLimiters;
        Dynamic_Camera camera;
        Rectangle[] tempRects;

        Vector2 previousCameraCenter; //used to adjust button placement

        Texture2D playerOneTexture;
        Texture2D playerTwoTexture;
        Texture2D defaultEnemySprite;

        //Gamepad Support
        GamePadCapabilities capabilities1;
        GamePadCapabilities capabilities2;

        GamePadState previousGp1;
        GamePadState gp1;

        GamePadState previousGp2;
        GamePadState gp2;

        //Level & Tile Information
        List<Texture2D> etcTextures = new List<Texture2D>();
        LevelMapCoordinator LvlCoordinator;
        List<string[]> levelMap;
        Texture2D defaultTile;
        List<Tile> tilesOnScreen = new List<Tile>();
        Texture2D mainTileSheet;
        Texture2D tableTexture;
        Room testRoom;
        Room testRoom2;

        //Maps
        //Map levelOne; THIS WILL BE IMPLEMENTED ONCE LEVELMAPCURRENT IS SORTED OUT (NOT SURE IF WE EXPLICITLY NEED IT) - Ian
        Map levelTwo;
        Map levelThree;
        Map levelFour;
        Map levelFive;
        Map levelSix;
        Map levelSeven;
        Map levelEight;
        Map levelNine;

        Map LevelMapCurrent; //level one for right now
        Map LevelMapOld;

        List<Map> levelList; //Used for storing all of the levels in the game 

        //Ladder textures
        Texture2D topLadderTexture;
        Texture2D bottomLadderTexture;
        Texture2D normalLadderTexture;

        //Buttons for Menu
        Button playButton;
        Button menuOptionsButton;
        Button levelSelectButton;
        Button creditsButton;
        Button menuExitButton;

        //Menu images
        Texture2D mainMenuTexture;
        Texture2D cursorTexture;
        Texture2D menuBoxTexture;

        //Buttons for Options
        Button fullscreenButton;
        Button returnButton;
        Slider masterVolumeSlider;
        Slider soundEffectSlider;
        Slider musicSlider;
        Button rebindButton;
        Button controllerMapButton;

        //Buttons for rebinding
        RebindingButton playerOneLeftButton;
        RebindingButton playerOneRightButton;
        RebindingButton playerOneUpButton;
        RebindingButton playerOneJumpButton;
        RebindingButton playerOneRollButton;
        RebindingButton playerOneDownDashButton;
        RebindingButton playerOneThrowButton;

        RebindingButton playerTwoLeftButton;
        RebindingButton playerTwoRightButton;
        RebindingButton playerTwoUpButton;
        RebindingButton playerTwoJumpButton;
        RebindingButton playerTwoRollButton;
        RebindingButton playerTwoDownDashButton;
        RebindingButton playerTwoThrowButton;

        Button optionsReturnButton;
        Button resetButton;

        //Buttons for Game
        Button resumeButton;
        Button gameOptionsButton;
        Button gameExitButton;

        //Exit buttons
        Button yesButton;
        Button noButton;

        //Level Select Buttons
        Button levelSelectReturnButton;
        Button levelOneButton;
        Button levelTwoButton;
        Button levelThreeButton;
        Button levelFourButton;
        Button levelFiveButton;
        Button levelSixButton;
        Button levelSevenButton;
        Button levelEightButton;
        Button levelNineButton;

        //Level Select Textures;
        Texture2D levelStartTexture;
        Texture2D leveOneTexture;
        Texture2D levelTwoTexture;
        Texture2D levelThreeTexture;
        Texture2D levelFourTexture;
        Texture2D levelFiveTexture;
        Texture2D levelSixTexture;
        Texture2D levelSevenTexture;
        Texture2D levelEightTexture;

        //sets the appropriate image to this variable based on how many levels unlocked
        Texture2D levelSelectScreen;

        //Game Over stuff
        Timer gameOverTimer = new Timer(1);


        //Menu navigation using 2d array
        Button[,] menuChoices;
        int menuRow;
        int menuColumn;
        bool lockedSelection = false; //lock selection when using a slider
        bool mouseScrollLock = false; //prevent mouse from selecting multiple elements at a time (by holding down the mouse button)

        int buttonHoldingCounter = 0; //used for delay for fast navigation
        int sliderHoldingCounter = 0;

        double drawUnit;

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

            height = graphics.PreferredBackBufferHeight;
            width = graphics.PreferredBackBufferWidth;
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
            currentEnemyList = new List<Enemy>();
            levelList = new List<Map>();
            importantObjects = new List<GameObject>();

            bothPlayersDead = false;

            tilesOnScreen.Add(testPlatform);
            tilesOnScreen.Add(secondTestPlatform);
            tilesOnScreen.Add(testWall);
            previousGp1 = new GamePadState();
            gp1 = new GamePadState();

            previousGp2 = new GamePadState();
            gp2 = new GamePadState();

            backgroundRect = new Rectangle(0, 0, 1920, 1080);

            //Placeholder textures for now
            playerOneTexture = Content.Load<Texture2D>("white");
            playerTwoTexture = Content.Load<Texture2D>("white");
            defaultEnemySprite = Content.Load<Texture2D>("enemy");
            textFont = Content.Load<SpriteFont>("textFont");



            #region Player-Initalization AND loading
            players = new List<Player>();
            playerOne = new Player(1, 0, playerOneTexture, new Rectangle(300, 300, 60, 100), Color.White, Content);
            playerTwo = new Player(2, 1, playerTwoTexture, new Rectangle(400, 300, 60, 100), Color.Red, Content);

            masterVolumeSlider = new Slider(Content.Load<Texture2D>("menuImages\\sliderBar"), Content.Load<Texture2D>("menuImages\\sliderButton"), 
                100, Content.Load<Texture2D>("menuImages\\masterVolumeIcon"));
            musicSlider = new Slider(Content.Load<Texture2D>("menuImages\\sliderBar"), Content.Load<Texture2D>("menuImages\\sliderButton"), 
                100, Content.Load<Texture2D>("menuImages\\musicVolumeIcon"));
            soundEffectSlider = new Slider(Content.Load<Texture2D>("menuImages\\sliderBar"), Content.Load<Texture2D>("menuImages\\sliderButton"), 
                100, Content.Load<Texture2D>("menuImages\\sfxVolumeIcon"));

            levelOneButton = new Button(playerOneTexture, playerTwoTexture, 1);
            levelTwoButton = new Button(playerOneTexture, playerTwoTexture, 2);
            levelThreeButton = new Button(playerOneTexture, playerTwoTexture, 3);
            levelFourButton = new Button(playerOneTexture, playerTwoTexture, 4);
            levelFiveButton = new Button(playerOneTexture, playerTwoTexture, 5);
            levelSixButton = new Button(playerOneTexture, playerTwoTexture, 6);
            levelSevenButton = new Button(playerOneTexture, playerTwoTexture, 7);
            levelEightButton = new Button(playerOneTexture, playerTwoTexture, 8);
            levelNineButton = new Button(playerOneTexture, playerTwoTexture, 9);

            //loading from save file
            textReader = new StreamReader("save.txt");


            masterVolumeSlider.ReturnedValue = float.Parse(textReader.ReadLine());
            musicSlider.ReturnedValue = float.Parse(textReader.ReadLine());
            soundEffectSlider.ReturnedValue = float.Parse(textReader.ReadLine());

            //DEFAULT CONTROLS
            // A,D,W,Space,LeftShift,S,P,C  left,right,RightShift,up,RightControl,Down,P,Enter

            playerOne.BindableKb.Add("left", (Keys)Enum.Parse(typeof(Keys), textReader.ReadLine()));
            playerOne.BindableKb.Add("right", (Keys)Enum.Parse(typeof(Keys), textReader.ReadLine()));
            playerOne.BindableKb.Add("up", (Keys)Enum.Parse(typeof(Keys), textReader.ReadLine())); //added for menu navigation
            playerOne.BindableKb.Add("jump", (Keys)Enum.Parse(typeof(Keys), textReader.ReadLine()));
            playerOne.BindableKb.Add("roll", (Keys)Enum.Parse(typeof(Keys), textReader.ReadLine()));
            playerOne.BindableKb.Add("downDash", (Keys)Enum.Parse(typeof(Keys), textReader.ReadLine()));
            playerOne.BindableKb.Add("throw", (Keys)Enum.Parse(typeof(Keys), textReader.ReadLine()));


            playerTwo.BindableKb.Add("left", (Keys)Enum.Parse(typeof(Keys), textReader.ReadLine()));
            playerTwo.BindableKb.Add("right", (Keys)Enum.Parse(typeof(Keys), textReader.ReadLine()));
            playerTwo.BindableKb.Add("up", (Keys)Enum.Parse(typeof(Keys), textReader.ReadLine()));
            playerTwo.BindableKb.Add("jump", (Keys)Enum.Parse(typeof(Keys), textReader.ReadLine()));
            playerTwo.BindableKb.Add("roll", (Keys)Enum.Parse(typeof(Keys), textReader.ReadLine()));
            playerTwo.BindableKb.Add("downDash", (Keys)Enum.Parse(typeof(Keys), textReader.ReadLine()));
            playerTwo.BindableKb.Add("throw", (Keys)Enum.Parse(typeof(Keys), textReader.ReadLine()));



            if (bool.Parse(textReader.ReadLine()))
            {
                graphics.ToggleFullScreen();
            }



            levelTwoButton.IsLocked = bool.Parse(textReader.ReadLine());
            levelThreeButton.IsLocked = bool.Parse(textReader.ReadLine());
            levelFourButton.IsLocked = bool.Parse(textReader.ReadLine());
            levelFiveButton.IsLocked = bool.Parse(textReader.ReadLine());
            levelSixButton.IsLocked = bool.Parse(textReader.ReadLine());
            levelSevenButton.IsLocked = bool.Parse(textReader.ReadLine());
            levelEightButton.IsLocked = bool.Parse(textReader.ReadLine());
            levelNineButton.IsLocked = bool.Parse(textReader.ReadLine());
            textReader.Close();

            coopManager = new Coop_Manager(playerOne, playerTwo, Content);
            pathManager = new PathManager(GraphicsDevice.Viewport);
            cameraLimiters = new CameraLimiters(GraphicsDevice.Viewport, playerOne.Hitbox);
            camera = new Dynamic_Camera(GraphicsDevice.Viewport, playerOne.Width, cameraLimiters.MaxWidthDistance, pathManager.WidthConstant);
            //camera.SetMapEdge(LvlCoordinator.MapEdge); <- Correct
            camera.SetMapEdge(new Vector2(5000, 5000)); //<- Correct

            previousCameraCenter = new Vector2(camera.CameraCenter.X, camera.CameraCenter.Y);

            //adjust first two values to set spawn point for cake
            cake = new Cake(200, 400, playerOneTexture);

            players.Add(playerOne);
            players.Add(playerTwo);
            #endregion Player-Initalization AND loading

            #region Menu stuff
            
            //used for determining width of buttons for rebinding keys
            drawUnit = width * (2.5 / 100.0);

            //Menu textures
            mainMenuTexture = Content.Load<Texture2D>("menuImages\\partyTowerMenuBG");
            cursorTexture = Content.Load<Texture2D>("menuImages\\selector");
            menuBoxTexture = Content.Load<Texture2D>("menuImages/menuBox");

            //Menu buttons
            playButton = new Button(Content.Load<Texture2D>("menuImages\\playNeutral"), Content.Load<Texture2D>("menuImages\\playHovered"));
            menuOptionsButton = new Button(Content.Load<Texture2D>("menuImages\\optionsNeutral"), Content.Load<Texture2D>("menuImages\\optionsHovered"));
            levelSelectButton = new Button(Content.Load<Texture2D>("menuImages\\level_unbolded"), Content.Load<Texture2D>("menuImages\\level_bolded"));
            creditsButton = new Button(Content.Load<Texture2D>("menuImages\\credits_unbolded"), Content.Load<Texture2D>("menuImages\\credits_bolded"));
            menuExitButton = new Button(Content.Load<Texture2D>("menuImages\\exitNeutral"), Content.Load<Texture2D>("menuImages\\exitHovered"));

            //Menu button locations and areas
            playButton.StartLocation = new Point(width * 2 / 9 - Nudge(true, 1.2), height * 3 / 9 + Nudge(false, 3.7));
            levelSelectButton.StartLocation = new Point(width * 2 / 9 - Nudge(true, 1.2), height * 4 / 9 + Nudge(false, 3.9));
            menuOptionsButton.StartLocation = new Point(width * 2 / 9 - Nudge(true, 1.2), height * 5 / 9 + Nudge(false, 2.9));
            creditsButton.StartLocation = new Point(width * 2 / 9 - Nudge(true, 1.2), height * 2 / 3 + Nudge(false, 2.4));
            menuExitButton.StartLocation = new Point(width * 2 / 9 - Nudge(true, 1.2), height * 7 / 9 + Nudge(false, .5));

            playButton.Area = new Rectangle(playButton.StartX, playButton.StartY, 190, 115);
            levelSelectButton.Area = new Rectangle(levelSelectButton.StartX, levelSelectButton.StartY, 260, 90);
            menuOptionsButton.Area = new Rectangle(menuOptionsButton.StartX, menuOptionsButton.StartY, 295, 115);
            creditsButton.Area = new Rectangle(creditsButton.StartX, creditsButton.StartY, 295, 115);
            menuExitButton.Area = new Rectangle(menuExitButton.StartX, menuExitButton.StartY, 165,116);

            //Options buttons (SLIDERS ARE IN LOADING SECTION ABOVE TO PREVENT NULL EXCEPTIONS)
            returnButton = new Button(Content.Load<Texture2D>("menuImages\\ok_unbolded"), Content.Load<Texture2D>("menuImages\\ok_bolded"));
            fullscreenButton = new Button(Content.Load<Texture2D>("menuImages\\fullscreenNormal"), Content.Load<Texture2D>("menuImages\\fullscreenHighlighted"));
            rebindButton = new Button(Content.Load<Texture2D>("menuImages\\playNeutral"), Content.Load<Texture2D>("menuImages\\playHovered"));
            controllerMapButton = new Button(Content.Load<Texture2D>("menuImages\\optionsNeutral"), Content.Load<Texture2D>("menuImages\\optionsHovered"));

            playerOneLeftButton = new RebindingButton(Content.Load<Texture2D>("menuImages\\playNeutral"), Content.Load<Texture2D>("menuImages\\playHovered"), "left", playerOne);
            playerOneRightButton = new RebindingButton(Content.Load<Texture2D>("menuImages\\playNeutral"), Content.Load<Texture2D>("menuImages\\playHovered"), "right", playerOne);
            playerOneUpButton = new RebindingButton(Content.Load<Texture2D>("menuImages\\playNeutral"), Content.Load<Texture2D>("menuImages\\playHovered"), "up", playerOne);
            playerOneJumpButton = new RebindingButton(Content.Load<Texture2D>("menuImages\\playNeutral"), Content.Load<Texture2D>("menuImages\\playHovered"), "jump", playerOne);
            playerOneRollButton = new RebindingButton(Content.Load<Texture2D>("menuImages\\playNeutral"), Content.Load<Texture2D>("menuImages\\playHovered"), "roll", playerOne);
            playerOneDownDashButton = new RebindingButton(Content.Load<Texture2D>("menuImages\\playNeutral"), Content.Load<Texture2D>("menuImages\\playHovered"), "downDash", playerOne);
            playerOneThrowButton = new RebindingButton(Content.Load<Texture2D>("menuImages\\playNeutral"), Content.Load<Texture2D>("menuImages\\playHovered"), "throw", playerOne);

            playerTwoLeftButton = new RebindingButton(Content.Load<Texture2D>("menuImages\\playNeutral"), Content.Load<Texture2D>("menuImages\\playHovered"), "left", playerTwo);
            playerTwoRightButton = new RebindingButton(Content.Load<Texture2D>("menuImages\\playNeutral"), Content.Load<Texture2D>("menuImages\\playHovered"), "right", playerTwo);
            playerTwoUpButton = new RebindingButton(Content.Load<Texture2D>("menuImages\\playNeutral"), Content.Load<Texture2D>("menuImages\\playHovered"), "up", playerTwo);
            playerTwoJumpButton = new RebindingButton(Content.Load<Texture2D>("menuImages\\playNeutral"), Content.Load<Texture2D>("menuImages\\playHovered"), "jump", playerTwo);
            playerTwoRollButton = new RebindingButton(Content.Load<Texture2D>("menuImages\\playNeutral"), Content.Load<Texture2D>("menuImages\\playHovered"), "roll", playerTwo);
            playerTwoDownDashButton = new RebindingButton(Content.Load<Texture2D>("menuImages\\playNeutral"), Content.Load<Texture2D>("menuImages\\playHovered"), "downDash", playerTwo);
            playerTwoThrowButton = new RebindingButton(Content.Load<Texture2D>("menuImages\\playNeutral"), Content.Load<Texture2D>("menuImages\\playHovered"), "throw", playerTwo);

            optionsReturnButton = new Button(Content.Load<Texture2D>("menuImages\\ok_unbolded"), Content.Load<Texture2D>("menuImages\\ok_bolded"));
            resetButton = new Button(Content.Load<Texture2D>("menuImages\\optionsNeutral"), Content.Load<Texture2D>("menuImages\\optionsHovered"));


            //options buttons/sliders start locations
            returnButton.StartLocation = new Point(width * 4 / 9 + Nudge(true, 1), height * 1 / 9 - Nudge(false, 1));
            fullscreenButton.StartLocation = new Point(width * 4 / 9 + Nudge(true, 1), height * 2 / 9 - Nudge(false, 1));
            masterVolumeSlider.StartLocation = new Point(width * 4 / 9 - Nudge(true, 4), height * 3 / 9);
            musicSlider.StartLocation = new Point(width * 4 / 9 - Nudge(true, 4), height * 3 / 9 + Nudge(false, 10));
            soundEffectSlider.StartLocation = new Point(width * 4 / 9 - Nudge(true, 4), height * 3 / 9 + Nudge(false, 20));
            controllerMapButton.StartLocation = new Point(width * 4 / 9, height * 6 / 9);
            rebindButton.StartLocation = new Point(width * 4 / 9 + Nudge(true, 1), height * 7 / 9);


            playerOneLeftButton.StartLocation = new Point(width / 3, height * 1 / 9 + Nudge(false, 10));
            playerOneRightButton.StartLocation = new Point(width / 3, height * 1 / 9 + Nudge(false, 18));
            playerOneUpButton.StartLocation = new Point(width / 3, height * 1 / 9 + Nudge(false, 26));
            playerOneJumpButton.StartLocation = new Point(width / 3, height * 1 / 9 + Nudge(false, 34));
            playerOneRollButton.StartLocation = new Point(width / 3, height * 1 / 9 + Nudge(false, 42));
            playerOneDownDashButton.StartLocation = new Point(width / 3, height * 1 / 9 + Nudge(false, 50));
            playerOneThrowButton.StartLocation = new Point(width / 3, height * 1 / 9 + Nudge(false, 66));

            playerTwoLeftButton.StartLocation = new Point(width * 6 / 9, height * 1 / 9 + Nudge(false, 10));
            playerTwoRightButton.StartLocation = new Point(width * 6 / 9, height * 1 / 9 + Nudge(false, 18));
            playerTwoUpButton.StartLocation = new Point(width * 6 / 9, height * 1 / 9 + Nudge(false, 26));
            playerTwoJumpButton.StartLocation = new Point(width * 6 / 9, height * 1 / 9 + Nudge(false, 34));
            playerTwoRollButton.StartLocation = new Point(width * 6 / 9, height * 1 / 9 + Nudge(false, 42));
            playerTwoDownDashButton.StartLocation = new Point(width * 6 / 9, height * 1 / 9 + Nudge(false, 50));
            playerTwoThrowButton.StartLocation = new Point(width * 6 / 9, height * 1 / 9 + Nudge(false, 66));

            optionsReturnButton.StartLocation = new Point(width / 3, height * 1 / 9);
            resetButton.StartLocation = new Point(width * 6 / 9, height * 1 / 9);



            //area of buttons in options
            returnButton.Area = new Rectangle(returnButton.StartX, returnButton.StartY, width / 14, height / 12);
            fullscreenButton.Area = new Rectangle(fullscreenButton.StartX, fullscreenButton.StartY, width / 11, height / 10);
            masterVolumeSlider.Area = new Rectangle(masterVolumeSlider.StartX, masterVolumeSlider.StartY, width / 5, (width / 5) / (53 / 3));
            musicSlider.Area = new Rectangle(musicSlider.StartX, musicSlider.StartY, width / 5, (width / 5) / (53 / 3));
            soundEffectSlider.Area = new Rectangle(soundEffectSlider.StartX, soundEffectSlider.StartY, width / 5, (width / 5) / (53 / 3));
            controllerMapButton.Area = new Rectangle(controllerMapButton.StartX, controllerMapButton.StartY, width / 11, height / 10);
            rebindButton.Area = new Rectangle(rebindButton.StartX, rebindButton.StartY, width / 9, height / 10);

            playerOneLeftButton.Area = new Rectangle(playerOneLeftButton.StartX, playerOneLeftButton.StartY, width / 11, height / 20);
            playerOneRightButton.Area = new Rectangle(playerOneRightButton.StartX, playerOneRightButton.StartY, width / 11, height / 20);
            playerOneUpButton.Area = new Rectangle(playerOneUpButton.StartX, playerOneUpButton.StartY, width / 11, height / 20);
            playerOneJumpButton.Area = new Rectangle(playerOneJumpButton.StartX, playerOneJumpButton.StartY, width / 11, height / 20);
            playerOneRollButton.Area = new Rectangle(playerOneRollButton.StartX, playerOneRollButton.StartY, width / 11, height / 20);
            playerOneDownDashButton.Area = new Rectangle(playerOneDownDashButton.StartX, playerOneDownDashButton.StartY, width / 11, height / 20);
            playerOneThrowButton.Area = new Rectangle(playerOneThrowButton.StartX, playerOneThrowButton.StartY, width / 11, height / 20);


            playerTwoLeftButton.Area = new Rectangle(playerTwoLeftButton.StartX, playerTwoLeftButton.StartY, width / 11, height / 20);
            playerTwoRightButton.Area = new Rectangle(playerTwoRightButton.StartX, playerTwoRightButton.StartY, width / 11, height / 20);
            playerTwoUpButton.Area = new Rectangle(playerTwoUpButton.StartX, playerTwoUpButton.StartY, width / 11, height / 20);
            playerTwoJumpButton.Area = new Rectangle(playerTwoJumpButton.StartX, playerTwoJumpButton.StartY, width / 11, height / 20);
            playerTwoRollButton.Area = new Rectangle(playerTwoRollButton.StartX, playerTwoRollButton.StartY, width / 11, height / 20);
            playerTwoDownDashButton.Area = new Rectangle(playerTwoDownDashButton.StartX, playerTwoDownDashButton.StartY, width / 11, height / 20);
            playerTwoThrowButton.Area = new Rectangle(playerTwoThrowButton.StartX, playerTwoThrowButton.StartY, width / 11, height / 20);


            optionsReturnButton.Area = new Rectangle(optionsReturnButton.StartX, optionsReturnButton.StartY, width / 14, height / 12);
            resetButton.Area = new Rectangle(resetButton.StartX, resetButton.StartY, width / 11, height / 10);

            //set the positions of the sliderButtons on the actual sliders
            masterVolumeSlider.SetSliderButtonArea();
            musicSlider.SetSliderButtonArea();
            soundEffectSlider.SetSliderButtonArea();


            //Level select textures
            levelStartTexture = Content.Load<Texture2D>("menuImages/levelSelect/none_unlocked");
            leveOneTexture = Content.Load<Texture2D>("menuImages/levelSelect/1_unlocked");
            levelTwoTexture = Content.Load<Texture2D>("menuImages/levelSelect/2_unlocked");
            levelThreeTexture = Content.Load<Texture2D>("menuImages/levelSelect/3_unlocked");
            levelFourTexture = Content.Load<Texture2D>("menuImages/levelSelect/4_unlocked");
            levelFiveTexture = Content.Load<Texture2D>("menuImages/levelSelect/5_unlocked");
            levelSixTexture = Content.Load<Texture2D>("menuImages/levelSelect/6_unlocked");
            levelSevenTexture = Content.Load<Texture2D>("menuImages/levelSelect/7_unlocked");
            levelEightTexture = Content.Load<Texture2D>("menuImages/levelSelect/all_unlocked");

            levelSelectScreen = levelStartTexture;

            //Level select buttons
            levelSelectReturnButton = new Button(Content.Load<Texture2D>("menuImages\\exitNeutral"), Content.Load<Texture2D>("menuImages\\exitHovered"));
            //ACTUAL BUTTONS ARE ABOVE HERE FOR TEXTREADER




            //level select buttons start location
            levelSelectReturnButton.StartLocation = new Point(width / 7, height * 8 / 9);
            levelOneButton.StartLocation = new Point(width / 4 + Nudge(true,5.5), height * 8 / 9);
            levelTwoButton.StartLocation = new Point(width * 3 / 9 + Nudge(true, 4.5), height * 7 / 9 + Nudge(false, 1));
            levelThreeButton.StartLocation = new Point(width * 3 / 9 + Nudge(true, 7.5), height * 6 / 9 + Nudge(false, 2.5));
            levelFourButton.StartLocation = new Point(width * 3 / 9 + Nudge(true, 9.5), height * 5 / 9 + Nudge(false, 4));
            levelFiveButton.StartLocation = new Point(width * 3 / 9 + Nudge(true, 11), height * 4 / 9 + Nudge(false, 6));
            levelSixButton.StartLocation = new Point(width * 3 / 9 + Nudge(true, 12), height * 3 / 9 + Nudge(false, 9));
            levelSevenButton.StartLocation = new Point(width * 3 / 9 + Nudge(true, 13), height * 2 / 9 + Nudge(false, 12));
            levelEightButton.StartLocation = new Point(width * 3 / 9 + Nudge(true, 13.5), height * 1 / 9 + Nudge(false, 13));
            levelNineButton.StartLocation = new Point(width * 3 / 9 + Nudge(true, 14), height * 1 / 9 - Nudge(false, 1));

            //level select buttons area
            levelSelectReturnButton.Area = new Rectangle(levelSelectReturnButton.StartX, levelSelectReturnButton.StartY,
                width / 12, height / 10);

            levelOneButton.Area = new Rectangle(levelOneButton.StartX, levelOneButton.StartY, width / 3 + Nudge(true,5), height / 9);
            levelTwoButton.Area = new Rectangle(levelTwoButton.StartX, levelTwoButton.StartY, width / 4 - Nudge(true, 2), height / 9 - Nudge(false, 1));
            levelThreeButton.Area = new Rectangle(levelThreeButton.StartX, levelThreeButton.StartY, width / 5 - Nudge(true, 3) , height / 9 - Nudge(false, 2));
            levelFourButton.Area = new Rectangle(levelFourButton.StartX, levelFourButton.StartY, width / 6 - Nudge(true, 3), height / 9-Nudge(false, 2));
            levelFiveButton.Area = new Rectangle(levelFiveButton.StartX, levelFiveButton.StartY, width / 6 - Nudge(true, 6), height / 9 - Nudge(false, 2));
            levelSixButton.Area = new Rectangle(levelSixButton.StartX, levelSixButton.StartY, width / 7 - Nudge(true, 6), height / 9 - Nudge(false,3));
            levelSevenButton.Area = new Rectangle(levelSevenButton.StartX, levelSevenButton.StartY, width / 7 - Nudge(true, 7.5), height / 9 - Nudge(false,3));
            levelEightButton.Area = new Rectangle(levelEightButton.StartX, levelEightButton.StartY, width / 7 - Nudge(true, 8.8), height / 9 - Nudge(false,1.5));
            levelNineButton.Area = new Rectangle(levelNineButton.StartX, levelNineButton.StartY, width / 7 - Nudge(true, 9.5), height / 9 + Nudge(false, 3));

            //game buttons
            resumeButton = new Button(Content.Load<Texture2D>("menuImages\\playNeutral"), Content.Load<Texture2D>("menuImages\\playHovered"));
            gameOptionsButton = new Button(Content.Load<Texture2D>("menuImages\\optionsNeutral"), Content.Load<Texture2D>("menuImages\\optionsHovered"));
            gameExitButton = new Button(Content.Load<Texture2D>("menuImages\\exitNeutral"), Content.Load<Texture2D>("menuImages\\exitHovered"));

            //game buttons start locations
            resumeButton.StartLocation = new Point(width / 2 - Nudge(true, 5), height / 4);
            gameOptionsButton.StartLocation = new Point(width / 2 - Nudge(true, 5), height / 4 + Nudge(false, 17));
            gameExitButton.StartLocation = new Point(width / 2 - Nudge(true, 5), height / 4 + Nudge(false, 34));

            //area of buttons in game escape screen
            resumeButton.Area = new Rectangle(resumeButton.StartX, resumeButton.StartY, width / 10, height / 10);
            gameOptionsButton.Area = new Rectangle(gameOptionsButton.StartX, gameOptionsButton.StartY, width / 10, height / 10);
            gameExitButton.Area = new Rectangle(gameExitButton.StartX, gameExitButton.StartY, width / 10, height / 10);

            //Exit Buttons
            yesButton = new Button(Content.Load<Texture2D>("menuImages\\exitNeutral"), Content.Load<Texture2D>("menuImages\\exitHovered"));
            noButton = new Button(Content.Load<Texture2D>("menuImages\\playNeutral"), Content.Load<Texture2D>("menuImages\\playHovered"));

            //exit buttons start locations
            noButton.StartLocation = new Point(width / 3, height / 2 - Nudge(false, 3));
            yesButton.StartLocation = new Point(width * 2 / 3 - Nudge(true, 10), height / 2 - Nudge(false, 3));

            //exit buttons area
            noButton.Area = new Rectangle(noButton.StartX, noButton.StartY, width / 10, height / 10);
            yesButton.Area = new Rectangle(yesButton.StartX, yesButton.StartY, width / 10, height / 10);


            gameState = GameState.Menu;

            //arranging the buttons in the correct order
            menuChoices = new Button[5, 1];
            menuChoices[0, 0] = playButton;
            menuChoices[1, 0] = levelSelectButton;
            menuChoices[2, 0] = menuOptionsButton;
            menuChoices[3, 0] = creditsButton;
            menuChoices[4, 0] = menuExitButton;
            menuRow = 0;
            menuColumn = 0;

            //adjust volumes using formula based on sliders
            MediaPlayer.Volume = (masterVolumeSlider.ReturnedValue / 100) * (musicSlider.ReturnedValue / 100);
            SoundEffect.MasterVolume = (masterVolumeSlider.ReturnedValue / 100) * (soundEffectSlider.ReturnedValue / 100);
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

            testTable = new Table(new Rectangle(200, 900, 200, 200), playerOneTexture);

            topLadderTexture = playerOneTexture;
            bottomLadderTexture = playerTwoTexture;
            normalLadderTexture = playerOneTexture;
            backgroundTexture = Content.Load<Texture2D>("tempBackArt");
            fading_screen_texture = Content.Load<Texture2D>("black_pixel");
            cakeManager = new CakeManager(players, cake, Content, testTable);

            #region Etc Textures
            //########### Add Textures Here #############
            defaultTile = Content.Load<Texture2D>("default");
            mainTileSheet = Content.Load<Texture2D>(@"textures\basicTileSheet");
            tableTexture = Content.Load<Texture2D>(@"textures\table");

            etcTextures.Add(tableTexture);

            
            #endregion

            //Instantiate level coordinator and add the starter tiles to the list
            LvlCoordinator = new LevelMapCoordinator("tileOrientationTest", etcTextures, defaultEnemySprite, mainTileSheet);
            Tile tempMeasuringStick = new Tile(false, false, false, false, mainTileSheet);
            Tile[,] tempHolder = new Tile[9, 16];

            #region Create Empty # Levels for Game and Add

            levelTwo = new Map(tempMeasuringStick, 1);
            levelThree = new Map(tempMeasuringStick, 2);
            levelFour = new Map(tempMeasuringStick, 3);
            levelFive = new Map(tempMeasuringStick, 4);
            levelSix = new Map(tempMeasuringStick, 5);
            levelSeven = new Map(tempMeasuringStick, 6);
            levelEight = new Map(tempMeasuringStick, 7);
            levelNine = new Map(tempMeasuringStick, 8);

            //Instantiating the current map
            LevelMapCurrent = new Map(tempMeasuringStick, 1);

            #endregion Create Empty # Levels for Game and Add

            #region How To Add Room to Map

            // ### STEPS FOR MAKING A NEW MAP ###
            // 
            // Step 1. Specify how many rooms will be added... 
            // from above of the root  ---->                        mapName.Above = # of Rooms Above
            // from below of the root  ---->                        mapName.Above = # of Rooms Below
            // from the right of the root.  ---->                   mapName.Above = # of Rooms Left
            // from the left of the root.  ---->                    mapName.Above = # of Rooms Right


            // Step 2. Generate the proper pathmanager Map ->       mapName.GenerateMap();

            // ### STEPS FOR ADDING A ROOM TO A MAP ###
            // Step 1. Load the tileset into tempHolder -->         LvlCoordinator.UpdateMapFromPath("<your level>");
            // Step 2. Instantiate your room -->                    roomName = nnew Room(tempHolder, LvlCoordinator.LadderHolder, LvlCoordinator.TableHolder, LvlCoordinator.CakeHolder, LvlCoordinator.ExitHolder, LvlCoordinator.PathManagerMap, LvlCoordinator.EnemyHolder);
            // Step 3. Add your room to your map -->                mapName.AddRoom(roomName);
            // Step 4. Place level with respect to root level -->   mapName.PlaceLeft(mapName.Root.Above);

            // Repeat as needed for each new room 
            // For more in depth info about level placement, see the Architecture doc

            #endregion How to Add Room to Map

            #region Testing Levels

            tempHolder = LvlCoordinator.UpdateMapFromPath("levelOne");
            testRoom = new Room(tempHolder, LvlCoordinator.LadderHolder, LvlCoordinator.TableHolder, LvlCoordinator.CakeHolder, LvlCoordinator.ExitHolder, LvlCoordinator.PathManagerMap, LvlCoordinator.EnemyHolder);
            //Comment this out eventaully
            levelMap[0] = LvlCoordinator.PathManagerMap;

            LevelMapCurrent.AddRoom(testRoom);
            importantObjects.AddRange(testRoom.ImportantObjects());
            //first room is automatically placed as the root

            tempHolder = LvlCoordinator.UpdateMapFromPath("levelTwo");
            testRoom2 = new Room(tempHolder, LvlCoordinator.LadderHolder, LvlCoordinator.TableHolder, LvlCoordinator.CakeHolder, LvlCoordinator.ExitHolder, LvlCoordinator.PathManagerMap, LvlCoordinator.EnemyHolder);
            LevelMapCurrent.AddRoom(testRoom2);
            LevelMapCurrent.PlaceRight(LevelMapCurrent.Root);

            LevelMapOld = null;

            #endregion Testing Levels

            #region Level 1

            #endregion Level 1

            #region Level 2

            #endregion Level 2

            #region Level 3

            #endregion Level 3

            #region Level 4

            #endregion Level 4

            #region Level 5

            #endregion Level 5

            #region Level 6

            #endregion Level 6

            #region Level 7

            #endregion Level 7

            #region Level 8

            #endregion Level 8

            #region Level 9

            #endregion Level 9

            #region Add Levels to the Level List

            //Add all the levels to the list
            levelList.Add(LevelMapCurrent);
            levelList.Add(levelTwo);
            levelList.Add(levelThree);
            levelList.Add(levelFour);
            levelList.Add(levelFive);
            levelList.Add(levelSix);
            levelList.Add(levelSeven);
            levelList.Add(levelEight);
            levelList.Add(levelNine);

            #endregion Add Levels to the Level List

            // Test Enemy Manually Made
            currentEnemyList.Add(new Enemy(EnemyType.Stationary, new Rectangle(1200, 500, 64, 64), defaultEnemySprite, 500));

            testPlatform.TileSheet = mainTileSheet;
            secondTestPlatform.TileSheet = mainTileSheet;
            testWall.TileSheet = mainTileSheet;

            //had to move this to load content because the textures are null if you try to instantiate a player in Initialize



            //sound stuff
            gameSongs.Add(Content.Load<Song>("sound/gamemusic1"));
            gameSongs.Add(Content.Load<Song>("sound/gamemusic2"));
            gameSongs.Add(Content.Load<Song>("sound/gamemusic3"));

            menuMusic = Content.Load<Song>("sound/menumusic");
            menuSelectSound = Content.Load<SoundEffect>("sound/menuselect");
            errorSound = Content.Load<SoundEffect>("sound/cakeError");
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
                previousGp1 = gp1;
                gp1 = GamePad.GetState(PlayerIndex.One);
            }

            //check if there is a player two controller
            if (capabilities2.IsConnected)
            {
                previousGp2 = gp2;
                gp2 = GamePad.GetState(PlayerIndex.Two);
            }

            UpdateGameState(gameTime);

            //Write logic for each gameState in here
            switch (gameState)
            {
                case GameState.Game:
                    if (!menuPaused) //do normal stuff
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

                        //check for ladder and exit interaction
                        foreach (Player currentPlayer in players)
                        {
                            foreach (GameObject currentObject in importantObjects)
                            {
                                if (currentObject is Ladder)
                                {
                                    //check if the player is in the position that they can climb a ladder
                                    if (currentPlayer.CheckLadderCollision((Ladder)currentObject) && currentObject.IsActive)
                                    {
                                        currentPlayer.CanClimb = true;
                                        break; //this will only break out of ladder list
                                    }
                                    else
                                    {
                                        currentPlayer.CanClimb = false;
                                    }
                                }
                                else if (currentObject is Exit)
                                {
                                    if (currentPlayer.Hitbox.Intersects(currentObject.Hitbox)) //player touches the exit
                                    {
                                        gameState = GameState.LoadScreen; //this triggers the transition
                                    }
                                }
                            }

                        }
                        if (secondaryGameState == GameState.Game)
                        {
                            playerOne.FiniteState(gp1, previousGp1);
                            playerTwo.FiniteState(gp2, previousGp2);
                        }
                        else
                        {
                            secondaryGameState = GameState.Game; //used to prevent the player from jumping on first frame of game starting
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
                            foreach (Player currentPlayer in players)
                            {
                                currentPlayer.CollisionCheck(t);
                            }

                            //cake collision with tiles
                            cake.CheckTileCollision(t);

                            if (t != null)
                            {
                                //don't let the player put the cake down if it is touching a tile, but allow placing if the table is it is touching that
                                if (cakeManager.PuttingDownChecker.Intersects(t.Hitbox) && !cakeManager.PuttingDownChecker.Intersects(testTable.Hitbox))
                                {
                                    cakeManager.CakeBlockedByTile = true;
                                }
                                else if (cakeManager.PuttingDownChecker.Intersects(testTable.Hitbox))
                                {
                                    cakeManager.CakeBlockedByTile = false;
                                }
                            }
                        }


                        //then check if each player is dying while carrying the cake or dropping it intentionally

                        //PLAYER ONE carrying cake
                        if (playerOne.CakeCarrying)
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
                        //PLAYER TWO
                        else if (playerTwo.CakeCarrying)
                        {
                            //player dead / drop cake straight down
                            if (playerTwo.PlayerState == PlayerState.Die)
                            {
                                cakeManager.DropCake(playerTwo, false);
                            }
                            //presses throw button
                            else if (gp2.IsButtonDown(Buttons.RightTrigger) || playerTwo.SingleKeyPress(playerTwo.BindableKb["throw"]))
                            {
                                cakeManager.DropCake(playerTwo, true);
                            }

                        }

                        //finally, check to emulate the cake physics then emulate it if needed
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
                        pathManager.UpdatePlayersOnMap(/*String of Constructed A* Map goes here*/ levelMap[0], playerOne.Hitbox, playerTwo.Hitbox);

                        previousCameraCenter = camera.CameraCenter;

                        // Update Camera's
                        camera.UpdateCamera(GraphicsDevice.Viewport, playerOne.Hitbox, playerTwo.Hitbox);


                        if (previousCameraCenter.X != camera.CameraCenter.X && previousCameraCenter.X != 0) //if the camera has moved
                        {
                            int difference = (int)(camera.CameraCenter.X - previousCameraCenter.X); 

                            resumeButton.X += difference; //adjust the location of the buttons by the difference
                            gameOptionsButton.X += difference;
                            gameExitButton.X += difference;
                            yesButton.X += difference;
                            noButton.X += difference;

                        }
                        if (previousCameraCenter.Y != camera.CameraCenter.Y && previousCameraCenter.Y != 0)
                        {
                            int difference = (int)(camera.CameraCenter.Y - previousCameraCenter.Y);

                            resumeButton.Y += difference;
                            gameOptionsButton.Y += difference;
                            gameExitButton.Y += difference;
                            yesButton.Y += difference;
                            noButton.Y += difference;
                        }

                        if (currentEnemyList != null)
                        {
                            foreach (Enemy e in currentEnemyList)
                            {
                                e.IsDrawn = camera.IsDrawn(e.Hitbox);
                                e.IsActive = camera.IsUpdated(e.Hitbox);
                            }
                        }

                        /* if the cake has been set down, simply check to see if each ladder should be active and drawn if it's
                         * hit box interacts with the camera.IsDrawn and camera.IsUpdated scene.*/



                        #endregion

                        #region UPDATE ENEMY

                        foreach (Enemy currentEnemy in currentEnemyList)
                        {
                            if (currentEnemy.Hitpoints == 0)
                            {
                                currentEnemy.IsActive = false;
                            }
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
                            foreach (Enemy currentEnemy in currentEnemyList)
                            {
                                if (currentEnemy.Hitpoints > 0 && currentPlayer.PlayerState != PlayerState.Die)
                                {
                                    currentPlayer.CheckColliderAgainstEnemy(currentEnemy);
                                }
                            }
                        }

                        foreach (Enemy currentEnemy in currentEnemyList)
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
                            foreach (Room map in LevelMapCurrent.Levels)
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
                            //tilesOnScreen.Add(testPlatform);
                            //tilesOnScreen.Add(secondTestPlatform);
                            //tilesOnScreen.Add(testWall);

                            LevelMapOld = LevelMapCurrent;
                        }

                        #endregion

                    }
                    else //paused using escape
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

                        #region Hard Coding Button positions
                        resumeDifference = new Vector2(resumeButton.X - resumeButton.StartX, resumeButton.Y - resumeButton.StartY);
                        optionsDifference = new Vector2(gameOptionsButton.X - gameOptionsButton.StartX, gameOptionsButton.Y - gameOptionsButton.StartY);
                        exitDifference = new Vector2(gameExitButton.X - gameExitButton.StartX, gameExitButton.Y - gameExitButton.StartY);
                        yesDifference = new Vector2(yesButton.X - yesButton.StartX, yesButton.Y - yesButton.StartY);
                        noDifference = new Vector2(noButton.X - noButton.StartX, noButton.Y - noButton.StartY);

                        resumeButton.X = resumeButton.StartX;
                        resumeButton.Y = resumeButton.StartY;

                        gameOptionsButton.X = gameOptionsButton.StartX;
                        gameOptionsButton.Y = gameOptionsButton.StartY;

                        gameExitButton.X = gameExitButton.StartX;
                        gameExitButton.Y = gameExitButton.StartY;

                        yesButton.X = yesButton.StartX;
                        yesButton.Y = yesButton.StartY;

                        noButton.X = noButton.StartX;
                        noButton.Y = noButton.StartY;
                        #endregion


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

                        #region Hard Coding Button positions
                        resumeButton.X += (int)resumeDifference.X;
                        resumeButton.Y += (int)resumeDifference.Y;

                        gameOptionsButton.X += (int)optionsDifference.X;
                        gameOptionsButton.Y += (int)optionsDifference.Y;

                        gameExitButton.X += (int)exitDifference.X;
                        gameExitButton.Y += (int)exitDifference.Y;

                        yesButton.X += (int)yesDifference.X;
                        yesButton.Y += (int)yesDifference.Y;

                        noButton.X += (int)noDifference.X;
                        noButton.Y += (int)noDifference.Y;
                        #endregion

                        NavigateMenu(menuRow, menuColumn);

                    }
                    break;
                case GameState.Options:
                    //navigate the menu
                    NavigateMenu(menuRow, menuColumn);

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
                    if (menuFirstFrame)
                    {
                        menuChoices = new Button[5, 1];
                        menuChoices[0, 0] = playButton;
                        menuChoices[1, 0] = levelSelectButton;
                        menuChoices[2, 0] = menuOptionsButton;
                        menuChoices[3, 0] = creditsButton;
                        menuChoices[4, 0] = menuExitButton;

                        yesButton.X = yesButton.StartX;
                        yesButton.Y = yesButton.StartY;

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

                    yesButton.X = yesButton.StartX; //reset the buttons for the menu after adjusting them in game
                    yesButton.Y = yesButton.StartY;
                    noButton.X = noButton.StartX;
                    noButton.Y = noButton.StartY;


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
                    NavigateMenu(menuRow, menuColumn);

                    //anything else
                    break;

                case GameState.LevelSelect:
                    if (levelSelectFirstFrame)
                    {
                        //adjust this as we add levels
                        menuChoices = new Button[10, 1];
                        menuChoices[0, 0] = levelNineButton;
                        menuChoices[1, 0] = levelEightButton;
                        menuChoices[2, 0] = levelSevenButton;
                        menuChoices[3, 0] = levelSixButton;
                        menuChoices[4, 0] = levelFiveButton;
                        menuChoices[5, 0] = levelFourButton;
                        menuChoices[6, 0] = levelThreeButton;
                        menuChoices[7, 0] = levelTwoButton;
                        menuChoices[8, 0] = levelOneButton;
                        menuChoices[9, 0] = levelSelectReturnButton;

                        menuRow = 9;
                        menuColumn = 0;

                        levelSelectFirstFrame = false;
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
                    NavigateMenu(menuRow, menuColumn);
                    break;

                case GameState.Options:
                    //initialize all buttons and reset traversal indices on first frame
                    if (optionsFirstFrame)
                    {
                        menuChoices = new Button[7, 1];
                        menuChoices[0, 0] = returnButton;
                        menuChoices[1, 0] = fullscreenButton;
                        menuChoices[2, 0] = masterVolumeSlider;
                        menuChoices[3, 0] = musicSlider;
                        menuChoices[4, 0] = soundEffectSlider;
                        menuChoices[5, 0] = controllerMapButton;
                        menuChoices[6, 0] = rebindButton;
                        optionsFirstFrame = false;
                        menuRow = 0;
                        menuColumn = 0;

                    }
                    //rebind keys buttons
                    else if (rebindFirstFrame)
                    {
                        menuChoices = new Button[8, 2];
                        //put buttons here
                        menuChoices[0, 0] = optionsReturnButton;
                        menuChoices[0, 1] = resetButton;

                        menuChoices[1, 0] = playerOneLeftButton;
                        menuChoices[2, 0] = playerOneRightButton;
                        menuChoices[3, 0] = playerOneUpButton;
                        menuChoices[4, 0] = playerOneJumpButton;
                        menuChoices[5, 0] = playerOneRollButton;
                        menuChoices[6, 0] = playerOneDownDashButton;
                        menuChoices[7, 0] = playerOneThrowButton;

                        menuChoices[1, 1] = playerTwoLeftButton;
                        menuChoices[2, 1] = playerTwoRightButton;
                        menuChoices[3, 1] = playerTwoUpButton;
                        menuChoices[4, 1] = playerTwoJumpButton;
                        menuChoices[5, 1] = playerTwoRollButton;
                        menuChoices[6, 1] = playerTwoDownDashButton;
                        menuChoices[7, 1] = playerTwoThrowButton;


                        rebindFirstFrame = false;
                        menuRow = 0;
                        menuColumn = 0;

                        displayRebindWindow = true;
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
                    if ((SingleKeyPress(Keys.Escape) || SingleButtonPress(Buttons.B)) && !displayRebindWindow)
                    {
                        menuChoices[menuRow, menuColumn].IsHighlighted = false;
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
                        //escape to menu / resume game (only allow B on Controller to work if game is paused
                        if ((SingleKeyPress(Keys.Escape) || SingleButtonPress(Buttons.Back) || SingleButtonPress(Buttons.Start) || 
                            (SingleButtonPress(Buttons.B) && menuPaused) && !tryingToQuit))
                        {
                            menuSelectSound.Play();
                            if (menuPaused) //resume game
                            {
                                menuChoices[menuRow, menuColumn].IsHighlighted = false;
                                menuPaused = false;
                                MediaPlayer.Resume();
                            }
                            else //pause game
                            {
                                menuPaused = true;
                                escapeFirstFrame = true;
                                MediaPlayer.Pause();
                            }
                            break;
                        }
                    }
                    break;

                case GameState.GameOver:
                    //any key is pressed after a certain delay
                    if (gameOverTimer.UpdateTimer(gameTime))
                    {
                        gameOverTimer = new Timer(0); //turns off timer
                        if (kb.GetPressedKeys().Length > previousKb.GetPressedKeys().Length || AnyButtonPressed())
                        {
                            //reset everything
                            gameOverTimer = new Timer(1);
                            gameState = GameState.Menu;
                            menuFirstFrame = true;
                            startMenuMusic = true;
                        }
                    }

                    break;
                case GameState.Credits:
                    if (SingleKeyPress(Keys.Escape) || SingleKeyPress(Keys.Enter) || SingleButtonPress(Buttons.Back) || SingleButtonPress(Buttons.Start))
                    {
                        menuSelectSound.Play();
                        menuFirstFrame = true;
                        gameState = GameState.Menu;
                        startMenuMusic = true;
                    }
                    break;
                case GameState.LoadScreen:
                    //Instantiate fader
                    if (!fader_exists)
                    {
                        fader = new Fader(60);
                        fader_exists = true;
                        fading_screen_rect = new Rectangle((int)(camera.CameraCenter.X - (width / 2)), (int)(camera.CameraCenter.Y - (height / 2)), width, height);
                    }
                    else
                    {
                        //fade to black screen
                        if (!fader.finished)
                        {
                            fader_last_frame = fader.finished;
                            fading_screen_alpha_percent = fader.change_alpha_percent_positive(fading_screen_alpha_percent);
                        }

                        //once the screen is black, actually switch levels
                        if (fader_exists != fader_last_frame)
                        {
                            LevelMapCurrent = levelList[LevelMapCurrent.LevelNumber + 1];
                            importantObjects = LevelMapCurrent.MapImportantObjects();
                            currentEnemyList = LevelMapCurrent.MapEnemies();
                            //reset player & camera position manually to center of root node
                            //start playing different random track

                        }

                        //fade back from black screen
                        if (fader.finished)
                        {
                            fader_last_frame = fader.finished;
                            fading_screen_alpha_percent = fader.change_alpha_percent_negative(fading_screen_alpha_percent);
                        }
                    }
                    gameState = GameState.Game;
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
                    spriteBatch.Draw(mainMenuTexture, new Rectangle(0, 0, width, height), Color.White);

                    //Draw the menu box for the buttons
                    spriteBatch.Draw(Content.Load<Texture2D>("menuImages/baseBox"), new Rectangle(width / 6, height * 7 / 24 + Nudge(false, 3.5),
                        440, 650), Color.White);

                    //draw yes/no window
                    if (tryingToQuit)
                    {
                        spriteBatch.Draw(menuBoxTexture, new Rectangle(width / 4, height / 4,
                            width / 2, height / 2), Color.White);
                    }

                    //draw each button
                    foreach (Button currentButton in menuChoices)
                    {
                        if (currentButton.CorrespondingLevel == -1) //prevents level select buttons from being drawn as non transparent during game state swap for a single frame
                        {
                            spriteBatch.Draw(currentButton.DrawnTexture, currentButton.Area, Color.White);
                        }

                        if (currentButton.IsHighlighted)
                        {
                            //draw cursor next to button
                            spriteBatch.Draw(cursorTexture, new Rectangle(currentButton.StartX - Nudge(true, 3),
                               currentButton.StartY + (currentButton.Area.Height / 2) - (height / 40), width / 40, height / 20), Color.White);
                        }
                    }

                    spriteBatch.End();
                    break;
                case GameState.LevelSelect:
                    spriteBatch.Begin();

                    spriteBatch.Draw(levelSelectScreen, new Rectangle(0, 0, width, height), Color.White);
                    //draw each button
                    foreach (Button currentButton in menuChoices)
                    {
                        if (currentButton != levelSelectReturnButton)
                        {
                            spriteBatch.Draw(currentButton.DrawnTexture, currentButton.Area, Color.Red * 0.0f); //draw the texture as transparent
                        }
                        else
                        {
                            spriteBatch.Draw(currentButton.DrawnTexture, currentButton.Area, Color.White);
                        }

                        if (currentButton.IsHighlighted)
                        {
                            //draw cursor next to button
                            spriteBatch.Draw(cursorTexture, new Rectangle(currentButton.StartX - Nudge(true, 3), 
                                currentButton.StartY + (currentButton.Area.Height / 2) - (height / 40), width / 40, height / 20), Color.White);
                        }
                    }
                    spriteBatch.End();
                    break;
                case GameState.Options:
                    spriteBatch.Begin();
                    spriteBatch.Draw(mainMenuTexture, new Rectangle(0, 0, width, height), Color.White);
                    spriteBatch.Draw(menuBoxTexture, new Rectangle(width / 4, 0, width / 2, height), Color.White);

                    //draw rebind window
                    if (displayRebindWindow)
                    {
                        spriteBatch.Draw(menuBoxTexture, new Rectangle(0, 0,
                            width, height), Color.White);
                    }
                    //draw each button
                    foreach (Button currentButton in menuChoices)
                    {
                        if (!(currentButton is RebindingButton)) //determine if this button needs transparency
                        {
                            spriteBatch.Draw(currentButton.DrawnTexture, currentButton.Area, Color.White);
                        }


                        if (currentButton.IsHighlighted)
                        {
                            if (!(currentButton is RebindingButton))
                            {
                                spriteBatch.Draw(currentButton.DrawnTexture, currentButton.Area, Color.White);
                            }


                            if (currentButton.IsHighlighted)
                            {
                                if (currentButton is Slider) //position cursor differently if slider
                                {
                                    spriteBatch.Draw(cursorTexture, new Rectangle(currentButton.StartX - Nudge(true, 4), currentButton.StartY - Nudge(false, 1.4),
                                        width / 40, height / 20), Color.White);
                                }
                                else
                                {
                                    //draw cursor next to button
                                    spriteBatch.Draw(cursorTexture, new Rectangle(currentButton.StartX - Nudge(true, 3), 
                                        currentButton.StartY + (currentButton.Area.Height / 2) - (height / 40), width / 40, height / 20), Color.White);
                                }

                            }
                        }
                        if (currentButton is Slider)
                        {
                            spriteBatch.Draw(currentButton.DrawnTexture, currentButton.Area, Color.White);
                            //draw the displayed icon of the slider
                            spriteBatch.Draw(currentButton.DisplayedIcon, new Rectangle(currentButton.X - currentButton.Area.Width / 2,
                                currentButton.Y - ((width / 25) / 2), width / 20, width / 21), Color.White);

                            //draw the sliderButton of the slider
                            spriteBatch.Draw(currentButton.SliderButton.DrawnTexture, currentButton.SliderButton.Area, Color.White);

                            //draw the number value of the slider
                            spriteBatch.DrawString(textFont, currentButton.ReturnedValue.ToString(), new Vector2(currentButton.StartX + currentButton.Area.Width + Nudge(true, 2),
                                currentButton.StartY - Nudge(false, 2.5)), Color.Black);
                        }
                        //draw the overlaying text for the buttons, and adjust the width of the buttons for the variable text
                        if (currentButton is RebindingButton)
                        {
                            
                            if (currentButton.VisibleText.Length >= 4) //if button text exceeds certain length, calculate size of button differently
                            {
                                double tempDrawUnit = width * (1.5 / 100);
                                currentButton.Area = new Rectangle(currentButton.StartX, currentButton.StartY,
                                    (int)(tempDrawUnit * currentButton.VisibleText.Length), currentButton.Area.Height);
                            }
                            else
                            {
                                //each character correlates to 2.5% of the screen
                                currentButton.Area = new Rectangle(currentButton.StartX, currentButton.StartY,
                                    (int)(drawUnit * currentButton.VisibleText.Length), currentButton.Area.Height);
                            }

                            spriteBatch.DrawString(textFont, currentButton.VisibleText, new Vector2(currentButton.StartX, currentButton.StartY), Color.Purple);
                        }
                    }
                    spriteBatch.End();
                    break;

                case GameState.Game:
                    // (NULL, NULL, NULL, NULL, CAMERA.TRANSFORM IS HOW YOU USE THE CAMERA IN THE GAME! :D)
                    spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, camera.Transform); //setup for keeping pixel art nice
                    GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;                                                //sets interpolation to nearest neighbor

                    //draw background
                    spriteBatch.Draw(backgroundTexture, backgroundRect, Color.White);

                    testTable.Draw(spriteBatch);

                    foreach (Ladder ladder in ladders)
                    {
                        if (ladder.IsDrawn)
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

                    }

                    //Drawing each player
                    foreach (Player currentPlayer in players)
                    {
                        if (currentPlayer.PlayerState != PlayerState.Die)
                        {
                            currentPlayer.Draw(spriteBatch);
                        }
                    }


                    foreach (Enemy e in currentEnemyList)
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

                    //debugging text for bug stomping
                    if (playerOne.IsDebugging)
                    {
                        spriteBatch.DrawString(textFont, "Horizontal Velocity: " + playerOne.HorizontalVelocity, new Vector2(camera.CameraCenter.X - 900, camera.CameraCenter.Y - 500), Color.Cyan);
                        spriteBatch.DrawString(textFont, "Vertical Velocity: " + playerOne.VerticalVelocity, new Vector2(camera.CameraCenter.X - 900, camera.CameraCenter.Y - 465), Color.Cyan);
                        spriteBatch.DrawString(textFont, "Player State: " + playerOne.PlayerState, new Vector2(camera.CameraCenter.X - 900, camera.CameraCenter.Y - 430), Color.Cyan);
                        spriteBatch.DrawString(textFont, "Facing right?: " + playerOne.IsFacingRight, new Vector2(camera.CameraCenter.X - 900, camera.CameraCenter.Y - 395), Color.Cyan);
                        spriteBatch.Draw(playerTwoTexture, cakeManager.PuttingDownChecker, Color.Blue); //cake collision checker
                    }
                    if (playerTwo.IsDebugging)
                    {
                        spriteBatch.DrawString(textFont, "Horizontal Velocity: " + playerTwo.HorizontalVelocity, new Vector2(camera.CameraCenter.X + 300, camera.CameraCenter.Y - 500), Color.Red);
                        spriteBatch.DrawString(textFont, "Vertical Velocity: " + playerTwo.VerticalVelocity, new Vector2(camera.CameraCenter.X + 300, camera.CameraCenter.Y - 465), Color.Red);
                        spriteBatch.DrawString(textFont, "Player State: " + playerTwo.PlayerState, new Vector2(camera.CameraCenter.X + 300, camera.CameraCenter.Y - 430), Color.Red);
                        spriteBatch.DrawString(textFont, "Facing right?: " + playerTwo.IsFacingRight, new Vector2(camera.CameraCenter.X + 300, camera.CameraCenter.Y - 395), Color.Red);
                        spriteBatch.Draw(playerTwoTexture, cakeManager.PuttingDownChecker, Color.Blue); //cake collision checker
                    }

                    //drawing out level tiles
                    LvlCoordinator.Draw(spriteBatch);

                    if (playerTwo.IsDebugging)
                    {
                        spriteBatch.DrawString(textFont, "Horizontal Velocity: " + currentEnemyList[0].HorizontalVelocity, new Vector2(camera.CameraCenter.X + 300, camera.CameraCenter.Y + 260), Color.Yellow);
                        spriteBatch.DrawString(textFont, "Vertical Velocity: " + currentEnemyList[0].VerticalVelocity, new Vector2(camera.CameraCenter.X + 300, camera.CameraCenter.Y + 300), Color.Yellow);
                        spriteBatch.DrawString(textFont, "Enemy State: " + currentEnemyList[0].EnemyState, new Vector2(camera.CameraCenter.X + 300, camera.CameraCenter.Y + 340), Color.Yellow);
                        spriteBatch.DrawString(textFont, "Walking State: " + currentEnemyList[0].WalkingState, new Vector2(camera.CameraCenter.X + 300, camera.CameraCenter.Y + 380), Color.Yellow);
                        spriteBatch.DrawString(textFont, "Target: " + currentEnemyList[0].TargetDebug, new Vector2(camera.CameraCenter.X + 300, camera.CameraCenter.Y + 420), Color.Yellow);
                        spriteBatch.DrawString(textFont, "TargetLoc: " + pathManager.TargetLocation, new Vector2(camera.CameraCenter.X + 300, camera.CameraCenter.Y + 460), Color.Yellow);
                        spriteBatch.DrawString(textFont, "C Map: \n" + pathManager.CorrectMap, new Vector2(camera.CameraCenter.X + -900, camera.CameraCenter.Y - 300), Color.Yellow);
                    }

                    if (menuPaused)
                    {
                        spriteBatch.Draw(menuBoxTexture, new Rectangle((int)camera.CameraCenter.X - width / 4, (int)camera.CameraCenter.Y - height / 3,
                            width / 2, height * 3 / 5), Color.White);

                        if (tryingToQuit)
                        {
                            spriteBatch.Draw(menuBoxTexture, new Rectangle((int)camera.CameraCenter.X - width / 4, (int)camera.CameraCenter.Y - (height * 3 / 10),
                                width / 2, height / 2), Color.White);
                        }

                        //draw each button
                        foreach (Button currentButton in menuChoices)
                        {
                            spriteBatch.Draw(currentButton.DrawnTexture, currentButton.Area, Color.White);

                            if (currentButton.IsHighlighted)
                            {
                                //draw cursor next to button
                                spriteBatch.Draw(cursorTexture, new Rectangle(currentButton.X - Nudge(true, 3), 
                                    currentButton.Y + (currentButton.Area.Height / 2) - (height / 40), width / 40, height / 20), Color.White);
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
                    spriteBatch.DrawString(textFont, "Press any key to return to menu", new Vector2(width / 4, height / 3), Color.White);
                    spriteBatch.End();
                    break;

                case GameState.LoadScreen:
                    fader.Draw(spriteBatch, fading_screen_texture, fading_screen_rect, fading_screen_alpha_percent);
                    break;

                case GameState.Credits:
                    spriteBatch.Begin();
                    spriteBatch.DrawString(textFont,"Credits will go here!", new Vector2(width / 4, height / 3), Color.White);
                    spriteBatch.End();
                    break;
            }
        }

        /// <summary>
        /// uses 2d array variables (row/column) to navigate, then calls helper method for button selection
        /// </summary>
        /// <param name="beginningRow"></param>
        /// <param name="beginningColumn"></param>
        public void NavigateMenu(int beginningRow, int beginningColumn)
        {
            //start menu selection at correct spot
            int currentRow = beginningRow;
            int currentColumn = beginningColumn;


            //slider
            if (menuChoices[currentRow, currentColumn] is Slider)
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
                    if (SingleKeyPress(playerOne.BindableKb["right"]) || SingleKeyPress(Keys.Right) || SingleKeyPress(playerTwo.BindableKb["right"]) || SingleButtonPress(Buttons.DPadRight) || FlickGamepadRight())
                    {
                        sliderHoldingCounter = 0;
                        menuChoices[currentRow, currentColumn].CheckAndAlterSlider(true);
                    }
                    //player holding down a key
                    else if (HoldKey(playerOne.BindableKb["right"]) || HoldKey(Keys.Right) || HoldKey(playerTwo.BindableKb["right"]) || HoldButton(Buttons.DPadRight) || GamepadRight())
                    {
                        sliderHoldingCounter++;
                        //delay
                        if (sliderHoldingCounter > 50 && sliderHoldingCounter % 1 == 0)
                        {
                            menuChoices[currentRow, currentColumn].CheckAndAlterSlider(true);
                        }
                    }
                    else if (SingleKeyPress(playerOne.BindableKb["left"]) || SingleKeyPress(Keys.Left) || SingleKeyPress(playerTwo.BindableKb["left"]) || SingleButtonPress(Buttons.DPadLeft) || FlickGamepadLeft())
                    {
                        sliderHoldingCounter = 0;
                        menuChoices[currentRow, currentColumn].CheckAndAlterSlider(false);
                    }
                    //holding down a key
                    else if (HoldKey(playerOne.BindableKb["left"]) || HoldKey(Keys.Left) || HoldKey(playerTwo.BindableKb["left"]) || HoldButton(Buttons.DPadLeft) || GamepadLeft())
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

            //Single press up
            if ((SingleKeyPress(playerOne.BindableKb["up"]) || SingleKeyPress(playerTwo.BindableKb["up"]) || SingleKeyPress(Keys.Up) || SingleButtonPress(Buttons.DPadUp) || FlickGamepadUp())
                    && !lockedSelection)
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
            //holding up
            else if (HoldKey(playerOne.BindableKb["up"]) || HoldKey(Keys.Up) || HoldKey(playerTwo.BindableKb["up"]) || HoldButton(Buttons.DPadUp) || GamepadUp() && !lockedSelection)
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
            //Single press down
            else if ((SingleKeyPress(playerOne.BindableKb["downDash"]) || SingleKeyPress(playerTwo.BindableKb["downDash"]) || SingleKeyPress(Keys.Down) || SingleButtonPress(Buttons.DPadDown) || FlickGamepadDown())
            && !lockedSelection)
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
            //Holding down
            else if ((HoldKey(playerOne.BindableKb["downDash"]) || HoldKey(Keys.Down) || HoldKey(playerTwo.BindableKb["downDash"]) || HoldButton(Buttons.DPadDown) || GamepadDown()) && !lockedSelection)
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
            //Single press left
            else if ((SingleKeyPress(playerOne.BindableKb["left"]) || SingleKeyPress(playerTwo.BindableKb["left"]) || SingleKeyPress(Keys.Left) || SingleButtonPress(Buttons.DPadLeft) || FlickGamepadLeft())
            && !lockedSelection)
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
            //Holding left
            else if ((HoldKey(playerOne.BindableKb["left"]) || HoldKey(Keys.Left) || HoldKey(playerTwo.BindableKb["left"]) || HoldButton(Buttons.DPadLeft) || GamepadLeft()) && !lockedSelection)
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
            //Single press right
            else if ((SingleKeyPress(playerOne.BindableKb["right"]) || SingleKeyPress(playerTwo.BindableKb["right"]) || SingleKeyPress(Keys.Right) || SingleButtonPress(Buttons.DPadRight) || FlickGamepadRight())
            && !lockedSelection)
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
            //Holding right
            else if ((HoldKey(playerOne.BindableKb["right"]) || HoldKey(Keys.Right) || HoldKey(playerTwo.BindableKb["right"]) || HoldButton(Buttons.DPadRight) || GamepadRight()) && !lockedSelection)
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

            //always highlight the correct button
            menuChoices[currentRow, currentColumn].IsHighlighted = true;

            //used so that the array is at the correct indices in the next frame
            menuRow = currentRow;
            menuColumn = currentColumn;

            ButtonSelection(currentRow, currentColumn);
        }

        /// <summary>
        /// helper method for NavigateMenu that determines which gamestate to switch to based on selected button
        /// </summary>
        /// <param name="currentRow"></param>
        /// <param name="currentColumn"></param>
        public void ButtonSelection(int currentRow, int currentColumn)
        {
            #region Hard Coding Button positions
            resumeButton.X = resumeButton.StartX;
            resumeButton.Y = resumeButton.StartY;

            gameOptionsButton.X = gameOptionsButton.StartX;
            gameOptionsButton.Y = gameOptionsButton.StartY;

            gameExitButton.X = gameExitButton.StartX;
            gameExitButton.Y = gameExitButton.StartY;

            yesButton.X = yesButton.StartX;
            yesButton.Y = yesButton.StartY;

            noButton.X = noButton.StartX;
            noButton.Y = noButton.StartY;
            #endregion

            if (SingleKeyPress(Keys.Enter) || SingleButtonPress(Buttons.A) || 
                (LeftMouseSinglePress(ButtonState.Pressed) && mouseRect.Intersects(menuChoices[currentRow, currentColumn].Area)))
            {
                //not allowed to select this button
                if (menuChoices[currentRow, currentColumn].IsLocked)
                {
                    errorSound.Play();
                }
                else //allowed to select the button
                {
                    menuSelectSound.Play();
                    if (menuChoices[currentRow, currentColumn].Equals(playButton))
                    {
                        menuChoices[currentRow, currentColumn].IsHighlighted = false;
                        secondaryGameState = gameState;
                        gameState = GameState.Game;
                    }
                    else if (menuChoices[currentRow, currentColumn].Equals(levelSelectButton))
                    {
                        menuChoices[currentRow, currentColumn].IsHighlighted = false;
                        gameState = GameState.LevelSelect;
                        levelSelectFirstFrame = true;
                    }
                    else if (menuChoices[currentRow, currentColumn].Equals(menuOptionsButton))
                    {
                        menuChoices[currentRow, currentColumn].IsHighlighted = false;
                        gameState = GameState.Options;
                        optionsFirstFrame = true;
                    }
                    else if (menuChoices[currentRow, currentColumn].Equals(creditsButton))
                    {
                        menuChoices[currentRow, currentColumn].IsHighlighted = false;
                        gameState = GameState.Credits;
                        MediaPlayer.Play(gameSongs[0]);
                    }
                    else if (menuChoices[currentRow, currentColumn].Equals(menuExitButton))
                    {
                        menuChoices[currentRow, currentColumn].IsHighlighted = false;
                        quitFirstFrame = true;
                    }
                    else if (menuChoices[currentRow, currentColumn].Equals(levelSelectReturnButton))
                    {
                        menuChoices[currentRow, currentColumn].IsHighlighted = false;
                        gameState = GameState.Menu;
                        menuFirstFrame = true;
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
                            menuChoices = new Button[5, 1];
                            menuChoices[0, 0] = playButton;
                            menuChoices[1, 0] = levelSelectButton;
                            menuChoices[2, 0] = menuOptionsButton;
                            menuChoices[3, 0] = creditsButton;
                            menuChoices[4, 0] = menuExitButton;

                            menuRow = 4;
                            menuColumn = 0;
                        }
                    }
                    else if (menuChoices[currentRow, currentColumn].Equals(yesButton))
                    {
                        if (gameState == GameState.Game) //exit from the game to the menu
                        {
                            //RESET THE LEVEL HERE

                            menuChoices[currentRow, currentColumn].IsHighlighted = false;
                            gameState = GameState.Menu;
                            menuFirstFrame = true;
                            tryingToQuit = false;
                            menuPaused = false;

                        }
                        else //exit from the menu hence quit the game
                        {
                            //saving done in overriden "OnExiting" Method near the bottom of this class
                            Exit();
                        }

                    }
                    else if (menuChoices[currentRow, currentColumn].Equals(rebindButton))
                    {
                        menuChoices[currentRow, currentColumn].IsHighlighted = false;
                        rebindFirstFrame = true;
                    }
                    else if (menuChoices[currentRow, currentColumn].Equals(controllerMapButton))
                    {
                        //do controller image stuff once that's implemented
                    }
                    else if (menuChoices[currentRow, currentColumn].Equals(optionsReturnButton))
                    {
                        menuChoices = new Button[7, 1];
                        menuChoices[0, 0] = returnButton;
                        menuChoices[1, 0] = fullscreenButton;
                        menuChoices[2, 0] = masterVolumeSlider;
                        menuChoices[3, 0] = musicSlider;
                        menuChoices[4, 0] = soundEffectSlider;
                        menuChoices[5, 0] = controllerMapButton;
                        menuChoices[6, 0] = rebindButton;
                        menuRow = 6;
                        menuColumn = 0;
                        displayRebindWindow = false;
                    }
                    else if (menuChoices[currentRow, currentColumn].Equals(resetButton))
                    {
                        playerOneLeftButton.SetNewKey(Keys.A);
                        playerOneRightButton.SetNewKey(Keys.D);
                        playerOneUpButton.SetNewKey(Keys.W);
                        playerOneJumpButton.SetNewKey(Keys.Space);
                        playerOneRollButton.SetNewKey(Keys.LeftShift);
                        playerOneDownDashButton.SetNewKey(Keys.S);
                        playerOneThrowButton.SetNewKey(Keys.C);


                        playerTwoLeftButton.SetNewKey(Keys.Left);
                        playerTwoRightButton.SetNewKey(Keys.Right);
                        playerTwoUpButton.SetNewKey(Keys.RightShift);
                        playerTwoJumpButton.SetNewKey(Keys.Up);
                        playerTwoRollButton.SetNewKey(Keys.RightControl);
                        playerTwoDownDashButton.SetNewKey(Keys.Down);
                        playerTwoThrowButton.SetNewKey(Keys.Enter);
                    }
                    else if (menuChoices[currentRow, currentColumn] is RebindingButton)
                    {
                        menuChoices[currentRow, currentColumn].TryingToRebind = !menuChoices[currentRow, currentColumn].TryingToRebind; //toggle whether or not rebinding with enter
                    }
                    else //LevelSelectButtons
                    {
                        foreach(Map targetLevel in levelList)
                        {
                            if (menuChoices[currentRow, currentColumn].CorrespondingLevel == targetLevel.LevelNumber) //if the targeted button is the right level, start that level
                            {
                                menuChoices[currentRow, currentColumn].IsHighlighted = false;
                                LevelMapCurrent = targetLevel;
                                /* Set up rest of info from target LEvel into this.*/
                                gameState = GameState.Game;
                                break;
                            }
                        }
                    }
                }     
            }
            else if (SingleKeyPress(Keys.Escape) || SingleButtonPress(Buttons.B))
            {
                menuSelectSound.Play();
                //escape from quit popup
                if (tryingToQuit)
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
                        menuChoices = new Button[5, 1];
                        menuChoices[0, 0] = playButton;
                        menuChoices[1, 0] = levelSelectButton;
                        menuChoices[2, 0] = menuOptionsButton;
                        menuChoices[3, 0] = creditsButton;
                        menuChoices[4, 0] = menuExitButton;

                        menuRow = 4;
                        menuColumn = 0;
                    }
                }
                //escape from rebindpopup if not trying to rebind
                else if (displayRebindWindow && !menuChoices[currentRow,currentColumn].TryingToRebind)
                {
                    menuChoices[currentRow, currentColumn].IsHighlighted = false;

                    menuChoices = new Button[7, 1];
                    menuChoices[0, 0] = returnButton;
                    menuChoices[1, 0] = fullscreenButton;
                    menuChoices[2, 0] = masterVolumeSlider;
                    menuChoices[3, 0] = musicSlider;
                    menuChoices[4, 0] = soundEffectSlider;
                    menuChoices[5, 0] = controllerMapButton;
                    menuChoices[6, 0] = rebindButton;
                    menuRow = 6;
                    menuColumn = 0;
                    displayRebindWindow = false;
                }
                //escape from level select screen
                else if (gameState == GameState.LevelSelect)
                {
                    menuChoices[currentRow, currentColumn].IsHighlighted = false;
                    menuFirstFrame = true;
                    gameState = GameState.Menu;
                }
                else if (gameState == GameState.Menu)
                {
                    menuChoices[currentRow, currentColumn].IsHighlighted = false;
                    menuChoices = new Button[1, 2];
                    menuChoices[0, 0] = noButton;
                    menuChoices[0, 1] = yesButton;

                    menuRow = 0;
                    menuColumn = 0;

                    quitFirstFrame = false;
                    tryingToQuit = true;
                }
            }
            //only check for rebinding keys if in the right window
            if (displayRebindWindow)
            {
                //trying to rebind a key
                if (menuChoices[currentRow, currentColumn] is RebindingButton && menuChoices[currentRow, currentColumn].TryingToRebind)
                {
                    lockedSelection = menuChoices[currentRow, currentColumn].SetNewKey(); //lock the selection until a key is pressed, then adjust the key once it is pressed
                }
                //otherwise, unlock selection
                else if (menuChoices[currentRow, currentColumn] is RebindingButton)
                {
                    lockedSelection = false;
                }
            }

            #region Hard Coding Button positions
            resumeButton.X += (int)resumeDifference.X;
            resumeButton.Y += (int)resumeDifference.Y;

            gameOptionsButton.X += (int)optionsDifference.X;
            gameOptionsButton.Y += (int)optionsDifference.Y;

            gameExitButton.X += (int)exitDifference.X;
            gameExitButton.Y += (int)exitDifference.Y;

            if (gameState != GameState.Menu)
            {
                yesButton.X += (int)yesDifference.X;
                yesButton.Y += (int)yesDifference.Y;

                noButton.X += (int)noDifference.X;
                noButton.Y += (int)noDifference.Y;
            }
            #endregion


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

        /// <summary>
        /// check if a button has been held down for multiple frames
        /// </summary>
        /// <param name="pressedButton"></param>
        /// <returns></returns>
        public bool HoldButton(Buttons pressedButton)
        {
            //gamepad2 also working
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
                return (int)(width * (amount / 100));
            }
            else
            {
                return (int)(height * (amount / 100));
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

            //sound settings
            textWriter.WriteLine(masterVolumeSlider.ReturnedValue);
            textWriter.WriteLine(musicSlider.ReturnedValue);
            textWriter.WriteLine(soundEffectSlider.ReturnedValue);

            //keybinding settings
            
            //to iterate through a dictionary, use KeyValuePair
            foreach (KeyValuePair<string, Keys> entry in playerOne.BindableKb)
            {
                textWriter.WriteLine(entry.Value); //just need the actual key, not the associated string
            }
            foreach (KeyValuePair<string, Keys> entry in playerTwo.BindableKb)
            {
                textWriter.WriteLine(entry.Value); //just need the actual key, not the associated string
            }

            //fullscreen toggle
            textWriter.WriteLine(graphics.IsFullScreen.ToString());

            textWriter.WriteLine(levelTwoButton.IsLocked.ToString());
            textWriter.WriteLine(levelThreeButton.IsLocked.ToString());
            textWriter.WriteLine(levelFourButton.IsLocked.ToString());
            textWriter.WriteLine(levelFiveButton.IsLocked.ToString());
            textWriter.WriteLine(levelSixButton.IsLocked.ToString());
            textWriter.WriteLine(levelSevenButton.IsLocked.ToString());
            textWriter.WriteLine(levelEightButton.IsLocked.ToString());
            textWriter.WriteLine(levelNineButton.IsLocked.ToString());
            textWriter.Close();

        }
        #region GAMEPAD STICK CONTROL
        /// <summary>
        /// player trying to move left with dpad/leftThumbstick
        /// </summary>
        /// <returns></returns>
        public bool GamepadLeft()
        {
            if (gp1.ThumbSticks.Left.X < -0.5f || gp2.ThumbSticks.Left.X < -0.5f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Used for single button movement of the left stick, used for menu navigation
        /// </summary>
        /// <returns></returns>
        public bool FlickGamepadLeft()
        {
            if ((previousGp1.ThumbSticks.Left.X >= -0.5f && gp1.ThumbSticks.Left.X < -0.5f) || (previousGp2.ThumbSticks.Left.X >= -0.5f && gp2.ThumbSticks.Left.X < -0.5f))
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
            if (gp1.ThumbSticks.Left.X > 0.5f || gp2.ThumbSticks.Left.X > 0.5f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Used for single button movement of the left stick, used for menu navigation
        /// </summary>
        /// <returns></returns>
        public bool FlickGamepadRight()
        {
            if ((previousGp1.ThumbSticks.Left.X <= 0.5f && gp1.ThumbSticks.Left.X > 0.5f) || (previousGp2.ThumbSticks.Left.X <= 0.5f && gp2.ThumbSticks.Left.X > 0.5f))
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
            if (gp1.ThumbSticks.Left.Y > 0.5f || gp2.ThumbSticks.Left.Y > 0.5f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Used for single button movement of the left stick, used for menu navigation
        /// </summary>
        /// <returns></returns>
        public bool FlickGamepadUp()
        {
            if ((previousGp1.ThumbSticks.Left.Y <= 0.5f && gp1.ThumbSticks.Left.Y > 0.5f) || (previousGp2.ThumbSticks.Left.Y <= 0.5f && gp2.ThumbSticks.Left.Y > 0.5f))
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
            if (gp1.ThumbSticks.Left.Y < -0.5f || gp2.ThumbSticks.Left.Y < -0.5f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Used for single button movement of the left stick, used for menu navigation
        /// </summary>
        /// <returns></returns>
        public bool FlickGamepadDown()
        {
            if ((previousGp1.ThumbSticks.Left.Y >= -0.5f && gp1.ThumbSticks.Left.Y < -0.5f) || (previousGp2.ThumbSticks.Left.Y >= -0.5f && gp2.ThumbSticks.Left.Y < -0.5f))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
        /// <summary>
        /// returns true if any button is pressed by any controller (for a single frame), returns false otherwise
        /// </summary>
        /// <returns></returns>
        public bool AnyButtonPressed()
        {
            Buttons[] buttonArray = (Buttons[])Enum.GetValues(typeof(Buttons));

            foreach (Buttons button in buttonArray)
            {
                if (SingleButtonPress(button))
                {
                    return true;
                }
            }
            return false;
        }
    }
}