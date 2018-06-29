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
        public Texture2D TileSheet { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        private int currentFrame;
        private int totalFrames;
        private const int TILEPIXELSIZE = 16;
        Dictionary<string, int[]> directionInterpreter = new Dictionary<string, int[]>();

        public Tile(bool IsBackground, bool IsPlatform, bool IsDamaging, bool IsWall, Texture2D sheet)
        {
            TileSheet = sheet;
            isPlatform = IsPlatform;
            isBackground = IsBackground;
            isDamaging = IsDamaging;
            isWall = IsWall;
            Rows = 6;
            Columns = 4;

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

            #region Dictionary Setup
            //single ends
            directionInterpreter.Add("2", new int[] { 6, 0 });       //bottom
            directionInterpreter.Add("4", new int[] { 4, 3 });       //right
            directionInterpreter.Add("6", new int[] { 4, 0 });       //left
            directionInterpreter.Add("8", new int[] { 5, 2 });       //top

            //single wide extenders
            directionInterpreter.Add("28", new int[] { 5, 1 });      //vertical
            directionInterpreter.Add("46", new int[] { 1, 3 });      //horizontal

            //main rectangle
            directionInterpreter.Add("689", new int[] { 0, 0 });     //top left
            directionInterpreter.Add("46789", new int[] { 0, 1 });   //top middle
            directionInterpreter.Add("478", new int[] { 0, 2 });     //top right
            directionInterpreter.Add("23689", new int[] { 1, 0 });   //center left
            directionInterpreter.Add("12346789", new int[] { 1, 1 });//center middle (neutral)
            directionInterpreter.Add("12478", new int[] { 1, 2 });   //center right
            directionInterpreter.Add("236", new int[] { 2, 0 });     //bottom left
            directionInterpreter.Add("12346", new int[] { 2, 1 });   //bottom middle
            directionInterpreter.Add("248", new int[] { 2, 2 });     //bottom right

            //interior angles
            directionInterpreter.Add("2346789", new int[] { 3, 3 }); //top left missing
            directionInterpreter.Add("1246789", new int[] { 3, 2 }); //top right missing
            directionInterpreter.Add("1234689", new int[] { 3, 0 }); //bottom left missing
            directionInterpreter.Add("1234678", new int[] { 3, 1 }); //bottom right missing
            
            //duplicates
            directionInterpreter.Add("6789", new int[] { 0, 0 });    //top left
            directionInterpreter.Add("3689", new int[] { 0, 0 });    //top left
            directionInterpreter.Add("4789", new int[] { 0, 2 });    //top right
            directionInterpreter.Add("1478", new int[] { 0, 2 });    //top right
            directionInterpreter.Add("2369", new int[] { 2, 0 });    //bottom left
            directionInterpreter.Add("1236", new int[] { 2, 0 });    //bottom left
            directionInterpreter.Add("1247", new int[] { 2, 2 });    //bottom right
            directionInterpreter.Add("1234", new int[] { 2, 2 });    //bottom right


            /*
                         //single ends
            directionInterpreter.Add("2", new int[] { 6, 1 });      //bottom
            directionInterpreter.Add("4", new int[] { 5, 4 });      //right
            directionInterpreter.Add("6", new int[] { 5, 1 });      //left
            directionInterpreter.Add("8", new int[] { 6, 3 });      //top

            //single wide extenders
            directionInterpreter.Add("28", new int[] { 6, 2 });    //vertical
            directionInterpreter.Add("46", new int[] { 2, 4 });    //horizontal

            //main rectangle
            directionInterpreter.Add("689", new int[] { 1, 1 });     //top left
            directionInterpreter.Add("46789", new int[] { 1, 2 });     //top middle
            directionInterpreter.Add("478", new int[] { 1, 3 });     //top right
            directionInterpreter.Add("23689", new int[] { 2, 1 });     //center left
            directionInterpreter.Add("12346789", new int[] { 2, 2 });     //center middle (neutral)
            directionInterpreter.Add("12478", new int[] { 2, 3 });     //center right
            directionInterpreter.Add("236", new int[] { 3, 1 });     //bottom left
            directionInterpreter.Add("12346", new int[] { 3, 2 });     //bottom middle
            directionInterpreter.Add("248", new int[] { 3, 3 });     //bottom right 
            */
            #endregion

        }

        #endregion

        /// <summary>
        /// Update function for tiles based on passed-in directives
        /// </summary>
        /// <param name="directoryString"></param>
        public void GetTilePosFromString(string directoryString)
        {
            int[] position;

            //test if the string received is valid, otherwise just throw in a default tile position
            try
            {
                position = directionInterpreter[directoryString];
            }
            catch (Exception e)
            {
                position = directionInterpreter["12346789"];  //default tile
            }

            //Return null if something's broken
            Rows = position[0];
            Columns = position[1];
        }

        //Fill up as we develop
        public override void CheckColliderAgainstPlayer(Player p)
        {
            throw new NotImplementedException();
        }

        public override void Draw(SpriteBatch sb)
        {
            //int width = TileSheet.Width / Columns;
            //int height = TileSheet.Height / Rows;
            int width = TILEPIXELSIZE;
            int height = TILEPIXELSIZE;
            //int row = (int)((float)currentFrame / Columns);
            //int column = currentFrame % Columns;

            Rectangle sourceRectangle = new Rectangle(width * Columns, height * Rows, width, height);
            Rectangle destinationRectangle = new Rectangle((int)Hitbox.X, (int)Hitbox.Y, Width, Height);

            sb.Draw(TileSheet, destinationRectangle, sourceRectangle, Color.White);
        }
    }
}
