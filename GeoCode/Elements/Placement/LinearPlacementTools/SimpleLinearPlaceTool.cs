/*--------------------------------------------------------------------------------------+
|   SimpleLinearPlaceTool.cs
|
+--------------------------------------------------------------------------------------*/

#region Bentley Namespaces
using Bentley.DgnPlatformNET;
using Bentley.DgnPlatformNET.Elements;
using Bentley.GeometryNET;
using Bentley.MstnPlatformNET;
using GeoCode.Model;
using GeoCode.UI.LinearWindows;
using GeoCode.Utils;
using System;
using System.Collections.Generic;
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
        private DPoint3d _previous;
        private DPoint3d _preprevious;
        private CurveElement _curveElement;
        private List<DPoint3d> listPoints = new();
        private List<LineElement> tempLines = new();
        private bool first = true;
        private bool nextPointReady = false;


        public SimpleLinearPlaceTool(Linear linear, int toolName, int toolPrompt) : base(toolName, toolPrompt) { _linearElement = linear; }

        #region DgnPrimitiveTool Members
        protected override bool OnDataButton(DgnButtonEvent ev)
        {
            
            if (!DynamicsStarted)
            {
                BeginDynamics();
                _origin = ev.Point;
                _previous = ev.Point;
                CellPlacement.PlaceTopoPoint(ev);

               
                listPoints.Add(ev.Point);

                nextPointReady = true;

               
                return false;
            }
            
            if (nextPointReady)
            {
                if (SimpleLinearChoice.selectedType == "Line")
                {
                    var tempLine = new LineElement(Session.Instance.GetActiveDgnModel(), null, new DSegment3d() { StartPoint = _previous, EndPoint = ev.Point }); ;

                    tempLines.Add(
                        tempLine
                     );

                    tempLine.AddToModel();
                } else
                {
                    if (first)
                    {
                        
                        _preprevious = _previous;
                        _previous = ev.Point;



                        first = false;
                    } else
                    {
                        if (_preprevious == null) { Log.Write("Pas de Preprevious"); return false; }
                        var bissectorPoint = FindIntersectionOfPerpendicularBisectors(_preprevious, _previous, ev.Point);

                        var ellipse = new DEllipse3d()
                        {
                            
                        };

                        if (!ellipse.InitEllipticalArcFromCenterAxisEnd(bissectorPoint, _preprevious, ev.Point)) Log.Write("InitEllipticalArc ne fonctionne pas");

                        var arc = new ArcElement(
                            Session.Instance.GetActiveDgnModel(),
                            null,
                            ellipse
                        );
                        arc.AddToModel();

                        _preprevious = _previous;
                        _previous = ev.Point;

                        first = true;

                    }
                }

                CellPlacement.PlaceTopoPoint(ev);

                _previous = ev.Point;
                listPoints.Add(ev.Point);
                return false;
            }
            OnRestartTool();
            return true;
        }

        protected override void OnRestartTool()
        {
            InstallNewInstance(_linearElement);
        }

        protected override void ExitTool()
        {
            SimpleLinearChoice.SimpleLinearChoiceDockableWindow.Close();
            base.ExitTool();
        }

        protected override bool OnResetButton(DgnButtonEvent ev)
        {
            nextPointReady = false;

            if (listPoints.Count >= 2)
            {
                var level = DgnHelper.GetAllLevelsFromLibrary()
                .First(element => element.Name == _linearElement.Level);
                try
                {
                    _curveElement = new(Session.Instance.GetActiveDgnModel(), null, listPoints.ToArray());




                    new ElementPropertiesSetter().SetLevelChain(level.LevelId).SetColorChain(level.GetByLevelColor().Color).SetLineStyleChain(level.GetByLevelLineStyle()).Apply(_curveElement);
                    _curveElement.AddToModel();
                } catch (Exception e)
                {
                    Log.Write(e.ToString());    
                }

            }

            foreach (var line in tempLines)
            {
                line.DeleteFromModel();
            }

            ExitTool();

            return true;
        }
        protected override void OnPostInstall()
        {
            AccuSnap.SnapEnabled = true;
            AccuDraw.Active = true;
            SimpleLinearChoice.ShowWindow();
        }
        protected override void OnDynamicFrame(DgnButtonEvent ev)
        {
            if (_previous == null)
            {
                return;
            }
            if (nextPointReady)
            {
                if (SimpleLinearChoice.selectedType == "Line")
                {
                    DSegment3d dSegment3D = new()
                    {
                        StartPoint = _previous,
                        EndPoint = ev.Point
                    } ;

                    

                    var line = new LineElement(Session.Instance.GetActiveDgnModel(), null, dSegment3D);


                    var redraw = new RedrawElems();
                    redraw.SetDynamicsViewsFromActiveViewSet(ev.Viewport);
                    redraw.DrawMode = DgnDrawMode.TempDraw;
                    redraw.DrawPurpose = DrawPurpose.Dynamics;
                    redraw.DoRedraw(line);

                } else
                {
                    DEllipse3d ellipse =  new DEllipse3d()
                    {
                    } ;

                    if (!first)
                    {
                        ellipse = new DEllipse3d()
                        {
                        };
                        
                        var bissectorPoint = FindIntersectionOfPerpendicularBisectors(_preprevious, _previous, ev.Point);

                            
                        if (!ellipse.InitEllipticalArcFromCenterAxisEnd(bissectorPoint,_preprevious, ev.Point)) Log.Write("InitEllipticalArc ne fonctionne pas" );
                       
                    }

                    var arc = new ArcElement(Session.Instance.GetActiveDgnModel(),null,ellipse);

                    var redraw = new RedrawElems();
                    redraw.SetDynamicsViewsFromActiveViewSet(ev.Viewport);
                    redraw.DrawMode = DgnDrawMode.TempDraw;
                    redraw.DrawPurpose = DrawPurpose.Dynamics;
                    redraw.DoRedraw(arc);
                }
                
                return;
            }
        }

        protected override bool OnInstall()
        {
            
            return true;
        }


        public static void InstallNewInstance(Linear linear)
        {
            SimpleLinearPlaceTool tool = new(linear,0, 0);
            tool.InstallTool();
        }
        #endregion

        public DPoint3d FindIntersectionOfPerpendicularBisectors(DPoint3d A, DPoint3d B, DPoint3d C)
        {
            // Calculez les milieux des segments AB et BC
            var midAB = new DPoint3d((A.X + B.X) / 2, (A.Y + B.Y) / 2, (A.Z + B.Z) / 2);
            var midBC = new DPoint3d((B.X + C.X) / 2, (B.Y + C.Y) / 2, (B.Z + C.Z) / 2);

            double perpSlopeAB, perpSlopeBC;

            // Calculez les pentes des segments AB et BC, en gérant les cas verticaux
            if (B.X == A.X)
            {
                perpSlopeAB = 0; // La perpendiculaire à un segment vertical est horizontale
            }
            else
            {
                var slopeAB = (B.Y - A.Y) / (B.X - A.X);
                perpSlopeAB = -1 / slopeAB;
            }

            if (C.X == B.X)
            {
                perpSlopeBC = 0; // La perpendiculaire à un segment vertical est horizontale
            }
            else
            {
                var slopeBC = (C.Y - B.Y) / (C.X - B.X);
                perpSlopeBC = -1 / slopeBC;
            }

            double x, y;

            // Résolvez le système pour trouver l'intersection, en gérant les cas particuliers
            if (perpSlopeAB == perpSlopeBC)
            {
                
                return new DPoint3d(-1, -1, -1);
            }
            else if (B.X == A.X) // AB est vertical
            {
                x = midAB.X;
                y = perpSlopeBC * (x - midBC.X) + midBC.Y;
            }
            else if (C.X == B.X) // BC est vertical
            {
                x = midBC.X;
                y = perpSlopeAB * (x - midAB.X) + midAB.Y;
            }
            else if (perpSlopeAB == 0) // AB est horizontal
            {
                y = midAB.Y;
                x = (y - midBC.Y) / perpSlopeBC + midBC.X;
            }
            else if (perpSlopeBC == 0) // BC est horizontal
            {
                y = midBC.Y;
                x = (y - midAB.Y) / perpSlopeAB + midAB.X;
            }
            else
            {
                x = (perpSlopeBC * midBC.X - perpSlopeAB * midAB.X + midAB.Y - midBC.Y) / (perpSlopeBC - perpSlopeAB);
                y = perpSlopeAB * (x - midAB.X) + midAB.Y;
            }

            var z = (midAB.Z + midBC.Z) / 2; // En supposant que les points A, B et C sont coplanaires dans XY.

            return new DPoint3d(x, y, z);
        }
    }
}
