using Bentley.DgnPlatformNET;
using Bentley.DgnPlatformNET.Elements;
using Bentley.GeometryNET;

namespace GeoCode.Utils;

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

    public static Angle GetActualXYAngle(this DisplayableElement element, out bool isXYRotation)
    {
        element.GetOrientation(out var orientation);
        isXYRotation = orientation.IsXYRotation(out var angle);
        return angle;
    }
}