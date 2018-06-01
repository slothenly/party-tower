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
        JumpStraight,

        FallLeft,
        FallRight,
        FallStraight,

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
        }

        #endregion Constructor

        #region Methods

        public override void CheckColliderAgainstPlayer(Player p)
        {
            throw new NotImplementedException();
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

            if(enemyVision.Intersects(p1.Hitbox) || enemyVision.Intersects(p2.Hitbox))
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

            if (target.X != hitbox.X && target.Y!= hitbox.Y)
            {

                // Jump Right
                if (target.X > hitbox.X && target.Y < hitbox.Y)
                {
                    enemyState = EnemyState.JumpRight;
                }

                // Jump Left
                else if (target.X < hitbox.X && target.Y < hitbox.Y)
                {
                    enemyState = EnemyState.JumpLeft;
                }

                // Jump Straight Up
                else if (target.X == hitbox.X && target.Y < hitbox.Y)
                {
                    enemyState = EnemyState.JumpStraight;
                }

                // Fall Right
                else if (target.X > hitbox.X && target.Y > hitbox.Y)
                {
                    enemyState = EnemyState.FallRight;
                }

                // Fall Left
                else if (target.X < hitbox.X && target.Y > hitbox.Y)
                {
                    enemyState = EnemyState.FallLeft;
                }

                // Fall Straight Down
                else if (target.X == hitbox.X && target.Y > hitbox.Y)
                {
                    enemyState = EnemyState.FallStraight;
                }

                // Right Only
                else if (target.X > hitbox.X && target.Y == hitbox.Y)
                {
                    enemyState = EnemyState.WalkRight;
                }

                // Left Only
                else 
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
