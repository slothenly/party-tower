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

        private Vector2 mapEdge;
        public Vector2 MapEdge
        {
            get { return mapEdge; }
        }

        int Rows;       //both are intentionally capitalized to make for easier recognition
        int Columns;    //sorry if it irks you

        int initializationsRun = 0;     //tracking meta data to see if things are running multiple times
        List<Texture2D> textureList;
        List<Enemy> enemyHolder;
        Texture2D defaultEnemy;
        Texture2D defaultTileSheet;
        Dictionary<string, int> translator = new Dictionary<string, int>();
        Dictionary<string, Texture2D> tileRetriever = new Dictionary<string, Texture2D>();

        /// <summary>
        /// Constructor which pulls the initial path to take tile info from
        /// </summary>
        /// <param name="initialPath"></param>
        public LevelMapCoordinator(string initialPath, List<Texture2D> textureList, Texture2D defaultEnemy, Texture2D TileSheet)
        {
            #region Translator Info
            translator.Add("br", 0);    //brick
            translator.Add("di", 1);    //dirt
            translator.Add("gr", 2);    //grass
            translator.Add("mo", 3);    //moss
            //translator.Add("e1", 4);    //default enemy
            translator.Add("default", 5);
            #endregion

            this.textureList = textureList;
            this.defaultEnemy = defaultEnemy;
            defaultTileSheet = TileSheet;
            UpdateMapFromPath(initialPath);
        }

        /// <summary>
        /// Updates the current tileset based on a given path
        /// NOTE: Only feed in pre-set paths, there's no check in place to catch a bad name
        /// </summary>
        /// <param name="path"></param>
        public Tile[,] UpdateMapFromPath(string path)
        {
            //fields & initialization logic
            string line;
            int c = 0;
            string tempString = "";
            string[] infoTempHolder;
            enemyHolder = new List<Enemy>();

            initializationsRun++;

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
                    currentMap[rows, columns] = new Tile(false, false, false, false, null);
                    currentMap[rows, columns].X = columns * currentMap[rows, columns].Width;  //x pos
                    currentMap[rows, columns].Y = rows * currentMap[rows, columns].Height;    //y pos
                }
            }

            //Sets Map Edge stuff to give to Path Manager
            mapEdge = new Vector2((currentMap[Rows - 1, Columns - 1].X + currentMap[Rows - 1, Columns - 1].Width), (currentMap[Rows - 1, Columns - 1].Y + currentMap[Rows - 1, Columns - 1].Height));

            //main 2d loop that pulls tile data from raw and sets it into currentMap tile position
            for (int rows = 0; rows < Rows; rows++)
            {
                for (int columns = 0; columns < Columns; columns++)
                {
                    //if there's no tile, set the map spot to null
                    if (currentMapRaw[rows, columns] == "0000")
                    {
                        currentMap[rows, columns] = null;
                    }
                    //if there is a tile in this slot, fill it with the given information
                    else
                    {
                        //first, split up the raw, then distribute the info into the tile's slots
                        char[] currentRawSplit = currentMapRaw[rows, columns].ToCharArray();

                        //if the string indicates an enemy, add them to a list then add that list to the main list at the end
                        if (currentRawSplit[0].ToString() + currentRawSplit[1].ToString() == "e1")
                        {
                            enemyHolder.Add(GetEnemy(CurrentMap[rows, columns].X, currentMap[rows, columns].Y));
                            currentMap[rows, columns] = null;
                            CurrentMapRaw[rows, columns] = null;
                        }

                        //in the case where the string doesn't indicate an enemy, it's a tile
                        else
                        {
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

                    }
                    //otherwise clean out the preset
                }

            }

            //orient the tiles based on what other tiles surround them & add tile sheets
            for (int rows = 0; rows < Rows; rows++)
            {
                for (int columns = 0; columns < Columns; columns++)
                {
                    if (currentMap[rows, columns] != null)
                    {
                        orientTiles(currentMap[rows, columns], rows, columns);
                        currentMap[rows, columns].TileSheet = defaultTileSheet;
                    }
                }
            }

            //Creates a Map needed for Path Manager based on the tiles

            #region Example of Map as String[]
            /* 
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
            #endregion

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

            return currentMap;
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
                    t.Draw(sb);
                }
            }
        }

        /// <summary>
        /// Acts like a get function for the list of enemies
        /// </summary>
        /// <param name="holder"></param>
        /// <returns></returns>
        public Enemy[] GetEnemies()
        {
            Enemy[] temp = enemyHolder.ToArray();
            enemyHolder.Clear();
            return temp;
        }

        #region Helper Functions

        /// <summary>
        /// Replaces the tile's current texture with a correctly oriented texture
        /// currently on hold until we get more of the game working
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
            if (rowNumber + 1 >= Rows)
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
            if (colNumber + 1 >= Columns)
            {
                dc3 = true;
                dc6 = true;
                dc9 = true;
            }
            #endregion

            #region Check through all viable surrounding tiles
            if (dc1 != true)
            {
                if (currentMap[rowNumber - 1, colNumber - 1] != null)
                {
                    tileConnections += "1";
                }
            }

            if (dc2 != true)
            {
                if (currentMap[rowNumber - 1, colNumber] != null)
                {
                    tileConnections += "2";
                }
            }

            if (dc3 != true)
            {
                if (currentMap[rowNumber - 1, colNumber + 1] != null)
                {
                    tileConnections += "3";
                }
            }

            if (dc4 != true)
            {
                if (currentMap[rowNumber, colNumber - 1] != null)
                {
                    tileConnections += "4";
                }
            }

            //dc5 doesn't exist on purpose -- no need to check if the current tile exists

            if (dc6 != true)
            {
                if (currentMap[rowNumber, colNumber + 1] != null)
                {
                    tileConnections += "6";
                }
            }

            if (dc7 != true)
            {
                if (currentMap[rowNumber + 1, colNumber - 1] != null)
                {
                    tileConnections += "7";
                }
            }

            if (dc8 != true)
            {
                if (currentMap[rowNumber + 1, colNumber] != null)
                {
                    tileConnections += "8";
                }
            }

            if (dc9 != true)
            {
                if (currentMap[rowNumber + 1, colNumber + 1] != null)
                {
                    tileConnections += "9";
                }
            }
            #endregion

            //Based on the concatenated result, pull the correct tile from the tile bank
            currentTile.GetTilePosFromString(tileConnections);

            //Return null if something is broken
            return null;
        }

        /// <summary>
        /// Used for the creation and storing of enemies
        /// </summary>
        /// <param name="xPos"></param>
        /// <param name="yPos"></param>
        /// <returns></returns>
        private Enemy GetEnemy(int xPos, int yPos)
        {
            Rectangle hitbox = new Rectangle(xPos, yPos, 64, 64);
            Enemy tempE = new Enemy(EnemyType.Alive, hitbox, defaultEnemy, 20);
            return tempE;
        }

        #endregion
    }
}
