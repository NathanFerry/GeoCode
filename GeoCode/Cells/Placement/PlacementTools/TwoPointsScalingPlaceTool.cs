using System;
using Bentley.DgnPlatformNET;
using Bentley.DgnPlatformNET.Elements;
using Bentley.GeometryNET;

namespace GeoCode.Cells.Placement.PlacementTools;

//TODO: Various special case to handle 
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

            _cellElement.CalcElementRange(out var range);
            _cellElement.ApplyTransform(
                new TransformInfo(DTransform3d.FromTranslation(-range.XSize / 2, -range.YSize / 2, 0)));
        }
        else
        {
            _cellElement.ApplyTransform(new TransformInfo(
                DTransform3d.FromUniformScaleAndFixedPoint(_center.Value, _center.Value.DistanceXY(ev.Point))));
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