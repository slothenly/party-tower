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

        private List<Cake> cakes = new List<Cake>();
        public List<Cake> Cakes { get { return cakes; } }

        private List<Exit> exits = new List<Exit>();
        public List<Exit> Exits { get { return exits; } }

        private List<Enemy> enemies = new List<Enemy>();
        public List<Enemy> Enemies { get { return enemies; } }

        public Room Above { get; set; }
        public Room Below { get; set; }
        public Room Left { get; set; }
        public Room Right { get; set; }

        public int ShiftHoriztal { get; set; }
        public int ShiftVertical { get; set; }

        //A* Path Storage for this Room
        public string[] RoomMap { get; set; }


        /// <summary>
        /// instantiates the room and adds the tileset passed in to 
        /// </summary>
        /// <param name="tileset"></param>
        public Room(Tile[,] tileset, List<Ladder> ladderList, List<Table> tableList, List<Cake> cakeList, List<Exit> exitList, string[] roomMap, List<Enemy> enemiesList)
        {
            tiles = tileset;

            Above = null;
            Below = null;
            Left  = null;
            Right = null;

            // Add Lists Here
            ladders = ladderList;
            tables = tableList;
            cakes = cakeList;
            exits = exitList;
            enemies = enemiesList;

            //A Star Map Here
            RoomMap = roomMap;
        }

        /// <summary>
        /// Shifts the tiles based on the ints passed in
        /// </summary>
        /// <param name="horizontalShift"></param>
        /// <param name="verticalShift"></param>
        public void ShiftTiles()
        {
            // Shift Tiles

            foreach (Tile t in tiles)
            {
                if(t != null)
                {
                    t.X += ShiftHoriztal;
                    t.Y += ShiftVertical;
                }
            }

            // Shift Ladders

            if (ladders != null)
            {
                foreach (Ladder l in ladders)
                {
                    l.X += ShiftHoriztal;
                    l.Y += ShiftHoriztal;
                }
            }

            // Shift Table

            if (tables != null)
            {
                foreach (Table t in tables)
                {
                    t.X += ShiftHoriztal;
                    t.Y += ShiftHoriztal;
                }
            }

            // Shift Cake

            if (cakes != null)
            {
                foreach (Cake c in cakes)
                {
                    c.X += ShiftHoriztal;
                    c.Y += ShiftHoriztal;
                }
            }

            // Shift Exits

            if (exits != null)
            {
                foreach (Exit e in exits)
                {
                    e.X += ShiftHoriztal;
                    e.Y += ShiftHoriztal;
                }
            }

            // Shift Enemies

            if (enemies != null)
            {
                foreach (Enemy e in enemies)
                {
                    e.X += ShiftHoriztal;
                    e.Y += ShiftHoriztal;
                }
            }
        }

        //TODO: Add a music queueing variable & make it accessable via the map

    }
}
