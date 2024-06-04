
using System;
using System.Linq;
using Bentley.DgnPlatformNET;
using Bentley.DgnPlatformNET.Elements;
using Bentley.GeometryNET;
using Bentley.MstnPlatformNET;

namespace GeoCode.Cells.Placement;

public static class CellPlacement
{
    public static void PlacementTool(string cellName, string cellLevel, PlacementTypeElement placement)
    {
        var cellDefinition = Session.Instance.GetActiveDgnFile().GetNamedSharedCellDefinitions()
            .First(element => element.CellName == cellName);
        var level = Session.Instance.GetActiveDgnFile().GetLevelCache().GetHandles()
            .First(element => element.Name == cellLevel);
        new ElementPropertiesSetter().SetLevelChain(level.LevelId).Apply(cellDefinition);
    }
}