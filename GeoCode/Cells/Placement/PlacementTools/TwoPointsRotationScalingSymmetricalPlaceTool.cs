/*--------------------------------------------------------------------------------------+
|   TwoPointsRotationScalingSymmetricalPlaceTool.cs
|
+--------------------------------------------------------------------------------------*/

#region Bentley Namespaces
using Bentley.DgnPlatformNET;
using Bentley.DgnPlatformNET.Elements;
using Bentley.GeometryNET;
using Bentley.MstnPlatformNET;
using GeoCode.Utils;
using System;

#endregion

namespace GeoCode.Cells.Placement.PlacementTools
{
    internal class TwoPointsRotationScalingSymmetricalPlaceTool : DgnPrimitiveTool
    {
        private readonly SharedCellDefinitionElement _cellDefinition;
        private readonly SharedCellElement _cellElement;
        private DPoint3d? _origin;
        private DPoint3d? _vectorPoint;
        private int _oldPos = 1;
        public TwoPointsRotationScalingSymmetricalPlaceTool(SharedCellDefinitionElement cellDefinition, int toolName, int toolPrompt) : base(toolName, toolPrompt) 
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

            if (_vectorPoint is null)
            {
                _vectorPoint = ev.Point;
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
            }
            else if (_vectorPoint is null)
            {
                var direction = new DVector3d(_origin.Value, ev.Point);
                if (direction != DVector3d.Zero)
                {
                    var angle = DVector3d.UnitX.AngleToXY(direction) - _cellElement.GetActualXYAngle(out _);
                    var rotationMatrix = DTransform3d.FromRotationAroundLine(_origin.Value,DVector3d.UnitZ, angle);
                    _cellElement.ApplyTransform(new TransformInfo(rotationMatrix));
                }

                _cellElement.GetSnapOrigin(out var origin);
                var distance = _origin.Value.Distance(ev.Point);

                if (distance != 0)
                {
                    //distance = Math.Sqrt(Math.Pow(distance, 2) - Math.Pow(ev.Point.X - origin.X, 2));
                    var factor = Math.Abs(distance / SharedCellHelper.ComputeLength(_cellElement));
                    var scaling = DTransform3d.FromUniformScaleAndFixedPoint(origin, factor);
                    _cellElement.ApplyTransform(new TransformInfo(scaling));
                }
            }
            else
            {
                var pos = (_vectorPoint.Value.X - _origin.Value.X) * (ev.Point.Y - _origin.Value.Y) >
                          (_vectorPoint.Value.Y - _origin.Value.Y) * (ev.Point.X - _origin.Value.X)
                    ? 1
                    : -1;
                if (pos != _oldPos)
                {
                    _cellElement.GetSnapOrigin(out var origin);
                    var factor = Math.Abs(_origin.Value.Distance(_vectorPoint.Value) / SharedCellHelper.ComputeLength(_cellElement));
                    var side = DTransform3d.FromUniformScaleAndFixedPoint(origin, factor);
                    _cellElement.ApplyTransform(new TransformInfo(side));
                }
                _oldPos = pos;
            }

            var redraw = new RedrawElems();
            redraw.SetDynamicsViewsFromActiveViewSet(ev.Viewport);
            redraw.DrawMode = DgnDrawMode.TempDraw;
            redraw.DrawPurpose = DrawPurpose.Dynamics;
            redraw.DoRedraw(_cellElement);
        }

        

        public static void InstallNewInstance(SharedCellDefinitionElement cellDefinition)
        {
            TwoPointsRotationScalingSymmetricalPlaceTool tool = new(cellDefinition,0, 0);
            tool.InstallTool();
        }
        #endregion
    }
}
