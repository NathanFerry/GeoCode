/*--------------------------------------------------------------------------------------+
|   AddLinearElement.cs
|
+--------------------------------------------------------------------------------------*/
#region System Namespaces
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using GeoCode.Cells.Placement;
using GeoCode.Model;
using GeoCode.Utils;

#endregion

namespace GeoCode.UI
{
    public partial class AddLinearElement : UserControl
    {
        public static bool add = true;
        public static Linear l = null;
        public AddLinearElement()
        {
            InitializeComponent();

            this.LinearFunction.ItemsSource = LinearPlacementTypeElement.GetAllLinearPlacementTypes().Select(it=> it.ToString());
            this.LinearFunction.SelectedItem = LinearPlacementTypeElement.GetAllLinearPlacementTypes().Select(it => it.ToString()).FirstOrDefault();
            this.LinearLabel.Text = "Default";
            this.LevelSelection.ItemsSource = DgnHelper.GetAllLevelsFromLibrary().Select(it => it.Name);
            this.LevelSelection.SelectedItem = DgnHelper.GetAllLevelsFromLibrary().Select(it => it.Name).FirstOrDefault(); 
            
            if (l!=null)
            {
                LinearFunction.SelectedItem = l.Placement.ToString();
                LevelSelection.SelectedItem = l.Level;
                LinearLabel.Text = l.Label;
                if (l.Value.HasValue)
                {
                    NumberTextBox.Text = l.Value.Value.ToString();
                }
            }
        }

        
    private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            NumberFormatInfo nfi = CultureInfo.CurrentCulture.NumberFormat;

            Regex regex = new("[^0-9]" + nfi.NumberDecimalSeparator + "[^0-9]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void AddElementButton_OnClick(object sender, RoutedEventArgs e)
        {
            double value = -1.0;
            if ((string)this.LinearFunction.SelectedItem
                != "Linéaire simple")
            {
                double.TryParse(this.NumberTextBox.Text, out value);
            }
            if (add)
            {
                Log.Write("Essai ajout de linéaire");
                try
                {
                    ((ObservableCollection<Linear>)((ElementSelector)ElementSelector.ElementSelectorDockableWindow.Content).LinearControl.ItemsSource).Add(new Linear
                    {
                        Label = this.LinearLabel.Text,
                        Placement = LinearPlacementTypeElement.FromString(LinearFunction.SelectedItem.ToString()),
                        Level = LevelSelection.SelectedItem.ToString(),
                        Value = value
                    });
                } catch(Exception ex)
                {
                    Log.Write("<<<Erreur d'ajout de Linéaire>>> "+ex.ToString()
                        +"\n"+this.LinearLabel.Text
                        +"\n"+this.LinearFunction.SelectedItem.ToString()
                        +"\n"+this.LevelSelection.SelectedItem.ToString()
                        +"\n"+value);
                    MessageBox.Show("Le linéaire n'a pas pu être ajouté. Vérifiez que vous avez sélectionné un onglet.",
                          "Erreur d'ajout linéaire",
                          MessageBoxButton.OK,
                          MessageBoxImage.Error);
                }

                Log.Write("Linéaire ajouté de manière efficace");
            }
            else
            {
                l.Label = LinearLabel.Text;
                l.Placement = LinearPlacementTypeElement.FromString(LinearFunction.SelectedItem.ToString());
                l.Level = LevelSelection.SelectedItem.ToString();
                l.Value = value;

            }
            AddLinearElement.CloseWindow();
        }

        private void LinearFunction_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var itemSelected = (sender as ComboBox).SelectedItem;

            switch (itemSelected)
            {
                case "Linéaire simple":
                    labelLongueur.Content = "";
                    this.NumberTextBox.IsEnabled = false;
                    break;
                case "Linéaire avec décalage":
                    labelLongueur.Content = "Epaisseur";
                    this.NumberTextBox.IsEnabled = true;
                    break;
                case "Linéaire avec fuyante":
                    labelLongueur.Content = "Longueur fuyante";
                    this.NumberTextBox.IsEnabled = true;
                    break;
            }
        }
    }
}
