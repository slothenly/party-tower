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
    class CakeManager
    {
        //fields
        private List<Player> players;
        private Cake cake;
        private Rectangle puttingDownChecker;
        private bool cakeBlockedByTile;
        private Table table;

        SoundEffect errorSound;
        SoundEffect cakePickupSound;
        SoundEffect cakeSound;

        //Constructor
        public CakeManager(List<Player> players, Cake cake,  ContentManager content, Table table)
        {
            this.cake = cake;
            this.players = players;
            this.table = table;
            puttingDownChecker = cake.Hitbox;
            cakeBlockedByTile = false;

            errorSound = content.Load<SoundEffect>("sound/cakeError");
            cakePickupSound = content.Load<SoundEffect>("sound/cakePickup");
            cakeSound = content.Load<SoundEffect>("sound/cake");
        }

        //Properties
        public Rectangle PuttingDownChecker
        {
            get { return puttingDownChecker; }
            set { puttingDownChecker = value; }
        }
        public bool CakeBlockedByTile
        {
            get { return cakeBlockedByTile; }
            set { cakeBlockedByTile = value; }
        }

        //Methods
        /// <summary>
        /// Checks if a player is supposed to pickup the cake, then adjusts the player's state accordingly
        /// </summary>
        public void CheckCakePickUp(GameTime gameTime)
        {
            //the cake was dropped
            if (cake.Dropped)
            {
                //once the timer has finished
                if (cake.RespawnTimer.UpdateTimer(gameTime))
                {
                    //respawn/reset the cake
                    cake.Respawn();
                    cake.Dropped = false;
                    errorSound.Play();
                }
            }
            //every player can carry the cake
            foreach (Player currentPlayer in players)
            {
                //player touches the cake if no player is already carrying it and the player isn't dead, and the cake isn't on the table already
                if (currentPlayer.Hitbox.Intersects(cake.Hitbox) && !cake.Carried &&  currentPlayer.PlayerState != PlayerState.Die && !table.CakePlacedOnTable)
                {
                    //set the cake to being carried and is now being carried by a player
                    cake.Carried = true;
                    cake.Dropped = false;
                    currentPlayer.PlayerState = PlayerState.CakeCarrying;

                    cakePickupSound.Play();
                    break;
                }
            }
        }
        /// <summary>
        /// Positions the cake on top of the player (like they're carrying it)
        /// </summary>
        public void CakeCarry(Player carryingPlayer)
        {
            //put the cake above the player (like they're carrying it)
            cake.X = carryingPlayer.X + (carryingPlayer.Width / 2) - (cake.Width / 2); //add on velocity to avoid sluggish delay when carrying player is moving
            cake.Y = carryingPlayer.Y - cake.Height;
        }

        public void DropCake(Player carryingPlayer,bool tryingToDrop)
        {
            //drop the cake directly below (player dying)
            if (!tryingToDrop)
            {
                cakeSound.Play();
                cake.Carried = false;
                cake.Dropped = true;
                cake.ShouldStop = false;
                cake.RespawnTimer.ResetTimer();
                carryingPlayer.CakeCarrying = false;

                //cake middle is the middle of the player who died
                cake.X = (int)((carryingPlayer.Position.X + carryingPlayer.Width / 2) - (cake.Width / 2));
            }
            else //drop the cake next to the player
            {
                //trying to put down the cake on a table
                if (puttingDownChecker.Intersects(table.Hitbox))
                {
                    cakeSound.Play();
                    cake.Carried = false;

                    //center the cake on top of the table
                    cake.X = (table.X + (table.Width / 2)) - (cake.Width / 2);
                    cake.Y = table.Y - cake.Height;

                    carryingPlayer.CakeCarrying = false;

                    table.CakePlacedOnTable = true;
                }
                else if (carryingPlayer.IsFacingRight) //place cake right of player
                {

                    if (!cakeBlockedByTile)
                    {
                        cakeSound.Play();
                        cake.Carried = false;
                        cake.Dropped = true;
                        cake.ShouldStop = false;
                        cake.RespawnTimer.ResetTimer();


                        cake.X = carryingPlayer.X + (int)(carryingPlayer.Width * 1.6);
                        carryingPlayer.CakeCarrying = false;
                    }
                    else
                    {
                        errorSound.Play();
                    }
                }
                else //place cake left of player
                {
                    if (!cakeBlockedByTile)
                    {
                        cakeSound.Play();
                        cake.Carried = false;
                        cake.Dropped = true;
                        cake.ShouldStop = false;
                        cake.RespawnTimer.ResetTimer();


                        cake.X = cake.X - (int)(cake.Width * 1.3);
                        carryingPlayer.CakeCarrying = false;
                    }
                    else
                    {
                        errorSound.Play();
                    }
                }
            }
        }
    }
}
