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
        private Rectangle resolutionBounds;         // Resolution of the Current Chosen Screen
        private Rectangle visibleArea;              // Actual Visible Area of the screen as a Rectangle
        private Texture2D visibleTexture;           // Testable Textures used to make sure the visibleArea is actually the Visible Area
        private float zoom;                         // Zoom of Camera
        private float maxZoom;                      // Maximum Zoom in of Camera
        private float minZoom;                      // Minimum Zoom out of Camera

        #endregion Fields
        //#############################################################################################
        #region Properties

        public Matrix Transform { get { return transform; } }
        public Texture2D VisibleTexture { set { visibleTexture = value; } }

        #endregion Properties
        //#############################################################################################
        #region Constructor

       /// <summary>
       /// Creates the Camera class
       /// </summary>
       /// <param name="view"></param>
        public Dynamic_Camera(Viewport view)
        {
            resolutionBounds = view.Bounds;
            zoom = 1.0f;
            minZoom = .5f;
            maxZoom = 2f;
            cameraCenter = Vector2.Zero;
        }
        #endregion Constructor
        //#############################################################################################
        #region Methods
        /// <summary>
        /// Updates the Camera based on the singular position of a single player in the game
        /// </summary>
        /// <param name="bounds"> The bounds of the screen the camera must remain in </param>
        /// <param name="player1"> The Rectangle of the first player used to determine positioning </param>
        public void UpdateCamera(Viewport bounds, Rectangle player1)
        {
            /*
             * Determines the Camera based on the midpoint of the singular player
             * Likely to be used to effectively make sure our camera is at least working.
             * 
             * Prevent Camera Going Off Map Should Always be done last.
             */

            resolutionBounds = bounds.Bounds;
            Vector2 p1 = player1.Center.ToVector2();
            cameraCenter = p1;
            TransformMatrix();
            UpdateVisibleArea();
        }
    

        /// <summary>
        /// Updates the Camera based on the positions of two players and the current Screen Resoltion
        /// </summary>
        /// <param name="bounds">  The bounds of the screen the camera must remain in </param>
        /// <param name="player1"> The Rectangle of the first player used to determine positioning </param>
        /// <param name="player2"> The Rectangle of the second player used to determine positioning </param>
        public void UpdateCamera(Viewport bounds, Rectangle player1, Rectangle player2)//updates the cameras position
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
            resolutionBounds = bounds.Bounds;
            Vector2 p1 = player1.Center.ToVector2();
            Vector2 p2 = player2.Center.ToVector2();
            /*
             * Currently hard sets the center point of the camera as the midpoint between the two players. To account for smooth camera drift, 
             * a noncentered camera, and perhaps more natural viewing, this value can be fiddled with once testing using more variables.
            */

            cameraCenter = (p1 + p2) * .5f; // Midpoint Equation
            DetermineZoom(p1, p2); 
            ZoomCheck();
            TransformMatrix();
            UpdateVisibleArea();
            PreventCameraGoingOffMap();

        }

        /// <summary>
        /// Determines how much the Zoom needs to be based on how far the players are apart.
        /// </summary>
        /// <param name="p1"> Player 1's Position</param>
        /// <param name="p2"> Player 2's Position</param>
        private void DetermineZoom(Vector2 p1, Vector2 p2)
        {

        }

        /// <summary>
        /// Determines if the new Zoom is too small or too large and sets to it's min / max value if needed
        /// </summary>
        private void ZoomCheck()
        {

        }

        /// <summary>
        /// Determines the new transform matrix to be used in the spriteBach
        /// </summary>
        private void TransformMatrix()
        {
            transform = Matrix.CreateTranslation(new Vector3(-cameraCenter.X, -cameraCenter.Y, 0)) *
                               Matrix.CreateScale(new Vector3(zoom)) *
                               Matrix.CreateTranslation(new Vector3(resolutionBounds.Width * 0.5f, resolutionBounds.Height * 0.5f, 0));
        }

        /// <summary>
        /// Creates a visible Rectangle of what is being drawn. This can be used to determine what elements need to be drawn on screen on that time.
        /// </summary>
        private void UpdateVisibleArea()
        {
            var inverseViewMatrix = Matrix.Invert(transform);
            var topL = Vector2.Transform(Vector2.Zero, inverseViewMatrix);
            var topR = Vector2.Transform(new Vector2(resolutionBounds.X, 0), inverseViewMatrix);
            var botL = Vector2.Transform(new Vector2(0, resolutionBounds.Y), inverseViewMatrix);
            var botR = Vector2.Transform(new Vector2(resolutionBounds.Width, resolutionBounds.Height), inverseViewMatrix);

            var min = new Vector2(
                MathHelper.Min(topL.X, MathHelper.Min(topR.X, MathHelper.Min(botL.X, botL.X))),
                MathHelper.Min(topL.Y, MathHelper.Min(topR.Y, MathHelper.Min(botL.Y, botL.Y))));

            var max =  new Vector2(
                 MathHelper.Max(topL.X, MathHelper.Max(topR.X, MathHelper.Max(botL.X, botL.X))),
                 MathHelper.Max(topL.Y, MathHelper.Max(topR.Y, MathHelper.Max(botL.Y, botL.Y))));

            visibleArea = new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y));
        }

        /// <summary>
        /// Determines if the newly drawn camera will draw off any boundry of the map. If it does, it resets the camera. 
        /// </summary>
        private void PreventCameraGoingOffMap()
        {

        }

        /// <summary>
        /// Determines if a GameObject should be updated and drawn to the screen
        /// </summary>
        /// <param name="position"> Position of the GameObject being checked</param>
        /// <returns> True = Update and Draw Object / False = Do Not Update or Draw Object </returns>
        public bool IsDrawn(Rectangle position) 
        {
            if (visibleArea.Intersects(position))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Testing Draw Method to Make Sure Visible Area is Correct
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void DrawCam(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(visibleTexture, visibleArea, Color.White);
        }

        #endregion Methods
    }
}