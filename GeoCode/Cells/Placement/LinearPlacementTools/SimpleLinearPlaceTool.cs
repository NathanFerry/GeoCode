/*--------------------------------------------------------------------------------------+
|   SimpleLinearPlaceTool.cs
|
+--------------------------------------------------------------------------------------*/

#region Bentley Namespaces
using Bentley.DgnPlatformNET;
using Bentley.GeometryNET;
using Bentley.MstnPlatformNET;
using GeoCode.Model;
using GeoCode.Utils;
using System.Linq;
using System.Windows.Shapes;
using LineElement = Bentley.DgnPlatformNET.Elements.LineElement;

#endregion

namespace GeoCode.Cells.Placement.LinearPlacementTools
{
    internal class SimpleLinearPlaceTool : DgnPrimitiveTool
    {
        private readonly Linear _linearElement;
        private DPoint3d _origin;
        private LineElement _lineElement;
        private bool nextPointReady = true;

        public SimpleLinearPlaceTool(Linear linear, int toolName, int toolPrompt) : base(toolName, toolPrompt) { _linearElement = linear; }

        #region DgnPrimitiveTool Members
        protected override bool OnDataButton(DgnButtonEvent ev)
        {
            if (!DynamicsStarted)
            {
                BeginDynamics();
                return false;
            }
            if (_origin == null)
            {
                _origin = ev.Point;
                CellPlacement.PlaceTopoPoint(ev);
            }
            if (nextPointReady)
            {

                DSegment3d segment = new()
                {
                    StartPoint = _origin,
                    EndPoint = ev.Point
                };

                _lineElement = new(Session.Instance.GetActiveDgnModel(),null, segment);

                var level = DgnHelper.GetAllLevelsFromLibrary()
            .First(element => element.Name == _linearElement.Level);
                new ElementPropertiesSetter().SetLevelChain(level.LevelId).SetColorChain(level.GetByLevelColor().Color).SetThicknessChain(_linearElement.Thickness ?? 1).Apply(_lineElement);

                CellPlacement.PlaceTopoPoint(ev);
                _lineElement.AddToModel();

                _origin = ev.Point;
            }
            ExitTool();
            return true;
        }

        protected override void OnRestartTool()
        {
            InstallNewInstance(_linearElement);
        }

        protected override bool OnResetButton(DgnButtonEvent ev)
        {
            nextPointReady = false;
            return true;
        }

        protected override void OnDynamicFrame(DgnButtonEvent ev)
        {
            if (_origin == null)
            {

            }
            if (nextPointReady)
            {

            }
        }

        protected override bool OnInstall()
        {
            
            return true;
        }

        protected override void OnPostInstall()
        {
            base.OnPostInstall();
        }

        public void InstallNewInstance(Linear linear)
        {
            SimpleLinearPlaceTool tool = new(linear,0, 0);
            tool.InstallTool();
        }
        #endregion
    }
}
