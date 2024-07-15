/*--------------------------------------------------------------------------------------+
|   AddCategory.cs
|
+--------------------------------------------------------------------------------------*/

using System.Collections.ObjectModel;
using GeoCode.Model;

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

#endregion

namespace GeoCode.UI
{
    public partial class AddCategory : UserControl
    {
        public AddCategory()
        {
            InitializeComponent();
        }
        
        private void AddCategoryButton_OnClick(object sender, RoutedEventArgs e)
        {
            var category = new Category
            {
                Linears = new ObservableCollection<Linear>(),
                Cells = new ObservableCollection<Cell>(),
                Name = NameTextBox.Text
            };
            ((ObservableCollection<Category>)((ElementSelector)ElementSelector.ElementSelectorDockableWindow.Content).CategoryControl.ItemsSource).Add(category);
            category.Focus.Execute(null);
        }
    }
}
