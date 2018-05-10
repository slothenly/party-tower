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
    /// <summary>
    /// This class is meant to reposition the two players accordingly to keep them from ever going a max distance away from one another.
    /// This way, the zoom never needs readjusted for in the camera and can always assume to work 
    /// </summary>
    class CameraLimiters
    {
        #region Fields

        private int maxWidthDistance;
        private int maxHeightDistance;

        private Vector2 player1_Old;
        private Vector2 player1_New;
        private Vector2 player2_Old;
        private Vector2 player2_New;

        #endregion

        #region Private_Properties

        /// <summary>
        /// Old Mid Point
        /// </summary>
        private Vector2 OldMidpoint
        {
            get { return (player1_Old + player2_Old) * .5f; } 
        }

        /// <summary>
        /// New Mid Point
        /// </summary>
        private Vector2 NewMidpoint
        {
            get { return (player1_New + player2_New) * .5f; }
        }

        /// <summary>
        /// Influenced Mid Point based on the new and old
        /// </summary>
        private Vector2 InfluencedMidpoint
        {
            get { return (OldMidpoint + NewMidpoint) * .5f; }
        }

        #endregion

        #region Public_Properties

        /// <summary>
        /// Should be used to help determine if the max width is exceeded and this class needs to run
        /// </summary>
        public int MaxWidthDistance
        {
            get { return maxWidthDistance; }
        }

        /// <summary>
        /// Should be used to help determine if the max height is exceeded and this class needs to run
        /// </summary>
        public int MaxHeightDistance
        {
            get { return maxHeightDistance; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor used if there is a hardset max width / height we want to play with
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="playerDimensions"></param>
        public CameraLimiters(int width, int height, Rectangle playerDimensions)
        {
            maxWidthDistance = width - (playerDimensions.Width);
            maxHeightDistance = height - (playerDimensions.Height);
        }

        /// <summary>
        /// Constructor used if we want to determine the max width / height based on the screen resolution size
        /// </summary>
        /// <param name="view"></param>
        /// <param name="playerDimensions"></param>
        public CameraLimiters(Viewport view, Rectangle playerDimensions)
        {
            maxHeightDistance = view.Height + (view.Height / 3) - playerDimensions.Height; 
            maxWidthDistance = view.Width + (view.Width / 3) - playerDimensions.Width;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Only be called if the distance !FROM CENTER TO CENTER! between both players exceeds the max width / height distance allowed.
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        /// <param name="player1Old"></param>
        /// <param name="player2Old"></param>
        /// <returns> Returns an array of Recangles that holds the new Rectangle and Modified Poisitions for Player 1 and Player 2 </returns>
        public Rectangle[] RepositionPlayers(Rectangle player1, Rectangle player2, Rectangle player1Old, Rectangle player2Old)
        {

            player1_New = new Vector2(player1.Center.X, player1.Center.Y);
            player1_Old = new Vector2(player1Old.Center.X, player1Old.Center.Y);
            player2_New = new Vector2(player2.Center.X, player2.Center.Y);
            player2_Old = new Vector2(player2Old.Center.X, player2Old.Center.Y);

            // Adjust Horizontal

            if (Math.Abs(player1_New.X - player2_New.X) > maxWidthDistance)
            {
                Rectangle[] players = HorizontalReposition(player1, player2);
                player1 = players[0];
                player2 = players[1];
            }

            // Adjust Vertical

            if (Math.Abs(player1_New.Y - player2_New.Y) > maxHeightDistance)
            {
                Rectangle[] players = VerticalReposition(player1, player2);
                player1 = players[0];
                player2 = players[1];
            }

            Rectangle[] playersFinal = new Rectangle[] { player1, player2 };
            return playersFinal;
        }

        /// <summary>
        /// Horizontally Adjusts the Players positions and hitboxes accordingly and returns these values
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        /// <returns></returns>
        private Rectangle[] HorizontalReposition(Rectangle player1, Rectangle player2)
        {
            // Uses the the oldMidPoint of the characters to keep the camera grounded until both players move the same way
            // Also can play with an influenced Midpoint if wanted or the new Midpoint. 
            // This equation also assumes the dimensions of both players are the same. This can easily be changed if needed.

            int newX_behind = (int)OldMidpoint.X - (maxWidthDistance / 2) - (player1.Width / 2);
            int newX_front = (int)OldMidpoint.X + (maxWidthDistance / 2) - (player1.Width / 2);

            if(player1_New.X > player2_New.X)
            {
                player1 = new Rectangle(newX_front, player1.Y, player1.Width, player1.Height);
                player2 = new Rectangle(newX_behind, player2.Y, player2.Width, player2.Height);
            }
            else
            {
                player1 = new Rectangle(newX_behind, player1.Y, player1.Width, player1.Height);
                player2 = new Rectangle(newX_front, player2.Y, player2.Width, player2.Height);
            }

            Rectangle[] players = new Rectangle[] { player1, player2 };
            return players;
        }

        /// <summary>
        /// Vertically Adjusts the Players positions and hitboxes accordingly and returns these values
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        /// <returns></returns>
        private Rectangle[] VerticalReposition(Rectangle player1, Rectangle player2)
        {
            // Uses the the oldMidPoint of the characters to keep the camera grounded until both players move the same way
            // Also can play with an influenced Midpoint if wanted or the new Midpoint. 
            // This equation also assumes the dimensions of both players are the same. This can easily be changed if needed.

            int newY_above = (int)OldMidpoint.Y - (maxHeightDistance / 2) - (player1.Height / 2);
            int newY_below = (int)OldMidpoint.Y + (maxHeightDistance / 2) - (player1.Height / 2);

            if (player1_New.Y > player2_New.Y)
            {
                player1 = new Rectangle(player1.X, newY_below, player1.Width, player1.Height);
                player2 = new Rectangle(player2.X, newY_above, player2.Width, player2.Height);
            }
            else
            {
                player1 = new Rectangle(player1.X, newY_above, player1.Width, player1.Height);
                player2 = new Rectangle(player2.X, newY_below, player2.Width, player2.Height);
            }

            Rectangle[] players = new Rectangle[] { player1, player2 };
            return players;
        }

        #endregion

    }
}
