using Bentley.DgnPlatformNET;
using Bentley.DgnPlatformNET.Elements;
using Bentley.GeometryNET;
using GeoCode.Utils;

namespace GeoCode.Cells.Placement.PlacementTools;

public class TwoPointsRotationScalingPlaceTool : DgnPrimitiveTool
{
    private readonly SharedCellDefinitionElement _cellDefinition;
    private readonly SharedCellElement _cellElement;
    private DPoint3d? _origin;
    private DPoint3d? _vectorPoint;
    private int _oldPos = 1;
    public TwoPointsRotationScalingPlaceTool(SharedCellDefinitionElement cellDefinition, int toolName, int toolPrompt) : base(toolName, toolPrompt)
    {
        _cellDefinition = cellDefinition;
        _cellElement = SharedCellHelper.CreateSharedCell(cellDefinition, DPoint3d.Zero);
    }

    protected override bool OnDataButton(DgnButtonEvent ev)
    {
        if (!DynamicsStarted)
        {
            BeginDynamics();
            return false;
        }

        if (_origin is null)
        {
            _origin = ev.Point;
            CellPlacement.PlaceTopoPoint(ev);
            return false;
        }

        if (_vectorPoint is null)
        {
            _vectorPoint = ev.Point;
            CellPlacement.PlaceTopoPoint(ev);
            return false;
        }

        _cellElement.AddToModel();

        return true;
    }

    protected override void OnDynamicFrame(DgnButtonEvent ev)
    {
        if (_origin is null)
        {
            _cellElement.GetSnapOrigin(out var origin);
            _cellElement.ApplyTransform(new TransformInfo(DTransform3d.FromTranslation(ev.Point - origin)));
        } else if (_vectorPoint is null)
        {
            var direction = new DVector3d(_origin.Value, ev.Point);
            var angle = DVector3d.UnitX.AngleToXY(direction) - _cellElement.GetActualXYAngle(out _);
            var rotationMatrix = DTransform3d.FromRotationAroundLine(_origin.Value, DVector3d.UnitZ, angle);
            _cellElement.ApplyTransform(new TransformInfo(rotationMatrix));
            
            var factor = _origin.Value.DistanceXY(ev.Point) / SharedCellHelper.ComputeLength(_cellElement);
            DTransform3d.TryFixedPointAndDirectionalScale(_origin.Value, direction, factor, out var scaling);
            _cellElement.ApplyTransform(new TransformInfo(scaling));
        } else
        {
            var pos = (_vectorPoint.Value.X - _origin.Value.X) * (ev.Point.Y - _origin.Value.Y) >
                      (_vectorPoint.Value.Y - _origin.Value.Y) * (ev.Point.X - _origin.Value.X)
                ? 1
                : -1;
            if (pos != _oldPos)
            {
                DTransform3d.TryFixedPointAndDirectionalScale(_origin.Value,
                    new DVector3d(_origin.Value, _vectorPoint.Value).RotateXY(Angle.FromDegrees(90)), -1, out var side);
                _cellElement.ApplyTransform(new TransformInfo(side));
            }
            _oldPos = pos;
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
        new TwoPointsRotationScalingPlaceTool(cellDefinition, 1, 1).InstallTool();
    }
}