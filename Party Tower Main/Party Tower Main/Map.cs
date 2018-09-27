using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Party_Tower_Main
{
    /// <summary>
    /// Functions very similarly to a linked list, however works in 2 dimensions
    /// </summary>
    class Map
    {
        #region Marco Data 
        // Holds Camera Limiters to be set within Dynamic Camera.SetMapEdge() when Map's Switch

        public int Above { get; set; }
        public int Below { get; set; }
        public int Right { get; set; }
        public int Left { get; set; }

        #endregion Marco Data

        private Room root;
        public Room Root { get { return root; } }
        private Queue<Room> levelsToPlace = new Queue<Room>();
        LevelMapCoordinator lvlMap;
        Room[,] aStarArray;
        public Room[,] AStartArray { get { return aStarArray; } }
        int rowTicker;
        int columnTicker;
        int rowMax;

        //used to keep track of what the level number is (order of played and level selection), hard coded for now
        private int levelNumber;
        public int LevelNumber
        {
            get { return levelNumber; }
            set { levelNumber = value; }
        }

        private List<Room> levels;
        public List<Room> Levels { get { return levels; } }

        Tile measurementTile;

        private int count = 0;
        public int Count { get { return count; } }

        public Map(Tile exampleTile, int levelNumber, LevelMapCoordinator lvlMap, int rows, int columns)
        {
            measurementTile = exampleTile;
            levels = new List<Room>();
            this.levelNumber = levelNumber;
            this.lvlMap = lvlMap;

            aStarArray = new Room[rows, columns];
            rowTicker = 0;
            columnTicker = 0;
            rowMax = rows;
        }

        //stores the important objects which need to be returned after each room gets set up
        public List<GameObject> MapImportantObjects()
        {
            List<GameObject> importantItems = new List<GameObject>();

            foreach(Room r in levels)
            {
                importantItems.AddRange(r.ImportantObjects());
            }
            return importantItems;
        }

        //stores where all of the enemies are on this map
        public List<Enemy> MapEnemies()
        {
            List<Enemy> mapenemies = new List<Enemy>();

            foreach (Room r in levels)
            {
                mapenemies.AddRange(r.RoomEnemies());
            }
            return mapenemies;
        }

        /// <summary>
        /// Adds a level to the map
        /// </summary>
        /// <param name="importedRoom"></param>
        public List<GameObject> AddRoom(string path, string whereToPlace, string relativePlacement)
        {
            //fields
            char[] where;
            Room current = root;
            Tile[,] roomMap;

            //If the path is broken, just set the path to the null room. This is how to set empty rooms too.
            try
            {
                roomMap = lvlMap.UpdateMapFromPath(path);
            }
            catch (Exception)
            {
                roomMap = lvlMap.UpdateMapFromPath("nullRoom");               
            }
            
            //creates the room by pulling info from lvlMap
            Room temp = new Room(roomMap, lvlMap.LadderHolder, lvlMap.TableHolder, lvlMap.CakeHolder, lvlMap.ExitHolder, 
                                lvlMap.PathManagerMap, lvlMap.EnemyHolder);
            lvlMap.UpdateMapFromPath(path);

            //modify placement based on if it's the root node
            if (root == null)
            {
                temp.ShiftHoriztal = 0;
                temp.ShiftHoriztal = 0;
                root = temp;
            }
            else
            {
                levelsToPlace.Enqueue(temp);
            }
            levels.Add(temp);

            //extrapolate where to place the room based on whereToPlace relatice to the root node
            where = whereToPlace.ToCharArray();
            if (where != null)
            {
                foreach (char direction in where)
                {
                    switch (direction)
                    {
                        case 'a':
                            current = current.Above;
                            break;

                        case 'b':
                            current = current.Below;
                            break;

                        case 'l':
                            current = current.Left;
                            break;

                        case 'r':
                            current = current.Right;
                            break;

                        default:
                            break;
                    }
                }

            }

            //actually place based on "relaticePlacement"
            switch (relativePlacement)
            {
                case "a":
                    PlaceAbove(current);
                    break;

                case "b":
                    PlaceBelow(current);
                    break;

                case "l":
                    PlaceLeft(current);
                    break;

                case "r":
                    PlaceRight(current);
                    break;

                default:
                    break;
            }

            //change internal counters and place into aStarArray
            count++;
            aStarArray[rowTicker, columnTicker] = temp;
            columnTicker++;
            if (columnTicker >= rowMax)  
            {
                columnTicker = 0;
                rowTicker++;
            }


            //return the list of important objects
            return temp.ImportantObjects();
        }

        #region Placing Levels
        /// <summary>
        /// Places the next item in the levels queue to the left of the level included
        /// </summary>
        /// <param name="stem"></param>
        public void PlaceLeft(Room stem)
        {
            Room current = levelsToPlace.Dequeue();
            stem.Left = current;

            //All this is a fancy way of checking if this needs to hook up with other levels
            if (stem.Above != null)
            {
                if (stem.Above.Left != null) 
                {
                    //checking the one above current
                    stem.Above.Left.Below = current;

                    if (stem.Above.Left.Left != null)
                    {
                        if (stem.Above.Left.Left.Below != null)
                        {
                            //checking the one to the left of current
                            stem.Above.Left.Left.Below.Right = current;
                            if (stem.Above.Left.Left.Below.Below != null)
                            {
                                if (stem.Above.Left.Left.Below.Below.Right != null)
                                {
                                    //checking the one below current
                                    stem.Above.Left.Left.Below.Below.Right.Above = current;
                                }
                            }
                        }
                    }
                }
            }

            if (stem.Below != null)
            {
                if (stem.Below.Left != null) 
                {
                    //checking the one below current
                    stem.Below.Left.Above = current;

                    if (stem.Below.Left.Left != null)
                    {
                        if (stem.Below.Left.Left.Above != null)
                        {
                            //checking the one to the left of current
                            stem.Below.Left.Left.Above.Right = current;

                            if (stem.Below.Left.Left.Above.Above != null)
                            {
                                if (stem.Below.Left.Left.Above.Above.Right != null)
                                {
                                    //checking the one above current
                                    stem.Below.Left.Left.Above.Above.Right.Below = current;
                                }
                            }
                        }
                    }
                }
            }

            //change the positions of the tiles relative to the stem
            current.ShiftHoriztal = stem.ShiftHoriztal - (measurementTile.Width * 16);
            current.ShiftVertical = stem.ShiftVertical;
            current.ShiftTiles();
        }

        /// <summary>
        /// Places the next item in the queue to the right of the level included
        /// </summary>
        /// <param name="stem"></param>
        public void PlaceRight(Room stem)
        {
            Room current = levelsToPlace.Dequeue();
            stem.Right = current;

            if (stem.Below != null)
            {
                if (stem.Below.Right != null)
                {
                    //checking the one under current
                    stem.Below.Right.Above = current;

                    if (stem.Below.Right.Right != null)
                    {
                        if (stem.Below.Right.Right.Above != null)
                        {
                            //checking the one to the right of current
                            stem.Below.Right.Right.Above.Left = current;

                            if (stem.Below.Right.Right.Above.Above != null)
                            {
                                if (stem.Below.Right.Right.Above.Above.Left != null)
                                {
                                    //checking the one above current
                                    stem.Below.Right.Right.Above.Above.Left.Below = current;
                                }
                            }
                        }
                    }
                }
            }

            if (stem.Above != null)
            {
                if (stem.Above.Right != null)
                {
                    //checking above current
                    stem.Above.Right.Below = current;

                    if (stem.Above.Right.Right != null)
                    {
                        if (stem.Above.Right.Right.Below != null)
                        {
                            //checking to to the right of current
                            stem.Above.Right.Right.Below.Left = current;

                            if  (stem.Above.Right.Right.Below.Below != null)
                            {
                                if (stem.Above.Right.Right.Below.Below.Left != null)
                                {
                                    //checking below current
                                    stem.Above.Right.Right.Below.Below.Left.Above = current;
                                }
                            }
                        }
                    }
                }
            }

            //change the positions of the tiles relative to the stem
            current.ShiftHoriztal = stem.ShiftHoriztal + (measurementTile.Width * 16);
            current.ShiftVertical = stem.ShiftVertical;
            current.ShiftTiles();
        }

        /// <summary>
        /// Places the next item in the queue above the level included
        /// </summary>
        /// <param name="stem"></param>
        public void PlaceAbove(Room stem)
        {
            Room current = levelsToPlace.Dequeue();
            stem.Above = current;

            if (stem.Left != null)
            {
                if (stem.Left.Above != null)
                {
                    //checking the one under current
                    stem.Left.Above.Right = current;

                    if (stem.Left.Above.Above != null)
                    {
                        if (stem.Left.Above.Above.Right != null)
                        {
                            //checking the one to the right of current
                            stem.Left.Above.Above.Right.Below = current;

                            if (stem.Left.Above.Above.Right.Right != null)
                            {
                                if (stem.Left.Above.Above.Right.Right.Below != null)
                                {
                                    //checking the one above current
                                    stem.Left.Above.Above.Right.Right.Below.Left = current;
                                }
                            }
                        }
                    }
                }
            }

            if (stem.Right != null)
            {
                if (stem.Right.Above != null)
                {
                    //checking above current
                    stem.Right.Above.Left = current;

                    if (stem.Right.Above.Above != null)
                    {
                        if (stem.Right.Above.Above.Left != null)
                        {
                            //checking to to the right of current
                            stem.Right.Above.Above.Left.Below = current;

                            if (stem.Right.Above.Above.Left.Left != null)
                            {
                                if (stem.Right.Above.Above.Left.Left.Below != null)
                                {
                                    //checking below current
                                    stem.Right.Above.Above.Left.Left.Below.Right = current;
                                }
                            }
                        }
                    }
                }
            }

            //change the positions of the tiles relative to the stem
            current.ShiftHoriztal = stem.ShiftHoriztal;
            current.ShiftVertical = stem.ShiftVertical - (measurementTile.Width * 16); 
            current.ShiftTiles();
        }

        /// <summary>
        /// Places the next item in the queue below the level included
        /// </summary>
        /// <param name="stem"></param>
        public void PlaceBelow(Room stem)
        {
            Room current = levelsToPlace.Dequeue();
            stem.Below = current;

            if (stem.Left != null)
            {
                if (stem.Left.Below != null)
                {
                    //checking the one under current
                    stem.Left.Below.Right = current;

                    if (stem.Left.Below.Below != null)
                    {
                        if (stem.Left.Below.Below.Right != null)
                        {
                            //checking the one to the right of current
                            stem.Left.Below.Below.Right.Above = current;

                            if (stem.Left.Below.Below.Right.Right != null)
                            {
                                if (stem.Left.Below.Below.Right.Right.Above != null)
                                {
                                    //checking the one above current
                                    stem.Left.Below.Below.Right.Right.Above.Right = current;
                                }
                            }
                        }
                    }
                }
            }

            if (stem.Right != null)
            {
                if (stem.Right.Below != null)
                {
                    //checking above current
                    stem.Right.Below.Left = current;

                    if (stem.Right.Below.Below != null)
                    {
                        if (stem.Right.Below.Below.Left != null)
                        {
                            //checking to to the right of current
                            stem.Right.Below.Below.Left.Above = current;

                            if (stem.Right.Below.Below.Left.Left != null)
                            {
                                if (stem.Right.Below.Below.Left.Left.Above != null)
                                {
                                    //checking below current
                                    stem.Right.Below.Below.Left.Left.Above.Right = current;
                                }
                            }
                        }
                    }
                }
            }

            //change the positions of the tiles relative to the stem
            current.ShiftHoriztal = stem.ShiftHoriztal;
            current.ShiftVertical = stem.ShiftVertical + (measurementTile.Width * 16);
            current.ShiftTiles();
        }
        #endregion
    }
}
