using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Particle_Engine_Test
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        ParticleEngine particleEngine;

        List<GameObject> drawn = new List<GameObject>();
        Texture2D testTexture;
        Texture2D tileSheet;
        Texture2D blankParticle;
        Rectangle testRect;

        MouseState newMouse;   //from current frame
        MouseState oldMouse;    //from 1 frame ago
        Rectangle mouseRect;
        bool tileHover;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            testRect = new Rectangle(200, 100, 64, 64);
            this.IsMouseVisible = true;
            tileHover = false;
            base.Initialize();
            mouseRect = new Rectangle(0, 0, 1, 1);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            testTexture = Content.Load<Texture2D>("coloredSquare");
            tileSheet = Content.Load<Texture2D>("basicTileSheet");
            blankParticle = Content.Load<Texture2D>("singleWhitePx");

            particleEngine = new ParticleEngine(blankParticle);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            tileHover = false;
            newMouse = Mouse.GetState();
            mouseRect.X = newMouse.X;
            mouseRect.Y = newMouse.Y;
            foreach (Tile t in drawn)   //check if a the mouse is hovering over a tile
            {
                if (mouseRect.Intersects(t.Hitbox))
                {
                    tileHover = true;
                }
            }

            if (tileHover == false)
            {
                //place a new tile in the spot of the mouse when the mouse is clicked while not hovering and add it to the drawn list
                if (newMouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released)
                {
                    Tile t = new Tile(false, false, false, false, tileSheet);
                    t.GetTilePosFromString("8");
                    t.X = newMouse.X - (t.Width / 2);
                    t.Y = newMouse.Y - (t.Height / 2);
                    drawn.Add(t);
                }
            }
            else
            {
                //run the particle engine if the mouse is clicked on top of a tile
                if (newMouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released)
                {
                    //get all the tiles to feed into the engine and remove them from drawn
                    List<Tile> temp = new List<Tile>();
                    for (int current = 0; current < drawn.Count; current++)
                    {
                        if (drawn[current] is Tile)
                        {
                            temp.Add((Tile)drawn[current]);
                            drawn.Remove(drawn[current]);
                        }
                    }

                    //actually run the engine
                    particleEngine.Run(temp);
                }
            }

            oldMouse = newMouse;


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(SpriteSortMode.Immediate);
            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            //modify the color each frame to get the fade in from the overlayed tiles
            spriteBatch.Draw(testTexture, testRect, Color.White);

            //draw all tiles
            foreach (Tile t in drawn)
            {
                t.Draw(spriteBatch);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
