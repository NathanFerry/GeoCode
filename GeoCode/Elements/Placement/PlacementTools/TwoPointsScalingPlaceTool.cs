using Bentley.DgnPlatformNET;
using Bentley.DgnPlatformNET.Elements;
using Bentley.GeometryNET;

namespace GeoCode.Cells.Placement.PlacementTools;

public class TwoPointsScalingPlaceTool : DgnPrimitiveTool
{
    private readonly SharedCellDefinitionElement _cellDefinition;
    private readonly SharedCellElement _cellElement;
    private DPoint3d? _center;
    
    public TwoPointsScalingPlaceTool(SharedCellDefinitionElement cellDefinition, int toolName, int toolPrompt) : base(toolName, toolPrompt)
    {
        _cellDefinition = cellDefinition;
        _cellElement = SharedCellHelper.CreateSharedCell(cellDefinition, DPoint3d.Zero);
    }
    protected override void OnPostInstall()
    {
        AccuSnap.SnapEnabled = true;
    }
    protected override bool OnDataButton(DgnButtonEvent ev)
    {  
        if (!DynamicsStarted)
        {

            BeginDynamics();
            return false;
        }

        if (_center == null)
        {
            _center = ev.Point;
            CellPlacement.PlaceTopoPoint(ev);
            return false;
        }

        _cellElement.AddToModel();
        CellPlacement.PlaceTopoPoint(ev);

        return true;
    }
    
    protected override void OnDynamicFrame(DgnButtonEvent ev)
    {
        if (_center == null)
        {
            _cellElement.GetSnapOrigin(out var origin);
            _cellElement.ApplyTransform(new TransformInfo(DTransform3d.FromTranslation(ev.Point - origin)));
        }
        else
        {
            var factor = _center.Value.DistanceXY(ev.Point) / (SharedCellHelper.ComputeWidth(_cellElement) / 2);
            _cellElement.ApplyTransform(new TransformInfo(
                DTransform3d.FromUniformScaleAndFixedPoint(_center.Value, factor)));
        }
        
        var redraw = new RedrawElems();
        redraw.SetDynamicsViewsFromActiveViewSet(ev.Viewport);
        redraw.DrawMode = DgnDrawMode.TempDraw;
        redraw.DrawPurpose = DrawPurpose.Dynamics;
        redraw.DoRedraw(_cellElement);
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
        new TwoPointsScalingPlaceTool(cellDefinition, 1, 1).InstallTool();
    }
}