/*--------------------------------------------------------------------------------------+
|   OnePointOnLinearPlaceTool.cs
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

#endregion

namespace GeoCode.Cells.Placement.PlacementTools
{
    internal class OnePointOnLinearPlaceTool : DgnPrimitiveTool
    {
        private readonly SharedCellDefinitionElement _cellDefinition;
        private readonly SharedCellElement _cellElement;
        private List<LineStringElement> _lines = new List<LineStringElement>();
        private LineStringElement _lineElement;
        public DPoint3d _origin;

        public OnePointOnLinearPlaceTool(SharedCellDefinitionElement cellDefinition, int toolName, int toolPrompt) : base(toolName, toolPrompt) {
            _cellDefinition = cellDefinition;
            _cellElement = SharedCellHelper.CreateSharedCell(cellDefinition, DPoint3d.Zero);
        }

        #region DgnPrimitiveTool Members
        protected override bool OnDataButton(DgnButtonEvent ev)
        {
            if (!DynamicsStarted)
            {

                var elements = Scan.GetElements(new List<MSElementType>() { MSElementType.LineString });
                Log.Write("Elements retrouvés : " + elements.Count);
                foreach (var element in elements)
                {
                   
                    if (element.TypeName == "Ligne Brisée")
                    {
                        _lines.Add(element as LineStringElement);
                    }
                }
                BeginDynamics();
                return false;
            }
            
                if (_lineElement != null)
                {
                    var point = _lineElement.GetClosestPointFrom(ev.Point,out var angle);
                    if (point.Distance(ev.Point) <= GeoCode.Application.MaxDistClose*100)
                    {
                        _origin = point;
                        _cellElement.GetSnapOrigin(out var origin);
                        var translation = DTransform3d.FromTranslation(_origin - origin);
                        _cellElement.ApplyTransform(new TransformInfo(translation));

                   

                        var rotation = DTransform3d.FromRotationAroundLine(_origin,DVector3d.UnitZ,angle);
                        _cellElement.ApplyTransform(new TransformInfo(rotation));
                    }
                    _cellElement.AddToModel();
                    ExitTool();
                    return true;
                }
                else
                {
                    Log.Write("Aucun linéaire assez proche ");
            }
            

            return false;
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
            
            double distance = double.MaxValue;
            bool found = false;
            foreach (var element in _lines)
            {
                if (element.GetClosestPointFrom(ev.Point,out var _).Distance(ev.Point) < distance
                    && element.GetClosestPointFrom(ev.Point, out var _).Distance(ev.Point) <= GeoCode.Application.MaxDistClose*100)
                {
                    found = true;
                    _lineElement = element;
                    distance = element.GetClosestPointFrom(ev.Point, out var _).Distance(ev.Point);
                }
            }
            if (!found) {
                _lineElement = null; }
            
            if (_lineElement != null)
            {
               
                var point = _lineElement.GetClosestPointFrom(ev.Point, out var angle);
                _cellElement.GetSnapOrigin(out var origin);
                var translation = DTransform3d.FromTranslation(point - origin);
                _cellElement.ApplyTransform(new TransformInfo(translation));
               
            } else
            {
                
                _cellElement.GetSnapOrigin(out var origin);
                _cellElement.ApplyTransform(new TransformInfo(DTransform3d.FromTranslation(ev.Point - origin)));
            }

            var redraw = new RedrawElems();
            redraw.SetDynamicsViewsFromActiveViewSet(ev.Viewport);
            redraw.DrawMode = DgnDrawMode.TempDraw;
            redraw.DrawPurpose = DrawPurpose.Dynamics;
            redraw.DoRedraw(_cellElement);

        }
        public static void InstallNewInstance(SharedCellDefinitionElement cellDefinition)
        {
            Log.Write("Installation placement en cours");
            OnePointOnLinearPlaceTool tool = new OnePointOnLinearPlaceTool(cellDefinition,0, 0);
            
            tool.InstallTool();
        }
        #endregion
    }
}
