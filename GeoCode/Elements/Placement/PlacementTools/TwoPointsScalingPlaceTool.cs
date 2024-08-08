using Bentley.DgnPlatformNET;
using Bentley.DgnPlatformNET.Elements;
using Bentley.GeometryNET;
using GeoCode.Elements.Drawing;

namespace GeoCode.Cells.Placement.PlacementTools;

public class TwoPointsScalingPlaceTool : DgnPrimitiveTool
{
    private readonly SharedCellDefinitionElement _cellDefinition;
    private readonly SharedCellElement _cellElement;
    private DPoint3d? _origin;
    
    public TwoPointsScalingPlaceTool(SharedCellDefinitionElement cellDefinition, int toolName, int toolPrompt) : base(toolName, toolPrompt)
    {
        _cellDefinition = cellDefinition;
        _cellElement = SharedCellHelper.CreateSharedCell(cellDefinition, DPoint3d.Zero);
    }
    protected override void OnPostInstall()
    {
        AccuSnap.SnapEnabled = true;
        BeginDynamics();
    }
    protected override bool OnDataButton(DgnButtonEvent ev)
    {  
        
        if (_origin is null)
        {
            _origin = ev.Point;
            CellPlacement.PlaceTopoPoint(ev);
            return false;
        }

        _cellElement.AddToModel();
        CellPlacement.PlaceTopoPoint(ev);

        _origin = null;

        return true;
    }
    
    protected override void OnDynamicFrame(DgnButtonEvent ev)
    {
        if (_origin is null)
        {
            Draw.TranslateCell(_cellElement, ev.Point);
        }
        else
        {
            Draw.ScaleCellSquare(_cellElement,_origin.Value,ev.Point);
        }
        
        Draw.DrawDynamicElement(_cellElement, ev);
    } 

    protected override bool OnResetButton(DgnButtonEvent ev)
    {
        ExitTool();
        return true;
    }

    protected override void OnRestartTool()
    {
        _origin = null;
        InstallNewInstance(_cellDefinition);
    }

    public static void InstallNewInstance(SharedCellDefinitionElement cellDefinition)
    {
        new TwoPointsScalingPlaceTool(cellDefinition, 1, 1).InstallTool();
    }
}