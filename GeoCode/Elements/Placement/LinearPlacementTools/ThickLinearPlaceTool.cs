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
                    listPointsOn.Add(GetPointOnSegment(_previous, ev.Point, _linearElement.Value.Value * 10000));
                    listPointsUnder.Add(GetPointUnderSegment(_previous, ev.Point, _linearElement.Value.Value * 10000));
                } else
                {
                    listPointsOn.Add(GetPointOnBisector(listPoints[listPoints.Count-2],_previous, ev.Point, _linearElement.Value.Value * 10000));
                    listPointsUnder.Add(GetPointUnderBisector(listPoints[listPoints.Count - 2], _previous, ev.Point, _linearElement.Value.Value * 10000));
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

                foreach (var line in tempLines)
                {
                    line.DeleteFromModel();
                }


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
            if (listPoints.Count >= 2 && nextPointReady)
            {
                listPointsOn.Add(GetPointUnderSegment(listPoints[listPoints.Count - 1], listPoints[listPoints.Count - 2], _linearElement.Value.Value * 10000));
                listPointsUnder.Add(GetPointOnSegment(listPoints[listPoints.Count - 1], listPoints[listPoints.Count - 2], _linearElement.Value.Value * 10000));
                listPointsUnder.Reverse();
                listPointsOn.Reverse();
            }

            

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
                    thirdPoint = GetPointOnBisector(listPoints[listPoints.Count - 2], _previous, ev.Point,_linearElement.Value.Value * 10000);

                    if (thirdPoint.X == -1 && thirdPoint.Y == -1)
                    {
                        thirdPoint = GetPointOnSegment(_previous, ev.Point, _linearElement.Value.Value * 10000);
                    }
                } else
                {
                    thirdPoint = GetPointOnSegment(_previous, ev.Point, _linearElement.Value.Value * 10000);
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
            AccuSnap.SnapEnabled = true;
        }


        public static void InstallNewInstance(Linear linear)
        {
            ThickLinearPlaceTool tool = new ThickLinearPlaceTool(linear,0, 0);
            tool.InstallTool();
        }

        public static DPoint3d GetPointOnSegment(DPoint3d A, DPoint3d B, double distance)
        {
            
                // Calculez le vecteur AB
                var AB_X = B.X - A.X;
                var AB_Y = B.Y - A.Y;

                // Calculez la longueur du vecteur AB
                var AB_length = Math.Sqrt(AB_X * AB_X + AB_Y * AB_Y);

                // Vérifiez si la longueur de AB est non nulle pour éviter la division par zéro
                if (AB_length == 0)
                {
                    return new DPoint3d(-1, -1, -1);
                }

                // Calculez le vecteur perpendiculaire à AB
                var perp_X = -AB_Y;
                var perp_Y = AB_X;

                // Normalisez le vecteur perpendiculaire
                var perp_length = Math.Sqrt(perp_X * perp_X + perp_Y * perp_Y);
                if (perp_length == 0)
                {
                    return new DPoint3d(-1, -1, -1);

            }

            // Calculez les coordonnées de C
            var x = A.X + (distance) * (perp_X / perp_length);
                var y = A.Y + (distance) * (perp_Y / perp_length);
                var z = A.Z; // Conservez la coordonnée Z de A

                return new DPoint3d(x, y, z);
            
        }

        public static DPoint3d GetPointUnderSegment(DPoint3d A, DPoint3d B, double distance)
        {
            // Calculez le vecteur AB
            var AB_X = B.X - A.X;
            var AB_Y = B.Y - A.Y;

            // Calculez la longueur du vecteur AB
            var AB_length = Math.Sqrt(AB_X * AB_X + AB_Y * AB_Y);

            // Vérifiez si la longueur de AB est non nulle pour éviter la division par zéro
            if (AB_length == 0)
            {
                return new DPoint3d(-1, -1, -1);

            }

            // Calculez le vecteur perpendiculaire à AB
            var perp_X = -AB_Y;
            var perp_Y = AB_X;

            // Normalisez le vecteur perpendiculaire
            var perp_length = Math.Sqrt(perp_X * perp_X + perp_Y * perp_Y);
            if (perp_length == 0)
            {
                return new DPoint3d(-1, -1, -1);

            }

            // Calculez les coordonnées de C
            var x = A.X + (-distance) * (perp_X / perp_length);
            var y = A.Y + (-distance) * (perp_Y / perp_length);
            var z = A.Z; // Conservez la coordonnée Z de A

            return new DPoint3d(x, y, z);
        }

        public static DPoint3d GetPointOnBisector(DPoint3d A, DPoint3d B, DPoint3d C, double distance)
        {
            // Calculez les distances des points A et C à B
            var distBA = Math.Sqrt(Math.Pow(A.X - B.X, 2) + Math.Pow(A.Y - B.Y, 2));
            var distBC = Math.Sqrt(Math.Pow(C.X - B.X, 2) + Math.Pow(C.Y - B.Y, 2));

            // Vérifiez si les distances sont non nulles pour éviter la division par zéro
            if (distBA == 0 || distBC == 0)
            {
                return new DPoint3d(-1, -1, -1);

            }

            // Normalisez les vecteurs BA et BC
            var unitBA_X = (A.X - B.X) / distBA;
            var unitBA_Y = (A.Y - B.Y) / distBA;
            var unitBC_X = (C.X - B.X) / distBC;
            var unitBC_Y = (C.Y - B.Y) / distBC;

            // Calculez les coordonnées de la bissectrice
            var bisectorX = unitBA_X + unitBC_X;
            var bisectorY = unitBA_Y + unitBC_Y;

            // Normalisez le vecteur de la bissectrice
            var bisectorLength = Math.Sqrt(bisectorX * bisectorX + bisectorY * bisectorY);
            if (bisectorLength == 0)
            {
                return new DPoint3d(-1, -1, -1);

            }


            var normX = (bisectorX / bisectorLength);
            var normY = (bisectorY / bisectorLength);

            var crossProduct = (A.X - B.X) * (C.Y - B.Y) - (A.Y - B.Y) * (C.X - B.X);
            if (crossProduct > 0)
            {
                normX = -normX;
                normY = -normY;
            }


            // Calculez les coordonnées de D
            var x = B.X + (distance) * (normX);
            var y = B.Y + (distance) * (normY);
            var z = B.Z; // Conservez la coordonnée Z de B

            return new DPoint3d(x, y, z);
        }

        public static DPoint3d GetPointUnderBisector(DPoint3d A, DPoint3d B, DPoint3d C, double distance)
        {
            // Calculez les distances des points A et C à B
            var distBA = Math.Sqrt(Math.Pow(A.X - B.X, 2) + Math.Pow(A.Y - B.Y, 2));
            var distBC = Math.Sqrt(Math.Pow(C.X - B.X, 2) + Math.Pow(C.Y - B.Y, 2));

            // Vérifiez si les distances sont non nulles pour éviter la division par zéro
            if (distBA == 0 || distBC == 0)
            {
                return new DPoint3d(-1,-1,-1);
            }

            // Normalisez les vecteurs BA et BC
            var unitBA_X = (A.X - B.X) / distBA;
            var unitBA_Y = (A.Y - B.Y) / distBA;
            var unitBC_X = (C.X - B.X) / distBC;
            var unitBC_Y = (C.Y - B.Y) / distBC;

            // Calculez les coordonnées de la bissectrice
            var bisectorX = unitBA_X + unitBC_X;
            var bisectorY = unitBA_Y + unitBC_Y;

            // Normalisez le vecteur de la bissectrice
            var bisectorLength = Math.Sqrt(bisectorX * bisectorX + bisectorY * bisectorY);
            if (bisectorLength == 0)
            {
                return new DPoint3d(-1, -1, -1);

            }

            var normX = (bisectorX / bisectorLength);
            var normY = (bisectorY / bisectorLength);


            var crossProduct = (A.X - B.X)*(C.Y-B.Y) - (A.Y-B.Y)*(C.X-B.X);
            if (crossProduct > 0)
            {
                normX = -normX;
                normY = -normY;
            }

            // Calculez les coordonnées de D
            var x = B.X + (-distance) * (normX);
            var y = B.Y + (-distance) * (normY);
            var z = B.Z; // Conservez la coordonnée Z de B

            return new DPoint3d(x, y, z);
        }
        #endregion
    }
}
