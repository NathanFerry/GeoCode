using System;
using Bentley.DgnPlatformNET;
using Bentley.GeometryNET;
using Bentley.MstnPlatformNET;
using SharedCellElement = Bentley.DgnPlatformNET.Elements.SharedCellElement;

namespace GeoCode.Cells
{
    public static class SharedCellHelper
    {
        public static SharedCellElement CreateSharedCell(SharedCellElement referenceCell, DgnButtonEvent ev)
        {
            return new SharedCellElement(
                Session.Instance.GetActiveDgnModel(),
                referenceCell,
                referenceCell.CellName,
                ev.Point,
                DMatrix3d.Identity, 
                new DPoint3d(referenceCell.Scale));
        }

        public static SharedCellElement CreateSharedCell(SharedCellElement referenceCell, DPoint3d origin)
        {
            return new SharedCellElement(
                Session.Instance.GetActiveDgnModel(),
                referenceCell,
                referenceCell.CellName,
                origin,
                DMatrix3d.Identity,
                new DPoint3d(referenceCell.Scale));
        }

        public static double ComputeLength(SharedCellElement cell)
        {
            cell.GetOrientation(out var orientation);
            orientation.IsXYRotation(out var angle);
            cell.CalcElementRange(out var range);
            cell.GetSnapOrigin(out var origin);

            var correctedAngle = CorrectAngle(angle);
            
            switch (correctedAngle)
            {
                case 0: return range.XSize;
                case 90: return range.YSize;
                default:
                    var direction = new DVector3d(origin, new DPoint3d(origin.X + 1, origin.Y)).RotateXY(angle);
                    var pointToOffset = angle.Degrees > 0 ? range.High : range.Low;
                    var signe = angle.Degrees > 0 ? -1 : 1;
                    var offsetLength = origin.DistanceXY(direction.X > 0 ? range.Low : range.High) * Math.Tan(correctedAngle.ToRadians());
                    return origin.DistanceXY(
                        direction.X * direction.Y > 0 
                        ? new DPoint3d(pointToOffset.X, pointToOffset.Y + signe * offsetLength) 
                        : new DPoint3d(pointToOffset.X + signe * offsetLength, pointToOffset.Y)
                    );
            }
        }

        public static double ComputeWidth(SharedCellElement cell)
        {
            cell.GetOrientation(out var orientation);
            orientation.IsXYRotation(out var angle);
            cell.CalcElementRange(out var range);
            cell.GetSnapOrigin(out var origin);

            var correctedAngle = CorrectAngle(angle);
            switch (correctedAngle)
            {
                case 0: return range.YSize;
                case 90: return range.XSize;
                default:
                    var direction = new DVector3d(origin, new DPoint3d(origin.X + 1, origin.Y)).RotateXY(angle);
                    var pointToOffset = angle.Degrees > 0 ? range.Low : range.High;
                    var signe = angle.Degrees > 0 ? 1 : -1;
                    var offsetLength = origin.DistanceXY(Math.Abs(angle.Degrees) > 90 ? range.High : range.Low) *
                                       Math.Tan(correctedAngle.ToRadians());
                    return origin.DistanceXY(
                        direction.X * direction.Y > 0 
                            ? new DPoint3d(pointToOffset.X, pointToOffset.Y + signe * offsetLength) 
                            : new DPoint3d(pointToOffset.X + signe * offsetLength, pointToOffset.Y)
                    );
            }
        }

        private static double CorrectAngle(Angle angle)
        {
            var angleAbs = Math.Abs(angle.Degrees);
            switch (angleAbs)
            {
                case 0: return 0;
                case 90: return 90;
                case 180: return 0;
                default: return angleAbs > 90 ? angleAbs - 90 : 90 - angleAbs;
            }
        }
    }
}