/*--------------------------------------------------------------------------------------+
|   OnePointRotationPlaceTool.cs
|
+--------------------------------------------------------------------------------------*/

#region Bentley Namespaces
using Bentley.DgnPlatformNET;
using Bentley.DgnPlatformNET.Elements;
using Bentley.GeometryNET;
using Bentley.MstnPlatformNET;
using GeoCode.Elements.Drawing;
using GeoCode.Utils;

#endregion

namespace GeoCode.Cells.Placement.PlacementTools
{
    public class TwoPointsRotationPlaceTool : DgnPrimitiveTool
    {
        private readonly SharedCellDefinitionElement _cellDefinition;
        private readonly SharedCellElement _cellElement;
        private DPoint3d? _origin;
        public TwoPointsRotationPlaceTool(SharedCellDefinitionElement cellDefinition, int toolName, int toolPrompt) : base(toolName, toolPrompt) 
        {
            _cellDefinition = cellDefinition;
            _cellElement = SharedCellHelper.CreateSharedCell(cellDefinition, DPoint3d.Zero);
        }
        

        #region DgnPrimitiveTool Members
        protected override bool OnDataButton(DgnButtonEvent ev)
        {
            // Origine de la cellule
            if (_origin is null)
            {
                _origin = ev.Point;
                CellPlacement.PlaceTopoPoint(ev);
                return false;
            }

            //Rotation faite (sans point Topo)
            _cellElement.AddToModel();

            _origin = null;
            return true;
        }


        protected override void OnPostInstall()
        {
            AccuSnap.SnapEnabled = true;
            BeginDynamics();
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
               Draw.TranslateCell(_cellElement, ev.Point);
            } else
            {
                var direction = new DVector3d(_origin.Value, ev.Point);
                var angle = DVector3d.UnitX.AngleToXY(direction) - _cellElement.GetActualXYAngle(out _);
                Draw.RotateCellAroundZ(_cellElement, angle);
            }

            Draw.DrawDynamicElement(_cellElement, ev);
        }

        public static void InstallNewInstance(SharedCellDefinitionElement cellDefinition)
        {
            new TwoPointsRotationPlaceTool(cellDefinition, 0, 0).InstallTool();
        }
        #endregion
    }
}
