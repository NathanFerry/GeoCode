using System;

namespace GeoCode.Utils
{
    public static class ExtensionMethods
    {
        public static double ToRadians(this double val)
        {
            return (Math.PI / 180) * val;
        }
    }
}