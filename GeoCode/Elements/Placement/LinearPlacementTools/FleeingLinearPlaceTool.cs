/*--------------------------------------------------------------------------------------+
|   FleeingLinearPlaceTool.cs
|
+--------------------------------------------------------------------------------------*/

#region Bentley Namespaces
using Bentley.DgnPlatformNET;
using Bentley.DgnPlatformNET.Elements;
using Bentley.GeometryNET;
using Bentley.MstnPlatformNET;
using GeoCode.Model;
using GeoCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Forms;

#endregion

namespace GeoCode.Cells.Placement.LinearPlacementTools
{
    internal class FleeingLinearPlaceTool : DgnPrimitiveTool
    {
        private Linear _linearElement;
        private DPoint3d _origin;
        private DPoint3d _previous;
        private List<DPoint3d> listPoints = new List<DPoint3d>();

        private List<DPoint3d> listPointsOn = new List<DPoint3d>();
        private List<DPoint3d> listPointsUnder = new List<DPoint3d>();

        private bool _nextPointReady = false;
        private bool _verticalPoint = true;
        private List<LineElement> tempLines = new List<LineElement>();
        private LineStringElement _lineElement;

        public FleeingLinearPlaceTool(Linear linear,int toolName, int toolPrompt) : base(toolName, toolPrompt) { _linearElement = linear; }

        #region DgnPrimitiveTool Members
        protected override bool OnDataButton(DgnButtonEvent ev)
        {
            if (!DynamicsStarted)
            {
                BeginDynamics();
                Log.Write("origin == null");
                try
                {
                    CellPlacement.PlaceTopoPoint(ev);
                } catch (Exception e)
                {
                    Log.Write(e.ToString());
                }
                
                _origin = ev.Point;
                _previous = ev.Point;

                listPoints.Add(_origin);

                _nextPointReady=true;
                return false;
            }
            if (_nextPointReady)
            {
                CellPlacement.PlaceTopoPoint(ev);

                var tempLine = new LineElement(Session.Instance.GetActiveDgnModel(), null, new DSegment3d() { StartPoint = _previous, EndPoint = ev.Point }); ;

                tempLines.Add(
                    tempLine
                    );
                tempLine.AddToModel();
                if (_previous == _origin)
                {
                    _previous = ev.Point;
                    listPointsOn.Add(ThickLinearPlaceTool.GetPointOnSegment(_origin, _previous,_linearElement.Value.Value * 10000));
                    listPointsUnder.Add(ThickLinearPlaceTool.GetPointUnderSegment(_origin, _previous, _linearElement.Value.Value * 10000));

                    listPointsOn.Add(_origin);
                    listPointsOn.Add(_previous);

                    listPointsUnder.Add(_origin);
                    listPointsUnder.Add(_previous);

                    listPoints.Add(_previous);
                } else
                {
                    listPointsOn.Add(ThickLinearPlaceTool.GetPointOnBisector(listPoints[listPoints.Count-2], _previous,ev.Point, _linearElement.Value.Value * 10000));
                    listPointsUnder.Add(ThickLinearPlaceTool.GetPointUnderBisector(listPoints[listPoints.Count-2], _previous,ev.Point, _linearElement.Value.Value * 10000));
                    listPoints.Add(_previous);
                    listPointsOn.Add(_previous);

                    listPointsUnder.Add(_previous);
                    _previous = ev.Point;
                    listPoints.Add(_previous);
                    listPointsOn.Add(_previous);
                    listPointsUnder.Add(_previous);
                }
                return false;
            }
            if (_verticalPoint == false)
            {
                _verticalPoint = true;
                var crossProductDirection = (_previous.X - _origin.X) * (ev.Point.Y - _origin.Y) >
                     (_previous.Y - _origin.Y) * (ev.Point.X - _origin.X)
               ? 1
               : -1;

                if (listPoints.Count > 1)
                {
                    var level = DgnHelper.GetAllLevelsFromLibrary()
                    .First(element => element.Name == _linearElement.Level);

                    if (crossProductDirection == -1)
                    {
                        _lineElement = new(Session.Instance.GetActiveDgnModel(), null, listPointsUnder.ToArray());
                    }
                    else
                    {
                        _lineElement = new(Session.Instance.GetActiveDgnModel(), null, listPointsOn.ToArray());
                    }

                    new ElementPropertiesSetter().SetLevelChain(level.LevelId).SetColorChain(level.GetByLevelColor().Color).SetLineStyleChain(level.GetByLevelLineStyle()).Apply(_lineElement);
                    _lineElement.AddToModel();
                }

                foreach (var line in tempLines)
                {
                    line.DeleteFromModel();
                }


                return true;
            }
            OnRestartTool();
            return true;
        }

        protected override void OnPostInstall()
        {
            AccuSnap.SnapEnabled = true;
            //AccuDraw.Active = true;
            //BeginDynamics();
        }

        protected override void OnRestartTool()
        {
            InstallNewInstance(_linearElement);
        }

        protected override bool OnResetButton(DgnButtonEvent ev)
        {
            if (listPoints.Count > 2 && _nextPointReady)
            {
                listPointsOn.Add(ThickLinearPlaceTool.GetPointUnderSegment(_previous, listPoints[listPoints.Count - 2], _linearElement.Value.Value * 10000));
                listPointsUnder.Add(ThickLinearPlaceTool.GetPointOnSegment(_previous, listPoints[listPoints.Count - 2], _linearElement.Value.Value * 10000));
            }
            _nextPointReady = false;

            
            _verticalPoint = false;

            return true;
        }


        protected override void OnDynamicFrame(DgnButtonEvent ev)
        {
            if (_nextPointReady)
            {
                Log.Write(_linearElement.Value.Value.ToString());
                var thirdPoint = new DPoint3d();
                if (_previous != _origin)
                {
                    thirdPoint = ThickLinearPlaceTool.GetPointOnBisector(listPoints[listPoints.Count - 2], _previous, ev.Point, _linearElement.Value.Value * 10000);
                }
                else
                {
                    thirdPoint = ThickLinearPlaceTool.GetPointOnSegment(_previous, ev.Point, _linearElement.Value.Value * 10000);
                }
                var redraw = new RedrawElems();
                redraw.SetDynamicsViewsFromActiveViewSet(ev.Viewport);
                redraw.DrawMode = DgnDrawMode.TempDraw;
                redraw.DrawPurpose = DrawPurpose.Dynamics;
                redraw.DoRedraw(new LineStringElement(
                    Session.Instance.GetActiveDgnModel(),
                    null,
                    new DPoint3d[3] { ev.Point, _previous, thirdPoint }
                    )
                );
                return;
            }
            if (_verticalPoint)
            {
                var crossProductDirection = (_previous.X - _origin.X) * (ev.Point.Y - _origin.Y) >
                    (_previous.Y - _origin.Y) * (ev.Point.X - _origin.X)
              ? 1
              : -1;

                var redraw = new RedrawElems();
                redraw.SetDynamicsViewsFromActiveViewSet(ev.Viewport);
                redraw.DrawMode = DgnDrawMode.TempDraw;
                redraw.DrawPurpose = DrawPurpose.Dynamics;

                if (crossProductDirection == -1)
                {
                    redraw.DoRedraw(new LineStringElement(Session.Instance.GetActiveDgnModel(), null, listPointsUnder.ToArray()));

                }
                else
                    redraw.DoRedraw(new LineStringElement(Session.Instance.GetActiveDgnModel(), null, listPointsOn.ToArray()));

            }
        }

        protected override bool OnInstall()
        {

            return true;
        }

        

        public static void InstallNewInstance(Linear linear )
        {
            FleeingLinearPlaceTool tool = new FleeingLinearPlaceTool(linear,0, 0);
            tool.InstallTool();
        }
        #endregion
    }
}
