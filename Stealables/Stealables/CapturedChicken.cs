using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Egg
{
    //Defines a chicken that needs to be saved by the player
    class CapturedChicken : GameObject
    {
        Color color;
        Guid guid = Guid.NewGuid();
        Random rn = new Random();
        Color[] Colors = new Color[] { Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Purple };

        Screen originScreen;

        public Color Color
        {
            get { return color; }
        }

        public CapturedChicken(int drawLevel, Texture2D defaultSprite, Rectangle hitbox)
        {
            this.drawLevel = drawLevel;
            this.defaultSprite = defaultSprite;
            this.hitbox = hitbox;
            this.color = RandomColor();
            this.isActive = true;            
        }

        //Unknown what will be done with this
        public override void FiniteState()
        {
            throw new NotImplementedException();
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
        //Enemies pass through chickens
        public override void CheckColliderAgainstEnemy(Enemy e)
        {
            //throw new NotImplementedException();
        }
        //Chickens won't move, so left unimplemented.
        public override void Movement()
        {
            throw new NotImplementedException();
        }

        public Color RandomColor()
        {
            return Colors[rn.Next(Colors.Length)];
        }
    }
}
