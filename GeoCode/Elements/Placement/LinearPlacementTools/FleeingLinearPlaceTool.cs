/*--------------------------------------------------------------------------------------+
|   FleeingLinearPlaceTool.cs
|
+--------------------------------------------------------------------------------------*/

#region Bentley Namespaces
using Bentley.DgnPlatformNET;
using Bentley.DgnPlatformNET.Elements;
using Bentley.GeometryNET;
using Bentley.MstnPlatformNET;
using GeoCode.Elements.Drawing;
using GeoCode.Model;
using GeoCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Windows.Documents;
using System.Windows.Forms;

#endregion

namespace GeoCode.Cells.Placement.LinearPlacementTools
{
    internal class FleeingLinearPlaceTool : DgnPrimitiveTool
    {
        private Linear _linearElement;

        private Node _preprevious;
        private Node _previous;
        private Node _origin;

        private List<Node> _nodesOn = new List<Node>();
        private List<Node> _nodesUnder = new List<Node>();

        private bool _nextPointReady = false;
        private bool _verticalPoint = false;
        private List<LineElement> tempLines = new List<LineElement>();
        private LineStringElement _lineElement;

        public FleeingLinearPlaceTool(Linear linear,int toolName, int toolPrompt) : base(toolName, toolPrompt) { _linearElement = linear; }

        #region DgnPrimitiveTool Members
        protected override bool OnDataButton(DgnButtonEvent ev)
        {
            if (_origin == null)
            {
                CellPlacement.PlaceTopoPoint(ev);
               
                _origin = new()
                {
                    Point = ev.Point,
                    LType = LineType.DROITE
                };
                _previous = _origin;
                _nextPointReady =true;


                return false;
            }
            if (_nextPointReady)
            {
                var level = DgnHelper.GetAllLevelsFromLibrary()
                    .First(element => element.Name == _linearElement.Level);

                CellPlacement.PlaceTopoPoint(ev);

                var n = new Node() { Point = ev.Point, LType=LineType.DROITE };
                var tempLine = CreateElement.Line(_previous,n);

                Draw.DrawElement(tempLine, level);

                switch (_previous.LType)
                {
                    case LineType.DROITE:

                        if (_previous == _origin)
                        {
                            _nodesOn.Add(new()
                            {
                                Point = GeometricFunctions.GetPointOnSegment(_previous.Point, n.Point, _linearElement.ThicknessOrlength.Value * 10000),
                                LType = LineType.DROITE
                            });
                            _nodesOn.Add(_origin);
                            _nodesOn.Add(n);

                            _nodesUnder.Add(new()
                            {
                                Point = GeometricFunctions.GetPointUnderSegment(_previous.Point, n.Point, _linearElement.ThicknessOrlength.Value * 10000),
                                LType = LineType.DROITE
                            });
                            _nodesUnder.Add(_origin);
                            _nodesUnder.Add(n);

                        }
                        else
                        {
                            _nodesOn.Add(new()
                            {
                                Point = GeometricFunctions.GetPointOnBisector(_preprevious.Point,_previous.Point, n.Point, _linearElement.ThicknessOrlength.Value * 10000),
                                LType = LineType.DROITE
                            });
                            _nodesOn.Add(_previous);
                            _nodesOn.Add(n);

                            _nodesUnder.Add(new()
                            {
                                Point = GeometricFunctions.GetPointUnderBisector(_preprevious.Point,_previous.Point, n.Point, _linearElement.ThicknessOrlength.Value * 10000),
                                LType = LineType.DROITE
                            });
                            _nodesUnder.Add(_previous);
                            _nodesUnder.Add(n);
                        }

                        _preprevious = _previous;
                        _previous = n;

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

                if (_nodesOn.Count > 1)
                {
                    var level = DgnHelper.GetAllLevelsFromLibrary()
                    .First(element => element.Name == _linearElement.Level);

                    if (crossProductDirection == -1)
                    {
                        var complexElement = CreateElement.ComplexString(_nodesUnder);
                        Draw.DrawElement(complexElement, level);
                    }
                    else
                    {
                        var complexElement = CreateElement.ComplexString(_nodesOn);
                        Draw.DrawElement(complexElement, level);
                    }
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
            AccuDraw.Active = true;
            BeginDynamics();
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
                    thirdPoint = GeometricFunctions.GetPointOnBisector(_preprevious.Point, _previous.Point, ev.Point, _linearElement.ThicknessOrlength.Value * 10000);
                }
                else
                {
                    thirdPoint = GeometricFunctions.GetPointOnSegment(_previous.Point, ev.Point, _linearElement.ThicknessOrlength.Value * 10000);
                }
                var line = CreateElement.Line(_previous, n);
                Draw.DrawDynamicElement(line, ev);

                line = CreateElement.Line(_previous, new() { Point = thirdPoint, LType = LineType.DROITE });
                Draw.DrawDynamicElement(line, ev);
                return;
            } else 
            if (_verticalPoint)
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
                    var complex = CreateElement.ComplexString(_nodesUnder);
                    Draw.DrawDynamicElement(complex, ev);
                }
                else
                {
                    var complex = CreateElement.ComplexString(_nodesOn);
                    Draw.DrawDynamicElement(complex, ev);
                }

            }
        }

        protected override bool OnResetButton(DgnButtonEvent ev)
        {
            if (_nodesOn.Count > 2 && _nextPointReady)
            {
                _nodesOn.Add(new()
                {
                    Point = GeometricFunctions.GetPointUnderSegment(_previous.Point, _preprevious.Point, _linearElement.ThicknessOrlength.Value * 10000),
                    LType = LineType.DROITE
                });
                _nodesUnder.Add(new()
                {
                    Point = GeometricFunctions.GetPointOnSegment(_previous.Point, _preprevious.Point, _linearElement.ThicknessOrlength.Value * 10000),
                    LType = LineType.DROITE
                });

            }
            _nextPointReady = false;


            _verticalPoint = false;

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
