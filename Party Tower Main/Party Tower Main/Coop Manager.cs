using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
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

        //sound
        private SoundEffect throwSound;
        private SoundEffect respawnSound;
        private SoundEffect carrySound;

        //Class
        public Coop_Manager(Player playerOne, Player playerTwo, ContentManager content)
        {
            this.playerOne = playerOne;
            this.playerTwo = playerTwo;

            throwSound = content.Load<SoundEffect>("sound/throw");
            respawnSound = content.Load<SoundEffect>("sound/respawn");
            carrySound = content.Load<SoundEffect>("sound/carry");
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
                //stop carry if a player dies
                if (playerTwo.PlayerState == PlayerState.Carried || playerTwo.PlayerState == PlayerState.Carrying)
                {
                    playerTwo.PlayerState = PlayerState.Fall;
                }
                //Might want to add some sort of delay here so player doesn't spawn instantly
                //Added delay using timer
                if (playerOne.RespawnTimer.UpdateTimer(gameTime) == true) //the 3 second timer has finished
                {
                    respawnSound.Play();
                    playerOne.Position = GetAlivePlayerPosition(playerTwo); //put dead player at alive player's position
                    playerOne.PlayerState = PlayerState.IdleRight;
                    playerOne.X = (int)playerOne.Position.X;
                    playerOne.Y = (int)playerOne.Position.Y;
                    playerOne.HorizontalVelocity = 0;
                    playerOne.VerticalVelocity = 0;

                    return true;
                }
                else if (playerTwo.PlayerState == PlayerState.Die) //both players are dead
                {
                    return false;
                }
            }
            else if (playerTwo.PlayerState == PlayerState.Die)
            {
                //stop carry if a player dies
                if (playerOne.PlayerState == PlayerState.Carried || playerOne.PlayerState == PlayerState.Carrying)
                {
                    playerOne.PlayerState = PlayerState.Fall;
                }
                //Might want to add some sort of delay here so player doesn't spawn instantly
                //Added delay using timer
                if (playerTwo.RespawnTimer.UpdateTimer(gameTime) == true) //the 3 second timer has finished
                {
                    respawnSound.Play();
                    playerTwo.Position = GetAlivePlayerPosition(playerOne); //put dead player at alive player's position
                    playerTwo.PlayerState = PlayerState.IdleRight;
                    playerTwo.X = (int)playerTwo.Position.X;
                    playerTwo.Y = (int)playerTwo.Position.Y;
                    playerTwo.HorizontalVelocity = 0;
                    playerTwo.HorizontalVelocity = 0;
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
                carrySound.Play();
                onTopPlayer.PlayerState = PlayerState.Carried;
                carryingPlayer.PlayerState = PlayerState.Carrying;
            }
            onTopPlayer.InCarry = true;

            //The onTopPlayer follows the movement of the other player, but is on top
            onTopPlayer.X = (int)carryingPlayer.Position.X + carryingPlayer.HorizontalVelocity * 2; //add on velocity to avoid sluggish delay when carrying player is moving
            onTopPlayer.Y = (int)carryingPlayer.Position.Y - 75 + carryingPlayer.VerticalVelocity * 1;

        }

        /// <summary>
        /// Check if a player is rolling into another, then bounce accordinly
        /// </summary>
        public void CheckForRollAndThenBounce()
        {
            //player one is rolling
            if (playerOne.PlayerState == PlayerState.RollLeft || playerOne.PlayerState == PlayerState.RollRight)
            {
                //player one hits player two
                if (playerOne.Hitbox.Intersects(playerTwo.Hitbox))
                {
                    playerOne.BounceSound.Play();
                    //determine correct bounce based on direction
                    if (playerOne.PlayerState == PlayerState.RollRight)
                    {
                        playerOne.PlayerState = PlayerState.BounceLeft;
                    }
                    else
                    {
                        playerOne.PlayerState = PlayerState.BounceRight;
                    }
                }
            }
            //player two is rolling
            else if (playerTwo.PlayerState == PlayerState.RollLeft || playerTwo.PlayerState == PlayerState.RollRight)
            {
                //player two hits player one
                if (playerTwo.Hitbox.Intersects(playerOne.Hitbox))
                {
                    playerTwo.BounceSound.Play();
                    //determine correct bounce based on direction
                    if (playerTwo.PlayerState == PlayerState.RollRight)
                    {
                        playerTwo.PlayerState = PlayerState.BounceLeft;
                    }
                    else
                    {
                        playerTwo.PlayerState = PlayerState.BounceRight;
                    }
                }
            }
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
            if (thrownPlayer.PreviousPlayerState != PlayerState.BounceLeft && thrownPlayer.PreviousPlayerState != PlayerState.BounceRight)
            {
                throwSound.Play();
            }

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
