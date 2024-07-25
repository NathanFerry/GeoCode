/*-------
 * 
 * -------------------------------------------------------------------------------+
|   Settings.cs
|
+--------------------------------------------------------------------------------------*/

#region System Namespaces

using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
            MaxDist.Text = GeoCode.Application.MaxDistClose.ToString();


            PointTopo.ItemsSource = DgnHelper.GetAllSharedCellsFromLibrary()
                .Select(it => it.CellName);
      
            LevelTopo.ItemsSource = DgnHelper.GetAllLevelsFromLibrary()
                .Select(it => it.Name );
         
        }
        private void SaveSettingsButton_OnClick(object sender, RoutedEventArgs e)
        {
            GeoCode.Application.PtTopo = PointTopo.Text;
            GeoCode.Application.LevelTopo = LevelTopo.Text;
            if (double.TryParse(MaxDist.Text, out double dist))
            {
                GeoCode.Application.MaxDistClose = dist;
            }
            
            ((ElementSelector)ElementSelector.ElementSelectorDockableWindow.Content).IsEnabled = true;
            SettingsDockableWindow.Close();
            
        }

        private void MaxDist_TextChanged(object sender, TextCompositionEventArgs e)
        {
                Regex regex = new("[^0-9]+(.[^0-9]+)?");
                e.Handled = regex.IsMatch(e.Text);
            
        }


    }
}
