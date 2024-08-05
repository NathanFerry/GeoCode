/*--------------------------------------------------------------------------------------+
|   ElementSelector.cs
|
+--------------------------------------------------------------------------------------*/

#region System Namespaces

using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using GeoCode.Cells.Placement;
using GeoCode.Model;
using GeoCode.Saving;
using GeoCode.Utils;
using GeoCode.ViewModel;

#endregion

namespace GeoCode.UI
{
    public partial class ElementSelector : UserControl
    {

        public ElementSelector()
        {
            
            GeoCode.Application = SaveManager.Load();
            InitializeComponent();
            if (GeoCode.Application.PtTopo == null || GeoCode.Application.LevelTopo == null)
            {
                Settings.ShowWindow();    
            }
            var categoriesViewModel = new CategoriesViewModel
            {
                Categories = GeoCode.Application.Categories
            };

            CategoryControl.DataContext = categoriesViewModel;

            this.PlaceTopoCheckBox.IsChecked = GeoCode.Application.PlaceTopo;
           
           
        }

        private void PasteElement_OnClick(object sender, RoutedEventArgs e)
        {
            if (SavedCellToPaste.name != null)
            ((ObservableCollection<Cell>)((ElementSelector)ElementSelectorDockableWindow.Content).CellControl.ItemsSource).Add(new Cell
            {
                Name = SavedCellToPaste.name,
                Level = SavedCellToPaste.level,
                Placement = PlacementTypeElement.FromString(SavedCellToPaste.placement.ToString())
            });
            else
            {
                MessageBox.Show("Aucune cellule n'a été copiée",
                   "Erreur de Cellule",
                   MessageBoxButton.OK,
                   MessageBoxImage.Error);
            }
        }

        private void PasteLinearElement_OnClick(object sender, RoutedEventArgs e)
        {
            if (SaveLinearToPaste.label != null)
                if (this.LinearControl.HasItems)
                ((ObservableCollection<Linear>)this.LinearControl.ItemsSource).Add(new Linear
                {
                    Label = SaveLinearToPaste.label,
                    Level = SaveLinearToPaste.level,
                    Placement = LinearPlacementTypeElement.FromString(SaveLinearToPaste.placement.ToString()),
                    Value = SaveLinearToPaste.value,
                });
            else
            {
                MessageBox.Show("Aucun linéaire n'a été copiée",
                   "Erreur de Linéaire",
                   MessageBoxButton.OK,
                   MessageBoxImage.Error);
            }
        }

        private void OpenAddElementMenuButton_OnClick(object sender, RoutedEventArgs e)
        {
            AddElement.add = true;
            AddElement.c = null;
            AddElement.ShowWindow();
        }

        private void OpenAddLinearElementMenuButton_OnClick(object sender, RoutedEventArgs e)
        {
            AddLinearElement.add = true;
            AddLinearElement.l = null;  
            AddLinearElement.ShowWindow();
        }

        private void OpenAddCategoryMenuButton_OnClick(object sender, RoutedEventArgs e)
        {
            AddCategory.ShowWindow();
        }
        
        private void SettingsButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Settings.ShowWindow();
            }catch  (Exception ex) { Log.Write(ex.ToString()); }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Mouse_Click(object sender, MouseEventArgs e)
        {
            var cm = ContextMenuService.GetContextMenu(sender as DependencyObject);
            if (cm == null)
            {
                return;
            }
            cm.Placement = PlacementMode.Center;
            cm.PlacementTarget = sender as UIElement;
            cm.IsOpen = true;

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var booleen = PlaceTopoCheckBox.IsChecked.GetValueOrDefault();
            GeoCode.Application.PlaceTopo = booleen;
        }
    }
}
