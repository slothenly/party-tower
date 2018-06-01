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

        int Rows;       //both are intentionally capitalized to make for easier recognition
        int Columns;    //sorry if it irks you

        /// <summary>
        /// Constructor which pulls the initial path to take tile info from
        /// </summary>
        /// <param name="initialPath"></param>
        public LevelMapCoordinator(string initialPath)
        {
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
            int row;
            int column;
            int c = 0;
            string tempString = "";
            string[] infoTempHolder;

            StreamReader interpreter = new StreamReader(@"..\..\Resources\levelExports\" + path + ".txt");

            //Setup for creating the level's 2d array (pulls Row and Column counts, splits the rest into a 2D array)
            line = interpreter.ReadLine(); 
            infoTempHolder = line.Split(',');
            Rows = int.Parse(infoTempHolder[0]); 
            Columns = int.Parse(infoTempHolder[1]);
            string[,] importedTileInfo = new string[Rows, Columns];

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
        }
    }
}
