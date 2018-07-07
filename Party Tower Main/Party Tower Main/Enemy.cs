using System;
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

namespace Party_Tower_Main
{
    /// <summary>
    /// Depicts the current movement of the enemy
    /// </summary>
    enum EnemyWalkingState
    {
        Waiting,            // Enemy is doing nothing     
        Follow              // Enemy is using A* to walk toward the player
    }
    enum EnemyState
    {
        IdleLeft,
        IdleRight,

        WalkLeft,
        WalkRight,

        JumpLeft,
        JumpRight,

        Fall,

        Die
    }
    /// <summary>
    /// Subject to Change. Two Core Types = Stationary & Alive
    /// </summary>
    enum EnemyType
    {
        Stationary,
        Alive
    }
    class Enemy : GameObject
    {

        #region Fields

        // Stuff for enemy pathing
        private Rectangle enemyVision;
        private int visionStandard;

        // In the instance enemy's respawn or level restarts if both players die at once
        private Point enemySpawn;

        //Determines if player and enemy collided.
        private bool hurtPlayer = false;

        //Debug Value showing where enemy is moving too
        private Vector2 target;

        //Directionality and FSM
        private bool isFacingRight;
        private EnemyState enemyState;
        private EnemyState previousEnemyState;
        private EnemyWalkingState previousWalkingState;
        private EnemyWalkingState walkingState;
        private EnemyType type;

        //movement
        private int horizontalVelocity;
        private int verticalVelocity;

        //collision
        private bool bottomIntersects;
        private bool topIntersects;
        private Rectangle sideChecker;
        private Rectangle topChecker;
        private Rectangle bottomChecker;
        private bool goingDown;
        Tile singleTile;

        //More Movement Stuff?
        private Rectangle previousHitbox;
        private Vector2 previousPosition; //positions used to check if the player is going up or down
        private Vector2 position;

        //Movement Lockout Jumping
        private bool canJump;

        //Sound
        public static ContentManager myContent; //used to load content in non-Game1 Class
        SoundEffect deathSound;
        SoundEffect followSound;
        SoundEffect fallSound;
        SoundEffect jumpSound;
        SoundEffect walkSound;

        //Hitpoints
        private int hitpoints;

        #endregion Fields

        #region Properties

        public int Hitpoints
        {
            get { return hitpoints; }
            set { hitpoints = value; }
        }

        public EnemyType Type
        {
            get { return type; }
            set { type = value; }
        }

        public int HorizontalVelocity
        {
            get { return horizontalVelocity; }
        }

        public int VerticalVelocity
        {
            get { return verticalVelocity; }
        }

        public string PositionDebug
        {
            get { return "" + X + " , " + Y + ""; }
        }

        public EnemyState EnemyState
        {
            get { return enemyState; }
        }

        public EnemyWalkingState WalkingState
        {
            get { return walkingState; }
        }

        public string TargetDebug
        {
            get { return "" + target.X + " , " + target.Y; }
        }


        #endregion Properties

        #region Constructor

        /// <summary>
        /// Constructor for pulling enemies from level map coordinator
        /// </summary>
        /// <param name="type"> Type of Enemy [Stationary = type from Egg / Alive = Following with a*] </param>
        /// <param name="visionStandard"> determines how far away the enemy can "see"  </param>
        public Enemy(EnemyType type, Rectangle hitbox, Texture2D defaultSprite, int visionStandard)
        {
            hitpoints = 3;
            verticalVelocity = 0;
            horizontalVelocity = 0;

            this.hitbox = hitbox;


            Type = type;

            this.defaultSprite = defaultSprite;
            this.visionStandard = visionStandard;
            UpdateEnemyVision();

            // Starts the enemy as inactive and not being drawn. 
            isActive = false;
            isDrawn = false;
            isFacingRight = false;
            bottomIntersects = false;
            topIntersects = false;
            canJump = true;
            goingDown = false;

            // Enemy is facing left
            isFacingRight = false;
            enemyState = EnemyState.IdleLeft;
            walkingState = EnemyWalkingState.Waiting;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Updates the Enemy Appropriately
        /// </summary>
        /// <param name="pM"></param>
        public void UpdateEnemy(Player p1, Player p2, Vector2 target)
        {
            previousPosition = position;
            previousHitbox = hitbox;
            previousEnemyState = enemyState;
            previousWalkingState = walkingState;

            position = new Vector2(X, Y);


            if (previousPosition.Y < position.Y) //player is going downwards relative to last frame
            {
                goingDown = true;
            }
            else //the player is standing normally or going upwards
            {
                goingDown = false;
            }

            // Sets Collision Boxes

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

            // Double check Figure out this condition 
            if (verticalVelocity == 0 && goingDown == true)
            {
                bottomChecker = new Rectangle(X + 10, Y + hitbox.Height, hitbox.Width - 20, 1);
            }

            #endregion COLLISIONBOXES

            if (bottomIntersects)
            {
                canJump = true;
            }

            // Decides how the enemy will move

            enemyState = DecideState(target);

            #region FSM

            switch (enemyState)
            {
                case EnemyState.IdleLeft:
                    isFacingRight = false;
                    FollowMovementLogic();

                    break;

                case EnemyState.IdleRight:
                    isFacingRight = true;
                    FollowMovementLogic();

                    break;

                case EnemyState.WalkLeft:
                    isFacingRight = false;
                    FollowMovementLogic();

                    break;

                case EnemyState.WalkRight:
                    isFacingRight = true;
                    FollowMovementLogic();

                    break;

                case EnemyState.JumpLeft:
                    isFacingRight = false;
                    FollowMovementLogic();

                    break;

                case EnemyState.JumpRight:
                    isFacingRight = true;
                    FollowMovementLogic();

                    break;

                case EnemyState.Fall:
                    isFacingRight = false;
                    FollowMovementLogic();

                    break;

            }

            #endregion FSM 
        }

        /// <summary>
        /// Checks if hitboxes around player touch Tile t, and check input depending on if controller is present
        /// </summary>
        public void CollisionCheck(Tile t)
        {
            if (t != null)
            {
                #region Wall Collisions

                //wall collision (collision box next to enemy depending on direction facing)
                if (sideChecker.Intersects(t.Hitbox))
                {
                    if (!t.IsPlatform) //only do wall collision if the tile isn't a platform
                    {
                        horizontalVelocity = 0; //stop player from moving through wall
                        if (isFacingRight && t.X > hitbox.X) //player facing right and tile is to the right of player
                        {
                            hitbox.X = t.X - hitbox.Width + 1; //place player left of tile
                        }
                        else if (t.X < hitbox.X) //player facing left and tile is to the left of player
                        {
                            hitbox.X = t.X + t.Hitbox.Width - 1; //place player right of tile
                        }

                        if (enemyState == EnemyState.Fall)
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
                    }
                }

                #endregion Wall Collisions

                #region Ceiling Collision

                //ceiling collision (collision box above enemy)
                if (topChecker.Intersects(t.Hitbox))
                {

                    if (topIntersects) //this is used to ensure player is placed at the ceiling only once per jump
                    {
                        //only do ceiling collision if the tile isn't a platform
                        if (!t.IsPlatform)
                        {
                            hitbox.Y = t.Y + t.Hitbox.Height; //place player at ceiling (illusion of hitting it)
                        }
                    }
                    //only launch the player downwards if the tile isn't a platform
                    if (!t.IsPlatform)
                    {
                        topIntersects = false; //set to false so that the player isn't placed to the ceiling again until they touch the ground
                        verticalVelocity = (int)(Math.Abs(verticalVelocity) * .75); //launch the player downwards
                    }
                }

                #endregion Ceiling Collision

                #region Floor Collision

                //floor collision (collision box below enemy)
                else if (bottomChecker.Intersects(t.Hitbox))
                {
                    bottomIntersects = true;
                    if (goingDown)
                    {
                        hitbox.Y = t.Y - hitbox.Height - 1; //place the player on top of tile
                        bottomIntersects = true;

                        //FSM states are changed here so that the player can move after touching the ground
                        //Idle Right
                        if (isFacingRight)
                        {
                            enemyState = EnemyState.IdleRight;
                        }
                        //Idle Left
                        else if (!isFacingRight)
                        {
                            enemyState = EnemyState.IdleLeft;
                        }

                        singleTile = t;
                        //everytime the player lands from a jump (or falls), the next time they jump they will hit the ceiling
                        topIntersects = true;
                    }
                }
                else if (t.Equals(singleTile))
                {
                    bottomIntersects = false;
                }

                #endregion Floor Collision
            }
        }

        /// <summary>
        /// Determines the correct enemy state to enter for movement
        /// </summary>
        private EnemyState DecideState(Vector2 target)
        {
            // In air right now
            if (canJump)
            {
                if (target.Y == hitbox.Center.Y)
                {
                    if (target.X < hitbox.Center.X)
                    {
                        return EnemyState.WalkLeft;
                    }
                    else if (target.X > hitbox.Center.X)
                    {
                        return EnemyState.WalkRight;
                    }
                    else if (previousEnemyState == EnemyState.WalkLeft)
                    {
                        return EnemyState.WalkLeft;
                    }
                    else
                    {
                        return EnemyState.IdleLeft;
                    }
                }
                else if (target.Y < hitbox.Center.Y)
                {
                    if (target.X < hitbox.Center.X)
                    {
                        return EnemyState.JumpLeft;
                    }
                    else if (target.X > hitbox.Center.X)
                    {
                        return EnemyState.JumpRight;
                    }
                }
            }

            return EnemyState.Fall;
        }


        public override void CheckColliderAgainstPlayer(Player p)
        {
            if (hitbox.Intersects(p.Hitbox) && isActive)
            {
                isActive = false;
            }
        }

        #region Helper Methods


        #region Acceleration
        /// <summary>
        /// Accelerate either vertically or horizontally at a specific rate until a specific limit is reached
        /// </summary>
        /// <param name="velocityType"></param>
        /// <param name="rate"></param>
        /// <param name="limit"></param>
        /// <param name="vertical"></param>
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
        /// Decelerate either vertically or horizontally at a specific rate until a specific limit is reached
        /// </summary>
        /// <param name="velocityType"></param>
        /// <param name="rate"></param>
        /// <param name="limit"></param>
        /// <param name="vertical"></param>
        public void Decelerate(int velocityType, int rate, int limit, bool vertical)
        {
            if (isFacingRight)
            {
                if (velocityType > limit)
                {
                    velocityType -= rate; //reduce velocity normally

                    if (!bottomIntersects)
                    {
                        enemyState = EnemyState.Fall;
                    }
                }
            }
            else
            {
                if (velocityType < limit)
                {
                    velocityType += rate; //increase velocity since moving left is negative
                                          //needed to prevent enemy from hovering in air if they decelerate on an edge
                    if (!bottomIntersects)
                    {
                        enemyState = EnemyState.Fall;
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

        #endregion Acceleration

        #region Following Movement

        /// <summary>
        /// Logic of Enemy movement based on its enemyState enum
        /// </summary>
        /// <param name="target"></param>
        private void FollowMovementLogic()
        {
            //Idle
            if (enemyState == EnemyState.IdleLeft || enemyState == EnemyState.IdleRight)
            {
                //slow-down and stop the enemy if they are walking
                if (horizontalVelocity <= 10 && horizontalVelocity >= -10)
                {
                    Decelerate(horizontalVelocity, 1, 0, false);
                }
                //slow-down and stop the enemy if they are rolling
                else if (horizontalVelocity > 10 || horizontalVelocity < -10)
                {
                    Decelerate(horizontalVelocity, 2, 0, false);
                }
            }
            //Walk
            else if (enemyState == EnemyState.WalkLeft || enemyState == EnemyState.WalkRight)
            {
                if (enemyState == EnemyState.WalkLeft)
                {
                    isFacingRight = false;
                }
                else
                {
                    isFacingRight = true;
                }

                Accelerate(horizontalVelocity, 4, 8, false);
            }
            //Jump
            else if (enemyState == EnemyState.JumpLeft || enemyState == EnemyState.JumpRight)
            {
                if (enemyState == EnemyState.JumpLeft)
                {
                    isFacingRight = false;
                }
                else
                {
                    isFacingRight = true;
                }
                //give huge start velocity then transition to fall and allow gravity to create arch
                verticalVelocity = -30;

                canJump = false;
            }
            //Fall
            else if (enemyState == EnemyState.Fall)
            {
                //Gravity
                Accelerate(verticalVelocity, 2, 30, true);

                if (target.X > hitbox.X)
                {
                    //temp used to return isFacingRight to original state
                    bool temp = isFacingRight;
                    isFacingRight = true;
                    Accelerate(horizontalVelocity, 1, 10, false);
                    isFacingRight = temp;
                }
                else if (target.X < hitbox.X)
                {
                    bool temp = isFacingRight;
                    isFacingRight = false;
                    Accelerate(horizontalVelocity, 1, 10, false);
                    isFacingRight = temp;
                }
            }

            X += horizontalVelocity;
            Y += verticalVelocity;
        }

        #endregion Following Movement

        /// <summary>
        /// Use if Enemy has had position drastically changed (IE: Reset)
        /// </summary>
        /// <param name="newPos"></param>
        private void UpdateEnemyVision()
        {
            enemyVision = new Rectangle(X - visionStandard, Y - visionStandard,
                Width + (visionStandard * 2), Height + (visionStandard * 2));
        }

        #endregion Helper Methods

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(defaultSprite, hitbox, Color.White);
        }

        #endregion Methods
    }
}
