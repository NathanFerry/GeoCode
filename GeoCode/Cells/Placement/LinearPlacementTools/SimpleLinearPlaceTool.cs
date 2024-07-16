/*--------------------------------------------------------------------------------------+
|   SimpleLinearPlaceTool.cs
|
+--------------------------------------------------------------------------------------*/

#region Bentley Namespaces
using Bentley.DgnPlatformNET;
using Bentley.DgnPlatformNET.Elements;
using Bentley.GeometryNET;
using Bentley.Interop.MicroStationDGN;
using Bentley.MstnPlatformNET;
using GeoCode.Model;
using GeoCode.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Shapes;
using CurveElement = Bentley.DgnPlatformNET.Elements.CurveElement;
using LineElement = Bentley.DgnPlatformNET.Elements.LineElement;

#endregion

namespace GeoCode.Cells.Placement.LinearPlacementTools
{
    internal class SimpleLinearPlaceTool : DgnPrimitiveTool
    {
        private readonly Linear _linearElement;
        private DPoint3d _origin;
        private LineStringElement _lineElement;
        private List<DPoint3d> listPoints = new List<DPoint3d>();
        private List<LineElement> tempLines = new List<LineElement>();
        private bool nextPointReady = false;

        public SimpleLinearPlaceTool(Linear linear, int toolName, int toolPrompt) : base(toolName, toolPrompt) { _linearElement = linear; }

        #region DgnPrimitiveTool Members
        protected override bool OnDataButton(DgnButtonEvent ev)
        {
            
            if (!DynamicsStarted)
            {
                _origin = ev.Point;
                CellPlacement.PlaceTopoPoint(ev);

                listPoints.Add(ev.Point);

                nextPointReady = true;

                BeginDynamics();
                return false;
            }
            
            if (nextPointReady)
            {
                CellPlacement.PlaceTopoPoint(ev);

                var tempLine = new LineElement(Session.Instance.GetActiveDgnModel(), null, new DSegment3d() { StartPoint = _origin, EndPoint = ev.Point }); ;
                
                tempLines.Add(
                    tempLine
                    );
                tempLine.AddToModel();

                _origin = ev.Point;
                listPoints.Add(ev.Point);
                return false;
            }
            if (listPoints.Count >= 2)
            {
                var level = DgnHelper.GetAllLevelsFromLibrary()
                .First(element => element.Name == _linearElement.Level);

                _lineElement = new(Session.Instance.GetActiveDgnModel(), null,listPoints.ToArray());
                
                new ElementPropertiesSetter().SetLevelChain(level.LevelId).SetColorChain(level.GetByLevelColor().Color).SetLineStyleChain(level.GetByLevelLineStyle()).Apply(_lineElement);
                _lineElement.AddToModel();
               
            }

            

            foreach (var line in tempLines)
            {
                line.DeleteFromModel();
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

                return;
            }
            if (nextPointReady)
            {
                DSegment3d dSegment3D = new()
                {
                    StartPoint = _origin,
                    EndPoint = ev.Point,
                };

                var redraw = new RedrawElems();
                redraw.SetDynamicsViewsFromActiveViewSet(ev.Viewport);
                redraw.DrawMode = DgnDrawMode.TempDraw;
                redraw.DrawPurpose = DrawPurpose.Dynamics;
                redraw.DoRedraw(new LineElement(Session.Instance.GetActiveDgnModel(), null, dSegment3D));
                return;
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

        public static void InstallNewInstance(Linear linear)
        {
            SimpleLinearPlaceTool tool = new(linear,0, 0);
            tool.InstallTool();
        }
        #endregion
    }
}
