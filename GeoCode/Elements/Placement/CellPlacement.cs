using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using Bentley.DgnPlatformNET;
using Bentley.DgnPlatformNET.Elements;
using Bentley.GeometryNET;
using Bentley.MstnPlatformNET;
using GeoCode.Utils;

namespace GeoCode.Cells.Placement;

public static class CellPlacement
{
    //Store the value of the placement, along to its placement tool to not compute it everytime the PlacementTool method is called.
    private static readonly Dictionary<string, string> MethodNameDictionary = typeof(PlacementTypeElement).GetMethods()
        .Where(it => it.ReturnType == typeof(PlacementTypeElement))
        .Where(it => it.IsStatic)
        .Where(it => !it.GetParameters().Any())
        .ToDictionary(it => ((PlacementTypeElement)it.Invoke(null, null)).Value, it => it.Name);
    
    public static void PlacementTool(string cellName, string cellLevel, PlacementTypeElement placement)
    {

        SharedCellDefinitionElement cellDefinition = null;
            var cells = DgnHelper.GetAllSharedCellsFromLibrary();

            foreach (var cell in cells)
            {
                if (cell.CellName == cellName)
                {
                    cellDefinition = cell;
                    break;
                }
            }
           

        cellDefinition ??= Session.Instance.GetActiveDgnFile().GetNamedSharedCellDefinitions().FirstOrDefault(element=> element.CellName == cellName);

        if (cellDefinition == null)
        {
            MessageBox.Show("Aucune cellule à ce nom ("+ cellName +") n'a été retrouvée", "Erreur !");
            return;
        }
         
        var level = DgnHelper.GetAllLevelsFromLibrary()
            .First(element => element.Name == cellLevel);

        new ElementPropertiesSetter().SetLevelChain(level.LevelId).SetLineStyleChain(level.GetByLevelLineStyle()).SetColorChain(level.GetByLevelColor().Color).Apply(cellDefinition);

        
        Log.Write(level.Name);
        try
        {
            //Using reflection to invoke InstallNewInstance method of tools. Placement tool must be named following this pattern:
            //<Placement Type>PlaceTool.
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
        
        var cellDefinition = DgnHelper.GetAllSharedCellsFromLibrary()
            .First(element => element.CellName == GeoCode.Application.PtTopo);
        var level = DgnHelper.GetAllLevelsFromLibrary()
            .First(element => element.Name == GeoCode.Application.LevelTopo);


        if (GeoCode.Application.PlaceTopo)
        {
            new ElementPropertiesSetter().SetLevelChain(level.LevelId).SetColorChain(level.GetByLevelColor().Color).Apply(cellDefinition);
        
            var topoPoint = SharedCellHelper.CreateSharedCell(cellDefinition, ev.Point);
            topoPoint.ApplyTransform(new TransformInfo(DTransform3d.Scale(1)));
            topoPoint.AddToModel();
        }
    }
}