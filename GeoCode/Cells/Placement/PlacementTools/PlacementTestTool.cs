using Bentley.DgnPlatformNET;

namespace GeoCode.Cells.Placement.PlacementTools;

public class PlacementTestTool : DgnPrimitiveTool
{
    public PlacementTestTool(int toolName, int toolPrompt) : base(toolName, toolPrompt)
    {
    }

    protected override bool OnResetButton(DgnButtonEvent ev)
    {
        throw new System.NotImplementedException();
    }

    protected override bool OnDataButton(DgnButtonEvent ev)
    {
        
        throw new System.NotImplementedException();
    }

    protected override void OnRestartTool()
    {
        throw new System.NotImplementedException();
    }
}