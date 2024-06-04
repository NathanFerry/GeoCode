﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace GeoCode.Cells.Placement
{
     public class PlacementTypeElement
    {
        [JsonProperty]
        private string Value { get; }
        
        [JsonConstructor] public PlacementTypeElement(string value) { Value = value; }
        
        public static PlacementTypeElement OnePoint => new("1 Point");
        public static PlacementTypeElement OnePointScaling => new("1 Point - Mise à l'échelle");
        public static PlacementTypeElement TwoPointsRotationScaling => new("2 Points - Rotation - Mise à l'échelle");
        public static PlacementTypeElement ThreePointRotationScaling => new("3 Points - Rotation - Mise à l'échelle");

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