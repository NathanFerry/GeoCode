using Bentley.DgnPlatformNET;
using Bentley.DgnPlatformNET.Elements;
using Bentley.GeometryNET;
using GeoCode.Elements.Drawing;
using GeoCode.Utils;

namespace GeoCode.Cells.Placement.PlacementTools;

public class TwoPointsRotationScalingPlaceTool : DgnPrimitiveTool
{
    private readonly SharedCellDefinitionElement _cellDefinition;
    private readonly SharedCellElement _cellElement;
    private DPoint3d? _origin;
    private DPoint3d? _horizontalPoint;
    private bool _rotated = false;

    public TwoPointsRotationScalingPlaceTool(SharedCellDefinitionElement cellDefinition, int toolName, int toolPrompt) : base(toolName, toolPrompt)
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
        if (_origin == null)
        {
            _origin = ev.Point;
            CellPlacement.PlaceTopoPoint(ev);
            return false;
        }

        if (_horizontalPoint == null)
        {
            _horizontalPoint = ev.Point;
            CellPlacement.PlaceTopoPoint(ev);
            return false;
        }

        _cellElement.AddToModel();
        _origin = null;
        _horizontalPoint = null;
        return true;
    }

    protected override void OnDynamicFrame(DgnButtonEvent ev)
    {
        if (_origin == null)
        {
            Draw.TranslateCell(_cellElement, ev.Point);
        } else if (_horizontalPoint == null)
        {
            var direction = new DVector3d(_origin.Value, ev.Point);
            var angle = DVector3d.UnitX.AngleToXY(direction) - _cellElement.GetActualXYAngle(out _);
            Draw.RotateCellAroundZ(_cellElement, angle);

            Draw.ScaleCellFromDirectionHorizontal(_cellElement, _origin.Value, ev.Point);
        } else
        {
            _rotated = Draw.AxeRotateVertical(_cellElement, _origin.Value, _horizontalPoint.Value,ev.Point,_rotated);
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
        InstallNewInstance(_cellDefinition);
    }
    
    public static void InstallNewInstance(SharedCellDefinitionElement cellDefinition)
    {
        new TwoPointsRotationScalingPlaceTool(cellDefinition, 1, 1).InstallTool();
    }
}