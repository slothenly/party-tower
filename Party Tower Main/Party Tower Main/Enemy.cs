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
    /// Subject to Change. Two Core Types = Stationary [Static] & Walking
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
        #endregion Properties

        #region Constructor

        public Enemy()
        {
            hitpoints = 3;
            verticalVelocity = 0;
            horizontalVelocity = 0;
        }

        #endregion Constructor

        #region Methods

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


        public override void CheckColliderAgainstPlayer(Player p)
        {
            throw new NotImplementedException();
        }
        public void CollisionCheck(Tile t)
        {
            //wall collision (collision box next to player depending on direction facing)
            if (sideChecker.Intersects(t.Hitbox))
            {
                #region Default
                if (!t.IsPlatform) //only do wall collision if the tile isn't a platform
                {
                    horizontalVelocity = 0; //stop enemy from moving through wall
                    if (isFacingRight && t.X > hitbox.X) //enemy facing right and tile is to the right of enemy
                    {
                        hitbox.X = t.X - hitbox.Width + 1; //place enemy left of tile
                    }
                    else if (t.X < hitbox.X) //enemy facing left and tile is to the left of enemy
                    {
                        hitbox.X = t.X + t.Hitbox.Width - 1; //place enemy right of tile
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

                #endregion
            }
            //ceiling collision (collision box above enemy)
            if (topChecker.Intersects(t.Hitbox))
            {
                #region Default
                if (topIntersects) //this is used to ensure enemy is placed at the ceiling only once per jump
                {
                    //only do ceiling collision if the tile isn't a platform
                    if (!t.IsPlatform)
                    {
                        hitbox.Y = t.Y + t.Hitbox.Height; //place enemy at ceiling (illusion of hitting it)
                    }
                }
                //only launch the enemy downwards if the tile isn't a platform
                if (!t.IsPlatform)
                {
                    topIntersects = false; //set to false so that the enemy isn't placed to the ceiling again until they touch the ground
                    verticalVelocity = (int)(Math.Abs(verticalVelocity) * .75); //launch the enemy downwards
                }

                #endregion
            }
            //floor collision (collision box below enemy)
            else if (bottomChecker.Intersects(t.Hitbox) && goingDown) //only want ground collision to happen if the enemy is going downwards, 
                                                                      //otherwise the enemy should just not check, so that way the enemy can jump through from below 
                                                                      //but not from above
            {
                #region Default

                hitbox.Y = t.Y - hitbox.Height - 1; //place the enemy on top of tile
                bottomIntersects = true;

                //everytime the enemy lands from a jump (or falls), the next time they jump they will hit the ceiling
                topIntersects = true;
                #endregion
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
        /// <summary>
        /// Updates the Enemy Appropriately
        /// </summary>
        /// <param name="pM"></param>
        public void UpdateEnemy(Player p1, Player p2, Vector2 target)
        {
            previousEnemyState = enemyState;
            previousWalkingState = walkingState;

            // Determines if the enemy should be following the player

            if (enemyVision.Intersects(p1.Hitbox) || enemyVision.Intersects(p2.Hitbox))
            {
                walkingState = EnemyWalkingState.Follow;
            }
            else
            {
                walkingState = EnemyWalkingState.Waiting;
            }

            // Determines Enemy Logic based on current walking state

            if (walkingState == EnemyWalkingState.Follow)
            {
                FiniteStateFollowing(target);
            }

            // If enemy was following and now is not, needs logic to finish it's type of movement and wait. 
            // IE: Was jumping and following the player last frame. Is not following the player this frame.
            // Therefore, the enemy needs to complete its jump and relax.

            else if (previousWalkingState == EnemyWalkingState.Follow)
            {

            }

        }

        /// <summary>
        /// Determines the new Finite State of the enemy.
        /// </summary>
        /// <param name="target"></param>
        public void FiniteStateFollowing(Vector2 target)
        {
            //this isn't in exact same spot as target
            if (target.X != hitbox.X && target.Y != hitbox.Y)
            {
                //target is above this
                if (target.Y < hitbox.Y)
                {
                    //target is right of or directly above this
                    if (target.X >= hitbox.X)
                    {
                        enemyState = EnemyState.JumpRight;
                    }
                    //target is left of this
                    else
                    {
                        enemyState = EnemyState.JumpLeft;
                    }
                }

                //not touching a tile from above
                if (!bottomIntersects)
                {
                    enemyState = EnemyState.Fall;
                }

                //target is directly right of player
                if (target.X > hitbox.X && target.Y == hitbox.Y)
                {
                    enemyState = EnemyState.WalkRight;
                }
                else //target is directly left of player
                {
                    enemyState = EnemyState.WalkLeft;
                }

                FollowMovementLogic(target);

            }

            //Target and Enemy are already at same Position
            else
            {
                enemyState = previousEnemyState;
            }
        }

        private void FollowMovementLogic(Vector2 target)
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
                    isFacingRight = false;
                    Accelerate(horizontalVelocity, 1, 10, false);
                    isFacingRight = temp;
                }
                else if (target.X < hitbox.X)
                {
                    bool temp = isFacingRight;
                    isFacingRight = true;
                    Accelerate(horizontalVelocity, 1, 10, false);
                    isFacingRight = temp;
                }
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            if (isDrawn == true && enemyState != EnemyState.Die)
            {
                sb.Draw(defaultSprite, hitbox, Color.White);
            }

        }

        /// <summary>
        /// Use if Enemy has had position drastically changed (IE: Reset)
        /// </summary>
        /// <param name="newPos"></param>
        private void NewEnemyVision()
        {
            enemyVision = new Rectangle(X - visionStandard, Y - visionStandard,
                Width + (visionStandard * 2), Height + (visionStandard * 2));
        }

        public void EnemyHardReset()
        {
            hitpoints = 3;
            enemyState = EnemyState.IdleRight;
            hitbox.Location = enemySpawn;
            NewEnemyVision();
        }

        #endregion Methods
    }
}
