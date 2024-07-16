/*--------------------------------------------------------------------------------------+
|   ThickLinearPlaceTool.cs
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

#endregion

namespace GeoCode.Cells.Placement.LinearPlacementTools
{
    
    internal class ThickLinearPlaceTool : DgnPrimitiveTool
    {
        private readonly Linear _linearElement;
        private DPoint3d _origin;
        private DPoint3d _previous;
        private bool _verticalPoint = false;
        private List<DPoint3d> listPoints = new List<DPoint3d>();

        private List<DPoint3d> listPointsUnder = new List<DPoint3d>();
        private List<DPoint3d> listPointsOn = new List<DPoint3d>();

        private LineStringElement _lineElement;
        private bool nextPointReady = false;
        private List<LineElement> tempLines = new List<LineElement>();

        public ThickLinearPlaceTool(Linear linear,int toolName, int toolPrompt) : base(toolName, toolPrompt) { _linearElement = linear; }

        #region DgnPrimitiveTool Members
        protected override bool OnDataButton(DgnButtonEvent ev)
        {
            if (!DynamicsStarted)
            {
                BeginDynamics();
                _previous = ev.Point;
                _origin = ev.Point;
                CellPlacement.PlaceTopoPoint(ev);


                listPoints.Add(_origin);
                nextPointReady = true;
                return false;

            }

            if (nextPointReady)
            {
                CellPlacement.PlaceTopoPoint(ev);
                Log.Write("Cellule topo placée");

                var tempLine = new LineElement(Session.Instance.GetActiveDgnModel(), null, new DSegment3d() { StartPoint = _previous, EndPoint = ev.Point }); ;

                tempLines.Add(
                    tempLine
                    );
                tempLine.AddToModel();
                

                if (!_linearElement.Value.HasValue)
                {
                    Log.Write("Le linéaire n'a pas d'épaisseur donnée");
                    return true;
                }

                if (_previous == _origin)
                {
                    listPointsOn.Add(GetPointOnSegment(_previous, ev.Point, _linearElement.Value.Value));
                    listPointsUnder.Add(GetPointUnderSegment(_previous, ev.Point, _linearElement.Value.Value));
                } else
                {
                    listPointsOn.Add(GetPointOnBisector(listPoints[listPoints.Count-2],_previous, ev.Point, _linearElement.Value.Value));
                    listPointsUnder.Add(GetPointUnderBisector(listPoints[listPoints.Count - 2], _previous, ev.Point, _linearElement.Value.Value));
                }


                _previous = ev.Point;
                listPoints.Add(ev.Point);

                return false;
            }
            if (_verticalPoint == false)
            {
               
                _verticalPoint = true;
                var crossProductDirection = (_previous.X - _origin.X) * (ev.Point.Y - _origin.Y) >
                     (_previous.Y - _origin.Y) * (ev.Point.X - _origin.X)
               ? 1
               : -1;

               
         

                listPoints.AddRange(crossProductDirection == -1 ?  listPointsUnder : listPointsOn);

                listPoints.Add(_origin);

                if (listPoints.Count > 1)
                {
                    var level = DgnHelper.GetAllLevelsFromLibrary()
                    .First(element => element.Name == _linearElement.Level);

                    _lineElement = new(Session.Instance.GetActiveDgnModel(), null, listPoints.ToArray());

                    new ElementPropertiesSetter().SetLevelChain(level.LevelId).SetColorChain(level.GetByLevelColor().Color).SetLineStyleChain(level.GetByLevelLineStyle()).Apply(_lineElement);
                    _lineElement.AddToModel();
                }
                ExitTool();

                return true;
            }
            
            return true;
        }

        protected override void OnRestartTool()
        {
            InstallNewInstance(_linearElement);
        }

        protected override bool OnResetButton(DgnButtonEvent ev)
        {
            if (listPoints.Count >= 2)
            {
                listPointsOn.Add(GetPointUnderSegment(listPoints[listPoints.Count - 1], listPoints[listPoints.Count - 2], _linearElement.Value.Value));
                listPointsUnder.Add(GetPointOnSegment(listPoints[listPoints.Count - 1], listPoints[listPoints.Count - 2], _linearElement.Value.Value));
            }

            listPointsUnder.Reverse();
            listPointsOn.Reverse();

            nextPointReady = false;
            return true;
        }

        protected override void OnDynamicFrame(DgnButtonEvent ev)
        {
            if (nextPointReady)
            {
                var thirdPoint = new DPoint3d();
                if (_previous != _origin)
                {
                    thirdPoint = GetPointOnBisector(listPoints[listPoints.Count - 2], _previous, ev.Point,_linearElement.Value.Value);
                } else
                {
                    thirdPoint = GetPointOnSegment(_previous, ev.Point, _linearElement.Value.Value);
                }
                var redraw = new RedrawElems();
                redraw.SetDynamicsViewsFromActiveViewSet(ev.Viewport);
                redraw.DrawMode = DgnDrawMode.TempDraw;
                redraw.DrawPurpose = DrawPurpose.Dynamics;
                redraw.DoRedraw(new LineStringElement(
                    Session.Instance.GetActiveDgnModel(), 
                    null, 
                    new DPoint3d[3] { ev.Point,_previous, thirdPoint}
                    )
                );
                return;
            }

            if (_verticalPoint == false)
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
                    redraw.DoRedraw(new LineStringElement(Session.Instance.GetActiveDgnModel(), null, listPoints.Concat(listPointsUnder).ToArray()));

                } else 
                redraw.DoRedraw(new LineStringElement(Session.Instance.GetActiveDgnModel(), null, listPoints.Concat(listPointsOn).ToArray()));
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
            ThickLinearPlaceTool tool = new ThickLinearPlaceTool(linear,0, 0);
            tool.InstallTool();
        }

        public DPoint3d GetPointOnSegment(DPoint3d A, DPoint3d B, double distance)
        {
            var x = A.X - distance * (-(B.Y - A.Y) / (Math.Sqrt(Math.Pow(B.Y - A.Y, 2) + Math.Pow(B.X - A.X, 2))));
            var y = A.Y + distance * ((B.X - A.X) / (Math.Sqrt(Math.Pow(B.Y - A.Y, 2) + Math.Pow(B.X - A.X, 2))));
            var z = A.Z;

            return new DPoint3d(x, y, z);
        }

        public DPoint3d GetPointUnderSegment(DPoint3d A, DPoint3d B, double distance)
        {
            var x = A.X - (-distance) * (-(B.Y - A.Y) / (Math.Sqrt(Math.Pow(B.Y - A.Y, 2) + Math.Pow(B.X - A.X, 2))));
            var y = A.Y + (-distance) * ((B.X - A.X) / (Math.Sqrt(Math.Pow(B.Y - A.Y, 2) + Math.Pow(B.X - A.X, 2))));
            var z = A.Z;

            return new DPoint3d(x, y, z);
        }

        public DPoint3d GetPointOnBisector(DPoint3d A, DPoint3d B,DPoint3d C, double distance) 
        {
            // Dénominateur commun des calculs en X et en Y 


            var distBA = Math.Sqrt(Math.Pow(A.Y - B.Y, 2) + Math.Pow(A.X - B.X, 2));
            var distBC = Math.Sqrt(Math.Pow(C.Y - B.Y, 2) + Math.Pow(C.X - B.X, 2));

            var qX = distBC * ((A.X - B.X) / distBA);
                qX += distBA * ((C.X - B.X) / distBC);
            var qY = distBC * ((A.Y - B.Y) / distBA);
                qY += distBA * ((C.Y - B.Y) / distBC);

            var denominator = Math.Sqrt(Math.Pow(qX, 2) + Math.Pow(qY, 2));

            var x = B.X + (distance) * (qX / denominator);
            var y = B.Y + (distance) * (qY / denominator);
            var z = B.Z;
            return new DPoint3d(x, y, z);
        }

        public DPoint3d GetPointUnderBisector(DPoint3d A, DPoint3d B, DPoint3d C, double distance)
        {
            // Dénominateur commun des calculs en X et en Y 
            
                
            var distBA = Math.Sqrt(Math.Pow(A.Y - B.Y, 2) + Math.Pow(A.X - B.X, 2));
            var distBC = Math.Sqrt(Math.Pow(C.Y - B.Y, 2) + Math.Pow(C.X - B.X, 2));

            var qX  = distBC * ((A.X - B.X) / distBA);
                qX += distBA * ((C.X - B.X) / distBC);
            var qY  = distBC * ((A.Y - B.Y) / distBA);
                qY += distBA * ((C.Y - B.Y) / distBC);

            var denominator = Math.Sqrt(Math.Pow(qX, 2) + Math.Pow(qY, 2));

            var x = B.X + (-distance) * (qX / denominator);
            var y = B.Y + (-distance) * (qY / denominator);
            var z = B.Z;
            return new DPoint3d(x, y, z);
        }
        #endregion
    }
}
