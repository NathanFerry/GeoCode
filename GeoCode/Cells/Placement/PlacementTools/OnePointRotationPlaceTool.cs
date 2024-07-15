/*--------------------------------------------------------------------------------------+
|   OnePointRotationPlaceTool.cs
|
+--------------------------------------------------------------------------------------*/

#region Bentley Namespaces
using Bentley.DgnPlatformNET;
using Bentley.DgnPlatformNET.Elements;
using Bentley.GeometryNET;
using Bentley.MstnPlatformNET;
using GeoCode.Utils;

#endregion

namespace GeoCode.Cells.Placement.PlacementTools
{
    public class OnePointRotationPlaceTool : DgnPrimitiveTool
    {
        private readonly SharedCellDefinitionElement _cellDefinition;
        private readonly SharedCellElement _cellElement;
        private DPoint3d? _origin;
        public OnePointRotationPlaceTool(SharedCellDefinitionElement cellDefinition, int toolName, int toolPrompt) : base(toolName, toolPrompt) 
        {
            _cellDefinition = cellDefinition;
            _cellElement = SharedCellHelper.CreateSharedCell(cellDefinition, DPoint3d.Zero);
        }
        

        #region DgnPrimitiveTool Members
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

            _cellElement.AddToModel();
            
            return true;
        }

        protected override void OnRestartTool()
        {
            InstallNewInstance(_cellDefinition);
        }

        protected override bool OnResetButton(DgnButtonEvent ev)
        {
            ExitTool();
            return true;
        }

        protected override void OnDynamicFrame(DgnButtonEvent ev)
        {
            if (_origin is null)
            {
                _cellElement.GetSnapOrigin(out var origin);
                _cellElement.ApplyTransform(new TransformInfo(DTransform3d.FromTranslation(ev.Point - origin)));
            } else
            {
                var direction = new DVector3d(_origin.Value, ev.Point);
                var angle = DVector3d.UnitX.AngleToXY(direction) - _cellElement.GetActualXYAngle(out _);
                var rotationMatrix = DTransform3d.FromRotationAroundLine(_origin.Value, DVector3d.UnitZ, angle);
                _cellElement.ApplyTransform(new TransformInfo(rotationMatrix));
            }

            var redraw = new RedrawElems();
            redraw.SetDynamicsViewsFromActiveViewSet(ev.Viewport);
            redraw.DrawMode = DgnDrawMode.TempDraw;
            redraw.DrawPurpose = DrawPurpose.Dynamics;
            redraw.DoRedraw(_cellElement);
        }

        public static void InstallNewInstance(SharedCellDefinitionElement cellDefinition)
        {
            OnePointRotationPlaceTool tool = new(cellDefinition, 0, 0);
            tool.InstallTool();
        }
        #endregion
    }
}
