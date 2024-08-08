using Bentley.DgnPlatformNET.Elements;
using Bentley.GeometryNET;
using Bentley.MstnPlatformNET;
using GeoCode.Model;
using GeoCode.Utils;
using System.Collections.Generic;
using System.Text;
namespace GeoCode.Elements.Drawing
{
    public static class CreateElement
    {

        public static ArcElement Arc(Node n1, Node n2, Node n3)
        {

            var ellipse = new DEllipse3d();

            var bissectorPoint = FindIntersectionOfPerpendicularBisectors(n1.Point, n2.Point, n3.Point);

            if (!ellipse.InitEllipticalArcFromCenterAxisEnd(bissectorPoint, n1.Point, n3.Point)) { return null; }


            var arc = new ArcElement(
                Session.Instance.GetActiveDgnModel(),
                null,
                ellipse
            );

            return arc;
        }

        public static LineElement Line(Node n1, Node n2)
        {

            var segment = new DSegment3d()
            {
                StartPoint = n1.Point,
                EndPoint = n2.Point
            };


            var line = new LineElement(
                Session.Instance.GetActiveDgnModel(),
                null,
                segment
            );

            return line;
        }

        public static ComplexStringElement ComplexString(List<Node> nodes)
        {
            if (nodes.Count < 2 ) return null;

            Log.Write("Nombres de noeuds suffisant");

            ComplexStringElement complex = new(Session.Instance.GetActiveDgnModel(),null);
            Node previous = null;
            Node preprevious = null;

            var arcDone = false;

            foreach ( Node node in nodes )
            {
                Log.Write($"Previous = null : {previous == null}");
                Log.Write($"Preprevious = null : {preprevious == null}");
                if (previous is null) { previous = node; continue; }
                if (preprevious is null) { 
                    if (previous.LType == LineType.DROITE)
                    {
                        Log.Write(complex.AddComponentElement(Line(previous, node)).ToString());
                        
                    } 
                    preprevious = previous; previous = node; continue;

                }

                switch (previous.LType)
                {
                    case LineType.DROITE:

                        // Création Ligne
                        Log.Write("Droite");
                        complex.AddComponentElement(Line(previous, node));

                        if (arcDone) arcDone = false;

                        break;

                    case LineType.ARC:
                        if (preprevious != null && preprevious.LType == LineType.ARC)
                        {

                            // Création d'Arc
                            Log.Write("Arc");
                            var arc = Arc(preprevious, previous, node);
                            if (arc != null && !arcDone)
                            {
                                complex.AddComponentElement(arc);
                                arcDone = true;
                            } else
                            {
                                arcDone = false;
                            }
                        }
                        break;
                }
                preprevious = previous;
                previous = node;
            }

            Log.Write($"Validité de la chaîne complexe {complex.IsValidChainComponentType}");

            return complex;

        }

        private static DPoint3d FindIntersectionOfPerpendicularBisectors(DPoint3d A, DPoint3d B, DPoint3d C)
        {
            // Calculez les milieux des segments AB et BC
            var midAB = new DPoint3d((A.X + B.X) / 2, (A.Y + B.Y) / 2, (A.Z + B.Z) / 2);
            var midBC = new DPoint3d((B.X + C.X) / 2, (B.Y + C.Y) / 2, (B.Z + C.Z) / 2);

            double perpSlopeAB, perpSlopeBC;

            // Calculez les pentes des segments AB et BC, en gérant les cas verticaux
            if (B.X == A.X)
            {
                perpSlopeAB = 0; // La perpendiculaire à un segment vertical est horizontale
            }
            else
            {
                var slopeAB = (B.Y - A.Y) / (B.X - A.X);
                perpSlopeAB = -1 / slopeAB;
            }

            if (C.X == B.X)
            {
                perpSlopeBC = 0; // La perpendiculaire à un segment vertical est horizontale
            }
            else
            {
                var slopeBC = (C.Y - B.Y) / (C.X - B.X);
                perpSlopeBC = -1 / slopeBC;
            }

            double x, y;

            // Résolvez le système pour trouver l'intersection, en gérant les cas particuliers
            if (perpSlopeAB == perpSlopeBC)
            {

                return new DPoint3d(-1, -1, -1);
            }
            else if (B.X == A.X) // AB est vertical
            {
                x = midAB.X;
                y = perpSlopeBC * (x - midBC.X) + midBC.Y;
            }
            else if (C.X == B.X) // BC est vertical
            {
                x = midBC.X;
                y = perpSlopeAB * (x - midAB.X) + midAB.Y;
            }
            else if (perpSlopeAB == 0) // AB est horizontal
            {
                y = midAB.Y;
                x = (y - midBC.Y) / perpSlopeBC + midBC.X;
            }
            else if (perpSlopeBC == 0) // BC est horizontal
            {
                y = midBC.Y;
                x = (y - midAB.Y) / perpSlopeAB + midAB.X;
            }
            else
            {
                x = (perpSlopeBC * midBC.X - perpSlopeAB * midAB.X + midAB.Y - midBC.Y) / (perpSlopeBC - perpSlopeAB);
                y = perpSlopeAB * (x - midAB.X) + midAB.Y;
            }

            var z = (midAB.Z + midBC.Z) / 2; // En supposant que les points A, B et C sont coplanaires dans XY.

            return new DPoint3d(x, y, z);
        }


    }
}
