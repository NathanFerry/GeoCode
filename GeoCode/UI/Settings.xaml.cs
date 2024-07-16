/*--------------------------------------------------------------------------------------+
|   Settings.cs
|
+--------------------------------------------------------------------------------------*/

#region System Namespaces

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Bentley.MstnPlatformNET;
using GeoCode.Utils;

#endregion

#region Bentley Namespaces

#endregion

namespace GeoCode.UI
{
    public partial class Settings : UserControl
    {
        public Settings()
        {
            InitializeComponent();
                
            ElementSelector.OwnedWindows.Add(SettingsDockableWindow);
            PointTopo.Text = GeoCode.Application.PtTopo ?? "";
            LevelTopo.Text = GeoCode.Application.LevelTopo ?? "";
            
            PointTopo.ItemsSource = DgnHelper.GetAllSharedCellsFromLibrary()
                .Select(it => it.CellName);
      
            LevelTopo.ItemsSource = DgnHelper.GetAllLevelsFromLibrary()
                .Select(it => it.Name );
         
        }
        private void SaveSettingsButton_OnClick(object sender, RoutedEventArgs e)
        {
            GeoCode.Application.PtTopo = PointTopo.Text;
            GeoCode.Application.LevelTopo = LevelTopo.Text;
            ((ElementSelector)ElementSelector.ElementSelectorDockableWindow.Content).IsEnabled = true;
            SettingsDockableWindow.Close();
            
        }
    }
}
