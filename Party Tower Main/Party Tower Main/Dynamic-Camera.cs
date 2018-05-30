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
        private int updateAreaStandard;             // Standard which is used to create the Update Rectangle
        private Rectangle updateRectangle;          // Rectangle that determines what non-player objects get updated in game
        private Rectangle visibleArea;              // Actual Visible Area of the screen as a Rectangle
        private Texture2D visibleTexture;           // Testable Textures used to make sure the visibleArea is actually the Visible Area
        private float scaleCorrectly;               // Added amount of distance to properly scale the camera with both players within it's bounds
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
        /// <param name="scaleableAmount"> Equivelant to the width (or height) of the player since they are the same in a 64 x 64 box </param>
        /// <param name="maxWidthDistance"> Equivelant to the maxWidth from CameraLimiter </param>
        /// <param name="updateAreaStandard"> Equivelant to the widthConstant from PathManager </param>
        public Dynamic_Camera(Viewport view, int scaleableAmount, int maxWidthDistance, int updateAreaStandard)
        {
            resolutionBounds = view.Bounds;
            scaleCorrectly = scaleableAmount;
            this.updateAreaStandard = updateAreaStandard;
            zoom = 1.0f;
            minZoom = resolutionBounds.Width / (scaleCorrectly + maxWidthDistance); // Should be .75 if we're using 1/6 the screen of each side
            maxZoom = 1f;
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
            bool onMap;
            do
            {
                TransformMatrix();
                UpdateVisibleArea();
                onMap = PreventCameraGoingOffMap();
            } while (onMap);

        }

        /// <summary>
        /// Determines how much the Zoom needs to be based on how far the players are apart.
        /// </summary>
        /// <param name="p1"> Player 1's Position</param>
        /// <param name="p2"> Player 2's Position</param>
        private void DetermineZoom(Vector2 p1, Vector2 p2)
        {
            // If the change in distance for height > width, scale by height
            if ((Math.Abs(p1.X - p2.X) - resolutionBounds.Width) < (Math.Abs(p1.Y - p2.Y) - resolutionBounds.Height))
            {
                zoom = resolutionBounds.Height / (Math.Abs(p1.Y - p2.Y) + scaleCorrectly);
            }
            // Else, go by the width
            else
            {
                zoom = resolutionBounds.Width / (Math.Abs(p1.X - p2.X) + scaleCorrectly);
            }
        }

        /// <summary>
        /// Determines if the new Zoom is too small or too large and sets to it's min / max value if needed
        /// </summary>
        private void ZoomCheck()
        {
            if(zoom > maxZoom)
            {
                zoom = maxZoom;
            }

            if(zoom < minZoom)
            {
                zoom = minZoom;
            }

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
            updateRectangle = new Rectangle(visibleArea.X - (3 * updateAreaStandard), visibleArea.Y - (3 * updateAreaStandard),
                    visibleArea.Width + (updateAreaStandard * 6), visibleArea.Height + (updateAreaStandard * 6));
        }

        /// <summary>
        /// Determines if the visible area being shown is currently off of the map.
        /// </summary>
        /// <returns></returns>
        private bool PreventCameraGoingOffMap()
        {
            bool mustAdjust = false;

            /*
             * The Logic here is as follows....
             * 
             * If I know how big the visible area should be, according to the original matrix which has scaled the current visible area,
             * Then I can check to see if the visible area is being placed at a location that is off my gamemap.
             * 
             * If the rectangle is never placed off my gamemap, cool. No problem. Return false, break the do while in update camera, 
             * and everything is cool.
             * 
             * If the rectangle IS! placed out of bounds of my gamemap, then I need to adjust it!
             * How? Well, my visible area has ALREADY been scaled accordingly to my original matrix. 
             * Ultimately I'm going to use the same matrix to scale this new point, so the only thing that gets changed in my "transform" matrix will be
             * the first vector 3. 
             * 
             * To find this new center point, I simply need 1/2 the width [or height] of the current visible area, as the width/height of the new visible area
             * that will be recaluclated will be the same, just the points it's drawn too will be different.
             * 
             * I simply adjust the camera mid point [X || Y] accordingly by 1/2 the currently scaled visible areas width/height 
             * and recalculate my transform matrix with this new mid point. I DONT need to adjust for zoom, as zoom wouldn't become any smaller
             * since the distance between my players would be the same. 
             * 
             * Then I'd run through and check this stuff again. Presumable, I'd redraw my visible area, run this method again, and find that the redrawn
             * visible are will not intersect my edges of the game map.
             * 
             * Thus, the camera is now on screen.
             */

            if(visibleArea.X < 0)
            {
                mustAdjust = true;
                cameraCenter.X = visibleArea.Width / 2;
            }
            if(visibleArea.X + visibleArea.Width > mapEdge.X)
            {
                mustAdjust = true;
                cameraCenter.X = mapEdge.X - visibleArea.Width / 2;
            }
            if (visibleArea.Y < 0)
            {
                mustAdjust = true;
                cameraCenter.Y = visibleArea.Height / 2;
            }
            if(visibleArea.Y + visibleArea.Height > mapEdge.Y)
            {
                mustAdjust = true;
                cameraCenter.Y = mapEdge.Y - visibleArea.Height / 2;
            }

            return mustAdjust;
        }

        /// <summary>
        /// Determines if a GameObject should be drawn to the screen
        /// </summary>
        /// <param name="position"> Position of the GameObject being checked</param>
        /// <returns> True = Draw Object / False = Do Not Draw Object </returns>
        public bool DrawObject(Rectangle position) 
        {
            if (visibleArea.Intersects(position))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Determines if a GameObject should be updated to the screen
        /// </summary>
        /// <param name="position"> Position of the GameObject being checked</param>
        /// <returns> True = Update Object / False = Do Not Update Object </returns>
        public bool UpdateObject(Rectangle position)
        {
            if (updateRectangle.Intersects(position))
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