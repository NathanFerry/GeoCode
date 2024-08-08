using Bentley.DgnPlatformNET;
using Bentley.DgnPlatformNET.Elements;
using Bentley.GeometryNET;
using Bentley.MstnPlatformNET;
using GeoCode.Elements.Drawing;
using System.Windows.Media;

namespace GeoCode.Cells.Placement.PlacementTools;
public class OnePointPlaceTool : DgnPrimitiveTool
{
    private SharedCellDefinitionElement _cellDefinition;
    private SharedCellElement _cellElement;
    public OnePointPlaceTool(SharedCellDefinitionElement cellDefinition, int toolName, int toolPrompt) : base(toolName, toolPrompt)
    {
        _cellDefinition = cellDefinition;
        _cellElement = SharedCellHelper.CreateSharedCell(cellDefinition, DPoint3d.Zero);
    }

    protected override bool OnDataButton(DgnButtonEvent ev)
    {
        _cellElement.AddToModel();
        CellPlacement.PlaceTopoPoint(ev);

        return true;
    }

    protected override void OnPostInstall()
    {
        AccuSnap.SnapEnabled = true;
        AccuDraw.Active= true;
        BeginDynamics();
    }

    protected override void OnDynamicFrame(DgnButtonEvent ev)
    {

        Draw.TranslateCell(_cellElement, ev.Point);
        Draw.DrawDynamicElement(_cellElement, ev);
    } 

    protected override bool OnResetButton(DgnButtonEvent ev)
    {
        ExitTool();
        return true;
    }


    protected override void OnRestartTool()
    {
        InstallNewInstance(_cellDefinition);
    }

    public static void InstallNewInstance(SharedCellDefinitionElement cellDefinition)
    {
        new OnePointPlaceTool(cellDefinition, 1, 1).InstallTool();
    }
    
}