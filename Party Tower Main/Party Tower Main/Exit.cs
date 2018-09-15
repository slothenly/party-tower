using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Party_Tower_Main
{
    class Exit : GameObject
    {
        //Fields
        private bool shouldAppear;

        //Properties
        public bool ShouldAppear
        {
            get { return shouldAppear; }
            set { shouldAppear = value; }
        }

        public Exit(Rectangle hitbox, Texture2D defaultSprite)
        {
            this.hitbox = hitbox;
            this.defaultSprite = defaultSprite;
        }

        public override void CheckColliderAgainstPlayer(Player p)
        {
            throw new NotImplementedException();
        }

        public override void Draw(SpriteBatch sb)
        {
            throw new NotImplementedException();
        }
    }
}
