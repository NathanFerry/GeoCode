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
            PointTopo.Text = GeoCode.Application.PtTopo != null ? GeoCode.Application.PtTopo : "";
            LevelTopo.Text = GeoCode.Application.LevelTopo != null ? GeoCode.Application.LevelTopo : "";
            PointTopo.ItemsSource = Session.Instance.GetActiveDgnFile().GetNamedSharedCellDefinitions()
                .Select(it => it.CellName);
            LevelTopo.ItemsSource = Session.Instance.GetActiveDgnFile().GetLevelCache().GetHandles()
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
