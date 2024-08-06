using Bentley.GeometryNET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GeoCode.Model
{
    public enum LineType
    {
        DROITE,
        ARC
    }

    public sealed class Node
    {
        // Le point avec les coordonées du noeud
        private DPoint3d _point;
        public DPoint3d Point
        {
            get => _point;
            set
            {
                _point = value;
            }
        }

        // Le type de ligne
        private LineType _type;
        public LineType LType
        {
            get => _type;
            set
            {
                _type = value;
            }
        }
    }
}
