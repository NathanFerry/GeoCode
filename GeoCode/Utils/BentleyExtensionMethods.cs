using Bentley.DgnPlatformNET;
using Bentley.DgnPlatformNET.Elements;
using Bentley.GeometryNET;

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

    public static ElementPropertiesSetter SetThicknessChain(this ElementPropertiesSetter eps, double thickness)
    {
        eps.SetThickness(thickness,false);
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
}