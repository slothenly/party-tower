using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Party_Tower_Main
{
    class Button
    {
        private Rectangle area;
        private Point startLocation;
        private bool isHighlighted;
        private Texture2D normalTexture;
        private Texture2D highlightedTexture;
        
        public Rectangle Area
        {
            get { return area; }
            set { area = value; }
        }
        public Point StartLocation
        {
            get { return startLocation; }
            set { startLocation = value; }
        }
        public int X
        {
            get { return startLocation.X; }
        }
        public int Y
        {
            get { return startLocation.Y; }
        }
        public bool IsHighlighted
        {
            get { return isHighlighted; }
            set { isHighlighted = value; }
        }
        public Texture2D DrawnTexture
        {
            get { return normalTexture; }
            //no implementation of highlighting due to different dimensions
            //to change to highlighted change the above get property to:
            /*            
             *            get
             *{
             *  if (isHighlighted)
             *   {
             *       return highlightedTexture;
             *   }
             *   else
             *   {
             *       return normalTexture;
             *   }
             * }
             */
        }

        public Button(Texture2D normalTexture, Texture2D highlightedTexture)
        {
            this.normalTexture = normalTexture;
            this.highlightedTexture = highlightedTexture;
        }
    }
}
