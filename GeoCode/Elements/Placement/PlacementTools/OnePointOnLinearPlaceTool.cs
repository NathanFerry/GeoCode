/*--------------------------------------------------------------------------------------+
|   OnePointOnLinearPlaceTool.cs
|
+--------------------------------------------------------------------------------------*/

#region Bentley Namespaces
using Bentley.DgnPlatformNET;
using Bentley.DgnPlatformNET.Elements;
using Bentley.GeometryNET;
using Bentley.MstnPlatformNET;
using GeoCode.Elements.Drawing;
using GeoCode.Model;
using GeoCode.Utils;
using System;
using System.Collections.Generic;

#endregion

namespace GeoCode.Cells.Placement.PlacementTools
{
    internal class OnePointOnLinearPlaceTool : DgnPrimitiveTool
    {
        // Variables d'initialisation de cellule partagée
        private readonly SharedCellDefinitionElement _cellDefinition;
        private readonly SharedCellElement _cellElement;

        // Liste de ComplexString dans le dessin
        private List<ComplexStringElement> _complexElements = new List<ComplexStringElement>();

        //Element le plus proche
        private ComplexStringElement _complexStringElement;

        

        public OnePointOnLinearPlaceTool(SharedCellDefinitionElement cellDefinition, int toolName, int toolPrompt) : base(toolName, toolPrompt) {
            _cellDefinition = cellDefinition;
            _cellElement = SharedCellHelper.CreateSharedCell(cellDefinition, DPoint3d.Zero);
        }

        #region DgnPrimitiveTool Members
        protected override bool OnDataButton(DgnButtonEvent ev)
        {
            
                if (_complexStringElement != null)
                {
                    var point = _complexStringElement.GetClosestPointFrom(ev.Point,out var angle);
                    if (point.Distance(ev.Point) <= GeoCode.Application.MaxDistClose*100)
                    {
                        Draw.TranslateCell(_cellElement, point);

                        Draw.RotateCellAroundZ(_cellElement, angle);

                }
                    _cellElement.AddToModel();
                    return true;
                }
                else
                {
                    Log.Write("Aucun linéaire assez proche ");
                }
            

            return false;
        }

        protected override void OnRestartTool()
        {
            InstallNewInstance(_cellDefinition);
        }

        protected override void OnPostInstall()
        {
            AccuSnap.SnapEnabled = true;

            // On scan les ComplexString Elements dans le dessin
            var elements = Scan.GetElements(new List<MSElementType>() { MSElementType.ComplexString });
            Log.Write("Elements retrouvés : " + elements.Count);
            foreach (var element in elements)
            {
                _complexElements.Add(element as ComplexStringElement);
            }

            BeginDynamics();
        }

        protected override bool OnResetButton(DgnButtonEvent ev)
        {
            ExitTool();
            return true;
        }

       
        protected override void OnDynamicFrame(DgnButtonEvent ev)
        {
            double distance = double.MaxValue;
            bool found = false;

            //Complex Element le plus proche dans la zone de recherche
            foreach (var complex in _complexElements)
            {
                if (complex.GetClosestPointFrom(ev.Point,out var _).Distance(ev.Point) < distance
                    && complex.GetClosestPointFrom(ev.Point, out var _).Distance(ev.Point) <= GeoCode.Application.MaxDistClose*100)
                {
                    found = true;
                    _complexStringElement = complex;
                    distance = complex.GetClosestPointFrom(ev.Point, out var _).Distance(ev.Point);
                }
            }

            if (!found) _complexStringElement = null;
            
            // Complexe trouvé
            if (_complexStringElement != null)
            {
                //Translation de la cellule sur le point le plus proche du curseur
                var point = _complexStringElement.GetClosestPointFrom(ev.Point, out var angle);
                Draw.TranslateCell(_cellElement, point);
            }
            else
            {
                //Translation de la cellule sur le point du curseur
                Draw.TranslateCell(_cellElement, ev.Point);
            }

            Draw.DrawDynamicElement(_cellElement, ev);

        }
        public static void InstallNewInstance(SharedCellDefinitionElement cellDefinition)
        {
            new OnePointOnLinearPlaceTool(cellDefinition,0, 0).InstallTool();
        }
        #endregion
    }
}
