/*--------------------------------------------------------------------------------------+
|   AddElement.cs
|
+--------------------------------------------------------------------------------------*/
#region System Namespaces

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Bentley.MstnPlatformNET;
using Bentley.MstnPlatformNET.InteropServices;
using GeoCode.Cells.Placement;
using GeoCode.Model;
using GeoCode.Saving;
using GeoCode.Utils;
using GeoCode.ViewModel;

#endregion

namespace GeoCode.UI
{
    public partial class AddElement : UserControl
    {
        

        public static bool add = true;
        public static Cell c = null;
        public AddElement()
        {
            InitializeComponent();
            // Utilisation de l'ancienne API Interop
            
            
            CellSelection.ItemsSource = DgnHelper.GetAllSharedCellsFromLibrary()
                .Select(it => it.CellName);
            LevelSelection.ItemsSource = DgnHelper.GetAllLevelsFromLibrary().Select(it => it.Name);
            PlacementSelection.ItemsSource = PlacementTypeElement.GetAllPlacementTypes().Select(it => it.ToString());

            

            if (c != null)
            {
                this.CellLabel.Text = c.Label;
                this.CellSelection.SelectedItem = c.Name;
                this.LevelSelection.SelectedItem = c.Level;
                this.PlacementSelection.SelectedItem = c.Placement.ToString();
            }
        }

        

        private void AddElementButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (add) {
                try
                {
                    ((ObservableCollection<Cell>)((ElementSelector)ElementSelector.ElementSelectorDockableWindow.Content).CellControl.ItemsSource).Add(new Cell
                    {
                        Label = CellLabel.Text,
                        Name = CellSelection.SelectedItem.ToString(),
                        Level = LevelSelection.SelectedItem.ToString(),
                        Placement = PlacementTypeElement.FromString(PlacementSelection.SelectedItem.ToString())
                    });
                }
                catch (Exception ex)
                {
                    Log.Write("<<<Erreur d'ajout de Cellule>>> " + ex.ToString());
                    MessageBox.Show("La cellule n'a pas pu être ajoutée. Vérifiez que vous avez sélectionné un onglet.",
                         "Erreur d'ajout cellule",
                         MessageBoxButton.OK,
                         MessageBoxImage.Error);

                
                }
            }
            else
            {
                c.Label = CellLabel.Text;
                c.Name = CellSelection.SelectedItem.ToString();
                c.Level = LevelSelection.SelectedItem.ToString();
                c.Placement = PlacementTypeElement.FromString(PlacementSelection.SelectedItem.ToString());
                
            }
            AddElement.CloseWindow();  
            
        }

        private void PasteButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (SavedCellToPaste.name == null)
            {
                MessageBox.Show("Aucune cellule n'a été copiée",
                    "Cellule",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            else
            {
                this.CellLabel.Text = SavedCellToPaste.label;
                this.CellSelection.SelectedItem = SavedCellToPaste.name;
                this.LevelSelection.SelectedItem = SavedCellToPaste.level;
                this.PlacementSelection.SelectedItem = SavedCellToPaste.placement.ToString();
            }
        }       
            

    }
}
