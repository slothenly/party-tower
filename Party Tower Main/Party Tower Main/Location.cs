using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Party_Tower_Main
{
    class Location
    {
        public int X; // X cordinate in array of strings
        public int Y; // Y cordinate in array of string
        public int F; // Score for each Square - F = G + H
        public int G; // Cost from Starting Square to Tile
        public int H; // Estimated Cost of from Location to Target
        public Location Parent; // Parent Location this one came from
    }
}
