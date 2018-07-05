using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Party_Tower_Main
{
    class Cake : GameObject
    {
        //Fields
        private Vector2 spawnLocation;
        private bool carried;
        private bool shouldStop;
        private bool dropped;
        private Rectangle bottomChecker;
        private int verticalVelocity = 0;
        private Timer respawnTimer;

        //properties
        public Vector2 SpawnLocation
        {
            get { return spawnLocation; }
            set { spawnLocation = value; }
        }
        public bool Carried
        {
            get { return carried; }
            set { carried = value; }
        }
        public bool Dropped
        {
            get { return dropped; }
            set { dropped = value; }
        }
        public bool ShouldStop
        {
            get { return shouldStop; }
            set { shouldStop = value; }
        }
        public Timer RespawnTimer
        {
            get { return respawnTimer; }
            set { respawnTimer = value; }
        }

        //Constructor
        public Cake(int x, int y, Texture2D defaultTexture)
        {
            spawnLocation = new Vector2(x, y);
            hitbox = new Rectangle(x, y, 25, 25);

            carried = false;
            shouldStop = true;
            dropped = false;

            //adjust this to change cake respawn delay
            respawnTimer = new Timer(2);

            DefaultSprite = defaultTexture;
        }

        //methods
        /// <summary>
        /// simply place the cake back at its original spawn point
        /// </summary>
        public void Respawn()
        {
            X = (int)spawnLocation.X;
            Y = (int)spawnLocation.Y;
        }

        /// <summary>
        /// checks collision with tiles to make sure cake doesn't go through them when falling
        /// </summary>
        /// <param name="t"></param>
        public void CheckTileCollision(Tile t)
        {
            //instantiate the checker based 
            bottomChecker = new Rectangle(X + 2, Y + hitbox.Height, hitbox.Width - 4, Math.Abs(verticalVelocity));

            if (t != null)
            {
                //cake touches a tile
                if (bottomChecker.Intersects(t.Hitbox) && !carried)
                {
                    hitbox.Y = t.Y - hitbox.Height - 1; //place the cake on top of tile
                    shouldStop = true;
                }
            }
        }
        /// <summary>
        /// method called every frame that will check whether or not the cake should fall, and will make the cake fall if it should
        /// </summary>
        public void Movement()
        { 
            if (!carried) //if the cake isn't being carried, emulate gravity
            {
                if (shouldStop)
                {
                    verticalVelocity = 0; //stop the cake
                }
                else //cake is falling
                {
                    //accelerate the cake
                    Accelerate(2, 30);
                }
                Y += verticalVelocity; //adjust the actual value of the cake's vertical position
            }
        }

        /// <summary>
        /// speed up the object by the rate until the limit velocity is reached
        /// </summary>
        /// <param name="velocityType"></param>
        /// <param name="rate"></param>
        /// <param name="limit"></param>
        public void Accelerate(int rate, int limit)
        {
            if (verticalVelocity < limit)
            {
                verticalVelocity += rate;
            }
        }

        /// <summary>
        /// don't call this, silly
        /// </summary>
        /// <param name="p"></param>
        public override void CheckColliderAgainstPlayer(Player p)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// draws the cake
        /// </summary>
        /// <param name="sb"></param>
        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(defaultSprite, hitbox, Color.White);
        }
    }
}
