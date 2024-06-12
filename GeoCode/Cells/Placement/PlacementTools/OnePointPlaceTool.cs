using Bentley.DgnPlatformNET;
using Bentley.DgnPlatformNET.Elements;
using Bentley.GeometryNET;

namespace GeoCode.Cells.Placement.PlacementTools;
public class OnePointPlaceTool : DgnPrimitiveTool
{
    private readonly SharedCellDefinitionElement _cellDefinition;
    private readonly SharedCellElement _cellElement;
    public OnePointPlaceTool(SharedCellDefinitionElement cellDefinition, int toolName, int toolPrompt) : base(toolName, toolPrompt)
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
        
        _cellElement.AddToModel();
        CellPlacement.PlaceTopoPoint(ev);
        return true;
    }

    protected override void OnDynamicFrame(DgnButtonEvent ev)
    {
        _cellElement.GetSnapOrigin(out var origin);
        _cellElement.ApplyTransform(new TransformInfo(DTransform3d.FromTranslation(ev.Point - origin)));
        
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
        new OnePointPlaceTool(cellDefinition, 1, 1).InstallTool();
    }
    
}