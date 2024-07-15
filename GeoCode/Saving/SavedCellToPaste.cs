using GeoCode.Cells.Placement;
using GeoCode.Model;

namespace GeoCode.Saving
{
    static class SavedCellToPaste
    {
        public static string label;
        public static string name;
        public static string level;
        public static PlacementTypeElement placement;

        public static void Copy(Cell cell)
        {
            if (cell == null) { return; }
            label = cell.Label;
            name = cell.Name;
            level = cell.Level;
            placement = cell.Placement;
        }
    }
}
