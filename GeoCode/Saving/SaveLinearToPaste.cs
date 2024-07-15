using GeoCode.Cells.Placement;
using GeoCode.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoCode.Saving
{
    static class SaveLinearToPaste
    {
        
            public static string label;
            public static double? value;
            public static string level;
            public static LinearPlacementTypeElement placement;

            public static void Copy(Linear linear)
            {
                if (linear == null) { return; }
                label = linear.Label;
                value = linear.Value;
                level = linear.Level;
                placement = linear.Placement;
            }
        
    }
}
