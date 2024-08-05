using System;
using System.Collections.Generic;
using System.Linq;
using GeoCode.Utils;
using Newtonsoft.Json;

namespace GeoCode.Cells.Placement
{
    /// <summary>
    /// Type of placement for a cellule.
    /// The point of this class is to "replicate" an Enum.
    /// C# default Enum uses int to represent values but in this case, strings where optimal.
    /// This is an implementation of an "Enum like" class using string as value.
    /// </summary>
     public class LinearPlacementTypeElement
    {
        [JsonProperty]
        public string Value { get; }
        
        [JsonConstructor] public LinearPlacementTypeElement(string value) { Value = value; }
        
        public static LinearPlacementTypeElement SimpleLinear() => new("Linéaire simple");
        public static LinearPlacementTypeElement ThickLinear() => new("Linéaire avec décalage");
        public static LinearPlacementTypeElement FleeingLinear() => new("Linéaire avec fuyante");


        public static LinearPlacementTypeElement FromString(string placementType)
        {
            if (!GetAllLinearPlacementTypes().Select(it => it.ToString()).Contains(placementType))
            {
                
                throw new InvalidOperationException($@"{placementType} is not a valid value for Enum {typeof(LinearPlacementTypeElement)}");
            }
            return new LinearPlacementTypeElement(placementType);
        }
        
        public static IEnumerable<LinearPlacementTypeElement> GetAllLinearPlacementTypes()
        {
            // Using reflection to get a list of all the Placement types.
            // This means you don't have to update anything when you add a new one.
            return typeof(LinearPlacementTypeElement).GetMethods().Where(it => it.IsStatic && it.ReturnType == typeof(LinearPlacementTypeElement) && !it.GetParameters().Any())
                .Select(it => (LinearPlacementTypeElement)it.Invoke(null, null)).ToList();
        }

        public bool Equals(LinearPlacementTypeElement other)
        {
            return Value == other.Value;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}