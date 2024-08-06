using Bentley.GeometryNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoCode.Utils
{
    public static class GeometricFunctions
    {

        public static DPoint3d GetPointOnSegment(DPoint3d A, DPoint3d B, double distance)
        {

            // Calculez le vecteur AB
            var AB_X = B.X - A.X;
            var AB_Y = B.Y - A.Y;

            // Calculez la longueur du vecteur AB
            var AB_length = Math.Sqrt(AB_X * AB_X + AB_Y * AB_Y);

            // Vérifiez si la longueur de AB est non nulle pour éviter la division par zéro
            if (AB_length == 0)
            {
                return new DPoint3d(-1, -1, -1);
            }

            // Calculez le vecteur perpendiculaire à AB
            var perp_X = -AB_Y;
            var perp_Y = AB_X;

            // Normalisez le vecteur perpendiculaire
            var perp_length = Math.Sqrt(perp_X * perp_X + perp_Y * perp_Y);
            if (perp_length == 0)
            {
                return new DPoint3d(-1, -1, -1);

            }

            // Calculez les coordonnées de C
            var x = A.X + (distance) * (perp_X / perp_length);
            var y = A.Y + (distance) * (perp_Y / perp_length);
            var z = A.Z; // Conservez la coordonnée Z de A

            return new DPoint3d(x, y, z);

        }

        public static DPoint3d GetPointUnderSegment(DPoint3d A, DPoint3d B, double distance)
        {
            // Calculez le vecteur AB
            var AB_X = B.X - A.X;
            var AB_Y = B.Y - A.Y;

            // Calculez la longueur du vecteur AB
            var AB_length = Math.Sqrt(AB_X * AB_X + AB_Y * AB_Y);

            // Vérifiez si la longueur de AB est non nulle pour éviter la division par zéro
            if (AB_length == 0)
            {
                return new DPoint3d(-1, -1, -1);

            }

            // Calculez le vecteur perpendiculaire à AB
            var perp_X = -AB_Y;
            var perp_Y = AB_X;

            // Normalisez le vecteur perpendiculaire
            var perp_length = Math.Sqrt(perp_X * perp_X + perp_Y * perp_Y);
            if (perp_length == 0)
            {
                return new DPoint3d(-1, -1, -1);

            }

            // Calculez les coordonnées de C
            var x = A.X + (-distance) * (perp_X / perp_length);
            var y = A.Y + (-distance) * (perp_Y / perp_length);
            var z = A.Z; // Conservez la coordonnée Z de A

            return new DPoint3d(x, y, z);
        }

        public static DPoint3d GetPointOnBisector(DPoint3d A, DPoint3d B, DPoint3d C, double distance)
        {
            // Calculez les distances des points A et C à B
            var distBA = Math.Sqrt(Math.Pow(A.X - B.X, 2) + Math.Pow(A.Y - B.Y, 2));
            var distBC = Math.Sqrt(Math.Pow(C.X - B.X, 2) + Math.Pow(C.Y - B.Y, 2));

            // Vérifiez si les distances sont non nulles pour éviter la division par zéro
            if (distBA == 0 || distBC == 0)
            {
                return new DPoint3d(-1, -1, -1);

            }

            // Normalisez les vecteurs BA et BC
            var unitBA_X = (A.X - B.X) / distBA;
            var unitBA_Y = (A.Y - B.Y) / distBA;
            var unitBC_X = (C.X - B.X) / distBC;
            var unitBC_Y = (C.Y - B.Y) / distBC;

            // Calculez les coordonnées de la bissectrice
            var bisectorX = unitBA_X + unitBC_X;
            var bisectorY = unitBA_Y + unitBC_Y;

            // Normalisez le vecteur de la bissectrice
            var bisectorLength = Math.Sqrt(bisectorX * bisectorX + bisectorY * bisectorY);
            if (bisectorLength == 0)
            {
                return new DPoint3d(-1, -1, -1);

            }


            var normX = (bisectorX / bisectorLength);
            var normY = (bisectorY / bisectorLength);

            var crossProduct = (A.X - B.X) * (C.Y - B.Y) - (A.Y - B.Y) * (C.X - B.X);
            if (crossProduct > 0)
            {
                normX = -normX;
                normY = -normY;
            }


            // Calculez les coordonnées de D
            var x = B.X + (distance) * (normX);
            var y = B.Y + (distance) * (normY);
            var z = B.Z; // Conservez la coordonnée Z de B

            return new DPoint3d(x, y, z);
        }

        public static DPoint3d GetPointUnderBisector(DPoint3d A, DPoint3d B, DPoint3d C, double distance)
        {
            // Calculez les distances des points A et C à B
            var distBA = Math.Sqrt(Math.Pow(A.X - B.X, 2) + Math.Pow(A.Y - B.Y, 2));
            var distBC = Math.Sqrt(Math.Pow(C.X - B.X, 2) + Math.Pow(C.Y - B.Y, 2));

            // Vérifiez si les distances sont non nulles pour éviter la division par zéro
            if (distBA == 0 || distBC == 0)
            {
                return new DPoint3d(-1, -1, -1);
            }

            // Normalisez les vecteurs BA et BC
            var unitBA_X = (A.X - B.X) / distBA;
            var unitBA_Y = (A.Y - B.Y) / distBA;
            var unitBC_X = (C.X - B.X) / distBC;
            var unitBC_Y = (C.Y - B.Y) / distBC;

            // Calculez les coordonnées de la bissectrice
            var bisectorX = unitBA_X + unitBC_X;
            var bisectorY = unitBA_Y + unitBC_Y;

            // Normalisez le vecteur de la bissectrice
            var bisectorLength = Math.Sqrt(bisectorX * bisectorX + bisectorY * bisectorY);
            if (bisectorLength == 0)
            {
                return new DPoint3d(-1, -1, -1);

            }

            var normX = (bisectorX / bisectorLength);
            var normY = (bisectorY / bisectorLength);


            var crossProduct = (A.X - B.X) * (C.Y - B.Y) - (A.Y - B.Y) * (C.X - B.X);
            if (crossProduct > 0)
            {
                normX = -normX;
                normY = -normY;
            }

            // Calculez les coordonnées de D
            var x = B.X + (-distance) * (normX);
            var y = B.Y + (-distance) * (normY);
            var z = B.Z; // Conservez la coordonnée Z de B

            return new DPoint3d(x, y, z);
        }
    }
}
