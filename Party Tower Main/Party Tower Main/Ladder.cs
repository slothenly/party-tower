using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Party_Tower_Main
{
    class Ladder : GameObject
    {
        //Fields
        private bool isTop;
        private bool isBottom;

        //Properties
        public bool IsTop
        {
            get { return isTop; }
            set { isTop = value; }
        }
        public bool IsBottom
        {
            get { return isBottom; }
            set { isBottom = value; }
        }

        //Constructor
        public Ladder(bool isTop, bool isBottom, int x, int y)
        {
            this.isTop = isTop;
            this.isBottom = isBottom;
            X = x;
            Y = y;

            hitbox = new Rectangle(x, y, 1920 / 16, 1080 / 8);
        }

        //Methods
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
