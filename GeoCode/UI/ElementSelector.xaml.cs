/*--------------------------------------------------------------------------------------+
|   ElementSelector.cs
|
+--------------------------------------------------------------------------------------*/

#region System Namespaces

using System;
using System.Windows;
using System.Windows.Controls;
using GeoCode.Saving;
using GeoCode.ViewModel;

#endregion

namespace GeoCode.UI
{
    public partial class ElementSelector : UserControl
    {
        public ElementSelector()
        {
            InitializeComponent();
            GeoCode.Application = SaveManager.Load();
            if (GeoCode.Application.PtTopo == null || GeoCode.Application.LevelTopo == null)
            {
                Settings.ShowWindow();    
            }
            var categoriesViewModel = new CategoriesViewModel
            {
                Categories = GeoCode.Application.Categories
            };
            CategoryControl.DataContext = categoriesViewModel;
        }

        private void OpenAddElementMenuButton_OnClick(object sender, RoutedEventArgs e)
        {
            AddElement.ShowWindow();
        }

        private void OpenAddCategoryMenuButton_OnClick(object sender, RoutedEventArgs e)
        {
            AddCategory.ShowWindow();
        }
        
        private void SettingsButton_OnClick(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            Settings.ShowWindow();
        }
    }
}
