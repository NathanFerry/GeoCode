using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace GeoCode.Cells.Placement
{
    /// <summary>
    /// Type of placement for a cellule.
    /// The point of this class is to "replicate" an Enum.
    /// C# default Enum uses int to represent values but in this case, strings where optimal.
    /// This is an implementation of an "Enum like" class using string as value.
    /// </summary>
     public class PlacementTypeElement
    {
        [JsonProperty]
        public string Value { get; }
        
        [JsonConstructor] public PlacementTypeElement(string value) { Value = value; }
        
        public static PlacementTypeElement OnePoint() => new("1 Point");
        public static PlacementTypeElement OnePointRotation() => new("1 point - Rotation");
        public static PlacementTypeElement OnePointOnLinear() => new("1 point sur linéaire");
        public static PlacementTypeElement TwoPointsScaling() => new("2 Points - Mise à l'échelle");
        public static PlacementTypeElement TwoPointsRotationScaling() => new("2 Points - Rotation - Mise à l'échelle");
        public static PlacementTypeElement TwoPointsRotationScalingSymmetrical() => new("2 Points - Rotation - Mise à l'échelle symétrique");
        public static PlacementTypeElement ThreePointsRotationScaling() => new("3 Points - Rotation - Mise à l'échelle");

        public static PlacementTypeElement FromString(string placementType)
        {
            if (!GetAllPlacementTypes().Select(it => it.ToString()).Contains(placementType))
            {
                throw new InvalidOperationException($@"{placementType} is not a valid value for Enum {typeof(PlacementTypeElement)}");
            }

            return new PlacementTypeElement(placementType);
        }
        
        public static IEnumerable<PlacementTypeElement> GetAllPlacementTypes()
        {
            // Using reflection to get a list of all the Placement types.
            // This means you don't have to update anything when you add a new one.
            return typeof(PlacementTypeElement).GetMethods().Where(it => it.IsStatic && it.ReturnType == typeof(PlacementTypeElement) && !it.GetParameters().Any())
                .Select(it => (PlacementTypeElement)it.Invoke(null, null)).ToList();
        }

        public bool Equals(PlacementTypeElement other)
        {
            return Value == other.Value;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}