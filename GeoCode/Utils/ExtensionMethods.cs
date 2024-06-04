using System;
using Bentley.DgnPlatformNET;
using Bentley.GeometryNET;
using Point3d = Bentley.Interop.MicroStationDGN.Point3d;

namespace GeoCode
{
    public static class ExtensionMethods
    {
        public static Point3d ToPoint3d(this DPoint3d point)
        {
            return new Point3d()
            {
                X = point.X,
                Y = point.Y,
                Z = point.Z
            };
        }
        
        public static double ToRadians(this double val)
        {
            return (Math.PI / 180) * val;
        }

        public static ElementPropertiesSetter SetLevelChain(this ElementPropertiesSetter eps, LevelId level)
        {
            eps.SetLevel(level);
            return eps;
        }
    }
}