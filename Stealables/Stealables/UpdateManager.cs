using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LumberjackFantasy
{
	class UpdateManager
	{
        #region Fields
        private Random rng;                             // Random Number Generator used for bears and bear speeds.
		private GameTime gameTime;                      // Holds the current GameTime
		private Player pCurrent;                        // Holds the player's values
		private List<Bear> bearsCurrent;                // Holds all of the bears in the game
		private List<Tree> treesCurrent;                // Holds all of the treesInTheGame
        private List<PickUp> pickUpsCurrent;            // Holds all the pickups in the game
		private List<BackgroundTile> backgrounds;       // Holds all of the backgrounds for the game

		private KeyboardState currentKB;                // Holds the current Kb State
		private KeyboardState previousKB;               // Holds the previous Kb State (if needed)

        private double waitTimeNextLevel;               //time to wait between levels

        private AnimationManager aM;                    // Animation Manager
        private bool animate;                           // Tells things to animate
		private Rectangle[] healthCords;                // Holds location of the helath hearts.
		private Vector2 hsCord;							// Holds location of the high score
        private OpenSeasonManager oS;                   // Manages what occurs if the open season state occurs
        private bool runOpenSeason;                     // Determines if OpenSeason needs to be set, updated, or return to default state
        public Camera camera;                           // Holds the cameras positions
        private int currentLevel;                       // Holds the current Level (-1) (Makes sense in context of # of trees to cut this level)
        private int gameMaxLevel;                       // Max Amount of Levels in the game (use in # trees to cut this level logic)
        private int totalTreesToCut;                    // Total Trees to cut this level! (Determined based on Trees Currnent, current level, and max level!)

        private List<Texture2D> uiTextures;             // Holds the textures used in the UI
        private Texture2D nextLevelTexture;             // Holds the next level texture. Not apart of UI
        
        private int screenWidth;                        // Holds the Map/Level Width
        private int screenHeight;                       // Holds the Map/Level Height

		VelocityManager velocityManager = new VelocityManager(0);
        CollisionManager collisionManager;
        #endregion Fields
        #region Properties
        // Properties
        public List<Bear> BearsCurrent
		{
			get { return bearsCurrent; }
			set { bearsCurrent = value; }
		}
		public List<Tree> TreesCurrent
		{
			get { return treesCurrent; }
			set { treesCurrent = value; }
		}
		public List<PickUp> PickUpsCurrent
		{
			get { return pickUpsCurrent; }
			set { pickUpsCurrent = value; }
		}

        public int CurrentLevel
        {
            set { currentLevel = value; }
        }

        #endregion Properties
        #region Constructor
        /// <summary>
        /// Constructor - Leave Blank. Update Manager should recieve data based on it's data retrieving methods 
        /// </summary>
        public UpdateManager(int screenWidthMax, int screenHeightMax, Texture2D camera, int gameMaxLevel, List<Texture2D> textures, Texture2D nextLevelTexture, List<BackgroundTile> list)
		{
            // Sets Textures
            uiTextures = textures;
            this.nextLevelTexture = nextLevelTexture;

            // Creates Managers and rng
            aM = new AnimationManager();
            animate = false;
            collisionManager = new CollisionManager(screenWidthMax, screenHeightMax);
			rng = new Random();
            this.camera = new Camera(10, camera); //instantiate the camera, 10 is a placeholder value


            // Cords for UI stuff
            healthCords = new Rectangle[5] {new Rectangle(178,16, 23, 18), new Rectangle(156,16,23,18), new Rectangle(133, 16,23,18), new Rectangle(111, 16, 23, 18), new Rectangle(88, 16, 23, 18) };
			hsCord = new Vector2(777, 10);

            // Open Season
            oS = new OpenSeasonManager();
            runOpenSeason = false;

            // Other Info needed 
            this.gameMaxLevel = gameMaxLevel;
            screenHeight = screenHeightMax;
            screenWidth = screenWidthMax;
            waitTimeNextLevel = 3;

			backgrounds = list;
        }

        #endregion Constructor
        #region uM Universal Stuff
        // --------------------------------------------------------------------- Universal Updates and Draws for  Screen State ------------------------------------------------------

        /// <summary>
        /// Update method called when the game is actually being played
        /// </summary>
        public GameState UpdateGameScreen()
		{
			GameState toreturn = GameState.gameLoop;


           
            if(runOpenSeason == true)
            {
                DetermineOpenSeason();
            }
            UpdateAnimations();
			UpdatePlayer();
            UpdateCamera();
            UpdateAllBears();
            UpdateBearsAttack();
            UpdatePlayerBearInteraction();
			RemoveStuffFromStoredLists();
            

			if (pCurrent.Health <= 0)
			{
				toreturn = GameState.gameOver;
			}
            if(totalTreesToCut <= 0)
            {
                toreturn = GameState.loadLevel;
                if(gameMaxLevel == (currentLevel + 1))
                {
                    toreturn = GameState.gameOver;
                    currentLevel = 0;
                    return toreturn;
                }
                currentLevel++;
            }
			if (currentKB.IsKeyDown(Keys.P))
			{
				toreturn = GameState.pause;
			}
			return toreturn;
		}

        /// <summary>
        /// Provides a visual change between levels.
        /// </summary>
        /// <returns></returns>
        public bool WaitNextLevel(GameTime gametime)
        {
            waitTimeNextLevel -= gametime.ElapsedGameTime.TotalSeconds;

            if(waitTimeNextLevel<= 0)
            {
                waitTimeNextLevel = 3;
                return false;
            }
            return true;
        }

        public void DrawWaitingScreen(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            // Draws Background Texture.
            spriteBatch.Draw(nextLevelTexture, new Vector2(0,0), Color.White);
            // Draws Level Text
            spriteBatch.DrawString(spriteFont,"LVL "+ (currentLevel+1), new Vector2(240, 300), Color.White);

        }

        ///<summary>
        ///Draw method called during the gameLoop
        ///</summary>
        public void DrawGame(SpriteBatch spriteBatch, SpriteFont spriteFont)
		{
			if (backgrounds != null)
			{
				foreach (BackgroundTile thisBg in backgrounds)
				{
					if (thisBg.OnScreen)
					{
						thisBg.Draw(spriteBatch, camera.CameraPosition.Location.ToVector2());
					}
				}
			}

			//draw player
			pCurrent.Draw(spriteBatch, camera.CameraPosition.Location.ToVector2());

			//draws all trees on screen
			if (treesCurrent != null)
			{
				foreach (Tree thisObject in treesCurrent)
				{
					if (thisObject.OnScreen)
					{
						thisObject.Draw(spriteBatch, camera.CameraPosition.Location.ToVector2());
					}
				}
			}

			//if attacking draw attack
			if (pCurrent.IsAttacking)
			{
				if (pCurrent.Attack == AttackVariation.axe)
				{
					if (pCurrent.AttackDirection == PlayerAttackDirection.left)
					{
						spriteBatch.Draw(uiTextures[9], new Rectangle(pCurrent.PlayerAttackBox.X - camera.CameraPosition.X, pCurrent.PlayerAttackBox.Y - camera.CameraPosition.Y, 
							pCurrent.PlayerAttackBox.Width, pCurrent.PlayerAttackBox.Height), new Rectangle(0,pCurrent.AttackAnimationF * 100 ,50, 100) , Color.White, 0.0f, 
							Vector2.Zero, SpriteEffects.FlipHorizontally, 0.0f);
					}
					else if (pCurrent.AttackDirection == PlayerAttackDirection.right)
					{
						spriteBatch.Draw(uiTextures[9], new Rectangle(pCurrent.PlayerAttackBox.X - camera.CameraPosition.X, pCurrent.PlayerAttackBox.Y - camera.CameraPosition.Y,
							pCurrent.PlayerAttackBox.Width, pCurrent.PlayerAttackBox.Height), new Rectangle(0, pCurrent.AttackAnimationF * 100, 50, 100), Color.White);
					}
					else if (pCurrent.AttackDirection == PlayerAttackDirection.up)
					{
						spriteBatch.Draw(uiTextures[10], new Rectangle(pCurrent.PlayerAttackBox.X - camera.CameraPosition.X, pCurrent.PlayerAttackBox.Y - camera.CameraPosition.Y,
							pCurrent.PlayerAttackBox.Width, pCurrent.PlayerAttackBox.Height), new Rectangle(pCurrent.AttackAnimationF *100, 0, 100, 50), Color.White);
					}
					else
					{
						spriteBatch.Draw(uiTextures[10], new Rectangle(pCurrent.PlayerAttackBox.X - camera.CameraPosition.X, pCurrent.PlayerAttackBox.Y - camera.CameraPosition.Y,
							pCurrent.PlayerAttackBox.Width, pCurrent.PlayerAttackBox.Height), new Rectangle(pCurrent.AttackAnimationF * 100, 0, 100, 50), Color.White, 0.0f, 
							Vector2.Zero, SpriteEffects.FlipVertically,0.0f);
					}
				}
				else
				{
					if (pCurrent.AttackDirection == PlayerAttackDirection.left)
					{
						//draw shotgun
						spriteBatch.Draw(uiTextures[12], new Rectangle(pCurrent.ObjectCollisionBox.X - camera.CameraPosition.X, 
							pCurrent.ObjectCollisionBox.Y - camera.CameraPosition.Y + 28,
							78, 32), new Rectangle(0, pCurrent.AttackAnimationF * 32, 78, 32), Color.White, 0.0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.0f);

						//draw pattern
						spriteBatch.Draw(uiTextures[11], new Rectangle(pCurrent.PlayerAttackBox.X - camera.CameraPosition.X, pCurrent.PlayerAttackBox.Y - camera.CameraPosition.Y,
							pCurrent.PlayerAttackBox.Width, pCurrent.PlayerAttackBox.Height), new Rectangle(pCurrent.AttackAnimationF * 275, 0, 275, 275), Color.White, 0.0f,
							Vector2.Zero, SpriteEffects.FlipHorizontally, 0.0f);
					}
					else if (pCurrent.AttackDirection == PlayerAttackDirection.right)
					{
						//draw shotgun
						spriteBatch.Draw(uiTextures[12], new Rectangle(pCurrent.ObjectCollisionBox.X - camera.CameraPosition.X + 20, 
							pCurrent.ObjectCollisionBox.Y + 28 - camera.CameraPosition.Y,
							78, 32), new Rectangle (0, pCurrent.AttackAnimationF * 32, 78, 32),Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.0f);
						//draw pattern
						spriteBatch.Draw(uiTextures[11], new Rectangle(pCurrent.PlayerAttackBox.X - camera.CameraPosition.X, pCurrent.PlayerAttackBox.Y - camera.CameraPosition.Y,
							pCurrent.PlayerAttackBox.Width, pCurrent.PlayerAttackBox.Height), new Rectangle(pCurrent.AttackAnimationF * 275, 0, 275, 275), Color.White);
					}
					else if (pCurrent.AttackDirection == PlayerAttackDirection.up)
					{
						//draw shotgun
						spriteBatch.Draw(uiTextures[13], new Rectangle(pCurrent.ObjectCollisionBox.X - camera.CameraPosition.X + 31,
							pCurrent.ObjectCollisionBox.Y - camera.CameraPosition.Y,
							32, 78), new Rectangle(pCurrent.AttackAnimationF * 32, 0, 32, 78), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.0f);
						//draw pattern
						spriteBatch.Draw(uiTextures[14], new Rectangle(pCurrent.PlayerAttackBox.X - camera.CameraPosition.X, pCurrent.PlayerAttackBox.Y - camera.CameraPosition.Y,
							pCurrent.PlayerAttackBox.Width, pCurrent.PlayerAttackBox.Height), new Rectangle(0, pCurrent.AttackAnimationF * 275, 275, 275), Color.White, 0.0f,
							Vector2.Zero, SpriteEffects.FlipVertically, 0.0f);
					}
					else
					{
						//draw shotgun
						spriteBatch.Draw(uiTextures[13], new Rectangle(pCurrent.ObjectCollisionBox.X - camera.CameraPosition.X + 31,
							pCurrent.ObjectCollisionBox.Y + 20 - camera.CameraPosition.Y,
							32, 78), new Rectangle(pCurrent.AttackAnimationF * 32, 0, 32, 78), Color.White, 0.0f, Vector2.Zero, SpriteEffects.FlipVertically, 0.0f);
						//draw pattern
						spriteBatch.Draw(uiTextures[14], new Rectangle(pCurrent.PlayerAttackBox.X - camera.CameraPosition.X, pCurrent.PlayerAttackBox.Y - camera.CameraPosition.Y,
							pCurrent.PlayerAttackBox.Width, pCurrent.PlayerAttackBox.Height), new Rectangle(0, pCurrent.AttackAnimationF * 275, 275, 275), Color.White, 0.0f,
							Vector2.Zero, SpriteEffects.None, 0.0f);
					}
				}
			}
			//draw all bears on screen
			if (bearsCurrent != null)
			{
				foreach (Bear thisObject in bearsCurrent)
				{
					if (thisObject.OnScreen)
					{
						thisObject.Draw(spriteBatch, camera.CameraPosition.Location.ToVector2());
                        BearDirection attackDirection = thisObject.BearDirection;
                        if ((int)attackDirection > 3)
                        {
                            if((int)attackDirection == 4 || (int)attackDirection == 5)
                            {
                                if (Math.Abs(thisObject.PosX - pCurrent.PosX) > Math.Abs(thisObject.PosY - pCurrent.PosY))
                                {
                                    attackDirection = BearDirection.left;
                                }
                                if (Math.Abs(thisObject.PosX - pCurrent.PosX) < Math.Abs(thisObject.PosY - pCurrent.PosY))
                                {
                                    if (thisObject.PosY - pCurrent.PosY > 0)
                                    {
                                        attackDirection = BearDirection.down;
                                    }
                                    else
                                    {
                                        attackDirection = BearDirection.up;
                                    }
                                }
                            }
                            if ((int)attackDirection == 6 || (int)attackDirection == 7)
                            {
                                if (Math.Abs(thisObject.PosX - pCurrent.PosX) > Math.Abs(thisObject.PosY - pCurrent.PosY))
                                {
                                    attackDirection = BearDirection.right;
                                }
                                if (Math.Abs(thisObject.PosX - pCurrent.PosX) < Math.Abs(thisObject.PosY - pCurrent.PosY))
                                {
                                    if (thisObject.PosY - pCurrent.PosY > 0)
                                    {
                                        attackDirection = BearDirection.down;
                                    }
                                    else
                                    {
                                        attackDirection = BearDirection.up;
                                    }
                                }
                            }
                        }
                                if (thisObject.IsAttacking) //Draw Bears Attacking
                        {
                            if (attackDirection == BearDirection.up)
                            {
                                spriteBatch.Draw(uiTextures[16], new Rectangle(thisObject.AttackBox.X - camera.CameraPosition.X, thisObject.AttackBox.Y - camera.CameraPosition.Y,
                            100, 50), new Rectangle(0 , thisObject.AttackAnimationF * 50, 100, 50), Color.White, 0.0f,
                            Vector2.Zero, SpriteEffects.None, 0.0f);
                            }
                            if (attackDirection == BearDirection.down)
                            {
                                spriteBatch.Draw(uiTextures[16], new Rectangle(thisObject.AttackBox.X - camera.CameraPosition.X, thisObject.AttackBox.Y - camera.CameraPosition.Y,
                            100, 50), new Rectangle(0, thisObject.AttackAnimationF * 50, 100, 50), Color.White, 0.0f,
                            Vector2.Zero, SpriteEffects.FlipVertically, 0.0f);
                            }
                            if (attackDirection == BearDirection.left)
                            {
                                spriteBatch.Draw(uiTextures[15], new Rectangle(thisObject.AttackBox.X - camera.CameraPosition.X, thisObject.AttackBox.Y - camera.CameraPosition.Y,
                            50, 100), new Rectangle(thisObject.AttackAnimationF * 50, 0, 50, 100), Color.White, 0.0f,
                            Vector2.Zero, SpriteEffects.FlipHorizontally, 0.0f);
                            }
                            if (attackDirection == BearDirection.right)
                            {
                                spriteBatch.Draw(uiTextures[15], new Rectangle(thisObject.AttackBox.X - camera.CameraPosition.X, thisObject.AttackBox.Y - camera.CameraPosition.Y,
                            50, 100), new Rectangle(thisObject.AttackAnimationF * 50, 0, 50, 100), Color.White, 0.0f,
                            Vector2.Zero, SpriteEffects.None, 0.0f);
                            }
                        }
					}
				}
			}
			//draw all pickups on screen
			if (pickUpsCurrent != null)
			{
				foreach (PickUp thisObject in pickUpsCurrent)
				{
					if (thisObject.OnScreen)
					{
						thisObject.Draw(spriteBatch, camera.CameraPosition.Location.ToVector2());
					}
				}
			}

			//draws score
			spriteBatch.Draw(uiTextures[7], new Vector2(680, 18), Color.White);

			//draws highscore
			spriteBatch.DrawString(spriteFont, pCurrent.TotalScore.ToString(), hsCord, Color.White);

            //draws HP
            spriteBatch.Draw(uiTextures[5], new Vector2(45, 18), Color.White);

			//draws log
			spriteBatch.Draw(uiTextures[4], new Vector2(768, 42), Color.White);

			//draws number of tocut left
			spriteBatch.DrawString(spriteFont, totalTreesToCut.ToString(), new Vector2(807, 30), Color.White);

			//draws health
			switch (pCurrent.Health)
			{
				case 5:
					spriteBatch.Draw(uiTextures[1], healthCords[0], Color.White);
					spriteBatch.Draw(uiTextures[1], healthCords[1], Color.White);
					spriteBatch.Draw(uiTextures[1], healthCords[2], Color.White);
					spriteBatch.Draw(uiTextures[1], healthCords[3], Color.White);
					spriteBatch.Draw(uiTextures[1], healthCords[4], Color.White);
					break;

				case 4:
					spriteBatch.Draw(uiTextures[0], healthCords[0], Color.White);
					spriteBatch.Draw(uiTextures[1], healthCords[1], Color.White);
					spriteBatch.Draw(uiTextures[1], healthCords[2], Color.White);
					spriteBatch.Draw(uiTextures[1], healthCords[3], Color.White);
					spriteBatch.Draw(uiTextures[1], healthCords[4], Color.White);
					break;

				case 3:
					spriteBatch.Draw(uiTextures[0], healthCords[0], Color.White);
					spriteBatch.Draw(uiTextures[0], healthCords[1], Color.White);
					spriteBatch.Draw(uiTextures[1], healthCords[2], Color.White);
					spriteBatch.Draw(uiTextures[1], healthCords[3], Color.White);
					spriteBatch.Draw(uiTextures[1], healthCords[4], Color.White);
					break;

				case 2:
					spriteBatch.Draw(uiTextures[0], healthCords[0], Color.White);
					spriteBatch.Draw(uiTextures[0], healthCords[1], Color.White);
					spriteBatch.Draw(uiTextures[0], healthCords[2], Color.White);
					spriteBatch.Draw(uiTextures[1], healthCords[3], Color.White);
					spriteBatch.Draw(uiTextures[1], healthCords[4], Color.White);
					break;

				case 1:
					spriteBatch.Draw(uiTextures[0], healthCords[0], Color.White);
					spriteBatch.Draw(uiTextures[0], healthCords[1], Color.White);
					spriteBatch.Draw(uiTextures[0], healthCords[2], Color.White);
					spriteBatch.Draw(uiTextures[0], healthCords[3], Color.White);
					spriteBatch.Draw(uiTextures[1], healthCords[4], Color.White);
					break;
			}

			//draws weapon text
			spriteBatch.Draw(uiTextures[8], new Vector2(246, 18), Color.White);

			//draws the correct weapon
			if (pCurrent.Attack == AttackVariation.axe)
			{
				spriteBatch.Draw(uiTextures[2], new Vector2(349, 9), Color.White);
			}
			else
			{
				spriteBatch.Draw(uiTextures[3], new Vector2(352, 20), Color.White);
			}

			//draws openSeason
			if (runOpenSeason == true)
			{
				spriteBatch.Draw(uiTextures[6], new Vector2(465, 18), Color.White);
			}

		}

        #endregion uM Universal Stuff
        #region Add & Remove Stuff from UM Lists

        // ----------------------------------------------------------------------- Add / Remove Stuff from Lists ---------------------------------------------------------

        /// <summary>
        /// Set's update Manager up for the next level
        /// </summary>
        /// <param name="p"></param>
        /// <param name="trees"></param>
        /// <param name="bears"></param>
        /// <param name="pickUps"></param>
        public void NextLevel(Player p, List<Tree> trees, List<Bear> bears, List<PickUp> pickUps, int currentLevel, KeyboardState nullState)
        {
            pCurrent = p;
            treesCurrent = trees;
            bearsCurrent = bears;
			pickUpsCurrent = pickUps;
            this.currentLevel = currentLevel;
            totalTreesToCut = (treesCurrent.Count / (gameMaxLevel - (currentLevel)));     // Current Trees in level / (Max level - current Level)
                                                                                          // IE: Current trees = 28, Max Level = 5, current level = 0(+1) [As level 0 is really lvl 1]
                                                                                          // So Level 0 [1] has 7 trees total to cut before passing. 
            // Sets players position to the middle of the level.
            pCurrent.ObjectCollisionBox = new Rectangle((screenWidth)-(pCurrent.Width/2), (screenHeight) - (pCurrent.Height / 2), pCurrent.Width, pCurrent.Height);
            pCurrent.PlayerVision = new Rectangle(pCurrent.ObjectCollisionBox.X - pCurrent.VisionStandard, pCurrent.ObjectCollisionBox.Y - pCurrent.VisionStandard,
            pCurrent.PlayerVision.Width, pCurrent.PlayerVision.Height);

            // Sets kb down to null
            currentKB = nullState;
            previousKB = nullState;
            animate = false;
            p.AttackAnimationF = 0;
            p.IsAttacking = false;

            // Handles if Game was in Open Season when Level Ended

            if (runOpenSeason == true)
            {
                oS.ResetOpenSeasonTimer();
                pCurrent = oS.EndOpenSeasonPlayer(pCurrent);
                oS.BearMax = bearsCurrent[0].MaxSpeed;
                camera = oS.OpenSeasonCamera(camera);
            }

            runOpenSeason = false;

        }
		
        /// <summary>
		/// Sets the neccessary fields for when the game is in Update Game Screen
		/// </summary>
		/// <param name="p"> the player</param>
		/// <param name="bears"> the bears List</param>
		/// <param name="trees"> the trees List</param>
		/// <param name="pickUps"> the pickUps List</param>
		public void UpdateGameScreenFields(KeyboardState currentKB, KeyboardState previousKB, GameTime gameTime)
		{
            this.currentKB = currentKB;
            this.previousKB = previousKB;
            this.gameTime = gameTime;
		}

        public int ReturnScore() //method that returns player score
        {
            return pCurrent.TotalScore;
        }


        /// <summary>
        /// Determines stuff to remove from Lists (Tree, Bear, PickUp)
        /// </summary>
        private void RemoveStuffFromStoredLists()
		{
			//Removes all trees from the list that have less than or equal to 0 health
			for (int i = 0; i < treesCurrent.Count; i++)
			{
				if (treesCurrent[i].Health < 0 || treesCurrent[i].Health == 0)
				{
					treesCurrent.RemoveAt(i);
                    totalTreesToCut--; // Decreases total Trees to cut since the tree is now dead
                    pCurrent.TotalScore += 3;
                }
			}

			//Removes all bears from the list that have less than or equal to 0 health
			for (int i = 0; i < bearsCurrent.Count; i++)
			{
				if (bearsCurrent[i].Health < 0 || bearsCurrent[i].Health == 0)
				{
                    pCurrent.TotalScore += bearsCurrent[i].ScoreValue;
					bearsCurrent.RemoveAt(i);
				}
			}


			//Removes all the pickups from the list when item is retrieved
			for (int i = 0; i < pickUpsCurrent.Count; i++)
			{
				if (pickUpsCurrent[i].ItemState == ItemState.Retrieved)
				{
					pickUpsCurrent.RemoveAt(i);
				}
			}
		}

        #endregion Add & Remove Stuff from UM Lists
        #region Updating Player
        // ------------------------------------------------------------------------ Player Specific Methods ---------------------------------------------------------------


        /// <summary>
        /// Responsible for Updating all neccessary variables of the player
        /// </summary>
        private void UpdatePlayer()
		{

			// Creates a Player Velocity Manager
			velocityManager = new VelocityManager(pCurrent.MaxSpeed)
			{
				// Sets the current X and Y speed that should be in pl
				VelocityX = pCurrent.SpeedX,
				VelocityY = pCurrent.SpeedY
			};


			// 0 - Saves player's UnAdjusted State to be compared to through-out Update Player

			Player oldPos = new Player(pCurrent);

			// 1 - Finds Players "Un-Collided" Position with new Speed 

			UpdatePlayerMovement();

			// 2 - Check for Collisions with Trees in Game. Check for collision with side of screen. Adjust Speed and Pos Accordingly if needed.

			UpdatePlayerPosition(oldPos);

            // 3 - Determines if the Player should now Attack

            UpdatePlayerAttack(oldPos);

			// 4 - Updates the Animations of the player

			UpdatePlayerMovementAnimation(oldPos);

            // 5 - Update PickUps

            UpdatePickUps();           

            // temp code to check if player has pressing keys to move. runs after updating all the movement.
            // Do not delete this. may use in future.  FAIL-SAFE TO DEACCELERATION STUFF
            /*
            var keys = currentKB.GetPressedKeys();

          
            if (keys.Length == 0)
            {
                pCurrent.SpeedX = 0;
                pCurrent.SpeedY = 0;
            }
            */
            
            




        }

		/// <summary>
		/// Calculates the new position of the Player & Field of Vision, and updates the Players current Speed in X and Y Directions
		/// </summary>
		private void UpdatePlayerMovement()
		{
			// Adds Speed to the Velocity Manager Based on the Current Keys Pressed
			if (currentKB.IsKeyDown(Keys.W) == true)
			{
                if (velocityManager.VelocityY > 0)
                {
                    velocityManager.Decelerate(1);
                }
				velocityManager.AddVelocity(0, -1 * (pCurrent.MaxSpeed / 16));
			}
			if (currentKB.IsKeyDown(Keys.S) == true)
			{
                if (velocityManager.VelocityY < 0)
                {
                    velocityManager.Decelerate(1);
                }
                velocityManager.AddVelocity(0, (pCurrent.MaxSpeed / 16));
			}
            if (currentKB.IsKeyDown(Keys.S) == true && currentKB.IsKeyDown(Keys.W) == true)
            {
                velocityManager.VelocityY = 0;
            }
            if (currentKB.IsKeyDown(Keys.W) != true && currentKB.IsKeyDown(Keys.S) != true)
            {
                velocityManager.Decelerate(1);
            }
			if (currentKB.IsKeyDown(Keys.A) == true)
			{
                if (velocityManager.VelocityX > 0)
                {
                    velocityManager.Decelerate(0);
                }
                velocityManager.AddVelocity(-1 * (pCurrent.MaxSpeed / 16), 0);
			}
			if (currentKB.IsKeyDown(Keys.D) == true)
			{
				velocityManager.AddVelocity((pCurrent.MaxSpeed / 16), 0);
                if (velocityManager.VelocityX < 0)
                {
                    velocityManager.Decelerate(0);
                }
            }
            if (currentKB.IsKeyDown(Keys.A) == true && currentKB.IsKeyDown(Keys.D) == true)
            {
                velocityManager.VelocityY = 0;
            }
            if (currentKB.IsKeyDown(Keys.A) != true && currentKB.IsKeyDown(Keys.D) != true)
            {
                velocityManager.Decelerate(0);
            }

            // Sets the new Sprite Location & Player Field of Vision
            pCurrent.ObjectCollisionBox = velocityManager.UpdatePosition(pCurrent.ObjectCollisionBox);
			pCurrent.PlayerVision = velocityManager.UpdatePosition(pCurrent.PlayerVision);

			// Updates the Current Speed of the Player within the Player from the Calculated speed in Players VM
			pCurrent.SpeedX = velocityManager.VelocityX;
			pCurrent.SpeedY = velocityManager.VelocityY;
		}

        private void UpdatePlayerAttack(Player oldPos)
        {
            // If the player was not attacking previously or was attacking but hit the 4th frame
            if ((oldPos.IsAttacking == true && oldPos.AttackAnimationF >= 7) || (oldPos.IsAttacking == false))
            {
                // Will Attack Up
                if (currentKB.IsKeyDown(Keys.Up) == true)
                {
                    pCurrent.IsAttacking = true; // Player is now attacking!
                    pCurrent.AttackAnimationF = 0; // Player must draw the attacking first frame
                    pCurrent.AttackDirection = PlayerAttackDirection.up; // Gives a direction of the hitbox and animation
                    ResetBearsAttackBool();
                    ResetTreesAttackBool();
                }
                // Will Attack Up
                else if (currentKB.IsKeyDown(Keys.Left) == true)
                {
                    pCurrent.IsAttacking = true; // Player is now attacking!
                    pCurrent.AttackAnimationF = 0; // Player must draw the attacking first frame
                    pCurrent.AttackDirection = PlayerAttackDirection.left; // Gives a direction of the hit box and animation
                    ResetBearsAttackBool();
                    ResetTreesAttackBool();
                }
                // Will Attack Down
                else if (currentKB.IsKeyDown(Keys.Down) == true)
                {
                    pCurrent.IsAttacking = true; // Player is now attacking!
                    pCurrent.AttackAnimationF = 0; // Player must draw the attacking first frame 
                    pCurrent.AttackDirection = PlayerAttackDirection.down; // Gives a direction of the hit box and animation
                    ResetBearsAttackBool();
                    ResetTreesAttackBool();
                }
                // Will AttackRight
                else if (currentKB.IsKeyDown(Keys.Right) == true)
                {
                    pCurrent.IsAttacking = true; // Player is now attacking!
                    pCurrent.AttackAnimationF = 0; // Player must draw the attacking first frame 
                    pCurrent.AttackDirection = PlayerAttackDirection.right; // Gives a direction of the hit box and animation
                    ResetBearsAttackBool();
                    ResetTreesAttackBool();
                }
                // If player was attacking and is now [what would be] a frame over his animation for attacking,
                // make him no longer attacking and he is now back to frame 1
                else if (oldPos.AttackAnimationF > 7 && oldPos.IsAttacking == true)
                {
                    pCurrent.IsAttacking = false; // Player is no longer Attacking!!!!
                    ResetBearsAttackBool();
                    ResetTreesAttackBool();
                }
            }
        }

		/// <summary>
		/// Updates the Player's direction enum to be properly set. Used to determine what animation of Player should be drawn
		/// </summary>
		/// <param name="oldPos">The old player</param>
		private void UpdatePlayerMovementAnimation(Player oldPos)
		{

            // Updates Direction Animation 
            if (animate == true)
            {
                if (oldPos.PosX > pCurrent.PosX && oldPos.PosY == pCurrent.PosY)         // Player walking in Left Direction
                {
                    pCurrent.PlayerDirection = PlayerDirection.left;
                    if (oldPos.PlayerDirection == pCurrent.PlayerDirection)
                    {
                        pCurrent.AnimateMoveFrame++;
                        if (pCurrent.AnimateMoveFrame > 7)
                        {
                            pCurrent.AnimateMoveFrame = 0;
                        }
                    }
                    else
                    {
                        pCurrent.AnimateMoveFrame = 0;
                    }
                }
                else if (oldPos.PosX < pCurrent.PosX && oldPos.PosY == pCurrent.PosY)   // Player walking in the Right Direction
                {
                    pCurrent.PlayerDirection = PlayerDirection.right;
                    if (oldPos.PlayerDirection == pCurrent.PlayerDirection)
                    {
                        pCurrent.AnimateMoveFrame++;
                        if (pCurrent.AnimateMoveFrame > 7)
                        {
                            pCurrent.AnimateMoveFrame = 0;
                        }
                    }
                    else
                    {
                        pCurrent.AnimateMoveFrame = 0;
                    }
                }
                else if (oldPos.PosX == pCurrent.PosX && oldPos.PosY > pCurrent.PosY)   // Player walking Up Direction
                {
                    pCurrent.PlayerDirection = PlayerDirection.up;
                    if (oldPos.PlayerDirection == pCurrent.PlayerDirection)
                    {
                        pCurrent.AnimateMoveFrame++;
                        if (pCurrent.AnimateMoveFrame > 7)
                        {
                            pCurrent.AnimateMoveFrame = 0;
                        }
                    }
                    else
                    {
                        pCurrent.AnimateMoveFrame = 0;
                    }
                }
                else if (oldPos.PosX == pCurrent.PosX && oldPos.PosY < pCurrent.PosY)   // Player walking Down Direction
                {
                    pCurrent.PlayerDirection = PlayerDirection.down;
                    if (oldPos.PlayerDirection == pCurrent.PlayerDirection)
                    {
                        pCurrent.AnimateMoveFrame++;
                        if (pCurrent.AnimateMoveFrame > 7)
                        {
                            pCurrent.AnimateMoveFrame = 0;
                        }
                    }
                    else
                    {
                        pCurrent.AnimateMoveFrame = 0;
                    }
                }
                else if (oldPos.PosX > pCurrent.PosX && oldPos.PosY > pCurrent.PosY)   // Player is walking Up-Left
                {
                    pCurrent.PlayerDirection = PlayerDirection.upleft;
                    if (oldPos.PlayerDirection == pCurrent.PlayerDirection)
                    {
                        pCurrent.AnimateMoveFrame++;
                        if (pCurrent.AnimateMoveFrame > 7)
                        {
                            pCurrent.AnimateMoveFrame = 0;
                        }
                    }
                    else
                    {
                        pCurrent.AnimateMoveFrame = 0;
                    }
                }
                else if (oldPos.PosX < pCurrent.PosX && oldPos.PosY > pCurrent.PosY)   // Player is walking Up-Right
                {
                    pCurrent.PlayerDirection = PlayerDirection.upright;
                    if (oldPos.PlayerDirection == pCurrent.PlayerDirection)
                    {
                        pCurrent.AnimateMoveFrame++;
                        if (pCurrent.AnimateMoveFrame > 7)
                        {
                            pCurrent.AnimateMoveFrame = 0;
                        }
                    }
                    else
                    {
                        pCurrent.AnimateMoveFrame = 0;
                    }
                }
                else if (oldPos.PosX > pCurrent.PosX && oldPos.PosY < pCurrent.PosY)   // Player is walking Down-Left
                {
                    pCurrent.PlayerDirection = PlayerDirection.downleft;
                    if (oldPos.PlayerDirection == pCurrent.PlayerDirection)
                    {
                        pCurrent.AnimateMoveFrame++;
                        if (pCurrent.AnimateMoveFrame > 7)
                        {
                            pCurrent.AnimateMoveFrame = 0;
                        }
                    }
                    else
                    {
                        pCurrent.AnimateMoveFrame = 0;
                    }
                }
                else if (oldPos.PosX < pCurrent.PosX && oldPos.PosY < pCurrent.PosY)   // Player is walking Down-Right
                {
                    pCurrent.PlayerDirection = PlayerDirection.downright;
                    if (oldPos.PlayerDirection == pCurrent.PlayerDirection)
                    {
                        pCurrent.AnimateMoveFrame++;
                        if (pCurrent.AnimateMoveFrame > 7)
                        {
                            pCurrent.AnimateMoveFrame = 0;
                        }
                    }
                    else
                    {
                        pCurrent.AnimateMoveFrame = 0;
                    }
                }
                else                                                                    // Player Stood Still
                {
                    pCurrent.PlayerDirection = oldPos.PlayerDirection;
                }
            }
		}

		/// <summary>
		/// Adjusts Players Rectangle Fields and Pos if a collision with a tree has occured.
		/// </summary>
		/// <param name="oldPos"></param>
		private void UpdatePlayerPosition(Player oldPos)
		{

            // -- Collision With Trees --

			// @ pos 0 = X Value Adjust, @ pos 1 = Y Value Adjust

			int[] adjustPosValues = collisionManager.PosAdjust(pCurrent, oldPos, treesCurrent);

			// If any sort of adjustment value was found, then the object collided with something.
			// If no adjustment was found, then nothing needs to be offset and the speed doesnt need to be adjusted

			if (adjustPosValues[0] != 0 || adjustPosValues[1] != 0)
			{
                // Makes the player and trees collision no longer occur and sets all player rectangles equally offset
                pCurrent.ObjectCollisionBox = new Rectangle(pCurrent.PosX + adjustPosValues[0], pCurrent.PosY + adjustPosValues[1], pCurrent.Width, pCurrent.Height);
                pCurrent.PlayerVision = new Rectangle(pCurrent.PlayerVision.X + adjustPosValues[0], pCurrent.PlayerVision.Y + adjustPosValues[1], 
                    pCurrent.PlayerVision.Width, pCurrent.PlayerVision.Height);

                // Changes the speed to 0 in the direction of which a potential collision has now occured. 
                if (adjustPosValues[0] != 0)
				{
					pCurrent.SpeedX = 0;
				}
				if (adjustPosValues[1] != 0)
				{
					pCurrent.SpeedY = 0;
				}
			}

            // -- MAKE IT STAY ON SCREEN -- 

            // Changes adjustPosValues to fixed values of placement if tries to go off screen;
            adjustPosValues = collisionManager.StayOnScreen(pCurrent);

            // gets new Absolute X and Y cords 

            // Only Adjusts Cords if the object has touched the side of the screen

            // Four Cases: A ) X & Y == 0 (nothing changes, nothing needs to run)
            // B) X Changes, Y doesnt == X speed needs set to 0 and all rectangles need changed
            // C) Y changes, X doesnt == Y speed needs set to 0 and all rectangles need changed
            // D) IT ALL DAMN CHANGES (x && y both change, both speeds set to 0, both x and y cords of all rectangles need changed)

            if (adjustPosValues[0] != 0 || adjustPosValues[1] != 0)
            {
                
                // X value is changed and Y is not
                if (adjustPosValues[0] != 0 && adjustPosValues[1] == 0)
                {
                    // Position
                    pCurrent.ObjectCollisionBox = new Rectangle(adjustPosValues[0], pCurrent.PosY, pCurrent.Width, pCurrent.Height);
                }
                // Y value is changed and X is not
                else if (adjustPosValues[0] == 0 && adjustPosValues[1] != 0)
                {
                    // Position
                    pCurrent.ObjectCollisionBox = new Rectangle(pCurrent.PosX, adjustPosValues[1], pCurrent.Width, pCurrent.Height);
                }
                // Both X value and Y value have changed
                else if((adjustPosValues[0] != 0 && adjustPosValues[1] != 0))
                {
                    // Position
                    pCurrent.ObjectCollisionBox = new Rectangle(adjustPosValues[0], adjustPosValues[1], pCurrent.Width, pCurrent.Height);
                }

                // Player vision based on new Position
                pCurrent.PlayerVision = new Rectangle(pCurrent.ObjectCollisionBox.X - pCurrent.VisionStandard, pCurrent.ObjectCollisionBox.Y - pCurrent.VisionStandard,
                    pCurrent.PlayerVision.Width, pCurrent.PlayerVision.Height);

                // Changes X speed if it had to be modified along the X axis
                // Changes Y speed if it had to be modified along the Y axis.

                if (adjustPosValues[0] != 0)
                {
                    pCurrent.SpeedX = 0;
                }
                if (adjustPosValues[1] != 0)
                {
                    pCurrent.SpeedY = 0;
                }
            }

            
        }


        #endregion Updating Player
        #region Updating Bear
        // -------------------------------------------------------------------------- Bear Specific Methods ------------------------------------------------------------


        /// <summary>
        /// Update Responsible for updating all parts of every bear for each bear.
        /// </summary>
        private void UpdateAllBears()
		{

			for (int i = 0; i < bearsCurrent.Count; i++)
			{
                // ONLY UPDATES BEARS THAT ARE NOW INTERSECTING WITH THE UPSREEN. THIS CUTS DOWN THE TOTAL AMOUNT OF UPDATING THAT NEEDS TO OCCUR
                // THIS ALSO MAKES IT SO WE ARNT CHECKING COLLISIONS FOR EVERY DAMN BEAR IN THE GAME. AMEN!
                if (bearsCurrent[i].UPScreen == true)
                {
                    // Creates a Bear Velocity Manager
                    velocityManager = new VelocityManager(bearsCurrent[i].MaxSpeed)
                    {
                        // Sets the current X and Y speed
                        VelocityX = bearsCurrent[i].SpeedX,
                        VelocityY = bearsCurrent[i].SpeedY
                    };


                    // 0 - Saves bears UnAdjusted State to be compared to through-out UpdateAllBears

                    Bear oldPos = new Bear(bearsCurrent[i]);

                    // 1 - Compares Old Bear to New Bear and Decideds New Bear's Type of Movement. - Increments Timers for Statationary / Looking Accordingly.

                    // FIRST - DECIDES TO INCREMENT TIMER IF UPDATED PLAYERVISION BOX DOESN'T INTERACT WITH BEAR FIELD OF VISION
                    // SECOND - DETERMINES THE NEW STATE OF THE BEAR
                    // THIRD - DECIDES ON THE MOVEMENT OF THE NEW BEAR AND TO RESET TIMERS BASED ON OLD AND NEW STATE
                    // TYPES OF MOVEMENT: FOLLOW()  - Determines the path from the bear to the player in the quickest way
                    //                    LOOKING() - Determines the random path which the bear will walk for the bear's timer
                    // OTHER FUNCTIONS:   Bear.ResetTimer() - Will reset all the timers within the bear. (Timer for how long to stay still, and how long it should walk)

                    BearMovement(oldPos, i);

                    // 2 - Check for Collisions with Trees in Game. Adjust Speed and Pos Accordingly if needed.

                    UpdateBearsPosition(oldPos, i);

                    // 3 - Updates the Animations of the bear

                    UpdateBearAnimation(oldPos, i);


                }
            }

		}

		/// <summary>
		/// Adjusts Bear's Rectangle Fields and Pos if a collision with a tree has occured.
		/// </summary>
		/// <param name="oldPos">The old bear</param>
		/// <param name="i">The # of bear from it's list</param>
		private void UpdateBearsPosition(Bear oldPos, int i)
		{

            // -- Collision With Trees --

            // @ pos 0 = X Value Adjust, @ pos 1 = Y Value Adjust

            int[] adjustPosValues = collisionManager.PosAdjust(bearsCurrent[i], oldPos, treesCurrent);

			// If any sort of adjustment value was found, then the object collided with something.
			// If no adjustment was found, then nothing needs to be offset and the speed doesnt need to be adjusted

			if (adjustPosValues[0] != 0 || adjustPosValues[1] != 0)
			{
                // Makes the bear and trees collision no longer occur and sets all bear rectangles equally offset
                bearsCurrent[i].ObjectCollisionBox = new Rectangle(bearsCurrent[i].PosX + adjustPosValues[0], bearsCurrent[i].PosY + adjustPosValues[1], bearsCurrent[i].Width, bearsCurrent[i].Height);

                bearsCurrent[i].BearVision = new Rectangle(bearsCurrent[i].BearVision.X + adjustPosValues[0], bearsCurrent[i].BearVision.Y + adjustPosValues[1], 
                    bearsCurrent[i].BearVision.Width, bearsCurrent[i].BearVision.Height);

                bearsCurrent[i].FieldOfAttack = new Rectangle(bearsCurrent[i].FieldOfAttack.X + adjustPosValues[0], bearsCurrent[i].FieldOfAttack.Y + adjustPosValues[1],
                    bearsCurrent[i].FieldOfAttack.Width, bearsCurrent[i].FieldOfAttack.Height);

                // Changes the speed to 0 in the X and/or Y direction of which a potential collision has now occured. 
                if (adjustPosValues[0] != 0)
				{
					bearsCurrent[i].SpeedX = 0;
				}
				if (adjustPosValues[1] != 0)
				{
					bearsCurrent[i].SpeedY = 0;
				}
			}

            // -- MAKE IT STAY ON SCREEN --

            // Changes adjustPosValues to fixed values of placement if tries to go off screen;
            adjustPosValues = collisionManager.StayOnScreen(bearsCurrent[i]);

            // gets new Absolute X and Y cords 

            // Only Adjusts Cords if the object has touched the side of the screen

            // Four Cases: A ) X & Y == 0 (nothing changes, nothing needs to run)
            // B) X Changes, Y doesnt == X speed needs set to 0 and all rectangles need changed
            // C) Y changes, X doesnt == Y speed needs set to 0 and all rectangles need changed
            // D) IT ALL DAMN CHANGES (x && y both change, both speeds set to 0, both x and y cords of all rectangles need changed)

            if (adjustPosValues[0] != 0 || adjustPosValues[1] != 0)
            {

                // X value is changed and Y is not
                if (adjustPosValues[0] != 0 && adjustPosValues[1] == 0)
                {

                    // Position
                    bearsCurrent[i].ObjectCollisionBox = new Rectangle(adjustPosValues[0], bearsCurrent[i].PosY, 
                        bearsCurrent[i].Width, bearsCurrent[i].Height);
                }
                // Y value is changed and X is not
                else if (adjustPosValues[0] == 0 && adjustPosValues[1] != 0)
                {
                    // Position
                    bearsCurrent[i].ObjectCollisionBox = new Rectangle(bearsCurrent[i].PosX, adjustPosValues[1],
                        bearsCurrent[i].Width, bearsCurrent[i].Height);
                }
                // Both X value and Y value have changed
                else if ((adjustPosValues[0] != 0 && adjustPosValues[1] != 0))
                {
                    // Position
                    bearsCurrent[i].ObjectCollisionBox = new Rectangle(adjustPosValues[0], adjustPosValues[1],
                        bearsCurrent[i].Width, bearsCurrent[i].Height);
                }

                // Vision Rectangle based on new Position
                bearsCurrent[i].BearVision = new Rectangle(bearsCurrent[i].ObjectCollisionBox.X - bearsCurrent[i].VisionStandard,
                    bearsCurrent[i].ObjectCollisionBox.Y - bearsCurrent[i].VisionStandard,
                    bearsCurrent[i].BearVision.Width, bearsCurrent[i].BearVision.Height);

                // Field of Attack based on new Position
                bearsCurrent[i].FieldOfAttack = new Rectangle(bearsCurrent[i].ObjectCollisionBox.X - bearsCurrent[i].FoAStandard,
                    bearsCurrent[i].ObjectCollisionBox.Y - bearsCurrent[i].FoAStandard,
                    bearsCurrent[i].FieldOfAttack.Width, bearsCurrent[i].FieldOfAttack.Height);

                // Changes X speed if it had to be modified along the X axis
                // Changes Y speed if it had to be modified along the Y axis.

                if (adjustPosValues[0] != 0)
                {
                    bearsCurrent[i].SpeedX = 0;
                }
                if (adjustPosValues[1] != 0)
                {
                    bearsCurrent[i].SpeedY = 0;
                }
            }


        }

		/// <summary>
		/// Updates the Bear's direction enum to be properly set. Used to determine what animation of Bear should be drawn
		/// </summary>
		/// <param name="oldPos">The old bear</param>
		/// <param name="i">The # of bear from it's list</param>
		private void UpdateBearAnimation(Bear oldPos, int i)
		{
			if (oldPos.PosX > bearsCurrent[i].PosX && oldPos.PosY == bearsCurrent[i].PosY)         // Bear walking in Left Direction
			{
				bearsCurrent[i].BearDirection = BearDirection.left;               
            }
			else if (oldPos.PosX < bearsCurrent[i].PosX && oldPos.PosY == bearsCurrent[i].PosY)   // Bear walking in the Right Direction
			{
				bearsCurrent[i].BearDirection = BearDirection.right;
			}
			else if (oldPos.PosX == bearsCurrent[i].PosX && oldPos.PosY > bearsCurrent[i].PosY)   // Bear walking Up Direction
			{
				bearsCurrent[i].BearDirection = BearDirection.up;
			}
			else if (oldPos.PosX == bearsCurrent[i].PosX && oldPos.PosY < bearsCurrent[i].PosY)   // Bear walking Down Direction
			{
				bearsCurrent[i].BearDirection = BearDirection.down;
			}
			else if (oldPos.PosX > bearsCurrent[i].PosX && oldPos.PosY > bearsCurrent[i].PosY)   // Bear is walking Up-Left
			{
				bearsCurrent[i].BearDirection = BearDirection.upleft;
			}
			else if (oldPos.PosX < bearsCurrent[i].PosX && oldPos.PosY > bearsCurrent[i].PosY)   // Bear is walking Up-Right
			{
				bearsCurrent[i].BearDirection = BearDirection.upright;
			}
			else if (oldPos.PosX > bearsCurrent[i].PosX && oldPos.PosY < bearsCurrent[i].PosY)   // Bear is walking Down-Left
			{
				bearsCurrent[i].BearDirection = BearDirection.downleft;
			}
			else if (oldPos.PosX < bearsCurrent[i].PosX && oldPos.PosY < bearsCurrent[i].PosY)   // Bear is walking Down-Right
			{
				bearsCurrent[i].BearDirection = BearDirection.downright;
			}
			else                                                                    // Bear Stood Still
			{
                bearsCurrent[i].BearDirection = oldPos.BearDirection;
			}

            if (animate == true)
            {
                if (oldPos.BearDirection == bearsCurrent[i].BearDirection)
                {
                    if (oldPos.PosX != bearsCurrent[i].PosX && oldPos.PosY != bearsCurrent[i].PosY)
                    {
                        bearsCurrent[i].AnimateMoveFrame++;
                        if (bearsCurrent[i].AnimateMoveFrame > 7)
                        {
                            bearsCurrent[i].AnimateMoveFrame = 0;
                        }
                    }
                    else
                    {
                        bearsCurrent[i].AnimateMoveFrame = 0;
                    }
                }
            }       
		}

		/// <summary>
		/// Determines how the bear is currently looking (wandering) around in a random way
		/// </summary>
		private void Looking(int i)
		{
			// Formula: Adds speed to the velocity manager thats is BETWEEN! -1/2 of it's potential max speed and 1/2 is potential max speed
			// in both the x and y direction of the bear, randomly creating movement for the bear that could be super fast, or super slow.
			// Tip: Changing the (/2) is the easiest way to adjust this value.
			velocityManager.AddVelocity(rng.Next(-1 * bearsCurrent[i].MaxSpeed, bearsCurrent[i].MaxSpeed) / 6, rng.Next(-1 * bearsCurrent[i].MaxSpeed, bearsCurrent[i].MaxSpeed) / 6);

			// Sets the new Sprite Location & Player Field of Vision
			bearsCurrent[i].ObjectCollisionBox = velocityManager.UpdatePosition(bearsCurrent[i].ObjectCollisionBox);
			bearsCurrent[i].BearVision = velocityManager.UpdatePosition(bearsCurrent[i].BearVision);
			bearsCurrent[i].FieldOfAttack = velocityManager.UpdatePosition(bearsCurrent[i].FieldOfAttack);

			// Updates the Current Speed of the Player within the Player from the Calculated speed in Players VM
			bearsCurrent[i].SpeedX = velocityManager.VelocityX;
			bearsCurrent[i].SpeedY = velocityManager.VelocityY;
		}
		/// <summary>
		/// Used for the bear to follow the player
		/// </summary>
		private void FollowPlayer(int i)
		{

            switch (bearsCurrent[i].BearDirection)
            {
                case BearDirection.down:
                    velocityManager.AddVelocity(0, bearsCurrent[i].MaxSpeed / 10);
                    break;
                case BearDirection.up:
                    velocityManager.AddVelocity(0, -1 * bearsCurrent[i].MaxSpeed / 10);
                    break;
                case BearDirection.left:
                    velocityManager.AddVelocity(-1 * bearsCurrent[i].MaxSpeed / 10, 0);
                    break;
                case BearDirection.right:
                    velocityManager.AddVelocity(bearsCurrent[i].MaxSpeed / 10, 0);
                    break;
                case BearDirection.upleft:
                    velocityManager.AddVelocity(-1 * bearsCurrent[i].MaxSpeed / 10, -1 * bearsCurrent[i].MaxSpeed / 10);
                    break;
                case BearDirection.upright:
                    velocityManager.AddVelocity(bearsCurrent[i].MaxSpeed / 10, -1 * bearsCurrent[i].MaxSpeed / 10);
                    break;
                case BearDirection.downleft:
                    velocityManager.AddVelocity(-1 * bearsCurrent[i].MaxSpeed / 10, bearsCurrent[i].MaxSpeed / 10);
                    break;
                case BearDirection.downright:
                    velocityManager.AddVelocity(bearsCurrent[i].MaxSpeed / 10, bearsCurrent[i].MaxSpeed / 10);
                    break;
            }

            bearsCurrent[i].ObjectCollisionBox = velocityManager.UpdatePosition(bearsCurrent[i].ObjectCollisionBox);
            bearsCurrent[i].BearVision = velocityManager.UpdatePosition(bearsCurrent[i].BearVision);
            bearsCurrent[i].FieldOfAttack = velocityManager.UpdatePosition(bearsCurrent[i].FieldOfAttack);

            // Updates the Current Speed of the Player within the Player from the Calculated speed in Players VM
            bearsCurrent[i].SpeedX = velocityManager.VelocityX;
            bearsCurrent[i].SpeedY = velocityManager.VelocityY;
        }

        /// <summary>
        /// Determines the new State of Movement and how the bear should Move.
        /// </summary>
        /// <param name="oldBear"></param>
        /// <param name="i"></param>
		private void BearMovement(Bear oldBear, int i)
		{

			//Determines the bear's state
			if (pCurrent.PlayerVision.Intersects(bearsCurrent[i].BearVision) == true)
			{
				bearsCurrent[i].BearState = BearState.following;
			}
			else
			{
				if (oldBear.BearState == BearState.stationary)
				{
					bearsCurrent[i].WhenToMoveCounter += gameTime.ElapsedGameTime.TotalSeconds;
				}
				else if (oldBear.BearState == BearState.looking)
				{
					bearsCurrent[i].TimeOfMovementCounter += gameTime.ElapsedGameTime.TotalSeconds;
				}


				if (oldBear.BearState == BearState.stationary && bearsCurrent[i].WhenToMoveCounter >= bearsCurrent[i].WhenToMoveLimiter)
				{
					bearsCurrent[i].BearState = BearState.looking;
				}
				else if (oldBear.BearState == BearState.stationary && bearsCurrent[i].WhenToMoveCounter < bearsCurrent[i].WhenToMoveLimiter)
				{
					bearsCurrent[i].BearState = BearState.stationary;
				}
				else if (oldBear.BearState == BearState.looking && bearsCurrent[i].TimeOfMovementCounter >= bearsCurrent[i].TimeOfMovementLimiter)
				{
					bearsCurrent[i].BearState = BearState.stationary;
				}
				else if (oldBear.BearState == BearState.looking && bearsCurrent[i].TimeOfMovementCounter < bearsCurrent[i].TimeOfMovementLimiter)
				{
					bearsCurrent[i].BearState = BearState.looking;
				}
				else if (oldBear.BearState == BearState.following)
				{
					bearsCurrent[i].BearState = BearState.stationary;
				}



			}


			if (oldBear.BearState == BearState.stationary && bearsCurrent[i].BearState == BearState.looking)
			{
				Looking(i);
			}
			else if (oldBear.BearState == BearState.looking && bearsCurrent[i].BearState == BearState.looking)
			{
				Looking(i);
			}
			else if (oldBear.BearState == BearState.looking && bearsCurrent[i].BearState == BearState.stationary)
			{
				// Sets speed of bear to 0 since movement has now reset. 
				bearsCurrent[i].SpeedX = 0;
				bearsCurrent[i].SpeedY = 0;

				bearsCurrent[i].ResetBearTimers(rng);
			}
			else if (oldBear.BearState == BearState.following && bearsCurrent[i].BearState == BearState.following)
			{
			}
			else if (oldBear.BearState == BearState.stationary && bearsCurrent[i].BearState == BearState.following)
			{
				// Sets speed of velocity manager back to 
				bearsCurrent[i].ResetBearTimers(rng);
			}
			else if (oldBear.BearState == BearState.looking && bearsCurrent[i].BearState == BearState.following)
			{
				bearsCurrent[i].ResetBearTimers(rng);
			}
			if (bearsCurrent[i].BearState == BearState.following)
			{
				if (bearsCurrent[i].PosX == pCurrent.PosX && bearsCurrent[i].PosY < pCurrent.PosY) //down
				{
					bearsCurrent[i].BearDirection = BearDirection.down;
				}
				else if (bearsCurrent[i].PosX == pCurrent.PosX && bearsCurrent[i].PosY > pCurrent.PosY) //u[
				{
					bearsCurrent[i].BearDirection = BearDirection.up;
				}
				else if (bearsCurrent[i].PosX > pCurrent.PosX && bearsCurrent[i].PosY == pCurrent.PosY) //left
				{
					bearsCurrent[i].BearDirection = BearDirection.left;
				}
				else if (bearsCurrent[i].PosX < pCurrent.PosX && bearsCurrent[i].PosY == pCurrent.PosY) //right
				{
					bearsCurrent[i].BearDirection = BearDirection.right;
				}
				else if (bearsCurrent[i].PosX > pCurrent.PosX && bearsCurrent[i].PosY > pCurrent.PosY) //upleft
				{
					bearsCurrent[i].BearDirection = BearDirection.upleft;
				}
				else if (bearsCurrent[i].PosX < pCurrent.PosX && bearsCurrent[i].PosY > pCurrent.PosY) //upright
				{
					bearsCurrent[i].BearDirection = BearDirection.upright;
				}
				else if (bearsCurrent[i].PosX > pCurrent.PosX && bearsCurrent[i].PosY < pCurrent.PosY) //downleft
				{
					bearsCurrent[i].BearDirection = BearDirection.downleft;
				}
				else if (bearsCurrent[i].PosX < pCurrent.PosX && bearsCurrent[i].PosY < pCurrent.PosY) //downright
				{
					bearsCurrent[i].BearDirection = BearDirection.downright;
				}
				FollowPlayer(i);
			}




		}

        //Makes Bears allowed to be attacked again
        private void ResetBearsAttackBool()
        {
            if(bearsCurrent!= null)
            {
                foreach(Bear b in bearsCurrent)
                {
                    b.WasAttacked = false;
                }
            }
        }

        #endregion Updating Bear
        #region Updating Player & Bear Interactions

        // ----------------------------------------------------------------- PLAYER / BEAR INTERACTION --------------------------------------------------------------------

        /// <summary>
        /// Player runs into bear & attacking
        /// </summary>
        private void UpdatePlayerBearInteraction()
        {
            WalkIntoBear();
            
            if(pCurrent.IsAttacking == true)
            {
                UpdatePlayerAttacks();
            }
            if (bearsCurrent != null)
            {
                foreach (Bear b in bearsCurrent)
                {
                    UpdateBearsAttack();
                }
            }
            Invinicibility();

        }

        /// <summary>
        /// Determines if Player should lose health if running into a bear or if player is invincible. 
        /// </summary>
        private void WalkIntoBear()
        {
            if (bearsCurrent != null && pCurrent.Invincible == false)
            {
                foreach (Bear b in bearsCurrent)
                {
                    if(b.UPScreen == true)
                    {
                        if (b.ObjectCollisionBox.Intersects(pCurrent.ObjectCollisionBox))
                        {
                            pCurrent.ResetTimer();
                            pCurrent.Invincible = true;
                            pCurrent.Health--;
                            break;                        }
                    }
                }
            }
        }

        // ---------------------------------------------------------------------------- Attack Stuff ----------------------------------------------------------------------

        /// Pass in 1 or 0 to determine bear or player
        /// If bear, pass a 0 for weapon type
        /// If plr, pass 0 for axe and 1 for shotgun
        /// </summary>
        /// <param name=""></param>
        public void UpdatePlayerAttacks()
        {
            
            Rectangle attackArea;

            if (pCurrent.Attack == AttackVariation.axe)
            {
                attackArea = new Rectangle(pCurrent.Location.X, pCurrent.Location.Y, 100, 50);
                // changes the direction of the box based on player orientation
                /*
				 *    < ^ ^
				 *    < * >
				 *    \/\/>
				 * (diagram of how the layout is setup)
				 */
                switch (pCurrent.AttackDirection)
                {
					case PlayerAttackDirection.up:
						attackArea.Y -= (attackArea.Height);
						
						break;
					case PlayerAttackDirection.down:
						attackArea.Y += pCurrent.Height;
						break;
					case PlayerAttackDirection.left:
						attackArea.Width = 50;
						attackArea.Height = 100;
						attackArea.X -= (attackArea.Width);
						break;
					case PlayerAttackDirection.right:
						attackArea.Width = 50;
						attackArea.Height = 100;
						attackArea.X += pCurrent.Width;
						break;
				}

				pCurrent.PlayerAttackBox = attackArea;
                //calls collision on Bears
                bool[] bearHits = collisionManager.GenericAttack(attackArea, pCurrent, bearsCurrent);
                //checks and deals with the heath of the bears
                for (int i = 0; i < bearHits.Length; i++)
                {
                    if (bearHits[i] && bearsCurrent[i].WasAttacked == false)
                    {
                        bearsCurrent[i].Health--;
                        bearsCurrent[i].WasAttacked = true;
                    }
                }

                //calls collision on Trees
                bool[] treeHits = collisionManager.GenericAttack(attackArea, pCurrent, treesCurrent);
                //checks and deals with the health of trees.
                for (int i = 0; i < treeHits.Length; i++)
                {
                    if (treeHits[i] && treesCurrent[i].WasAttacked == false)
                    {
                        treesCurrent[i].Health--;
                        treesCurrent[i].WasAttacked = true;
                        treesCurrent[i].AnimateMoveFrame++;
                    }
                }

            }
            else if (pCurrent.Attack == AttackVariation.shotgun)
            {
                //sizes 4:3 - May need to Scale Down LOL
                attackArea = new Rectangle(pCurrent.Location.X, pCurrent.Location.Y, 275, 275);

                //expands the area based on direction, uses same layout as above
                switch (pCurrent.AttackDirection)
                {
                    case PlayerAttackDirection.up:
                        attackArea.Y -= (attackArea.Height);
						attackArea.X -= (attackArea.Width / 3);
						break;
                    case PlayerAttackDirection.down:
                        attackArea.Y += pCurrent.Height;
						attackArea.X -= (attackArea.Width / 3);
                        break;
                    case PlayerAttackDirection.left:
                        attackArea.X -= (attackArea.Width);
						attackArea.Y -= (pCurrent.ObjectCollisionBox.Height);
                        break;
                    case PlayerAttackDirection.right:
                        attackArea.X += pCurrent.Width;
						attackArea.Y -= (pCurrent.ObjectCollisionBox.Height);
						break;
                }

				pCurrent.PlayerAttackBox = attackArea;
				//calls collision on Bears
				bool[] bearHits = collisionManager.GenericAttack(attackArea, pCurrent, bearsCurrent);
                //checks and deals with the heath of the bears
                for (int i = 0; i < bearHits.Length; i++)
                {
                    if (bearHits[i] && bearsCurrent[i].WasAttacked == false)
                    {
                        bearsCurrent[i].Health--;
                        bearsCurrent[i].WasAttacked = true;
                    }
                }

                //calls collision on Trees
                bool[] treeHits = collisionManager.GenericAttack(attackArea, pCurrent, treesCurrent);
                //checks and deals with the health of trees.
                for (int i = 0; i < treeHits.Length; i++)
                {
                    if (treeHits[i] && treesCurrent[i].WasAttacked == false)
                    {
                        treesCurrent[i].Health--;
                        treesCurrent[i].WasAttacked = true;
                        treesCurrent[i].AnimateMoveFrame++;

                    }
                }
            }

            // Increment the Attack Animation Frame since the player has attacked!
            if (animate == true && pCurrent.IsAttacking == true)
            {
                pCurrent.AttackAnimationF++;
            }
        }

        public void UpdateBearsAttack()
        {
            foreach (Bear b in bearsCurrent)
            {
                if (!b.IsAttacking) //if bear is not already attacking
                {
                    // If Bears field of attack intersect Player 
                    if (b.FieldOfAttack.Intersects(pCurrent.ObjectCollisionBox))
                    {
                        b.IsAttacking = true;
                        Rectangle attackArea = new Rectangle(b.Location.X, b.Location.Y, 100, 100); // May be able to change to foa standard. idk
                        // May need to reconfigure to state Bears pos relative to Players pos to  decide its area
                        BearDirection attackDirection = (BearDirection)((int)b.BearDirection % 3);
                        switch (b.BearDirection)
                        {
                            case BearDirection.up:
                                attackArea.Height = 50;
                                attackArea.Width = 100;
                                attackArea.Y -= attackArea.Height;
                                attackArea.X -= 15;
                                break;
                            case BearDirection.down:
                                attackArea.Height = 50;
                                attackArea.Width = 100;
                                attackArea.Y += b.Height;
                                attackArea.X -= 15;
                                break;
                            case BearDirection.left:
                                attackArea.Width = 50;
                                attackArea.Height = 100;
                                attackArea.X -= attackArea.Width;
                                break;
                            case BearDirection.right:                                
                                attackArea.Width = 50;
                                attackArea.Height = 100;
                                attackArea.X += b.Width;
                                break;

                        }
                        b.AttackBox = attackArea;
                        // Calls for attack on the player
                        // May need to rewrite method for one bear, not the whole bear list  Attacking Here
                        bool[] playerHit = collisionManager.GenericAttack(attackArea, pCurrent, bearsCurrent);
                        if (playerHit[0] && pCurrent.Invincible != true)
                        {
                            pCurrent.Health--;
							pCurrent.Invincible = true;

							pCurrent.ResetTimer();
                        }

                    }
                }
                else
                {
                    if (animate == true && b.IsAttacking)
                    {
                        b.AttackAnimationF++;
                        if (b.AttackAnimationF >13)
                        {
                            b.AttackAnimationF = 0;
                            b.IsAttacking = false;
                        }
                    }
                }
            }
        }
        
        private void Invinicibility()
        {
            if (pCurrent.Invincible == true)
            {
                pCurrent.InvincibleTimer -= gameTime.ElapsedGameTime.TotalSeconds;

                if (pCurrent.InvincibleTimer <= 0)
                {
                    pCurrent.Invincible = false;
                }
            }
        }

        #endregion Updating Player & Bear Interactions
        #region Updating Pickups & Open Season
        // -------------------------------------------------------------Pickup Specific Methods && O.S. ---------------------------------------------------------------------

        /// <summary>
        /// Loops through list of all pickups, if pickup collides checks type and handles code
        /// </summary>
        private void UpdatePickUps()
		{
				foreach (PickUp thisPickup in pickUpsCurrent)
				{
					if (thisPickup.ObjectCollisionBox.Intersects(pCurrent.ObjectCollisionBox))
					{
						thisPickup.ItemState = ItemState.Retrieved;
						switch (thisPickup.PickupType)
						{
							case PickupType.Apple:
								pCurrent.Health += 1;
								break;

							case PickupType.MapleSyrup:
								pCurrent.TotalScore += 45;
								break;

							case PickupType.Shotgun:
                            CheckOpenSeason();
								break;
						}
					}
				}
			
		}

        /// <summary>
        /// Determines if the game State should be set or not.
        /// </summary>
        private void CheckOpenSeason()
        {
            // If open season was not occuring before, it now needs to be set.
            if (runOpenSeason == false)
            {
                runOpenSeason = true;
                SetOpenSeason();
            }
            // If open season was occuring before, then just the timer needs to be reset since all the values have already been modified. 
            else 
            {
                oS.ResetOpenSeasonTimer();
            }
        }

        /// <summary>
        /// Set's Open Season values where needed
        /// </summary>
        private void SetOpenSeason()
        {
            bearsCurrent = oS.SetOpenSeasonList(bearsCurrent);
            pCurrent = oS.SetOpenSeasonPlayer(pCurrent);
            camera = oS.OpenSeasonCamera(camera);
        }

        /// <summary>
        /// Determines if Open Season should end or not.
        /// </summary>
        private void DetermineOpenSeason()
        {
            if (oS.OpenSeason() == true)
            {
                oS.UpdateTimer(gameTime);
            }
            else
            {
                EndOpenSeason();

            }
        }

        /// <summary>
        /// Resets Values of Open Season
        /// </summary>
        private void EndOpenSeason()
        {
            bearsCurrent = oS.EndOpenSeasonList(bearsCurrent);
            pCurrent = oS.EndOpenSeasonPlayer(pCurrent);
            camera = oS.OpenSeasonCamera(camera);
            runOpenSeason = false;
        }

        #endregion Updating Pickups & Open Season
        #region Updating Camera
        // -------------------------------------------------------------------------- Camera Logic ---------------------------------------------------------------------------

        /// <summary>
        /// Updates the Camera and all variables in the Camera
        /// </summary>
        private void UpdateCamera()
        {
            camera.UpdatePosition(pCurrent.ObjectCollisionBox);
			if (backgrounds != null)
			{
				foreach (BackgroundTile thisBg in backgrounds)
				{
					if (camera.IsDrawn(thisBg.ObjectCollisionBox)) //checks if needs to be drawn
					{
						thisBg.OnScreen = true;
					}
					else
					{
						thisBg.OnScreen = false;
					}
				}
			}
			if (treesCurrent != null)
			{
				foreach (Tree thisTree in treesCurrent)
				{
					if (camera.IsDrawn(thisTree.ObjectCollisionBox)) //checks if needs to be drawn
					{
						thisTree.OnScreen = true;
					}
					else
					{
						thisTree.OnScreen = false;
					}

					if (camera.IsUpdating(thisTree.ObjectCollisionBox)) //checks if needs to be updated
					{
						thisTree.UPScreen = true;
					}
					else
					{
						thisTree.UPScreen = false;
					}
					
					if (camera.IsColliding(thisTree.ObjectCollisionBox)) //checks if things can collide with it
					{
						thisTree.Colliding = true;
					}
					else
					{
						thisTree.Colliding = false;
					}
				}
			}
			if (bearsCurrent != null)
			{
				foreach (Bear thisBear in bearsCurrent)
				{
					if (camera.IsDrawn(thisBear.ObjectCollisionBox)) //checks if needs to be drawn
					{
						thisBear.OnScreen = true;
					}
					else
					{
						thisBear.OnScreen = false;
					}

					if (camera.IsUpdating(thisBear.ObjectCollisionBox)) //checks if needs to be updated
					{
						thisBear.UPScreen = true;
					}
					else
					{
						thisBear.UPScreen = false;
					}
				}
			}
			if (pickUpsCurrent != null)
			{
				foreach (PickUp thisPickup in pickUpsCurrent)
				{
					if (camera.IsDrawn(thisPickup.ObjectCollisionBox)) //checks if needs to be drawn
					{
						thisPickup.OnScreen = true;
					}
					else
					{
						thisPickup.OnScreen = false;
					}

					if (camera.IsUpdating(thisPickup.ObjectCollisionBox)) //checks if needs to be updated
					{
						thisPickup.UPScreen = true;
					}
					else
					{
						thisPickup.UPScreen = false;
					}
				}
			}
        }

        #endregion Updating Camera
        #region Animation Help 
        // --------------------------------------------------------------------------------------------------------------------- Update Animations -------------------------------------------//

        public void UpdateAnimations()
        {
            animate = aM.UpdateAnimation(gameTime);
        }

        private void ResetTreesAttackBool()
        {
            if (treesCurrent != null)
            {
                foreach (Tree t in treesCurrent)
                {
                    t.WasAttacked = false;
                }
            }

        }

        #endregion Animation Help 
    }
}

