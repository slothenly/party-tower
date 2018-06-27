using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
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

        //spritesheet info
        public Texture2D Sheet { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        private int currentFrame;
        private int totalFrames;
        private const int TILEPIXELSIZE = 16;
        Dictionary<string, int[]> directionInterpreter = new Dictionary<string, int[]>();

        public Tile(bool IsBackground, bool IsPlatform, bool IsDamaging, bool IsWall, Texture2D sheet)
        {
            isPlatform = IsPlatform;
            isBackground = IsBackground;
            isDamaging = IsDamaging;
            isWall = IsWall;

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

            Rows = 6;
            Columns = 4;
            Sheet = sheet;
            #region Dictionary Setup
            //single ends
            directionInterpreter.Add("2", new int[] { 5, 0 });      //bottom
            directionInterpreter.Add("4", new int[] { 4, 3 });      //right
            directionInterpreter.Add("6", new int[] { 5, 0 });      //left
            directionInterpreter.Add("8", new int[] { 5, 2 });      //top

            //single wide extenders
            directionInterpreter.Add("28", new int[] { 1, 2 });    //vertical
            directionInterpreter.Add("46", new int[] { 1, 2 });    //horizontal

            //main rectangle
            directionInterpreter.Add("689", new int[] { 0, 0 });     //top left
            directionInterpreter.Add("46789", new int[] { 0, 1 });     //top middle
            directionInterpreter.Add("478", new int[] { 0, 2 });     //top right
            directionInterpreter.Add("23689", new int[] { 1, 0 });     //center left
            directionInterpreter.Add("12346789", new int[] { 1, 1 });     //center middle (neutral)
            directionInterpreter.Add("12478", new int[] { 1, 2 });     //center right
            directionInterpreter.Add("236", new int[] { 2, 0 });     //bottom left
            directionInterpreter.Add("12346", new int[] { 2, 1 });     //bottom middle
            directionInterpreter.Add("248", new int[] { 2, 2 });     //bottom right

            #endregion

        }

        public Tile(bool IsBackground, bool IsPlatform, bool IsDamaging, bool IsWall)
        {
            isPlatform = IsPlatform;
            isBackground = IsBackground;
            isDamaging = IsDamaging;
            isWall = IsWall;

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

            Rows = 6;
            Columns = 4;
            Sheet = null;
            #region Dictionary Setup
            //single ends
            directionInterpreter.Add("2", new int[] { 5, 0 });      //bottom
            directionInterpreter.Add("4", new int[] { 4, 3 });      //right
            directionInterpreter.Add("6", new int[] { 5, 0 });      //left
            directionInterpreter.Add("8", new int[] { 5, 2 });      //top

            //single wide extenders
            directionInterpreter.Add("28", new int[] { 1, 2 });    //vertical
            directionInterpreter.Add("46", new int[] { 1, 2 });    //horizontal

            //main rectangle
            directionInterpreter.Add("689", new int[] { 0, 0 });     //top left
            directionInterpreter.Add("46789", new int[] { 0, 1 });     //top middle
            directionInterpreter.Add("478", new int[] { 0, 2 });     //top right
            directionInterpreter.Add("23689", new int[] { 1, 0 });     //center left
            directionInterpreter.Add("12346789", new int[] { 1, 1 });     //center middle (neutral)
            directionInterpreter.Add("12478", new int[] { 1, 2 });     //center right
            directionInterpreter.Add("236", new int[] { 2, 0 });     //bottom left
            directionInterpreter.Add("12346", new int[] { 2, 1 });     //bottom middle
            directionInterpreter.Add("248", new int[] { 2, 2 });     //bottom right

            #endregion

        }
        #endregion

        /// <summary>
        /// Update function for tiles based on passed-in directives
        /// </summary>
        /// <param name="directoryString"></param>
        public void GetTilePosFromString(string directoryString)
        {
            //test if the string received is valid, otherwise just throw in a default tile position
            try
            {
                int[] position = directionInterpreter[directoryString];
            }
            catch (Exception e)
            {
                int[] position = directionInterpreter["12346789"];  //default tile
            }

            //Return null if something's broken
            ;
        }

        //Fill up as we develop
        public override void CheckColliderAgainstPlayer(Player p)
        {
            throw new NotImplementedException();
        }

        public override void Draw(SpriteBatch sb)
        {
            int width = Sheet.Width / Columns;
            int height = Sheet.Height / Rows;
            int row = (int)((float)currentFrame / Columns);
            int column = currentFrame % Columns;

            Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);
            Rectangle destinationRectangle = new Rectangle((int)Hitbox.X, (int)Hitbox.Y, width, height);

            sb.Draw(Sheet, destinationRectangle, sourceRectangle, Color.White);
        }
    }
}
