/*--------------------------------------------------------------------------------------+
|   TwoPointsRotationScalingSymmetricalPlaceTool.cs
|
+--------------------------------------------------------------------------------------*/

#region Bentley Namespaces
using Bentley.DgnPlatformNET;
using Bentley.DgnPlatformNET.Elements;
using Bentley.GeometryNET;
using Bentley.MstnPlatformNET;
using GeoCode.Elements.Drawing;
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
        private bool _rotated = false;
        public TwoPointsRotationScalingSymmetricalPlaceTool(SharedCellDefinitionElement cellDefinition, int toolName, int toolPrompt) : base(toolName, toolPrompt) 
        {
            _cellDefinition = cellDefinition;
            _cellElement = SharedCellHelper.CreateSharedCell(cellDefinition, DPoint3d.Zero);
        }

        #region DgnPrimitiveTool Members

        protected override void OnPostInstall()
        {
            AccuSnap.SnapEnabled = true;
            BeginDynamics();
        }
        protected override bool OnDataButton(DgnButtonEvent ev)
        {

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
            if (_origin == null)
            {
               Draw.TranslateCell(_cellElement,ev.Point);
            }
            else if (_vectorPoint == null)
            {
                var direction = new DVector3d(_origin.Value, ev.Point);
                if (direction != DVector3d.Zero)
                {
                    var angle = DVector3d.UnitX.AngleToXY(direction) - _cellElement.GetActualXYAngle(out _);
                    Draw.RotateCellAroundZ(_cellElement,angle);
                }

                Draw.ScaleCellSquare(_cellElement, _origin.Value, ev.Point);
            }
            else
            {
                _rotated = Draw.AxeRotateVertical(_cellElement, _origin.Value, _vectorPoint.Value, ev.Point,_rotated);
            }

            Draw.DrawDynamicElement(_cellElement, ev);
        }

        

        public static void InstallNewInstance(SharedCellDefinitionElement cellDefinition)
        {
            TwoPointsRotationScalingSymmetricalPlaceTool tool = new(cellDefinition,0, 0);
            tool.InstallTool();
        }
        #endregion
    }
}
