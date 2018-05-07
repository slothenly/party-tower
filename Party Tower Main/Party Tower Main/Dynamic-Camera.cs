using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Party_Tower_Main
{
    class Dynamic_Camera
    {
        //#############################################################################################
        #region Fields
        private Texture2D cameraTexture;    // Texture to test and make sure the camera is always functional
        private Rectangle cameraScreen;     // A Rectangle that is always the size of the selcted screen
        private int resolutionX;            // X Resolution Value of the Screen
        private int resolutionY;            // Y Resoultion Value of the Screen
        private int mapSizeX;               // Total Width of the Game Map 
        private int mapSizeY;               // Total Height of the Game Map 
        Vector2 cameraCenter;               // Center of the Camera
        #endregion Fields
        //#############################################################################################
        #region Properties
        public Rectangle CameraPosition { get { return cameraScreen; } }
        public Texture2D CamTexture { get { return cameraTexture; } set { cameraTexture = value; } }
        #endregion Properties
        //#############################################################################################
        #region Constructor
        public Dynamic_Camera(int startingResolutionX, int startingResolutionY, Texture2D texture)
        {
            cameraTexture = texture;
            resolutionX = startingResolutionX;
            resolutionY = startingResolutionY;
            cameraScreen = new Rectangle(0, 0, startingResolutionX, startingResolutionY);
        }
        #endregion Constructor
        //#############################################################################################
        #region Methods
        public bool IsDrawn(Rectangle position) 
        {
            if (cameraScreen.Intersects(position))
            {
                return true;
            }
            return false;
        }

        public void ChangeCameraResolution(int resolutionX, int resolutionY)
        {
            this.resolutionX = resolutionX;
            this.resolutionY = resolutionY;
        }

        /// <summary>
        /// Updates the Camera based on the positions of two players
        /// </summary>
        /// <param name="player1"> The Rectangle of the first player used to determine positioning </param>
        /// <param name="player2"> The Rectangle of the second player used to determine positioning </param>
        public void UpdatePosition(Rectangle player1, Rectangle player2)//updates the cameras position
        {

            /*
             * Internally, The camera takes the positions of the two players and determines a mid point between them. 
             * From that midpoint, the new camera is drawn. 
             *
             * If the X or Y between the midpoints of the players is greater than that of which is allowed on the camera, 
             * then the camera determines a scalable variable to externally draw things smaller to the screen. 
             *
             * If the camera tries to scale things smaller than its scalableMax value permits it to, then the camera remains in its original place. 
             *
             */

            PreventCameraGoingOffMap();

        }

        /// <summary>
        /// Updates the Camera based on a singular player playing the game
        /// </summary>
        /// <param name="player1">The rectangle of the player that is used to state the Player's position</param>
        public void UpdatePosition(Rectangle player1)
        {
            /*
             * Determines the Camera based on the midpoint of the singular player
             * Likely to be used to effectively make sure our camera is at least working.
             * 
             * Prevent Camera Going Off Map Should Always be done last.
             */
            cameraCenter = new Vector2(player1.X, player1.Y);
            cameraScreen = new Rectangle((int)cameraCenter.X - (resolutionX / 2), (int)cameraCenter.Y - (resolutionY / 2), resolutionX, resolutionY);
            PreventCameraGoingOffMap();
        }

        /// <summary>
        /// Determines if the newly drawn camera will draw off any boundry of the map. If it does, it resets the camera. 
        /// </summary>
        private void PreventCameraGoingOffMap()
        {
            if (cameraScreen.X < 0)
            {
                cameraScreen = new Rectangle(0, cameraScreen.Y, cameraScreen.Width, cameraScreen.Height);
            }
            if (cameraScreen.X + cameraScreen.Width > mapSizeX)
            {
                cameraScreen = new Rectangle(mapSizeX - cameraScreen.Width, cameraScreen.Y, cameraScreen.Width, cameraScreen.Height);
            }
            if (cameraScreen.Y < 0)
            {
                cameraScreen = new Rectangle(cameraScreen.Y, 0, 896, 896);
            }
            if (cameraScreen.Y + cameraScreen.Height > mapSizeY)
            {
                cameraScreen = new Rectangle(cameraScreen.Y- cameraScreen.Height, mapSizeY, cameraScreen.Width, cameraScreen.Height);
            }
        }

        public void DrawCam(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(cameraTexture, cameraScreen, Color.White);
        }
        #endregion Methods
    }
}