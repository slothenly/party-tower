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
        Roaming,            // Enemy is randomly walking about
        PreFollow,          // Enemy is following a pre-Set path
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
    class Enemy : GameObject
    {
        // Stuff for enemy pathing
        private Rectangle enemyVision;
        private int visionStandard;

        // In the instance enemy's respawn or level restarts if both players die at once
        private Vector2 enemySpawn; 

        //Determines if enemy needs drawn
        private bool enemyVisible = false;

        //Directionality and FSM
        private bool isFacingRight;
        private EnemyState enemyState;
        private EnemyState previousEnemyState;

        //Sound
        public static ContentManager myContent; //used to load content in non-Game1 Class
        SoundEffect deathSound;
        SoundEffect followSound;
        SoundEffect fallSound;
        SoundEffect jumpSound;
        SoundEffect walkSound;

        public override void CheckColliderAgainstPlayer(Player p)
        {
            throw new NotImplementedException();
        }


        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(defaultSprite, hitbox, Color.White);
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
        /// Dummy method to update enemy vision. May not need. We'll see
        /// </summary>
        /// <param name="newPos"></param>
        private void NewEnemyVision(Vector2 newPos)
        {
            enemyVision = new Rectangle(X - visionStandard, Y - visionStandard,
                Width + (visionStandard * 2), Height + (visionStandard * 2));
        }
    }
}
