﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

enum PlayerState
{
    IdleLeft,
    IdleRight,

    WalkLeft,
    WalkRight,

    RollLeft,
    RollRight,

    JumpLeft,
    JumpRight,

    Fall,
    DownDash,

    BounceLeft,
    BounceRight,

    Die,
}
namespace Party_Tower_Main
{
    class Player : GameObject
    {
        //################
        #region FIELDS
        //Fields

        private KeyboardState kb;
        private KeyboardState previousKb; //used to prevent jump spamming

        private double miliseconds; //used for downdash/roll
        private double downDashDelay; //used for downdash
        private double rollDelay;

        private Color color;
        private List<Collectible> collectiblesCollected;
        private Dictionary<string, Keys> bindableKb;

        //Collision
        private Rectangle bottomChecker;
        private Rectangle topChecker;
        private Rectangle sideChecker;
        private bool bottomIntersects;
        private bool topIntersects;
        private bool isDebugging = false;
        private bool debugEnemyCollision = false;
        private bool playerVisible = true;
        private bool bounceLockout = false;
        private bool shouldBounce = false;
        //Tile temp; //used to make sure player checks collision against only 
                   //1 tile when necessary (as opposed to all of them each frame like usual)

        //Directionality and FSM
        private bool isFacingRight;
        private PlayerState playerState;
        private PlayerState previousPlayerState;
        
        //Movement
        private bool rollInAir;
        private bool isRolling;
        private int verticalVelocity = 0;
        private int horizontalVelocity = 0;
        private bool rollEnd;
        private bool hasRolledInAir;
        private int jumpCount; //used for double jump

        private Vector2 previousPlayerPosition; //positions used to check if the player is going up or down
        private Vector2 playerPosition;

        //might not need this
        private bool goingUp; //used to allow player to float correctly when transitioning up a screen


        //sound
        public static ContentManager myContent; //used to load content in non-Game1 Class
        SoundEffect bounceSound;
        SoundEffect coinSound;
        SoundEffect downDashSound;
        SoundEffect jumpSound;
        SoundEffect rollSound;
        SoundEffect walkSound;
        SoundEffect checkpointSound;

        //Coop
        Vector2 playerSpawn; //position at which the dead player will spawn at
        

        GameTime gameTime;
        #endregion
        //################

        //################
        #region PROPERTIES
        //Properties
        public PlayerState PlayerState
        {
            get { return playerState; }
            set { playerState = value; }
        }
        public int VerticalVelocity
        {
            get { return verticalVelocity; }
            set { verticalVelocity = value; }
        }
        public int HorizontalVelocity
        {
            get { return horizontalVelocity; }
            set { verticalVelocity = value; }
        }
        public bool IsDebugging
        {
            get { return isDebugging; }
        }
        public bool IsFacingRight
        {
            get { return isFacingRight; }
        }
        public override Rectangle Hitbox
        {
            get { return hitbox; }
            set { hitbox = value; }
        }
        public bool InBounceLockout
        {
            get { return bounceLockout; }
        }

        public Vector2 PlayerSpawn
        {
            get { return playerSpawn; }
            set { playerSpawn = value; }
        }
        public Dictionary<string, Keys> BindableKb
        {
            get { return bindableKb; }
            set { bindableKb = value; }
        }
        public List<Collectible> CollectiblesCollected
        {
            get { return collectiblesCollected; }
            set { collectiblesCollected = value; }
        }
        public PlayerState PreviousPlayerState
        {
            get { return previousPlayerState; }
        }
        public bool GoingUp
        {
            get { return goingUp; }
            set { goingUp = value; }
        }
        public bool IsRolling
        {
            get { return isRolling; }
            set { isRolling = value; }
        }
        #endregion
        //################

        //################
        #region CONSTRUCTOR
        //Constructor for player
        public Player(int playerNumber, int drawLevel, Texture2D defaultSprite, Rectangle hitbox, Color color, ContentManager content)
        {
            this.drawLevel = drawLevel;
            this.defaultSprite = defaultSprite;
            this.hitbox = hitbox;
            this.color = color;
            myContent = content;
            isActive = true;

            hasGravity = true; //no point other than it must be implement since it inherets GameObject


            bottomIntersects = false;
            topIntersects = false;
            isRolling = false;
            rollInAir = false;
            rollEnd = false;
            hasRolledInAir = false;
            jumpCount = 0;

            gameTime = new GameTime();
            downDashDelay = 13;
            rollDelay = 30;
            miliseconds = 2;
            collectiblesCollected = new List<Collectible>();

            bindableKb = new Dictionary<string, Keys>();

            //sound
            bounceSound = myContent.Load<SoundEffect>("bounce");
            coinSound = myContent.Load<SoundEffect>("coin");
            downDashSound = myContent.Load<SoundEffect>("downdash");
            jumpSound = myContent.Load<SoundEffect>("jump");
            rollSound = myContent.Load<SoundEffect>("roll");
            walkSound = myContent.Load<SoundEffect>("walk");
        }
        #endregion
        //################

        /// <summary>
        /// speed up the object by the rate until the limit velocity is reached
        /// </summary>
        /// <param name="velocityType"></param>
        /// <param name="rate"></param>
        /// <param name="limit"></param>
        public void Accelerate(int velocityType, int rate, int limit, bool vertical)
        {
            if (vertical)
            {
                if (velocityType < limit)
                {
                    velocityType += rate;
                    verticalVelocity = velocityType;
                }
            }
            else //horizontal
            {
                if (isFacingRight)
                {
                    if (velocityType < limit)
                    {
                        velocityType += rate;
                    }
                }
                else //facing left
                {
                    //moving left means moving negatively (decrease value past 0 until negative limit is hit)
                    limit -= limit * 2;
                    if (velocityType > limit)
                    {
                        velocityType -= rate;
                    }
                }
                horizontalVelocity = velocityType;
            }
        }
        /// <summary>
        /// Checks if enemies are touching player
        /// </summary>
        /// <param name="e"></param>
        public void CheckColliderAgainstEnemy(Enemy e)
        {
            if (bottomChecker.Intersects(e.Hitbox))
            {
                shouldBounce = true;
            }
            else
            {
                shouldBounce = false;
            }
            if (hitbox.Intersects(e.Hitbox) && !bounceLockout)
            {
                debugEnemyCollision = true;
                if (playerState == PlayerState.RollRight)
                {
                    X = e.X - hitbox.Width - 1;
                    playerState = PlayerState.BounceLeft;
                    debugEnemyCollision = false;
                    rollEnd = true;
                    isRolling = false;
                    rollDelay = 30;
                }
                else if (playerState == PlayerState.RollLeft)
                {
                    X = e.X + (hitbox.Width * 2) + 1;
                    playerState = PlayerState.BounceRight;
                    debugEnemyCollision = false;
                    rollEnd = true;
                    isRolling = false;
                    rollDelay = 30;
                }
                else if (playerState == PlayerState.DownDash && shouldBounce)
                {
                    Y = e.Y - hitbox.Height - 1;
                    if (IsFacingRight)
                    {
                        playerState = PlayerState.BounceRight;
                    }
                    else
                    {
                        playerState = PlayerState.BounceLeft;
                    }
                    debugEnemyCollision = false;
                    rollEnd = false;
                }
                else
                {
                    rollEnd = false;
                }
                //player takes dies if not rolling, bouncing, downdashing
                if (playerState != PlayerState.RollLeft && playerState != PlayerState.RollRight && playerState != PlayerState.DownDash &&
                    playerState != PlayerState.BounceLeft && playerState != PlayerState.BounceRight)
                {
                    playerState = PlayerState.Die;
                }

                bounceLockout = true;
            }
            else
            {
                debugEnemyCollision = false;
            }
        }
        /// <summary>
        /// Checks if hitboxes around player touch Tile t
        /// </summary>
        public void CollisionCheck(Tile t)
        {
            //wall collision (collision box next to player depending on direction facing)
            if (sideChecker.Intersects(t.Hitbox))
            {
                #region Default
                horizontalVelocity = 0; //stop player from moving through wall
                if (isFacingRight && t.X > hitbox.X) //player facing right and tile is to the right of player
                {
                    hitbox.X = t.X - hitbox.Width + 1; //place player left of tile
                }
                else if (t.X < hitbox.X) //player facing left and tile is to the left of player
                {
                    hitbox.X = t.X + t.Hitbox.Width - 1; //place player right of tile
                }

                if (playerState == PlayerState.Fall)
                {
                    if (isFacingRight)
                    {
                        hitbox.X -= 1;
                    }
                    else
                    {
                        hitbox.X += 1;
                    }

                }
                #endregion

            }
            //ceiling collision (collision box above player)
            if (topChecker.Intersects(t.Hitbox))
            {
                #region Default
                if (topIntersects) //this is used to ensure player is placed at the ceiling only once per jump
                {
                    hitbox.Y = t.Y + t.Hitbox.Height; //place player at ceiling (illusion of hitting it)
                }
                topIntersects = false; //set to false so that the player isn't placed to the ceiling again until they touch the ground
                verticalVelocity = (int)(Math.Abs(verticalVelocity) * .75); //launch the player downwards
                #endregion
            }
            //floor collision (collision box below player)
            else if (bottomChecker.Intersects(t.Hitbox))
            {
                #region Default
                if (!(playerState == PlayerState.BounceLeft || playerState == PlayerState.BounceRight))
                {
                    verticalVelocity = 0; //stop the player from falling
                    hitbox.Y = t.Y - hitbox.Height; //place the player on top of tile
                }
                else
                {
                    hitbox.Y = t.Y - hitbox.Height - 1; //place the player on top of tile
                }


                hasRolledInAir = false;
                bottomIntersects = true;
                //FSM states are changed here so that the player can move after touching the ground
                //Roll Left
                if ((SingleKeyPress(bindableKb["roll"]) && !isFacingRight) || ((isRolling && !isFacingRight) && !rollEnd))
                {
                    playerState = PlayerState.RollLeft;
                }
                //Walk Left
                else if (kb.IsKeyDown(bindableKb["left"]) && !kb.IsKeyDown(bindableKb["right"]) && !isRolling)
                {
                    playerState = PlayerState.WalkLeft;
                }
                //Roll Right
                else if ((SingleKeyPress(bindableKb["roll"]) && isFacingRight) || ((isRolling && isFacingRight) && !rollEnd))
                {
                    playerState = PlayerState.RollRight;
                }
                //Walk Right
                else if (kb.IsKeyDown(bindableKb["right"]) && !isRolling)
                {
                    playerState = PlayerState.WalkRight;
                }
                //Idle Right
                else if (isFacingRight && !kb.IsKeyDown(bindableKb["right"]) && playerState != PlayerState.BounceLeft && playerState != PlayerState.RollRight)
                {
                    playerState = PlayerState.IdleRight;
                }
                //Idle Left
                else if (!isFacingRight && !kb.IsKeyDown(bindableKb["left"]) && playerState != PlayerState.BounceRight && playerState != PlayerState.RollLeft)
                {
                    playerState = PlayerState.IdleLeft;
                }
                //everytime the player lands from a jump (or falls), the next time they jump they will hit the ceiling
                topIntersects = true;
                #endregion
            }
        }
        /// <summary>
        /// slowdown the object by the rate until the limit velocity is reached 
        /// </summary>
        /// <param name="velocityType"></param>
        /// <param name="rate"></param>
        public void Decelerate(int velocityType, int rate, int limit, bool vertical)
        {

            if (isFacingRight)
            {
                if (velocityType > limit)
                {
                    velocityType -= rate; //reduce velocity normally

                    if (!bottomIntersects && !isRolling)
                    {
                        playerState = PlayerState.Fall;
                    }
                }
            }
            else
            {
                if (velocityType < limit)
                {
                    velocityType += rate; //increase velocity since moving left is negative
                                          //needed to prevent player from hovering in air if they decelerate on an edge
                    if (!bottomIntersects && !isRolling)
                    {
                        playerState = PlayerState.Fall;
                    }
                }
            }

            if (vertical)
            {
                verticalVelocity = velocityType;
            }
            else
            {
                horizontalVelocity = velocityType;
            }
        }
        /// <summary>
        /// draws the player for normal game and debugging
        /// </summary>
        /// <param name="sb"></param>
        public override void Draw(SpriteBatch sb)
        {
            if (isDebugging)
            {
                //press C to toggle player transparency
                if (SingleKeyPress(Keys.C))
                {
                    playerVisible = !playerVisible;
                }
                if (playerVisible)
                {
                    sb.Draw(defaultSprite, hitbox, Color.Black);
                    sb.Draw(defaultSprite, bottomChecker, Color.Black);
                    sb.Draw(defaultSprite, sideChecker, Color.Red);
                    sb.Draw(defaultSprite, topChecker, Color.Cyan);
                }
                else
                {
                    sb.Draw(defaultSprite, hitbox, Color.Transparent);
                    sb.Draw(defaultSprite, bottomChecker, Color.Black);
                    sb.Draw(defaultSprite, sideChecker, Color.Red);
                    sb.Draw(defaultSprite, topChecker, Color.Cyan);
                }

            }
            else
            {
                sb.Draw(defaultSprite, hitbox, color);
            }

        }
        /// <summary>
        /// determines player state based on input and collision with enemies/platforms
        /// </summary>
        public void FiniteState()
        {
            //previousPosition tracks player from previous frame
            previousPlayerPosition = playerPosition;
            previousPlayerState = PlayerState;
            playerPosition = new Vector2(X, Y); //player position of current frame


            //previousKb used to prevent jump spamming (holding down space) 
            previousKb = kb;
            kb = Keyboard.GetState();

            //Debugging code
            if (SingleKeyPress(Keys.F8))
            {
                //switch between debugging and not everytime you press combo
                isDebugging = !isDebugging;
            }

            //################
            #region COLLISIONBOXES
            if (horizontalVelocity > 0)
            {
                //X is right of player, Y is the same as player, width depends on horizontalVelocity, height is same as player
                sideChecker = new Rectangle(X + hitbox.Width, Y + 10, Math.Abs(horizontalVelocity), hitbox.Height - 20);
            }
            //Facing left
            else if (horizontalVelocity < 0)
            {
                //X is same as player (which is left edge), Y is the same as player
                //width depends on horizontalVelocity, height is same as player
                sideChecker = new Rectangle(X - Math.Abs(horizontalVelocity), Y + 10, Math.Abs(horizontalVelocity), hitbox.Height - 20);
            }
            else
            {
                if (isFacingRight)
                {
                    sideChecker = new Rectangle(X + hitbox.Width, Y + 10, Math.Abs(horizontalVelocity), hitbox.Height - 20);
                }
                else
                {
                    sideChecker = new Rectangle(X - Math.Abs(horizontalVelocity), Y + 10, Math.Abs(horizontalVelocity), hitbox.Height - 20);
                }
            }
            //height is player height with vertical velocity added on (subtracting makes the height go "up" aka toward the ceiling)
            if (verticalVelocity <= 0)
            {
                topChecker = new Rectangle(X + 10, Y - Math.Abs(verticalVelocity), hitbox.Width - 20, Math.Abs(verticalVelocity));
            }
            else
            {
                topChecker = new Rectangle(X + 10, Y, hitbox.Width - 20, 0);
            }



            bottomChecker = new Rectangle(X + 10, Y + hitbox.Height, hitbox.Width - 20, Math.Abs(verticalVelocity));

            if (verticalVelocity == 0 && !kb.IsKeyDown(bindableKb["jump"]))
            {
                bottomChecker = new Rectangle(X + 10, Y + hitbox.Height, hitbox.Width - 20, 1);
            }
            #endregion
            //################

            //double jump counter gets reset when player touches ground
            if (bottomIntersects)
            {
                jumpCount = 0;
            }

            //################
            #region FINITESTATE
            //FSM
            switch (playerState)
            {
                //##################
                #region IDLE STATES
                //##################
                //Idle Left
                case PlayerState.IdleLeft:
                    isFacingRight = false;
                    if (debugEnemyCollision)
                    {
                        playerState = PlayerState.Die;
                    }
                    Movement();

                    if (!debugEnemyCollision)
                    {
                        if (SingleKeyPress(bindableKb["jump"]))
                        {
                            jumpSound.Play();
                            playerState = PlayerState.JumpLeft;
                        }
                        else if (kb.IsKeyDown(bindableKb["right"]))
                        {
                            playerState = PlayerState.WalkRight;
                        }
                        else if (kb.IsKeyDown(bindableKb["left"]))
                        {
                            playerState = PlayerState.WalkLeft;
                        }
                        else if (SingleKeyPress(bindableKb["roll"]))
                        {
                            rollSound.Play();
                            playerState = PlayerState.RollLeft;
                        }
                    }
                    break;

                //Idle Right
                case PlayerState.IdleRight:
                    isFacingRight = true;
                    if (debugEnemyCollision)
                    {
                        playerState = PlayerState.Die;
                    }
                    Movement();

                    if (!debugEnemyCollision)
                    {
                        if (SingleKeyPress(bindableKb["jump"]))
                        {
                            jumpSound.Play();
                            playerState = PlayerState.JumpRight;
                        }
                        else if (kb.IsKeyDown(bindableKb["right"]))
                        {
                            playerState = PlayerState.WalkRight;
                        }
                        else if (kb.IsKeyDown(bindableKb["left"]))
                        {
                            playerState = PlayerState.WalkLeft;
                        }
                        else if (SingleKeyPress(bindableKb["roll"]))
                        {
                            rollSound.Play();
                            playerState = PlayerState.RollRight;
                        }
                    }
                    break;

                #endregion
                //################

                //################
                #region WALK STATES
                //Walk Left
                case PlayerState.WalkLeft:
                    isFacingRight = false;
                    if (debugEnemyCollision)
                    {
                        playerState = PlayerState.Die;
                    }
                    Movement();

                    if (!debugEnemyCollision)
                    {
                        if (SingleKeyPress(bindableKb["jump"]))
                        {
                            jumpSound.Play();
                            playerState = PlayerState.JumpLeft;
                        }
                        else if (kb.IsKeyUp(bindableKb["left"])) //stop moving left
                        {
                            playerState = PlayerState.IdleLeft;
                        }
                        else if (kb.IsKeyDown(bindableKb["right"]))
                        {
                            playerState = PlayerState.WalkRight;
                        }
                        else if (SingleKeyPress(bindableKb["roll"]))
                        {
                            rollSound.Play();
                            playerState = PlayerState.RollLeft;
                        }
                        if (!bottomIntersects) //not touching ground
                        {
                            playerState = PlayerState.Fall;
                        }
                    }
                    break;
                //Walk Right
                case PlayerState.WalkRight:
                    isFacingRight = true;
                    if (debugEnemyCollision)
                    {
                        playerState = PlayerState.Die;
                    }
                    Movement();


                    if (!debugEnemyCollision)
                    {
                        if (SingleKeyPress(bindableKb["jump"]))
                        {
                            jumpSound.Play();
                            playerState = PlayerState.JumpRight;
                        }
                        else if (kb.IsKeyUp(bindableKb["right"])) //stop moving right
                        {
                            playerState = PlayerState.IdleRight;
                        }
                        else if (kb.IsKeyDown(bindableKb["left"])) //moving left
                        {
                            playerState = PlayerState.WalkLeft;
                        }
                        else if (SingleKeyPress(bindableKb["roll"]))
                        {
                            rollSound.Play();
                            playerState = PlayerState.RollRight;
                        }
                        if (!bottomIntersects) //not touching ground
                        {
                            playerState = PlayerState.Fall;
                        }
                    }
                    break;
                #endregion
                //################

                //################
                #region ROLL STATES
                //Roll Left
                case PlayerState.RollLeft:
                    isFacingRight = false;
                    Movement();
                    if (!bottomIntersects && !isRolling) //not touching ground
                    {
                        playerState = PlayerState.Fall;
                    }
                    if (!isRolling && kb.IsKeyDown(bindableKb["left"]))
                    {
                        playerState = PlayerState.WalkLeft;
                    }
                    else if (!isRolling && kb.IsKeyDown(bindableKb["right"]))
                    {
                        playerState = PlayerState.WalkRight;
                    }
                    else if (SingleKeyPress(bindableKb["jump"]) && !isRolling && bottomIntersects)
                    {
                        jumpSound.Play();
                        playerState = PlayerState.JumpLeft;
                    }
                    else if (!isRolling && bottomIntersects)
                    {
                        playerState = PlayerState.IdleLeft;
                    }

                    break;

                //Roll Right
                case PlayerState.RollRight:
                    isFacingRight = true;
                    Movement();
                    if (!bottomIntersects && !isRolling) //not touching ground
                    {
                        playerState = PlayerState.Fall;
                    }
                    if (SingleKeyPress(bindableKb["jump"]) && !isRolling && bottomIntersects)
                    {
                        jumpSound.Play();
                        playerState = PlayerState.JumpRight;
                    }
                    else if (!isRolling && kb.IsKeyDown(bindableKb["left"]))
                    {
                        playerState = PlayerState.WalkLeft;
                    }
                    else if (!isRolling && kb.IsKeyDown(bindableKb["right"]))
                    {
                        playerState = PlayerState.WalkRight;
                    }
                    else if (!isRolling && bottomIntersects)
                    {
                        playerState = PlayerState.IdleRight;
                    }
                    break;
                #endregion
                //################

                //################
                #region JUMP STATES
                //Jump Left
                case PlayerState.JumpLeft:
                    isFacingRight = false;
                    Movement();
                    playerState = PlayerState.Fall;
                    break;

                //Jump Right
                case PlayerState.JumpRight:
                    isFacingRight = true;
                    Movement();
                    playerState = PlayerState.Fall;
                    break;
                #endregion
                //################

                //################
                #region FALL STATE
                //Fall 
                case PlayerState.Fall:
                    Movement();
                    if (debugEnemyCollision && isFacingRight)
                    {
                        playerState = PlayerState.Die;
                    }
                    else if (debugEnemyCollision && !isFacingRight)
                    {
                        playerState = PlayerState.Die;
                    }
                    else
                    {
                        //can jump twice
                        if (SingleKeyPress(bindableKb["jump"]) && jumpCount <= 1)
                        {
                            jumpSound.Play();
                            if (kb.IsKeyDown(bindableKb["right"]))
                            {
                                playerState = PlayerState.JumpRight; //jump right if trying to move right
                            }
                            else if (kb.IsKeyDown(bindableKb["left"]))
                            {
                                playerState = PlayerState.JumpLeft; //jump left if trying to move left
                            }
                            //if no button is pressed, jump based on direction player is facing
                            else if (isFacingRight)
                            {
                                playerState = PlayerState.JumpRight;
                            }
                            else
                            {
                                playerState = PlayerState.JumpLeft;
                            }
                        }
                        if (SingleKeyPress(bindableKb["roll"]) && !hasRolledInAir)
                        {
                            rollSound.Play();
                            rollInAir = true;
                            if (isFacingRight)
                            {
                                playerState = PlayerState.RollRight;
                            }
                            else
                            {
                                playerState = PlayerState.RollLeft;
                            }
                        }
                        if (SingleKeyPress(bindableKb["downDash"]))
                        {
                            downDashSound.Play();
                            playerState = PlayerState.DownDash;
                        }
                    }

                    //adjust delays to determine how long delay is for downdash
                    downDashDelay = 13;
                    break;
                #endregion
                //################

                //################
                #region DOWNDASH
                case PlayerState.DownDash:
                    if (debugEnemyCollision && shouldBounce)
                    {
                        if (isFacingRight)
                        {
                            playerState = PlayerState.BounceRight;
                        }
                        else
                        {
                            playerState = PlayerState.BounceLeft;
                        }
                    }
                    Movement(); //movement is after to prevent player from touching ground during downdash if they touch an enemy

                    //Implement interaction with enemy here
                    break;
                #endregion
                //################

                //################
                #region BOUNCE STATES
                //Bounce Left
                case PlayerState.BounceLeft:
                    Movement();
                    break;

                //Bounce Right
                case PlayerState.BounceRight:
                    Movement();
                    break;
                    #endregion
                    //################
            }
            #endregion
            //################
        }
        /// <summary>
        /// Calls accelerate/decelerate methods based on FSM state, the direction is accounted for in the methods
        /// </summary>
        public void Movement()
        {
            if (kb.IsKeyUp(bindableKb["left"]) && kb.IsKeyUp(bindableKb["right"]) && verticalVelocity == 0)
            {
                bool temp = isFacingRight;
                isFacingRight = !isFacingRight;
                Decelerate(horizontalVelocity, 1, 0, false);
                isFacingRight = temp;
            }

            //Idle
            if (playerState == PlayerState.IdleLeft || playerState == PlayerState.IdleRight)
            {
                //slow-down and stop the player if they are walking
                if (horizontalVelocity <= 10 && horizontalVelocity >= -10)
                {
                    Decelerate(horizontalVelocity, 1, 0, false);
                }
                //slow-down and stop the player if they are rolling
                else if (horizontalVelocity > 10 || horizontalVelocity < -10)
                {
                    Decelerate(horizontalVelocity, 2, 0, false);
                }

                bounceLockout = false;
            }
            //Walk
            else if (playerState == PlayerState.WalkLeft || playerState == PlayerState.WalkRight)
            {
                //slow-down the player if they were rolling
                if (horizontalVelocity > 10 || horizontalVelocity < -10)
                {
                    Decelerate(horizontalVelocity, 1, 10, false);
                }
                Accelerate(horizontalVelocity, 5, 10, false);
            }
            //Roll
            else if (playerState == PlayerState.RollLeft || playerState == PlayerState.RollRight)
            {
                bounceLockout = false;
                isRolling = true;
                Accelerate(horizontalVelocity, 6, 18, false);
                if (SingleKeyPress(bindableKb["jump"]))
                {
                    isRolling = false;
                    rollDelay = 30;
                }
                if (!bottomIntersects && !rollInAir) //mimic gravity while rolling
                {
                    Accelerate(verticalVelocity, 2, 30, true);
                }
                if (rollInAir)
                {
                    verticalVelocity = 0;
                }
                rollDelay -= miliseconds;
                if (rollDelay <= 0)
                {
                    if (!bottomIntersects)
                    {
                        hasRolledInAir = true;
                    }

                    isRolling = false;
                    if (rollInAir == true)
                    {
                        Decelerate(horizontalVelocity, 10, 10, false);
                    }
                    rollInAir = false;
                    rollDelay = 30;
                    if (SingleKeyPress((bindableKb["left"])))
                    {
                        playerState = PlayerState.WalkLeft;
                    }
                    else if (SingleKeyPress((bindableKb["right"])))
                    {
                        playerState = PlayerState.WalkRight;
                    }
                    else if (!isFacingRight)
                    {
                        playerState = PlayerState.IdleLeft;
                    }
                    else if (isFacingRight)
                    {
                        playerState = PlayerState.IdleRight;
                    }
                }
            }
            //Jump
            else if (playerState == PlayerState.JumpLeft || playerState == PlayerState.JumpRight)
            {
                //give huge start velocity then transition to fall and allow gravity to create arch
                verticalVelocity = -30;
                jumpCount++; //increment to keep track of how many times player has jumped
            }
            //Fall
            else if (playerState == PlayerState.Fall)
            {
                bounceLockout = false;
                //Gravity
                Accelerate(verticalVelocity, 2, 30, true);

                if (kb.IsKeyDown(bindableKb["left"]))
                {
                    //temp used to return isFacingRight to original state
                    bool temp = isFacingRight;
                    isFacingRight = false;
                    Accelerate(horizontalVelocity, 1, 10, false);
                    isFacingRight = temp;
                }
                if (kb.IsKeyDown(bindableKb["right"]))
                {
                    bool temp = isFacingRight;
                    isFacingRight = true;
                    Accelerate(horizontalVelocity, 1, 10, false);
                    isFacingRight = temp;
                }
            }
            //Down-dash
            else if (playerState == PlayerState.DownDash)
            {
                //stop in midair (illusion of delay)
                horizontalVelocity = 0;
                verticalVelocity = 0;

                bounceLockout = false;

                downDashDelay -= miliseconds;
                if (downDashDelay <= 0)
                {
                    Accelerate(verticalVelocity, 35, 60, true);
                }
            }
            //Bounce
            else if (playerState == PlayerState.BounceLeft || playerState == PlayerState.BounceRight)
            {
                if (isFacingRight && verticalVelocity != 0) //in air
                {
                    horizontalVelocity = 20;
                }
                else if (!isFacingRight && verticalVelocity != 0) //in air
                {
                    horizontalVelocity = -20;
                }
                else if (isFacingRight)
                {
                    horizontalVelocity = -20;
                }
                else
                {
                    horizontalVelocity = 20;
                }

                verticalVelocity = -30;
                if (playerState == PlayerState.BounceLeft)
                {
                    isFacingRight = false;
                }
                else
                {
                    isFacingRight = true;
                }
                playerState = PlayerState.Fall;
            }

            X += horizontalVelocity;
            Y += verticalVelocity;
        }
        /// <summary>
        /// used to prevent holding down a key from spamming an action
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
        /// Whenever the player touches an active chicken, they "save" it (counter is incremented)
        /// </summary>
        public void UpdateCollectibleList(Collectible thing) //called thing until we figure out what collectible is
        {
            coinSound.Play();
            collectiblesCollected.Add(thing);
        }
        //not applicable
        public override void CheckColliderAgainstPlayer(Player p)
        {
            //do nothing
        }

        public void PutInFallState()
        {
            playerState = PlayerState.DownDash;
        }
    }
}