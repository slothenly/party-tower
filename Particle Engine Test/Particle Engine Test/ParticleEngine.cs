using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Particle_Engine_Test
{
    class ParticleEngine
    {
        //Give the engine all the accessory information that it needs
        private List<Tile> affectedTiles;

        /// <summary>
        /// Runs the particle engine affecting the imported tiles
        /// </summary>
        /// <param name="importedTiles"></param>
        public void Run(List<Tile> importedTiles)
        {
            //Make a collection of all the tiles that need to be covered
            List<Rectangle> coverRects = new List<Rectangle>();

            //Create 4 rectangles to cover ea/ of those
            foreach (Tile t in importedTiles)
            {
                int halfW = t.Width / 2;
                int halfH = t.Height / 2;
                Rectangle rectTL = new Rectangle(t.X, t.Y, halfW, halfH);
                Rectangle rectTR = new Rectangle(t.X + halfW, t.Y, halfW, halfH);
                Rectangle rectBL = new Rectangle(t.X, t.Y + halfH, halfW, halfH);
                Rectangle rectBR = new Rectangle(t.X + halfW, t.Y + halfH, halfW, halfH);

                coverRects.Add(rectTL);
                coverRects.Add(rectTR);
                coverRects.Add(rectBL);
                coverRects.Add(rectBR);
            }


            //Add fade in, lift, and spin effects to those rectangles
            for (int i = 0; i < importedTiles.Count; i++)
            {

            }

        }

        public Color ColorUpdater()
        {
            Color newC = new Color(255, 255, 255, 0);
            int modifier = 1;
            return newC;
        }

        private Color ColorUpdater(int oldColor, int modifier)
        {
            modifier = modifier * 2;
            int newColor = oldColor + modifier;
            return new Color(255, 255, 255, newColor);
        }
    }
}
