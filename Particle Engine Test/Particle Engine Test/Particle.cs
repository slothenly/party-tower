using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Particle_Engine_Test
{
    class Particle
    {
        private Texture2D texture;
        public float angle = 0.0f;
        public float scale = 1.0f;

        public float rotationSpeed;
        public bool rotationDirection; //clockwise is true, countercw is false
        
        public Vector2 location { get; set; }
        public Rectangle sourceRectangle { get; set; }
        private Vector2 origin;
        Color color;

        public Particle(Texture2D t, Rectangle source, Color c, int spinDir, float spinSpeed)
        {
            texture = t;
            sourceRectangle = source;
            color = c;
            location = new Vector2(source.X + (source.Width / 2), source.Y + (source.Height / 2));
            origin = new Vector2(source.Width / 2, source.Height / 2);

            //determine spin direction
            int temp = spinDir;
            if (temp == 0)
            {
                rotationDirection = true;
            }
            else
            {
                rotationDirection = false;
            }

            //determine spin speed
            rotationSpeed = 1.0f / spinSpeed;
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, location, sourceRectangle, color, angle, origin, scale, SpriteEffects.None, 1);
        }

        //examples
        //spriteBatch.Draw(arrow, location, sourceRectangle, Color.White, angle, origin, 1.0f, SpriteEffects.None, 1);
        //Vector2 origin = new Vector2(arrow.Width / 2, arrow.Height);

    }
}
