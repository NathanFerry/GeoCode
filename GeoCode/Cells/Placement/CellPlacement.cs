using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bentley.DgnPlatformNET;
using Bentley.GeometryNET;
using Bentley.MstnPlatformNET;
using GeoCode.Utils;

namespace GeoCode.Cells.Placement;

public static class CellPlacement
{
    private static readonly Dictionary<string, string> MethodNameDictionary = typeof(PlacementTypeElement).GetMethods()
        .Where(it => it.ReturnType == typeof(PlacementTypeElement))
        .Where(it => it.IsStatic)
        .Where(it => !it.GetParameters().Any())
        .ToDictionary(it => ((PlacementTypeElement)it.Invoke(null, null)).Value, it => it.Name);
    public static void PlacementTool(string cellName, string cellLevel, PlacementTypeElement placement)
    {
        var cellDefinition = Session.Instance.GetActiveDgnFile().GetNamedSharedCellDefinitions()
            .First(element => element.CellName == cellName);
        var level = Session.Instance.GetActiveDgnFile().GetLevelCache().GetHandles()
            .First(element => element.Name == cellLevel);
        new ElementPropertiesSetter().SetLevelChain(level.LevelId).SetColorChain(level.GetByLevelColor().Color).Apply(cellDefinition);
        try
        {
            var typeName =  "GeoCode.Cells.Placement.PlacementTools." + MethodNameDictionary[placement.Value] + "PlaceTool";
            Assembly.GetExecutingAssembly().GetType(typeName).GetMethod("InstallNewInstance")
                .Invoke(null, new[] { cellDefinition });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public static void PlaceTopoPoint(DgnButtonEvent ev)
    {
        //TODO: Altitude of the point (translate by moving it along the Z axis)
        if(ev.IsControlKey) return;
        
        var cellDefinition = Session.Instance.GetActiveDgnFile().GetNamedSharedCellDefinitions()
            .First(element => element.CellName == GeoCode.Application.PtTopo);
        var level = Session.Instance.GetActiveDgnFile().GetLevelCache().GetHandles() 
            .First(element => element.Name == GeoCode.Application.LevelTopo);
        new ElementPropertiesSetter().SetLevelChain(level.LevelId).SetColorChain(level.GetByLevelColor().Color).Apply(cellDefinition);
        
        var topoPoint = SharedCellHelper.CreateSharedCell(cellDefinition, ev.Point);
        topoPoint.ApplyTransform(new TransformInfo(DTransform3d.Scale(1)));
        topoPoint.AddToModel();
    }
}