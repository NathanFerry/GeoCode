using Bentley.DgnPlatformNET;
using Bentley.DgnPlatformNET.Elements;
using Bentley.GeometryNET;
using GeoCode.Cells;
using GeoCode.Utils;

namespace GeoCode.Elements.Drawing
{
    public static class Draw
    {

        public static void DrawElement(Element e, LevelHandle level)
        {
            if (e != null)
            {


                new ElementPropertiesSetter()
                   .SetLevelChain(level.LevelId)
                   .SetColorChain(level.GetByLevelColor().Color)
                   .SetLineStyleChain(level.GetByLevelLineStyle())
                   .Apply(e);


                if (e.AddToModel() != StatusInt.Success) { Log.Write("L'élément " + e.ElementId + " n'a pas pu être ajouté au modèle."); }
            }
        }

        public static void DrawDynamicElement(Element e,DgnButtonEvent ev)
        {
            if (e != null)
            {
                var redraw = new RedrawElems();
                redraw.SetDynamicsViewsFromActiveViewSet(ev.Viewport);
                redraw.DrawMode = DgnDrawMode.TempDraw;
                redraw.DrawPurpose = DrawPurpose.Dynamics;
                redraw.DoRedraw(e);
            }
        }


        //================== Transformation Cellules ==================\\ 

        // ---------------- Translation ---------------- \\
        public static void TranslateCell(SharedCellElement c, DPoint3d point) 
        {
            c.GetSnapOrigin(out var origin);
            var translation = DTransform3d.FromTranslation(point - origin);
            c.ApplyTransform(new TransformInfo(translation));
        }

        // ---------------- Rotation ---------------- \\
        public static void RotateCellAroundZ(SharedCellElement c, Angle angle)
        {
            c.GetSnapOrigin(out var origin);
            var rotation = DTransform3d.FromRotationAroundLine(origin, DVector3d.UnitZ, angle);
            c.ApplyTransform(new TransformInfo(rotation));
        }

        // ---------------- Mise à l'échelle ---------------- \\
        public static void ScaleCellSquare(SharedCellElement c, DPoint3d origin, DPoint3d eventPoint)
        {
            var factor = origin.DistanceXY(eventPoint) / (SharedCellHelper.ComputeLength(c));
            c.ApplyTransform(new TransformInfo(
                DTransform3d.FromUniformScaleAndFixedPoint(origin, factor)));
        }

        public static void ScaleCellFromDirectionHorizontal(SharedCellElement c, DPoint3d origin,DPoint3d eventPoint)
        {
            var factor = origin.DistanceXY(eventPoint) / SharedCellHelper.ComputeLength(c);
            DTransform3d.TryFixedPointAndDirectionalScale(origin, new DVector3d(origin,eventPoint), factor, out var scaling);
            c.ApplyTransform(new TransformInfo(scaling));
        }

        public static bool ScaleCellFromDirectionVertical(SharedCellElement c, DPoint3d origin, DPoint3d horizontalPoint, DPoint3d eventPoint,bool rotated)
        {
            var crossProductDirection = (horizontalPoint.X - origin.X) * (eventPoint.Y - origin.Y) >
                      (horizontalPoint.Y - origin.Y) * (eventPoint.X - origin.X)
                ? 1
                : -1;

            var direction = new DVector3d(origin, horizontalPoint);
            direction.TryUnitPerpendicularXY(out var perpendicular);


            var scalingVector = new DVector3d(origin, eventPoint);


            var factor = scalingVector.DotProduct(perpendicular) / SharedCellHelper.ComputeWidth(c);
            if ((crossProductDirection == -1 && !rotated))
            {
                perpendicular = -1 * perpendicular;

                rotated = true;
            }else if ((crossProductDirection == 1 && rotated))
            {
                perpendicular = -1 * perpendicular;
                rotated = false;
            }

            DTransform3d.TryFixedPointAndDirectionalScale(origin, perpendicular, factor, out var scale);


            c.ApplyTransform(new TransformInfo(scale));

            return rotated;
        }

        public static bool AxeRotateVertical(SharedCellElement c, DPoint3d origin, DPoint3d horizontalPoint, DPoint3d eventPoint,bool rotated)
        {
            var crossProductDirection = (horizontalPoint.X - origin.X) * (eventPoint.Y - origin.Y) >
                      (horizontalPoint.Y - origin.Y) * (eventPoint.X - origin.X)
                ? 1
                : -1;
            if ((crossProductDirection == -1 && !rotated) || (crossProductDirection == 1 && rotated))
            {
                DTransform3d.TryFixedPointAndDirectionalScale(origin, new DVector3d(origin, horizontalPoint).RotateXY(Angle.FromDegrees(90)), -1, out var scaling);
                c.ApplyTransform(new TransformInfo(scaling));
                return !rotated;
            }

            return rotated;
        }

    }
}
