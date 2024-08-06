using Bentley.DgnPlatformNET;
using Bentley.DgnPlatformNET.Elements;
using Bentley.GeometryNET;
using Bentley.MstnPlatformNET;
using System;

namespace GeoCode.Utils;

/*-----------------------------------------------------
|   Those are extension methods relative to Microstation's SDK elements,
-----------------------------------------------------*/
public static class BentleyExtensionMethods
{
    public static ElementPropertiesSetter SetLevelChain(this ElementPropertiesSetter eps, LevelId level)
    {
        
        eps.SetLevel(level);
        return eps;
    }

    public static ElementPropertiesSetter SetColorChain(this ElementPropertiesSetter eps, uint color)
    {
        eps.SetColor(color);
        return eps;
    }

    public static ElementPropertiesSetter SetLineStyleChain(this ElementPropertiesSetter eps,LevelDefinitionLineStyle style)
    {
        eps.SetLinestyle(0,style.GetStyleParams());
        return eps;
    }

    public static double GetDisplayableElementXYDiagonalLength(this DisplayableElement displayableElement)
    {
        displayableElement.CalcElementRange(out var range);
        return range.DiagonalVector.MagnitudeXY;
    }

    public static Angle GetActualXYAngle(this DisplayableElement element, out bool isXYRotation)
    {
        element.GetOrientation(out var orientation);
        isXYRotation = orientation.IsXYRotation(out var angle);
        return angle;
    }

    public static DPoint3d GetClosestPointFrom(this LineStringElement lineString,DPoint3d point, out Angle angle)
    {
        
        try
        {
            var curveVector = lineString.GetCurveVector();
            var curve = curveVector.ClosestPointBounded(point);

            var index = curveVector.CurveLocationDetailIndex(curve);

            var c = curveVector.GetAt((int)index);
            angle = new Angle();
            if (c.GetStartEnd(out var p1, out var p2))
            {
                angle = new LineElement(Session.Instance.GetActiveDgnModel(), null, new DSegment3d { StartPoint = p1, EndPoint = p2 }).GetActualXYAngle(out var _);
            }
            return curve.Point;
        } catch (Exception ex) 
        { 
            Log.Write(ex.ToString());
        }
        angle = new Angle();
        return new DPoint3d(1,1,1);
    }

    public static DPoint3d GetClosestPointFrom(this LineElement line, DPoint3d point, out Angle angle)
    {

        try
        {
            var curveVector = line.GetCurveVector();
            var curve = curveVector.ClosestPointBounded(point);

            var index = curveVector.CurveLocationDetailIndex(curve);

            var c = curveVector.GetAt((int)index);
            angle = new Angle();
            if (c.GetStartEnd(out var p1, out var p2))
            {
                angle = new LineElement(Session.Instance.GetActiveDgnModel(), null, new DSegment3d { StartPoint = p1, EndPoint = p2 }).GetActualXYAngle(out var _);
            }
            return curve.Point;
        }
        catch (Exception ex)
        {
            Log.Write(ex.ToString());
        }
        angle = new Angle();
        return new DPoint3d(1, 1, 1);
    }
}