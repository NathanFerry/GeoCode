/*--------------------------------------------------------------------------------------+
|   SimpleLinearPlaceTool.cs
|
+--------------------------------------------------------------------------------------*/

#region Bentley Namespaces
using Bentley.DgnPlatformNET;
using Bentley.DgnPlatformNET.Elements;
using Bentley.GeometryNET;
using Bentley.MstnPlatformNET;
using GeoCode.Elements.Drawing;
using GeoCode.Model;
using GeoCode.UI.LinearWindows;
using GeoCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Shapes;
using LineElement = Bentley.DgnPlatformNET.Elements.LineElement;

#endregion

namespace GeoCode.Cells.Placement.LinearPlacementTools
{
    internal class SimpleLinearPlaceTool : DgnPrimitiveTool
    {
        private readonly Linear _linearElement;

        //Points avant
        private Node _previous;
        private Node _preprevious;

        private List<Node> _nodes = new List<Node>();

        // Elements temporaires à supprimer en fin de commande
        private List<LineElement> _tempLines = new List<LineElement>();
        private List<ArcElement> _tempArcs = new List<ArcElement>();

        // Etape de 
        private bool nextPointReady = false;


        public SimpleLinearPlaceTool(Linear linear, int toolName, int toolPrompt) : base(toolName, toolPrompt) { _linearElement = linear; }

        #region DgnPrimitiveTool Members

        // Le boutton est cliqué
        protected override bool OnDataButton(DgnButtonEvent ev)
        {
            
            if (!DynamicsStarted)
            {
                //Lancement des dynamiques et placement origine
                BeginDynamics();

                _previous = new Node()
                {
                    Point = ev.Point,
                    LType = LineType.DROITE
                };


                CellPlacement.PlaceTopoPoint(ev);
                nextPointReady = true;

                return false;
            }
            
            // Points suivants
            if (nextPointReady)
            {
                // Niveau du linéaire
                var level = DgnHelper.GetAllLevelsFromLibrary().First(element => element.Name == _linearElement.Level);

                // Noeud courant
                Node n = new() { Point = ev.Point, LType =  LineType.DROITE };


                _nodes.Add(_previous);
                //====Traitement====\\

                CellPlacement.PlaceTopoPoint(ev);

                switch (_previous.LType) {
                    case LineType.DROITE:

                        // Création Ligne
                        var line = CreateElement.Line(_previous, n);
                        _tempLines.Add(line);

                        // Dessin ligne
                        Draw.DrawElement(line, level);

                        break;

                    case LineType.ARC:
                        if (_preprevious != null && _preprevious.LType == LineType.ARC)
                        {
                            // Création d'Arc
                          
                            var arc = CreateElement.Arc(_preprevious, _previous, n);
                            _tempArcs.Add(arc);

                            // Dessin Arc
                            Draw.DrawElement(arc, level);

                            _preprevious = null;
                        } else
                        {
                            _preprevious = _previous;
                        }
                        break;
                }

               
                _previous = n;
                return false;
            }
            OnRestartTool();
            return true;
        }

        // A chaque image  
        protected override void OnDynamicFrame(DgnButtonEvent ev)
        {
            if (_previous == null)
            {
                return;
            }
            if (nextPointReady)
            {

                // Niveau du linéaire
                var level = DgnHelper.GetAllLevelsFromLibrary().First(element => element.Name == _linearElement.Level);

                // Noeud courant
                Node n = new() { Point = ev.Point, LType = LineType.DROITE };
                _previous.LType = SimpleLinearChoice.selectedType == "Arc" ? LineType.ARC : LineType.DROITE;

                switch (_previous.LType)
                {
                    case LineType.DROITE:

                        // Création Ligne
                        var line = CreateElement.Line(_previous, n);

                        // Dessin ligne
                        Draw.DrawDynamicElement(line, ev);

                        break;

                    case LineType.ARC:
                        if (_preprevious != null && _preprevious.LType == LineType.ARC)
                        {
                            // Création d'Arc

                            var arc = CreateElement.Arc(_preprevious, _previous, n);

                            // Dessin Arc
                            Draw.DrawDynamicElement(arc, ev);
                        }
                        break;
                }

                return;
            }
        }

        protected override void OnRestartTool()
        {
            InstallNewInstance(_linearElement);
        }

        protected override void ExitTool()
        {
            SimpleLinearChoice.SimpleLinearChoiceDockableWindow.Close();
            base.ExitTool();
        }

        protected override bool OnResetButton(DgnButtonEvent ev)
        {
            nextPointReady = false;

            _nodes.Add(_previous);

            var level = DgnHelper.GetAllLevelsFromLibrary()
              .First(element => element.Name == _linearElement.Level);

            Log.Write("Suppression lignes et arcs temporaires");
            // Supprimer arcs et lignes temporaires
            foreach (var arc in _tempArcs) arc.DeleteFromModel();
            foreach (var line  in _tempLines) line.DeleteFromModel();

            Log.Write("Création complexElement");
            // Ajoute au modèle 
            var complexElement = CreateElement.ComplexString(_nodes);

            Draw.DrawElement(complexElement, level);

            ExitTool();
            return true;
        }


        protected override void OnPostInstall()
        {
            AccuSnap.SnapEnabled = true;
            AccuDraw.Active = true;
            SimpleLinearChoice.ShowWindow();
        }


       

        protected override bool OnInstall()
        {
            
            return true;
        }


        public static void InstallNewInstance(Linear linear)
        {
            SimpleLinearPlaceTool tool = new(linear,0, 0);
            tool.InstallTool();
        }
        #endregion

       
    }
}
