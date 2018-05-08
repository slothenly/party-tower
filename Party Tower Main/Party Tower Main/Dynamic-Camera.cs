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

        private Matrix transform;                   // Transform matrix
        private Vector2 cameraCenter;               // Center of the Camera
        private GraphicsDevice resolution;          // Resolution of the Current Chosen Screen

        private float zoom;                         // Zoom of Camera
        private float maxZoom;                      // Maximum Zoom in of Camera
        private float minZoom;                      // Minimum Zoom out of Camera

        #endregion Fields
        //#############################################################################################
        #region Properties

        public Matrix Transform { get { return transform; } }

        #endregion Properties
        //#############################################################################################
        #region Constructor
        public Dynamic_Camera(GraphicsDevice gD)
        {
            resolution = gD;
            zoom = 1.0f;
            minZoom = .5f;
            maxZoom = 2f;
            cameraCenter = Vector2.Zero;
        }
        #endregion Constructor
        //#############################################################################################
        #region Methods
        
        public void UpdateGraphicsDevice(GraphicsDevice newGraphics)
        {
            resolution = newGraphics;
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
            Vector2 p1 = player1.Center.ToVector2();
            Vector2 p2 = player2.Center.ToVector2();
            cameraCenter = (p1 + p2) * .5f;
            DetermineZoom(p1, p2);
            ZoomCheck();

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

            Vector2 p1Vector = new Vector2(player1.X, player1.Y);
            cameraCenter = p1Vector;
            PreventCameraGoingOffMap();
            TransformMatrix();
        }

        private void DetermineZoom(Vector2 p1, Vector2 p2)
        {
            
            
        }

        private void ZoomCheck()
        {

        }

        /// <summary>
        /// Determines if the newly drawn camera will draw off any boundry of the map. If it does, it resets the camera. 
        /// </summary>
        private void PreventCameraGoingOffMap()
        {

        }

        private void TransformMatrix()
        {
            transform = Matrix.CreateTranslation(new Vector3(-cameraCenter.X, -cameraCenter.Y, 0)) *
                               Matrix.CreateScale(new Vector3(zoom, zoom, 1)) *
                               Matrix.CreateTranslation(new Vector3(resolution.Viewport.Width * 0.5f, resolution.Viewport.Height * 0.5f, 0));
        }

        #endregion Methods
    }
}