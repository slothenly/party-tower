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

        private List<Ladder> ladders = new List<Ladder>();
        public List<Ladder> Ladders { get { return ladders; } }

        private List<Table> tables = new List<Table>();
        public List<Table> Table { get { return tables; } }

        public Room Above { get; set; }
        public Room Below { get; set; }
        public Room Left { get; set; }
        public Room Right { get; set; }

        public int ShiftHoriztal { get; set; }
        public int ShiftVertical { get; set; }


        /// <summary>
        /// instantiates the room and adds the tileset passed in to 
        /// </summary>
        /// <param name="tileset"></param>
        public Room(Tile[,] tileset, List<Ladder> ladderList, List<Table> tableList, List<Cake> cakeList, List<Exit> exitList)
        {
            tiles = tileset;

            Above = null;
            Below = null;
            Left  = null;
            Right = null;

            ladders = ladderList;
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

            if (ladders != null)
            {
                foreach (Ladder l in ladders)
                {
                    l.X += ShiftHoriztal;
                    l.Y += ShiftHoriztal;
                }
            }
        }

        //TODO: Add a music queueing variable & make it accessable via the map

    }
}
