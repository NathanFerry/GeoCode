using System;
using Bentley.DgnPlatformNET;
using Bentley.DgnPlatformNET.Elements;
using Bentley.GeometryNET;
using GeoCode.Utils;

namespace GeoCode.Cells.Placement.PlacementTools;
public class ThreePointsRotationScalingPlaceTool : DgnPrimitiveTool
{
    private readonly SharedCellDefinitionElement _cellDefinition;
    private readonly SharedCellElement _cellElement;
    private DPoint3d? _origin;
    private DPoint3d? _horizontalPoint;
    private DPoint3d? _verticalPoint;
    public ThreePointsRotationScalingPlaceTool(SharedCellDefinitionElement cellDefinition, int toolName, int toolPrompt) : base(toolName, toolPrompt)
    {
        _cellDefinition = cellDefinition;
        _cellElement = SharedCellHelper.CreateSharedCell(cellDefinition, DPoint3d.Zero);
        Console.WriteLine("New Three points");
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

        if (_horizontalPoint is null)
        {
            _horizontalPoint = ev.Point;
            CellPlacement.PlaceTopoPoint(ev);
            return false;
        }

        _verticalPoint = ev.Point;
        CellPlacement.PlaceTopoPoint(ev);
        
        _cellElement.AddToModel();

        return true;
    }

    protected override void OnDynamicFrame(DgnButtonEvent ev)
    {
        if (_origin is null)
        {
            _cellElement.GetSnapOrigin(out var origin);
            _cellElement.ApplyTransform(new TransformInfo(DTransform3d.FromTranslation(ev.Point - origin)));
        } else if (_horizontalPoint is null)
        {
            var direction = new DVector3d(_origin.Value, ev.Point);
            if (direction != DVector3d.Zero)
            {
                var angle = DVector3d.UnitX.AngleToXY(direction) - _cellElement.GetActualXYAngle(out _);
                var rotationMatrix = DTransform3d.FromRotationAroundLine(_origin.Value, DVector3d.UnitZ, angle);
                _cellElement.ApplyTransform(new TransformInfo(rotationMatrix));
            }

            var factor = _origin.Value.Distance(ev.Point) / SharedCellHelper.ComputeLength(_cellElement);
            DTransform3d.TryFixedPointAndDirectionalScale(_origin.Value, direction, factor, out var scaling);
            _cellElement.ApplyTransform(new TransformInfo(scaling));
        } else if (_verticalPoint is null)
        {
            var crossProductDirection = (_horizontalPoint.Value.X - _origin.Value.X) * (ev.Point.Y - _origin.Value.Y) >
                      (_horizontalPoint.Value.Y - _origin.Value.Y) * (ev.Point.X - _origin.Value.X)
                ? 1
                : -1;
            
            var direction = new DVector3d(_origin.Value, _horizontalPoint.Value);
            direction.TryUnitPerpendicularXY(out var perpendicular);

            

            var scalingVector = new DVector3d(_horizontalPoint.Value, ev.Point);


            var factor = scalingVector.DotProduct(perpendicular) / SharedCellHelper.ComputeWidth(_cellElement);
            perpendicular = crossProductDirection * perpendicular;

            DTransform3d.TryFixedPointAndDirectionalScale(_origin.Value, perpendicular, factor, out var scale);
            
            Console.WriteLine($@"{factor}");
            Console.WriteLine(scalingVector.DotProductXY(perpendicular));
            Console.WriteLine($@"{SharedCellHelper.ComputeWidth(_cellElement)}");
            
            _cellElement.ApplyTransform(new TransformInfo(scale));
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
        new ThreePointsRotationScalingPlaceTool(cellDefinition, 1, 1).InstallTool();
    }
}