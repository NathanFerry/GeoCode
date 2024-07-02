using GeoCode.Cells.Placement;
using GeoCode.Model;

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
