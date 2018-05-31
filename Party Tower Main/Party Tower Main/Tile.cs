using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

enum TileType
{
    platform, //can jump through from below
    wall //can't jump through from below
}

namespace Party_Tower_Main
{
    class Tile : GameObject
    {
        //fields
        public TileType type;

        //properties
        public TileType Type
        {
            get { return type; }
        }

        //constructor
        public Tile(TileType type)
        {
            this.type = type;
        }
        //Fill up as we develop
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
