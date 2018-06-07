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
        private bool isPlatform;
        //collisionary info
        public bool IsPlatform
        {
            get { return isPlatform; }
            set { isPlatform = value; }
        }
        public bool isBackground { get; set; }
        public bool isDamaging { get; set; }
        public bool isWall { get; set; }

        bool isPlatform;

        //visual displaying info
        bool contactTop = false;
        bool contactBot = false;
        bool contactLeft = false;
        bool contactRight = false;

        public Tile(bool IsBackground, bool IsPlatform, bool IsDamaging, bool IsWall, string TBLR)
        {
            isPlatform = IsPlatform;
            isBackground = IsBackground;
            isDamaging = IsDamaging;
            isWall = IsWall;

            //determines which tiles will contact with the current tile based on the string passed in
            //TBLR stands for top, bottom, left, and right respectively.
            char[] temp = TBLR.ToCharArray();
            if (temp[0].ToString() == "t")
                contactTop = true;
            if (temp[1].ToString() == "t")
                contactBot = true;
            if (temp[2].ToString() == "t")
                contactLeft = true;
            if (temp[3].ToString() == "t")
                contactRight = true;

            //determining tile heights/widths based on if its platform
            if (isPlatform != true)
            {
                Width = 1920 / 16;
                Height = Width;
            }
            else
            {
                Width = 1080 / 9;
                Height = Width / 2;
            }
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
