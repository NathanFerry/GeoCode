/*--------------------------------------------------------------------------------------+
|   RenameWindow.cs
|
+--------------------------------------------------------------------------------------*/
#region System Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
#endregion

#region Bentley Namespaces
using Bentley.DgnPlatformNET;
using Bentley.GeometryNET;
using Bentley.MstnPlatformNET;
using Bentley.UI.Drawing;
using Bentley.UI.Vendor.Wpf;
using GeoCode.Model;

#endregion

namespace GeoCode.UI.CategoryWindows
{
    public partial class RenameWindow : UserControl
    {
        public static Category cat; 
        public RenameWindow()
        {
            InitializeComponent();
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (cat != null)
            {
                if (!string.IsNullOrEmpty(NewNameTextBox.Text) && NewNameTextBox.Text != "(default)") { cat.Name = NewNameTextBox.Text; }
                else MessageBox.Show("La catégorie ne peut pas être renommée par "+NewNameTextBox.Text, "Renommer catégorie erreur ! ");
            } else
            {
                MessageBox.Show("Aucune catégorie sélectionnée pour renommer.", "Renommer catégorie erreur ! ");
            }

            RenameWindowDockableWindow.Close();
        }

        
    }
}
