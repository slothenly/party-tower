using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Egg
{
    abstract class GameObject
    {
        //fields
        protected Rectangle hitbox;
        protected Texture2D defaultSprite;
        protected int drawLevel;
        protected bool isActive;
        protected bool hasGravity;

        /// <summary>
        /// The x value of the object's hitbox
        /// </summary>
        public int X
        {
            get { return hitbox.X; }
            set { hitbox.X = value; }
        }

        /// <summary>
        /// The x value of the object's hitbox
        /// </summary>
        public int Y
        {
            get { return hitbox.Y; }
            set { hitbox.Y = value; }
        }

        public int Width
        {
            get { return hitbox.Width; }
            set { hitbox.Width = value; }
        }

        public int Height
        {
            get { return hitbox.Height; }
            set { hitbox.Height = value; }
        }
        /// <summary>
        /// The order/layer the sprite should be drawn on screen. The larger the DrawLevel, the closer the object is to the front of the screen 
        /// </summary>
        public int DrawLevel
        {
            get { return this.drawLevel; }
            set { this.drawLevel = value; }
        }

        /// <summary>
        /// The default sprite if the object has no other sprites to draw
        /// </summary>
        public Texture2D DefaultSprite
        {
            get { return this.defaultSprite; }
            set { defaultSprite = value; }
        }

        /// <summary>
        /// The rectangle of the GameObject
        /// </summary>
        public virtual Rectangle Hitbox
        {
            get { return this.hitbox; }
            set { this.hitbox = value; }
        }

        /// <summary>
        /// Returns true if the object is active.
        /// </summary>
        public bool IsActive
        {
            get { return this.isActive; }
            set { this.isActive = value; }
        } 

        /// <summary>
        /// Returns true if the object is pulled downward by gravity.
        /// </summary>
        public bool HasGravity
        {
            get { return hasGravity; }
            set { hasGravity = value; }
        }

        /// <summary>
        /// Tests for a collision between the object and an enemy
        /// </summary>
        /// <param name="e">The enemy to check collision against</param>
        public abstract void CheckColliderAgainstEnemy(Enemy e);
        /// <summary>
        /// Tests for a collision between the object and a player
        /// </summary>
        /// <param name="p">The player to check collision against</param>
        public abstract void CheckColliderAgainstPlayer(Player p);
        /// <summary>
        /// Draws the object to the screen
        /// </summary>
        /// <param name="sb">The SpriteBatch object used to draw the object (be sure to Open() & Close()!!!)</param>
        public abstract void Draw(SpriteBatch sb);

        /// <summary>
        /// Triggers an object to update its finite state machine, if applicable
        /// </summary>
        public abstract void FiniteState();
        /// <summary>
        /// Triggers an object to run its movement logic
        /// </summary>
        public abstract void Movement();



    }
}
