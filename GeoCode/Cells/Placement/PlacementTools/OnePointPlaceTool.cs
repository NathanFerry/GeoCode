using System.Reflection;
using System.Runtime.Remoting.Messaging;
using Bentley.DgnPlatformNET;
using static GeoCode.Cells.Placement.PlacementTypeElement;

namespace GeoCode.Cells.Placement.PlacementTools;

[PlacementMethod("")]
public class OnePointPlaceTool : DgnPrimitiveTool
{
    public OnePointPlaceTool(int toolName, int toolPrompt) : base(toolName, toolPrompt)
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