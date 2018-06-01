using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Party_Tower_Main
{
    class PathManager
    {
        ///<summary>
        ///</summary>

        //fields ------------------------------------------------------------------------
        private List<Location> openList;            // List of unchecked nodes
        private List<Location> closedList;          // List of checked nodes - final path held in here
        private Vector2 p1RealPosition;             // Cordinates of the Player's True Position
        private Vector2 p2RealPosition;             // Cordinates of the Players True Position
        private Location p1Location;                // Cordinates of the tile that player1 rests in
        private Location p2Location;                // Cordinates of the tile that player2 rests
        private string[] p1Map;                     // Curent Array of Strings that depicts the map of the level with Player 1
        private string[] p2Map;                     // Curent Array of Strings that depicts the map of the level with Player 2
        private string[] correctMap;                // Field that gets set to either p1Map or p2Map based on what player the enemy is looking for. 
        private Location current;                   // Current Location being checked
        private Location start;                     // Starting Location - The location of the enemy during an update
        private Location target;                    // Target Location - The location of the Player during an update
        private bool usePlayer1;                    // Bool that determines if the most recent player checked for this enemy was Player 1 or Player 2

        int g = 0;                                  

        //tile constants to determine player and enemy positions on map
        private int heightConstant;                 // based on minimum # of tiles it base resolution allows - height
        private int widthConstant;                  // based on minimum # of tiles it base resolution allows - width

        //properties --------------------------------------------------------------------

        public string[] P1MapOfLevel
        {
            get { return p1Map; }
        }

        public string[] P2MapOfLevel
        {
            get { return p1Map; }
        }

        public int HeightConstant
        {
            get { return heightConstant; }
        }

        public int WidthConstant
        {
            get { return widthConstant; }
        }

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
            };
         */

        /*
         * PATHMANAGER KEY
         * P = Player
         * E = Enemy
         * |, +, - = "OUT OF BOUNDS"
         * _ = "PLATFORM"
         * X = "DAMAGING PLATFORM"
         * ^, >, <. ~ = MOVEABLE PLATFORMS IN CERTAIN DIRECTIONS
         */

        //constructor -------------------------------------------------------------------
        public PathManager(Viewport view)
        {
            openList = new List<Location>();
            closedList = new List<Location>();
            p1Map = new string[] { };
            p2Map = new string[] { };
            SetMapConstants(view.Height, view.Width);
        }

        //methods -----------------------------------------------------------------------

        /// <summary>
        /// Updates inner constants needed for calculations. Call if the user changes resolution sizes.
        /// (IE: 1920 by 1080 to 1080 by 720, etc.)
        /// </summary>
        /// <param name="resolutionHeight">Current Screen Resolution Height</param>
        /// <param name="resolutionWidth"> Current Screen Resolution Width</param>
        public void SetMapConstants(int resolutionHeight, int resolutionWidth )
        {
            // Confirm this logic w/ Justin

            // 9 tiles up / down loaded on screen at minimum
            // 16 tiles left right loaded on screen at minimum.

            // So, the logic is to divide the screen size by the total tiles allowed in each direction to get the tile constants of x and y
            heightConstant = resolutionHeight / 9;
            widthConstant = widthConstant / 16;
            
        }

        /// <summary>
        /// Creates a map of the level for the enemy to follow for either player. 
        /// Only needs called once per Update Loop, before checking enemys.
        /// </summary>
        /// <param name="newMap"> Map of the current level </param>
        /// <param name="player1"> Hitbox of Player 1 </param>
        /// <param name="player2"> Hitbox of Player 2 </param>
        public void UpdatePlayersOnMap(string[] newMap, Rectangle player1, Rectangle player2)
        {
            // Center point of player
            Vector2 p1RealPosition = new Vector2(player1.X, player1.Y);
            Vector2 p2RealPosition = new Vector2(player2.X, player2.Y);

            //Records the correct tile cordinates based on the location of the player
            p1Location = new Location { X = (player1.Center.X / widthConstant) + 1, Y =  (player1.Center.Y / heightConstant) + 1 };
            p2Location = new Location { X = (player2.Center.X / widthConstant) + 1, Y = (player2.Center.Y / heightConstant) + 1};

            //Creates new "original" maps for player1 and player2 to be set in
            p1Map = newMap;
            p2Map = newMap;

            //Sets the respective char in the respective string within the string array to include Player 1
            var p1mapLine = newMap[p1Location.Y];
            var aStringBuilder = new StringBuilder(p1mapLine);
            aStringBuilder.Remove(p1Location.X, 1);
            aStringBuilder.Insert(p1Location.X, "P");
            p1mapLine = aStringBuilder.ToString();

            //Creates the map to find player 1 
            p1Map[p1Location.Y] = p1mapLine;

            //Sets the respective char in the respective string within the string array to include Player 2
            var p2mapLine = newMap[p2Location.Y];
            aStringBuilder = new StringBuilder(p2mapLine);
            aStringBuilder.Remove(p2Location.X, 1);
            aStringBuilder.Insert(p2Location.X, "P");
            p2mapLine = aStringBuilder.ToString();

            //Creates the map to find player 2
            p2Map[p2Location.Y] = p2mapLine;
        }

        /// <summary>
        /// Determines the target Location that this particular Enemy wants to go to.
        /// </summary>
        /// <param name="enemyX"> Enemy center X pos </param>
        /// <param name="enemyY"> Enemy center y pos </param>
        /// <returns></returns>
        private Location DetermineTarget(int enemyX, int enemyY)
        {
            // Calculates H scores before hand

            int p1H = ComputeHScore(enemyX, enemyY, p1Location.X, p1Location.Y);
            int p2H = ComputeHScore(enemyX, enemyY, p1Location.X, p1Location.Y);

            // If P2 is further , then enemy follows P1
            if (p2H > p1H)
            {
                correctMap = p1Map;
                usePlayer1 = true;
                return new Location { X = p1Location.X, Y = p1Location.Y };
            }

            // If P1 is further or they are roughly = distance away, then enemy follows P2
            correctMap = p2Map;
            usePlayer1 = false;
            return new Location { X = p2Location.X, Y = p2Location.Y };
        }

        /// <summary>
        /// Determines the closest Point in the game to which the Enemy should move too. 
        /// </summary>
        /// <param name="e"></param>
        /// <returns> Returns the Vector2 of the point to which the Enemy should move to!</returns>
        public Vector2 Following(Enemy e)
        {
            var start = new Location { X = (e.Hitbox.Center.X / widthConstant) + 1, Y = (e.Hitbox.Center.Y / heightConstant) + 1 };
            var target = DetermineTarget(start.X, start.Y);
            var openList = new List<Location>();
            var closedList = new List<Location>();
            int g = 0;

            // start by adding the original position to the open list
            openList.Add(start);

            while (openList.Count > 0)
            {
                // get the square with the lowest F score
                var lowest = openList.Min(l => l.F);
                current = openList.First(l => l.F == lowest);

                // add the current square to the closed list
                closedList.Add(current);

                // remove it from the open list
                openList.Remove(current);

                // if we added the destination to the closed list, we've found a path
                if (closedList.FirstOrDefault(l => l.X == target.X && l.Y == target.Y) != null)
                    break;

                var adjacentSquares = GetWalkableAdjacentSquares(current.X, current.Y, correctMap);
                g++;

                foreach (var adjacentSquare in adjacentSquares)
                {
                    // if this adjacent square is already in the closed list, ignore it
                    if (closedList.FirstOrDefault(l => l.X == adjacentSquare.X
                            && l.Y == adjacentSquare.Y) != null)
                        continue;

                    // if it's not in the open list...
                    if (openList.FirstOrDefault(l => l.X == adjacentSquare.X
                            && l.Y == adjacentSquare.Y) == null)
                    {
                        // compute its score, set the parent
                        adjacentSquare.G = g;
                        adjacentSquare.H = ComputeHScore(adjacentSquare.X, adjacentSquare.Y, target.X, target.Y);
                        adjacentSquare.F = adjacentSquare.G + adjacentSquare.H;
                        adjacentSquare.Parent = current;

                        // and add it to the open list
                        openList.Insert(0, adjacentSquare);
                    }
                    else
                    {
                        // test if using the current G score makes the adjacent square's F score
                        // lower, if yes update the parent because it means it's a better path
                        if (g + adjacentSquare.H < adjacentSquare.F)
                        {
                            adjacentSquare.G = g;
                            adjacentSquare.F = adjacentSquare.G + adjacentSquare.H;
                            adjacentSquare.Parent = current;
                        }
                    }
                }
            }

            float xReturn;  // X value of point this Enemy needs to move to
            float yReturn;  // Y value of point this Enemy needs to move to

            // The next square of which the enemy must move to.
            // Returns the mid point of the width of tile, and the height of the enemy sprite to set the point to which the enemy should move to.
            if (closedList.Count > 1)
            {
                xReturn = (closedList[1].X - 1) * widthConstant + (widthConstant / 2);
                yReturn = (closedList[1].Y - 1) * heightConstant + (heightConstant - e.Height);
                return new Vector2(xReturn, yReturn);
            }

            // This case occurs if the enemy and player are in the same square.

            // This instance of A* used P2, so travel to the center of Player 2
            if (!usePlayer1)
            {
                return p2RealPosition;
            }

            // This instance of A* used P1, so travel to the center of Player 1
            return p1RealPosition;
        }

        /// <summary>
        /// Returns a list of potental Adjacent Squares that could be the next "current" var when using A*
        /// </summary>
        /// <param name="x"> Current X Location Value </param>
        /// <param name="y"> Current Y Location Value </param>
        /// <param name="map"></param>
        /// <returns></returns>
        private List<Location> GetWalkableAdjacentSquares(int x, int y, string[] map)
        {
            var proposedLocations = new List<Location>()
            {
                new Location { X = x, Y = y - 1 },
                new Location { X = x, Y = y + 1 },
                new Location { X = x - 1, Y = y },
                new Location { X = x + 1, Y = y },
            };

            return proposedLocations.Where(l => map[l.Y][l.X] == ' ' || map[l.Y][l.X] == 'P').ToList();
        }

        /// <summary>
        /// Figures out the H Score
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="targetX"></param>
        /// <param name="targetY"></param>
        /// <returns></returns>
        private int ComputeHScore(int x, int y, int targetX, int targetY)
        {
            return Math.Abs(target.X - x) + Math.Abs(target.Y - y);
        }


    }
}



