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
        Idle,               // Enemy is doing nothing     
        Follow              // Enemy is using A* to walk toward the player
    }
    enum EnemyState
    {
        IdleLeft,
        IdleRight,

        WalkLeft,
        WalkRight,

        Fall,
        Die
    }
    enum EnemyType
    {
        Stationary,
        Walking
    }
    class Enemy : GameObject
    {

        #region Fields

        // Stuff for enemy pathing
        private Rectangle enemyVision;
        private int visionStandard;

        // In the instance enemy's respawn or level restarts if both players die at once
        private Point enemySpawn;

        //Determines if enemy needs drawn
        private bool enemyVisible = false;

        //Determines if player and enemy collided.
        private bool hurtPlayer = false;

        //Directionality and FSM
        private bool isFacingRight;
        private EnemyState enemyState;
        private EnemyState previousEnemyState;
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
        public void UpdateEnemy()
        {
            previousEnemyState = enemyState;
        }

        /// <summary>
        /// Used to determine where Walking Enemy should move to. 
        /// </summary>
        /// <param name="target"></param>
        private void GoToPoint(Vector2 target)
        {

        }


        public override void Draw(SpriteBatch sb)
        {
            if (hitpoints > 0)
            {
                sb.Draw(defaultSprite, hitbox, Color.White);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        public void FiniteState()
        {
            //FILL UP WITH CODE FROM PLAYER LIKE ACCELRATE() AND MOVEMENT()
        }

        public void IsEnemyVisible(Rectangle visibleArea)
        {
            if (hitbox.Intersects(visibleArea))
            {
                enemyVisible = true;
            }
            else
            {
                enemyVisible = false;
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
