using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace Party_Tower_Main
{
    /// <summary>
    /// Depicts the current movement of the enemy
    /// </summary>
    enum EnemyWalkingState
    {
        idle,               // Enemy is doing nothing     
        roaming,            // Enemy is randomly walking about
        preFollow,          // Enemy is following a pre-Set path
        follow              // Enemy is using A* to walk toward the player
    }
    enum CharacterState
    {

    }
    class Enemy : GameObject
    {
        //Fill up as we develop

        public override void CheckColliderAgainstPlayer(Player p)
        {
            throw new NotImplementedException();
        }

        public override void Draw(SpriteBatch sb)
        {
            throw new NotImplementedException();
        }
    }
}
