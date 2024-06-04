using System;

namespace GeoCode.Cells.Placement;

[AttributeUsage(AttributeTargets.Class)]
public class PlacementMethod : Attribute
{
    public string Placement;

    public PlacementMethod(string placement)
    {
        Placement = placement;
    }
}