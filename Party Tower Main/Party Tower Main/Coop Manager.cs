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
        /// Check if any player is dead, and depending on which one, respawn at the appropriate place, this will return true unless both players are dead
        /// </summary>
        public bool CheckAndRespawnPlayer(GameTime gameTime)
        {
            if (playerOne.PlayerState == PlayerState.Die)
            {

                //Might want to add some sort of delay here so player doesn't spawn instantly
                //Added delay using timer
                if (playerOne.RespawnTimer.UpdateTimer(gameTime) == true) //the 3 second timer has finished
                {
                    checkpointPosition = GetAlivePlayerPosition(playerTwo);
                    playerOne.PlayerSpawn = checkpointPosition; //put dead player at alive player's position
                    playerOne.PlayerState = PlayerState.IdleRight;
                    return true;
                }
                else if (playerTwo.PlayerState == PlayerState.Die) //both players are dead
                {
                    return false;
                }
            }
            else if (playerTwo.PlayerState == PlayerState.Die)
            {
                //Might want to add some sort of delay here so player doesn't spawn instantly
                //Added delay using timer
                if (playerTwo.RespawnTimer.UpdateTimer(gameTime) == true) //the 3 second timer has finished
                {
                    checkpointPosition = GetAlivePlayerPosition(playerOne);
                    playerTwo.PlayerSpawn = checkpointPosition; //put dead player at alive player's position
                    playerTwo.PlayerState = PlayerState.IdleRight;
                }
                else if (playerOne.PlayerState == PlayerState.Die) //both players are dead
                {
                    return false;
                }
            }
            return true;
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

        /// <summary>
        /// Check whether or not the player should be thrown based on collision, then actually throw them using a helper method
        /// </summary>
        public void CheckForThrowAndThenThrow()
        {
            if (playerOne.PlayerState == PlayerState.Throw && playerTwo.PlayerState != PlayerState.Throw) //If one player is trying to throw the other
            {
                if (playerOne.BeingTouchedByOtherPlayer(playerTwo))
                {
                    PlayerThrow(playerOne, playerTwo);
                }
            }
            if (playerOne.PlayerState != PlayerState.Throw && playerTwo.PlayerState == PlayerState.Throw) //switch roles for throw / thrown
            {
                if (playerTwo.BeingTouchedByOtherPlayer(playerOne))
                {
                    PlayerThrow(playerTwo, playerOne);
                }
            }
        }
        /// <summary>
        /// Throw the actual player by making them bounce in the appropriate direction
        /// </summary>
        /// <param name="throwingPlayer"></param>
        /// <param name="thrownPlayer"></param>
        public void PlayerThrow(Player throwingPlayer, Player thrownPlayer)
        {
            if (throwingPlayer.IsFacingRight) //bounce the player based on which direction the thrower is facing
            {
                thrownPlayer.PlayerState = PlayerState.BounceRight;
            }
            else
            {
                thrownPlayer.PlayerState = PlayerState.BounceLeft;
            }
        }
    }
}
