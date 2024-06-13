using System;
using Bentley.DgnPlatformNET.Elements;
using Bentley.GeometryNET;
using Bentley.MstnPlatformNET;
using GeoCode.Utils;
using SharedCellElement = Bentley.DgnPlatformNET.Elements.SharedCellElement;

namespace GeoCode.Cells;

public static class SharedCellHelper
{
    /// <summary>
    /// Use to create a new SharedCell because Microstation's SDK doesn't provide a method to do it.
    /// </summary>
    /// <param name="referenceCell"></param>
    /// <param name="origin"></param>
    /// <returns></returns>
    public static SharedCellElement CreateSharedCell(SharedCellElement referenceCell, DPoint3d origin)
    {
        return new SharedCellElement(
            Session.Instance.GetActiveDgnModel(),
            referenceCell,
            referenceCell.CellName,
            origin,
            DMatrix3d.Identity,
            referenceCell.Scale
        );
    }
    
    /// <summary>
    /// Use to create a new SharedCell because Microstation's SDK doesn't provide a method to do it.
    /// </summary>
    /// <param name="cellDefinition"></param>
    /// <param name="origin"></param>
    /// <returns></returns>
    public static SharedCellElement CreateSharedCell(SharedCellDefinitionElement cellDefinition, DPoint3d origin)
    {
        return new SharedCellElement(
            Session.Instance.GetActiveDgnModel(),
            cellDefinition,
            cellDefinition.CellName,
            origin,
            DMatrix3d.Identity,
            cellDefinition.Scale
        );
    }

    #region Documentation
    /*
    |   Microstation Cells are represented and displayed in two different ways:
    |   - The Cell itself, which is the "drawing" of the shape you see.
    |   - The DisplayableElement, which is the box containing the cell.
    |   I didn't find who to get the length and width of the cell.
    |   However, I did find who to get DisplayableElement length and width, so I used them, along with some trigonometry
    |   to calculate the value for the cell. 
     */ 
    #endregion
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cell"></param>
    /// <returns>The length of the drawing displayed</returns>
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cell"></param>
    /// <returns>The length of the drawing displayed</returns>
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