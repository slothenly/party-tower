using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        List<Particle> particles = new List<Particle>();
        private Texture2D blankParticle;
        public int Duration { get; set; }
        private int originalDuration;
        Random rng;

        /// <summary>
        /// Constructor for the particle engine that gives neccesary information for the engine to run
        /// </summary>
        /// <param name="blankParticle"></param>
        /// <param name="duration"></param>
        public ParticleEngine(Texture2D blankParticle, int duration)
        {
            this.blankParticle = blankParticle;
            Duration = duration;
            originalDuration = duration;
            rng = new Random();
        }


        /// <summary>
        /// Runs the particle engine affecting the imported tiles
        /// </summary>
        /// <param name="importedTiles"></param>
        public void Run(List<Tile> importedTiles)
        {
            //Make a collection of all the tiles that need to be covered
            List<Particle> particleCollection = new List<Particle>();

            //Create 4 rectangles to cover ea/ of those
            foreach (Tile t in importedTiles)
            {
                int halfW = t.Width / 2;
                int halfH = t.Height / 2;
                Rectangle rectTL = new Rectangle(t.X, t.Y, halfW, halfH);
                Rectangle rectTR = new Rectangle(t.X + halfW, t.Y, halfW, halfH);
                Rectangle rectBL = new Rectangle(t.X, t.Y + halfH, halfW, halfH);
                Rectangle rectBR = new Rectangle(t.X + halfW, t.Y + halfH, halfW, halfH);

                Particle p1 = new Particle(blankParticle, rectTL, Color.White, rng.Next(0, 2), rng.Next(10, 51));
                Particle p2 = new Particle(blankParticle, rectTR, Color.White, rng.Next(0, 2), rng.Next(10, 51));
                Particle p3 = new Particle(blankParticle, rectBL, Color.White, rng.Next(0, 2), rng.Next(10, 51));
                Particle p4 = new Particle(blankParticle, rectBR, Color.White, rng.Next(0, 2), rng.Next(10, 51));

                List<Particle> particles = new List<Particle>();
                particleCollection.Add(p1);
                particleCollection.Add(p2);
                particleCollection.Add(p3);
                particleCollection.Add(p4);
            }

            particles.AddRange(particleCollection);
        }

        /// <summary>
        /// Continues running the particle engine, returns whether the engine still needs to exist
        /// </summary>
        /// <returns></returns>
        public void Continue()
        {

            float rotater;
            float scaler;

            //actually modify each particle
            foreach (Particle p in particles)
                {
                    rotater = p.rotationSpeed;
                    scaler = 1.05f;

                    //Modify Spin
                    if (p.rotationDirection == true)
                        p.angle += rotater;
                    else if (p.rotationDirection == false)
                        p.angle -= rotater;

                    //Modify Scale
                    p.scale = p.scale / scaler;

                    //Modifty Position
                    Vector2 locationCopy = p.location;
                    locationCopy.Y -= 5;
                    p.location = locationCopy;
                }

            Duration--;         
        }

        public void DrawParticles(SpriteBatch sb)
        {
            foreach (Particle p in particles)
            {
                p.Draw(sb);
            }
        }

        public void AddParticles(List<Tile> importedTiles)
        {

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
