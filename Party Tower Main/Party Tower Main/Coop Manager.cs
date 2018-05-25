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
        //Fields
        private Player playerOne;
        private Player playerTwo;

        private Vector2 checkpointPosition;

        //

        //Class
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

        /// <summary>
        /// Check if any player is dead, and depending on which one, respawn at the appropriate place
        /// </summary>
        public void CheckAndRespawnPlayer()
        {
            if (playerOne.PlayerState == PlayerState.Die)
            {
                checkpointPosition = GetAlivePlayerPosition(playerTwo);

                //Might want to add some sort of delay here so player doesn't spawn instantly

                playerOne.PlayerSpawn = checkpointPosition; //put dead player at alive player's position
            }
            else if (playerTwo.PlayerState == PlayerState.Die)
            {
                checkpointPosition = GetAlivePlayerPosition(playerOne);

                //Might want to add some sort of delay here so player doesn't spawn instantly

                playerTwo.PlayerSpawn = checkpointPosition; //put dead player at alive player's position
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="onTopPlayer"></param>
        /// <param name="carryingPlayer"></param>
        public void PlayerCarry(Player onTopPlayer, Player carryingPlayer)
        {
            //Only set the state if it hasn't been set already
            if (!onTopPlayer.InCarry)
            {
                onTopPlayer.PlayerState = PlayerState.Carried;
                carryingPlayer.PlayerState = PlayerState.Carrying;
            }
            onTopPlayer.InCarry = true;

            //The onTopPlayer follows the movement of the other player, but is on top
            onTopPlayer.Position =  new Vector2(carryingPlayer.Position.X, carryingPlayer.Position.Y - 75);

        }
    }
}
