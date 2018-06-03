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
        #region Determines collisionary info & visual info about the tiles
        //collisionary info
        bool isPlatform;
        bool isBackground;
        bool isDamaging;

        //visual displaying info
        bool contactTop = false;
        bool contactBot = false;
        bool contactLeft = false;
        bool contactRight = false;

        public bool IsPlatform
        {
            get { return isPlatform; }
            set { isPlatform = value; }
        }

        public Tile(bool IsBackground, bool IsPlatform, bool IsDamaging, string TBLR)
        {
            isPlatform = IsPlatform;
            isBackground = IsBackground;
            isDamaging = IsDamaging;

            //determines which tiles will contact with the current tile based on the string passed in
            //TBLR stands for top, bottom, left, and right respectively.
            string[] temp = TBLR.Split();
            if (temp[0] == "t")
                contactTop = true;
            if (temp[1] == "t")
                contactBot = true;
            if (temp[2] == "t")
                contactLeft = true;
            if (temp[3] == "t")
                contactRight = true;
        }
        #endregion

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
