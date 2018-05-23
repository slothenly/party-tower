using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Party_Tower_Main
{
    //Defines a chicken that needs to be saved by the player
    class CapturedChicken : GameObject
    {
        //Fields

        Color color;
        Guid guid = Guid.NewGuid();

        //Used to give the collectibles random colors, doesn't work currently
        Random rn = new Random();
        Color[] Colors = new Color[] { Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Purple};

        //Constructor
        public CapturedChicken(int drawLevel, Texture2D defaultSprite, Rectangle hitbox)
        {
            this.drawLevel = drawLevel;
            this.defaultSprite = defaultSprite;
            this.hitbox = hitbox;

            //changed all eggs to be white for clarity
            this.color = Color.White;

            //this.color = RandomColor();
            this.isActive = true;
        }

        //Checks if player has collected chicken
        public override void CheckColliderAgainstPlayer(Player p)
        {
            if (hitbox.Intersects(p.Hitbox) && isActive)
            {
                //Run some method on P to update saved chickens
                p.UpdateChickenList(this);
                isActive = false;
            }
        }
        public override void Draw(SpriteBatch sb)
        {
            if (isActive)
            {
                sb.Draw(defaultSprite, hitbox, this.color);
            }

        }

        /// <summary>
        /// Returns a random color from the hard-built list of colors
        /// </summary>
        /// <returns></returns>
        public Color RandomColor()
        {
            return Colors[rn.Next(Colors.Length)];
        }
    }
}

