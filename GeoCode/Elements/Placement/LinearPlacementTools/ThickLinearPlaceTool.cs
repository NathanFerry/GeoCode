/*--------------------------------------------------------------------------------------+
|   ThickLinearPlaceTool.cs
|
+--------------------------------------------------------------------------------------*/

#region Bentley Namespaces
using Bentley.DgnPlatformNET;
using Bentley.DgnPlatformNET.Elements;
using Bentley.GeometryNET;
using GeoCode.Elements.Drawing;
using GeoCode.Model;
using GeoCode.Utils;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace GeoCode.Cells.Placement.LinearPlacementTools
{

    internal class ThickLinearPlaceTool : DgnPrimitiveTool
    {
        private readonly Linear _linearElement;

        // Les points à stocker pour faire les calculs
        private Node _origin;
        private Node _previous;
        private Node _preprevious;

        private List<Node> _nodes = new List<Node>();

        private List<Node> listPointsUnder = new List<Node>();
        private List<Node> listPointsOn = new List<Node>();

        
        private List<LineElement> _tempLines = new List<LineElement>();

        private bool _nextPointReady = false;
        private bool _verticalPoint = false;

        public ThickLinearPlaceTool(Linear linear,int toolName, int toolPrompt) : base(toolName, toolPrompt) { _linearElement = linear; }

        #region DgnPrimitiveTool Members
        protected override bool OnDataButton(DgnButtonEvent ev)
        {
            if (!DynamicsStarted)
            {
                
                BeginDynamics();
                _previous = new()
                {
                    Point = ev.Point,
                    LType = LineType.DROITE
                };
               
                _origin = _previous;

                _nodes.Add(_origin);
                _nextPointReady = true;

                CellPlacement.PlaceTopoPoint(ev);
                return false;

            }

            if (_nextPointReady)
            {
                // Niveau du linéaire
                var level = DgnHelper.GetAllLevelsFromLibrary().First(element => element.Name == _linearElement.Level);

                // Placement point Topo
                CellPlacement.PlaceTopoPoint(ev);

                //Nouveau noeud
                Node n = new() { Point = ev.Point, LType = LineType.DROITE };

                switch (_previous.LType)
                {
                    case LineType.DROITE:

                        var line = CreateElement.Line(_previous, n);
                        _tempLines.Add(line);
                        Draw.DrawElement(line, level);

                        if (!_linearElement.ThicknessOrlength.HasValue)
                        {
                            Log.Write("Le linéaire n'a pas d'épaisseur donnée");
                            return true;
                        }

                        if (_previous == _origin)
                        {
                            listPointsOn.Add(new() {
                                Point = GeometricFunctions.GetPointOnSegment(_previous.Point, n.Point, _linearElement.ThicknessOrlength.Value * 10000),
                                LType = LineType.DROITE
                            });


                            listPointsUnder.Add(new()
                            {
                                Point = GeometricFunctions.GetPointUnderSegment(_previous.Point, n.Point, _linearElement.ThicknessOrlength.Value * 10000),
                                LType = LineType.DROITE
                            });
                        }
                        else
                        {
                            listPointsOn.Add(new()
                            {
                                Point = GeometricFunctions.GetPointOnBisector(_preprevious.Point, _previous.Point, n.Point, _linearElement.ThicknessOrlength.Value * 10000),
                                LType = LineType.DROITE
                            });
                            listPointsUnder.Add(new()
                            {
                                Point = GeometricFunctions.GetPointUnderBisector(_preprevious.Point, _previous.Point, n.Point, _linearElement.ThicknessOrlength.Value * 10000),
                                LType = LineType.DROITE
                            });
                        }

                        _preprevious = _previous;
                        _previous = n;

                        _nodes.Add(n);

                        break;
                }
                return false;
            }
            if (_verticalPoint == false)
            {
               
                _verticalPoint = true;
                var crossProductDirection = (_previous.Point.X - _origin.Point.X) * (ev.Point.Y - _origin.Point.Y) >
                     (_previous.Point.Y - _origin.Point.Y) * (ev.Point.X - _origin.Point.X)
               ? 1
               : -1;

               
         

                _nodes.AddRange(crossProductDirection == -1 ?  listPointsUnder : listPointsOn);

                _nodes.Add(_origin);

                if (_nodes.Count > 1)
                {
                    var level = DgnHelper.GetAllLevelsFromLibrary()
                    .First(element => element.Name == _linearElement.Level);

                    var complexElement = CreateElement.ComplexString(_nodes);
                    Draw.DrawElement(complexElement,level);

                }

                foreach (var line in _tempLines)
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

        protected override void OnDynamicFrame(DgnButtonEvent ev)
        {
            if (_nextPointReady)
            {
                var n = new Node()
                {
                    Point = ev.Point,
                    LType = LineType.DROITE
                };
                var thirdPoint = new DPoint3d();
                if (_previous != _origin)
                {
                    thirdPoint = GeometricFunctions.GetPointOnBisector(_preprevious.Point, _previous.Point, ev.Point,_linearElement.ThicknessOrlength.Value * 10000);

                    if (thirdPoint.X == -1 && thirdPoint.Y == -1)
                    {
                        thirdPoint = GeometricFunctions.GetPointOnSegment(_previous.Point, ev.Point, _linearElement.ThicknessOrlength.Value * 10000);
                    }
                } else
                {
                    thirdPoint = GeometricFunctions.GetPointOnSegment(_previous.Point, ev.Point, _linearElement.ThicknessOrlength.Value * 10000);
                }
                var line = CreateElement.Line(_previous, n);
                Draw.DrawDynamicElement(line, ev);

                line = CreateElement.Line(_previous, new() { Point = thirdPoint, LType = LineType.DROITE});
                Draw.DrawDynamicElement(line, ev);
                return;
            }

            if (_verticalPoint == false)
            {
                var crossProductDirection = (_previous.Point.X - _origin.Point.X) * (ev.Point.Y - _origin.Point.Y) >
                     (_previous.Point.Y - _origin.Point.Y) * (ev.Point.X - _origin.Point.X)
               ? 1
               : -1;

                var redraw = new RedrawElems();
                redraw.SetDynamicsViewsFromActiveViewSet(ev.Viewport);
                redraw.DrawMode = DgnDrawMode.TempDraw;
                redraw.DrawPurpose = DrawPurpose.Dynamics;

                if (crossProductDirection == -1)
                {
                    var complex = CreateElement.ComplexString(_nodes.Concat(listPointsUnder).ToList());
                    Draw.DrawDynamicElement(complex, ev);

                } else
                {
                    var complex = CreateElement.ComplexString(_nodes.Concat(listPointsOn).ToList());
                    Draw.DrawDynamicElement(complex, ev);
                }
            }
        }

        protected override bool OnResetButton(DgnButtonEvent ev)
        {
            if (_nodes.Count >= 2 && _nextPointReady)
            {
                listPointsOn.Add(new()
                {
                    Point = GeometricFunctions.GetPointUnderSegment(_previous.Point, _preprevious.Point, _linearElement.ThicknessOrlength.Value * 10000),
                    LType = LineType.DROITE
                });
                listPointsUnder.Add(new()
                {
                    Point = GeometricFunctions.GetPointOnSegment(_previous.Point, _preprevious.Point, _linearElement.ThicknessOrlength.Value * 10000),
                    LType = LineType.DROITE
                });


                listPointsUnder.Reverse();
                listPointsOn.Reverse();
            }
            _nextPointReady = false;
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
        #endregion
    }
}
