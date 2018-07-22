using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Party_Tower_Main
{
    class Room
    {
        //TODO: Add the holder & accessability for the basic maps
        //TODO: Make a constructor to fill that holder
        //TODO: Add something that tells the level whats above, below, left, and right of it
        private Tile[,] tiles = new Tile[9, 16];
        public Tile[,] Tiles { get { return tiles; } }

        public Room Above { get; set; }
        public Room Below { get; set; }
        public Room Left { get; set; }
        public Room Right { get; set; }

        public int ShiftHoriztal { get; set; }
        public int ShiftVertical { get; set; }

        public Room(Tile[,] tileset)
        {
            tiles = tileset;

            Above = null;
            Below = null;
            Left  = null;
            Right = null;
        }

        /// <summary>
        /// Shifts the tiles based on the ints passed in
        /// </summary>
        /// <param name="horizontalShift"></param>
        /// <param name="verticalShift"></param>
        public void ShiftTiles()
        {
            foreach (Tile t in tiles)
            {
                if(t != null)
                {
                    t.X += ShiftHoriztal;
                    t.Y += ShiftVertical;
                }
            }
        }

        //TODO: Add a music queueing variable & make it accessable via the map

    }
}
