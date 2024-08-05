/*--------------------------------------------------------------------------------------+
|   SimpleLinearChoice.cs
|
+--------------------------------------------------------------------------------------*/
#region System Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
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

namespace GeoCode.UI.LinearWindows
{
    public partial class SimpleLinearChoice : UserControl
    {
        public static string selectedType = "Line";
        public SimpleLinearChoice()
        {
            InitializeComponent();

            this.LineChoice.ItemsSource = new List<string>() { "Line","Arc"};
            this.LineChoice.SelectedItem = "Line";
        }

        private void LineChoice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedType = (string)this.LineChoice.SelectedItem;
        }
    }
}
