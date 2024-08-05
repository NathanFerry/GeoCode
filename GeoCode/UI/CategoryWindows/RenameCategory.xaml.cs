/*--------------------------------------------------------------------------------------+
|   RenameCategory.cs
|
+--------------------------------------------------------------------------------------*/
#region System Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
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
    public partial class RenameCategory : UserControl
    {
        public static Category cat;
        public RenameCategory()
        {
            InitializeComponent();

            if (cat != null)
            {
                OldValue.Content = cat.Name;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (cat == null) { MessageBox.Show("Aucune catégorie identifiée", "Erreur Renomme Catégorie"); }
            else
            {
                if (NewValue.Text != "(default)" && NewValue.Text != "") 
                {
                    cat.Name = NewValue.Text;
                } else
                {
                    MessageBox.Show("Le nouveau nom n'est pas valide (default) ou vide", "Erreur Renomme Catégorie");
                }
            }

            cat = null;
            RenameCategoryDockableWindow.Close();
        }
    }
}
