using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using Bentley.DgnPlatformNET;
using Bentley.DgnPlatformNET.Elements;
using Bentley.GeometryNET;
using Bentley.MstnPlatformNET;
using GeoCode.Model;
using GeoCode.Utils;

namespace GeoCode.Cells.Placement;

public static class LinearPlacement
{
    //Store the value of the placement, along to its placement tool to not compute it everytime the PlacementTool method is called.
    private static readonly Dictionary<string, string> MethodNameDictionary = typeof(LinearPlacementTypeElement).GetMethods()
        .Where(it => it.ReturnType == typeof(LinearPlacementTypeElement))
        .Where(it => it.IsStatic)
        .Where(it => !it.GetParameters().Any())
        .ToDictionary(it => ((LinearPlacementTypeElement)it.Invoke(null, null)).Value, it => it.Name);
    
    public static void LinearPlacementTool(Linear linear, LinearPlacementTypeElement placement)
    {
        //new ElementPropertiesSetter().SetLevelChain(level.LevelId).SetColorChain(level.GetByLevelColor().Color).Apply(lineElement);
        try
        {
            //Using reflection to invoke InstallNewInstance method of tools. Placement tool must be named following this pattern:
            //<Placement Type>PlaceTool.
            var typeName =  "GeoCode.Cells.Placement.LinearPlacementTools." + MethodNameDictionary[placement.Value] + "PlaceTool";
            Log.Write(typeName);
            Assembly.GetExecutingAssembly().GetType(typeName).GetMethod("InstallNewInstance")
                .Invoke(null, new[] { linear, });
        }
        catch (Exception e)
        {
            Log.Write(e.ToString());
        }
    }
}