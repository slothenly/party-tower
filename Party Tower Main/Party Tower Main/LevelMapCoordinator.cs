using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Party_Tower_Main
{
    class LevelMapCoordinator
    {
        //Holds the info for all of the tiles currently on the map
        private Tile[,] currentMap;
        public Tile[,] CurrentMap
        {
            get { return currentMap; }
        }

        private string[,] currentMapRaw;    //raw, unsorted version of the current map for possible later use
        public string[,] CurrentMapRaw
        {
            get { return currentMapRaw; }
        }

        //Creates a readible map for the pathmanager
        private string[] pathManagerMap;
        public string[] PathManagerMap
        {
            get { return pathManagerMap; }
        }

        int Rows;       //both are intentionally capitalized to make for easier recognition
        int Columns;    //sorry if it irks you

        List<Texture2D> textureList;

        Dictionary<string, int> translator = new Dictionary<string, int>();
        Dictionary<string, Texture2D> tileRetriever = new Dictionary<string, Texture2D>();

        /// <summary>
        /// Constructor which pulls the initial path to take tile info from
        /// </summary>
        /// <param name="initialPath"></param>
        public LevelMapCoordinator(string initialPath, List<Texture2D> TextureList)
        {
            #region Translator Info
            translator.Add("br", 0);    //brick
            translator.Add("di", 1);    //dirt
            translator.Add("gr", 2);    //grass
            translator.Add("mo", 3);    //moss
            #endregion

            textureList = TextureList;
            UpdateMapFromPath(initialPath);
        }

        /// <summary>
        /// Updates the current tileset based on a given path
        /// NOTE: Only feed in pre-set paths, there's no check in place to catch a bad name
        /// </summary>
        /// <param name="path"></param>
        public void UpdateMapFromPath(string path)
        {
            //fields & initialization logic
            string line;
            int c = 0;
            string tempString = "";
            string[] infoTempHolder;

            #region Reading in info from text tile and plopping it into a 2d array
            StreamReader interpreter = new StreamReader(@"..\..\..\..\Resources\levelExports\" + path + ".txt");

            //Setup for creating the level's 2d array (pulls Row and Column counts, splits the rest into a 2D array)
            line = interpreter.ReadLine();
            infoTempHolder = line.Split(',');
            Rows = int.Parse(infoTempHolder[0]);
            Columns = int.Parse(infoTempHolder[1]);
            string[,] importedTileInfo = new string[Rows, Columns];
            pathManagerMap = new string[Rows];

            //Reading in the tiles from the file and placing them into the a holder string
            string tileInfoString = interpreter.ReadToEnd();
            infoTempHolder = tileInfoString.Split(',');

            //getting rid of "\r\n"
            for (int i = 0; i <= infoTempHolder.Length - 2; i++)
            {
                tempString = infoTempHolder[i];
                char[] individualTileString = infoTempHolder[i].ToCharArray();

                if (individualTileString.Length > 4)
                {
                    tempString = "";    //clears tempString so we dont get \r\nb1nt b1nt
                    char space = ' ';
                    //make first two char spaces
                    individualTileString[0] = space;

                    individualTileString[1] = space;
                    foreach (var item in individualTileString) //go through every char in temp
                    {
                        if (item != ' ') tempString += item.ToString(); //if char is not a space add it to tile ID temp string
                    }
                }
                infoTempHolder[i] = tempString;
            }

            //actually sets all of the individual tiles into the 2d array
            for (int i = 0; i <= importedTileInfo.GetLength(0) - 1; i++)
            {

                for (int j = 0; j <= importedTileInfo.GetLength(1) - 1; j++)
                {

                    importedTileInfo[i, j] = infoTempHolder[c]; //sets tileID in level array 

                    c++; //increments c because it is seperate array of different dimensions
                }

            }
            interpreter.Close();
            currentMapRaw = importedTileInfo;
            #endregion

            //At this point, the file has been read into the 2D array 'currentMapRaw'

            #region Interpreting raw text file

            //initialize all spots in the 2D array and sets x & y positions
            currentMap = new Tile[Rows, Columns];
            for (int rows = 0; rows < Rows; rows++)
            {
                for (int columns = 0; columns < Columns; columns++)
                {
                    currentMap[rows, columns] = new Tile(false, false, false, false, "FFFF");
                    currentMap[rows, columns].X = columns * currentMap[rows, columns].Width;  //x pos
                    currentMap[rows, columns].Y = rows * currentMap[rows, columns].Height;    //y pos
                }
            }


            //main 2d loop that pulls tile data from raw and sets it into currentMap tile position
            for (int rows = 0; rows < Rows; rows++)
            {
                for (int columns = 0; columns < Columns; columns++)
                {
                    //if there is a tile in this slot, fill it with the given information
                    if (currentMapRaw[rows, columns] != "0000")
                    {
                        //first, split up the raw, then distribute the info into the tile's slots
                        char[] currentRawSplit = currentMapRaw[rows, columns].ToCharArray();
                        int textureKey = translator[currentRawSplit[0].ToString() + currentRawSplit[1].ToString()];

                        //check damaging and platform conditions
                        if (currentRawSplit[2].ToString() == "T")
                        {
                            currentMap[rows, columns].isDamaging = true;
                        }
                        if (currentRawSplit[3].ToString() == "T")
                        {
                            currentMap[rows, columns].IsPlatform = true;
                        }

                        //set texture
                        currentMap[rows, columns].DefaultSprite = textureList[textureKey];
                    }
                    //otherwise clean out the preset
                    else
                    {
                        currentMap[rows, columns] = null;
                    }

                }
            }

            //Creates a Map needed for Path Manager based on the tiles

            /* Example of Map as String[]
    {
                 "+------+",
                 "|      |",
                 "|E X   |",
                 "|XXX   |",
                 "|   X  |",
                 "| P    |",
                 "|      |",
                 "+------+",

             * PATHMANAGER KEY
             * P = Player
             * E = Enemy
             * + = "CORNER PIECE WALL"
             * _ = "WALL"
             * X = "DAMAGING WALL"
             * ~ = CAN JUMP FROM BELOW
             */

            for (int rows = 0; rows < Rows; rows++)
            {
                string tempRow = "";

                for (int col = 0; col < Columns; col++)
                {
                    if (currentMap[rows, col] != null)
                    {
                        if (currentMap[rows, col].isDamaging == true)
                        {
                            tempRow += "X";
                        }
                        else if (currentMap[rows, col].IsPlatform == true)
                        {
                            tempRow += "~";
                        }
                        else if (currentMap[rows, col].isWall == true)
                        {
                            tempRow += "_";
                        }
                        else
                        {
                            tempRow += " ";
                        }
                    }
                    else
                    {
                        tempRow += " ";
                    }
                }
                pathManagerMap[rows] = tempRow;
            }

            #endregion
        }

        /// <summary>
        /// Draws all current tiles onto the screen
        /// </summary>
        /// <param name="sb"></param>
        public void Draw(SpriteBatch sb)

        {
            foreach (Tile t in CurrentMap)
            {
                if (t != null)
                {
                    Rectangle rect = new Rectangle(t.X, t.Y, t.Width, t.Height);
                    sb.Draw(t.DefaultSprite, rect, Color.White);
                }
            }
        }

        #region Helper Functions

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentTile"></param>
        /// <param name="rowNumber"></param>
        /// <param name="colNumber"></param>
        /// <returns></returns>
        private Tile orientTiles(Tile currentTile, int rowNumber, int colNumber)
        {
            string tileConnections = "";

            bool dc1 = false;   //tags telling the checkers to assume that spot is empty
            bool dc2 = false;   //dc means "don't check"
            bool dc3 = false;
            bool dc4 = false;               // 1 2 3
            bool dc6 = false;               // 4 X 6    <-- X marks the current tile
            bool dc7 = false;               // 7 8 9
            bool dc8 = false;
            bool dc9 = false;

            #region Special cases for tiles around the edges

            //invalidates checking for the spots that don't exist so the program doesn't break
            if (rowNumber - 1 < 0)
            {
                dc1 = true;
                dc2 = true;
                dc3 = true;
            }
            if (rowNumber + 1 > Rows)
            {
                dc7 = true;
                dc8 = true;
                dc9 = true;
            }
            if (colNumber - 1 < 0)
            {
                dc1 = true;
                dc4 = true;
                dc7 = true;
            }
            if (rowNumber + 1 > Columns)
            {
                dc3 = true;
                dc6 = true;
                dc9 = true;
            }
            #endregion

            #region Check through all viable surrounding tiles
            if (dc1 != true)
            {
                if (currentMap[rowNumber--, colNumber--] != null)
                {
                    tileConnections += "1";
                }
            }

            if (dc2 != true)
            {
                if (currentMap[rowNumber--, colNumber] != null)
                {
                    tileConnections += "2";
                }
            }

            if (dc3 != true)
            {
                if (currentMap[rowNumber--, colNumber++] != null)
                {
                    tileConnections += "3";
                }
            }

            if (dc4 != true)
            {
                if (currentMap[rowNumber, colNumber--] != null)
                {
                    tileConnections += "4";
                }
            }

            //dc5 doesn't exist on purpose -- no need to check if the current tile exists

            if (dc6 != true)
            {
                if (currentMap[rowNumber, colNumber++] != null)
                {
                    tileConnections += "6";
                }
            }

            if (dc7 != true)
            {
                if (currentMap[rowNumber++, colNumber--] != null)
                {
                    tileConnections += "7";
                }
            }

            if (dc8 != true)
            {
                if (currentMap[rowNumber++, colNumber] != null)
                {
                    tileConnections += "1";
                }
            }

            if (dc9 != true)
            {
                if (currentMap[rowNumber++, colNumber++] != null)
                {
                    tileConnections += "1";
                }
            }
            #endregion

            //Based on the concatenated result, pull the correct tile from the tile bank

            //Return null if something is broken
            return null;
        }

        #endregion
    }
}
