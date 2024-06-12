using Bentley.Interop.MicroStationDGN;
using GeoCode.Cells.Placement;
using GeoCode.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GeoCode.Saving
{
    static class SavedCellToPaste
    {
        public static string name;
        public static string level;
        public static PlacementTypeElement placement;

        public static void Copy(Cell cell)
        {
            if (cell == null) { return; }
            name = cell.Name;
            level = cell.Level;
            placement = cell.Placement;
        }
    }
}
