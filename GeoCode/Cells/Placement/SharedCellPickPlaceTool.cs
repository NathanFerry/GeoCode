using System;
using Bentley.DgnPlatformNET;
using Bentley.GeometryNET;
using Element = Bentley.DgnPlatformNET.Elements.Element;
using SharedCellElement = Bentley.DgnPlatformNET.Elements.SharedCellElement;

namespace GeoCode.Cells.Placement
{
    public class SharedCellPickPlaceTool : DgnElementSetTool
    {
        private SharedCellElement _selectedCell = null;
        private SharedCellElement _newSharedCell = null;
        private DPoint3d? _originPoint = null;
        private DPoint3d? _vectorPoint = null;
        private DTransform3d _rotation;
        private DTransform3d _xScale;
        private DTransform3d _yScale;

        public SharedCellPickPlaceTool(int toolId, int promptId) : base(1, 1) { }

        protected override bool OnDataButton(DgnButtonEvent ev)
        {
            var hitPath = DoLocate(ev, true, (int)ComponentMode.Innermost);

            if (hitPath?.GetHeadElement() is SharedCellElement sharedCellElement && _originPoint == null)
            {
                _selectedCell = sharedCellElement;
                
                BeginDynamics();
                return false;
            }
            
            if (_selectedCell == null) return false;
 
            if (_originPoint == null)
            {
                _originPoint = ev.Point;
                return false;
            }

            if (_vectorPoint == null)
            {
                _vectorPoint = ev.Point;
                return false;
            }

            _newSharedCell.AddToModel();
            Console.WriteLine($@"Placed {_newSharedCell.CellName} with ID : {_newSharedCell.ElementId}");

            _originPoint = null;
            _vectorPoint = null;
            
            return true;
        }
        
        protected override bool OnResetButton(DgnButtonEvent ev)
        {
            this.ExitTool();
            return true;
        }

        private void TransformSharedCellSetup(DgnButtonEvent ev)
        {

            _newSharedCell = _originPoint != null
                ? SharedCellHelper.CreateSharedCell(_selectedCell, (DPoint3d)_originPoint)
                : SharedCellHelper.CreateSharedCell(_selectedCell, ev.Point);
                
            
            if (_originPoint == null)
            {
                return;
            }

            if (_vectorPoint == null)
            {
                
                var vector = new DVector3d((DPoint3d)_originPoint);
                var hDirection = new DVector3d(ev.Point) - vector;
                var angleOffset = vector.AngleToXY(new DVector3d(vector.X + 1, vector.Y, vector.Z) - vector);
                var angle = vector.AngleToXY(hDirection) - angleOffset;
                var axis = DVector3d.UnitZ;
                var pointOnLine = (DPoint3d)_originPoint;
                _rotation = DTransform3d.FromRotationAroundLine(pointOnLine, axis, angle);
                
                var cellLength = SharedCellHelper.ComputeLength(_selectedCell);
                var hDistance = new DVector3d(_originPoint.Value).Distance(new DVector3d(ev.Point));
                var factor = hDistance / cellLength;
                DTransform3d.TryFixedPointAndDirectionalScale((DPoint3d)_originPoint, hDirection, factor,
                    out _xScale);

                _newSharedCell.ApplyTransform(new TransformInfo(_rotation));
                _newSharedCell.ApplyTransform(new TransformInfo(_xScale));
                
                return;
            }

            var vAngle = Angle.FromDegrees(_vectorPoint.Value.Y < ev.Point.Y ? 90 : -90);
            var vDirection = new DVector3d((DPoint3d)_originPoint, (DPoint3d)_vectorPoint).RotateXY(vAngle);
            var cellWidth = SharedCellHelper.ComputeWidth(_selectedCell);
            var vDistance = _vectorPoint.Value.DistanceXY(ev.Point);
            var vFactor = vDistance / cellWidth * (vDirection.Y < 0 ? -1 : 1);
            DTransform3d.TryFixedPointAndDirectionalScale((DPoint3d)_vectorPoint, vDirection, vFactor, out _yScale);

            _newSharedCell.ApplyTransform(new TransformInfo(_rotation));
            _newSharedCell.ApplyTransform(new TransformInfo(_xScale));
            _newSharedCell.ApplyTransform(new TransformInfo(_yScale));

        }

        protected override void OnDynamicFrame(DgnButtonEvent ev)
        {
            TransformSharedCellSetup(ev);
                
            var redraw = new RedrawElems();
            redraw.SetDynamicsViewsFromActiveViewSet(ev.Viewport);
            redraw.DrawMode = DgnDrawMode.TempDraw;
            redraw.DrawPurpose = DrawPurpose.Dynamics;
            redraw.SearchDgnAttachments = false;
            redraw.DoRedraw(_newSharedCell);
        }

        public override StatusInt OnElementModify(Element element)
        {
            return new StatusInt(0);
        }
        
        protected override void OnRestartTool()
        {
            InstallNewInstance();    
        }
        
        protected override void ExitTool()
        {
            this._selectedCell?.Invalidate();
            this._selectedCell = null;

            this._originPoint = null;
            
            base.ExitTool();
        }

        public static void InstallNewInstance()
        {
            new SharedCellPickPlaceTool(0, 0).InstallTool();
        }
    }
}