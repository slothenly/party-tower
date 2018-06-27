using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Party_Tower_Main
{
    class SpriteSheetController
    {
        public Texture2D Sheet { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        private int currentFrame;
        private int totalFrames;
        private const int TILEPIXELSIZE = 16;

        //slowing frame animation here
        int timeSinceLastFrame = 0;
        private const int MILLSPERFRAME = 50;


        /// <summary>
        /// Constructing the class
        /// </summary>
        public SpriteSheetController(Texture2D sheet, int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            Sheet = sheet;
            totalFrames = Rows * Columns;

        }

        /// <summary>
        /// Update function for frame animations based on game time
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastFrame > MILLSPERFRAME)
            {
                timeSinceLastFrame -= MILLSPERFRAME;

                //increment current frame
                currentFrame++;
                timeSinceLastFrame = 0;
                if (currentFrame == totalFrames)
                {
                    currentFrame = 0;
                }
            }
        }



        public void Draw(SpriteBatch sb, Vector2 location)
        {
            int width = Sheet.Width / Columns;
            int height = Sheet.Height / Rows;
            int row = (int)((float)currentFrame / Columns);
            int column = currentFrame % Columns;

            Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);
            Rectangle destinationRectangle = new Rectangle((int)location.X, (int)location.Y, width, height);

            sb.Draw(Sheet, destinationRectangle, sourceRectangle, Color.White);
        }
    }
}
