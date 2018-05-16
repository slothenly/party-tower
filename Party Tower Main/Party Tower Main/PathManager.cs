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
        ///Jacob LeBerth Jacob Marcovechio
        ///NodeManager class
        ///holds a list of nodes that encompass the games map
        ///tracks groupings of nodes and their neighbors for passability
        /// and handles other node based logic
        ///</summary>

        //fields ------------------------------------------------------------------------
        private List<Location> openList;            // List of unchecked nodes
        private List<Location> closedList;          // List of checked nodes - final path held in here
        private string[] map;                       // Curent Array of Strings that depicts the map of the level
        private Location current;                   // Current Location being checked
        private Location start;                     // Starting Location - The location of the enemy during an update
        private Location target;                    // Target Location - The location of the Player during an update
        int g = 0;                                  

        //properties --------------------------------------------------------------------

        public string[] mapLevel
        {
            set { map = value; }
        }

        //constructor -------------------------------------------------------------------
        public PathManager()
        {
            openList = new List<Location>();
            closedList = new List<Location>();
            map = new string[] { };
        }

        //methods -----------------------------------------------------------------------

        // 
        public void Following()
        {
            var start = new Location {};
            var target = new Location {};
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

                var adjacentSquares = GetWalkableAdjacentSquares(current.X, current.Y, map);
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
        }

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



