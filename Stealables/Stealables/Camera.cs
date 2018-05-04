using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LumberjackFantasy
{
    class Camera
    {
        ///summary
        ///Jacob LeBerth
        ///Camera object holds two rectangles, one is screen sized, one is slightly larger.  
        ///Its location is based on the players location, by taking the players centerpoint and positioning it so it is in the center 
        ///or at the furthest point along if the player is near the edge.
        ///Retruns bools for objects to see if they need to be drawn or need to be updated.
        ///Gameobject uses the position of the camera box to determine where to draw objects

        //fields ------------------------------------------------------------------------
        private Rectangle onScreen; //rectangle hitbox to determine if something needs to be drawn
        private Rectangle upScreen; //rectangle hitbox to determine if something needs to be updated
		private Rectangle coScreen; //rectangle hitbox to determine if trees need to run collisions
        private int visionStandard; //offset integer for how large the upScreen should be
		private int bearMax; //value based on bear speed to determine collisionScreen
		private Texture2D camTexture;

        //properties --------------------------------------------------------------------
		public int BearMax {get {return bearMax;} set {bearMax = value;} }
        public Rectangle CameraPosition { get { return onScreen; } }
		public Texture2D CamTexture { get { return camTexture; } set { camTexture = value; } }
        //constructor -------------------------------------------------------------------
        public Camera(int standard, Texture2D texture)
        {
			camTexture = texture;
            visionStandard = standard;
			bearMax = 0;
            onScreen = new Rectangle(0, 0, 896, 896);
            upScreen = new Rectangle(0 - visionStandard, 0 - visionStandard, 896 + (2 * visionStandard), 896 + (2 * visionStandard));
			coScreen = new Rectangle(upScreen.X - bearMax, upScreen.Y - bearMax, upScreen.Width + (2 * bearMax), upScreen.Height + (2 * bearMax));
        }
        //methods -----------------------------------------------------------------------
        public bool IsDrawn(Rectangle position) //method to check if collides with upScreen
        {
			if (onScreen.Intersects(position))
            {
                return true;
            }        
                return false;           
        }

        public bool IsUpdating(Rectangle position) //method to check if collides with upScreen
        {
            if(upScreen.Intersects(position))
            {
                return true;
            }
            return false;
        }

		public bool IsColliding(Rectangle position) //method to check if trees collide with coScreen
		{
			if (coScreen.Intersects(position))
				{
				return true;
				}
			return false;
		}

        public void UpdatePosition(Rectangle player)//updates the cameras position
        {

			//sets onscreen location based on player
			onScreen = new Rectangle(player.Center.X - 448, player.Center.Y - 448, 896, 896);
            

            //prevents onscreen from leaving the game map
            if (onScreen.X < 0)
            {
				onScreen = new Rectangle(0, onScreen.Y, 896, 896);
			}
            if (onScreen.X > 896)
            {
				onScreen = new Rectangle(896, onScreen.Y, 896, 896);
			}
            if (onScreen.Y < 0)
            {
				onScreen = new Rectangle(onScreen.X, 0, 896, 896);
			}
            if (onScreen.Y > 896)
            {
				onScreen = new Rectangle(onScreen.X , 896, 896, 896);
			}

            //places the update hitbox
            upScreen = new Rectangle(onScreen.X - visionStandard, onScreen.Y - visionStandard, 896 + (2 * visionStandard), 896 + (2 * visionStandard));

			//places the coScreen hitbox
			coScreen = new Rectangle(upScreen.X - bearMax, upScreen.Y - bearMax, upScreen.Width + (2 * bearMax), upScreen.Height + (2 * bearMax));

		}

		public void DrawCam(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(camTexture, onScreen, Color.White);
		}

    }
}
