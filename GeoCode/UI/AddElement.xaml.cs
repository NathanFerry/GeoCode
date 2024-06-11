/*--------------------------------------------------------------------------------------+
|   AddElement.cs
|
+--------------------------------------------------------------------------------------*/
#region System Namespaces

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Bentley.MstnPlatformNET;
using GeoCode.Cells.Placement;
using GeoCode.Model;
using GeoCode.Saving;
using GeoCode.ViewModel;

#endregion

namespace GeoCode.UI
{
    public partial class AddElement : UserControl
    {
        public AddElement()
        {
            InitializeComponent(); 
            CellSelection.ItemsSource = Session.Instance.GetActiveDgnFile().GetNamedSharedCellDefinitions()
                .Select(it => it.CellName);
            LevelSelection.ItemsSource = Session.Instance.GetActiveDgnFile().GetLevelCache().GetHandles()
                .Select(it => it.Name );
            PlacementSelection.ItemsSource = PlacementTypeElement.GetAllPlacementTypes();
        }

        private void AddElementButton_OnClick(object sender, RoutedEventArgs e)
        {
            ((ObservableCollection<Cell>)((ElementSelector)ElementSelector.ElementSelectorDockableWindow.Content).CellControl.ItemsSource).Add(new Cell
            {
                Name = CellSelection.SelectedItem.ToString(),
                Level = LevelSelection.SelectedItem.ToString(),
                Placement = PlacementTypeElement.FromString(PlacementSelection.SelectedItem.ToString())
            });
        }

        private void PasteButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (SavedCellToPaste.name == null)
            {
                MessageBox.Show("Aucune cellule n'a été copiée", 
                    "Cellule", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
            } else
            {
                this.CellSelection.SelectedItem = SavedCellToPaste.name;
                this.LevelSelection.SelectedItem = SavedCellToPaste.level;
                this.PlacementSelection.SelectedItem = SavedCellToPaste.placement;
            }
                
            
        }
    }
}
