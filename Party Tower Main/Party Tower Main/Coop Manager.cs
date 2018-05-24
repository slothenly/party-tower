using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Party_Tower_Main
{
    /// <summary>
    /// Used to manage the coop interactions between the two players
    /// </summary>
    class Coop_Manager
    {
        private Player playerOne;
        private Player playerTwo;

        public Coop_Manager(Player playerOne, Player playerTwo)
        {
            this.playerOne = playerOne;
            this.playerTwo = playerTwo;
        }

        /// <summary>
        /// Gets the position of the alive player, assign this to the Vector2 to be used as the checkpoint
        /// </summary>
        /// <param name="deadPlayer"></param>
        /// <param name="alivePlayer"></param>
        /// <returns></returns>
        public Vector2 GetAlivePlayerPosition(Player alivePlayer)
        {
            return new Vector2(alivePlayer.X, alivePlayer.Y);
        }
    }
}
