/*--------------------------------------------------------------------------------------+
|   AddElement.cs
|
+--------------------------------------------------------------------------------------*/
#region System Namespaces

using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Bentley.MstnPlatformNET;
using GeoCode.Cells.Placement;
using GeoCode.Model;
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
    }
}
