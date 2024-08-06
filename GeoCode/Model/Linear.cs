using Bentley.DgnPlatformNET.Elements;
using Bentley.MstnPlatformNET;
using Bentley.UI.Mvvm;
using GeoCode.Cells.Placement;
using GeoCode.Saving;
using GeoCode.UI;
using GeoCode.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GeoCode.Model
{
    public sealed class Linear
    {
        private string _label;
        public string Label
        {
            get => _label;
            set
            {
                _label = value;
                OnPropertyChanged();
            }
        }
      

        private double? _thicknessOrLength;
        public double? ThicknessOrlength
        {
            get => _thicknessOrLength;
            set
            {
                _thicknessOrLength = value;
                OnPropertyChanged();
            }
        }

        private string _level;
        public string Level
        {
            get => _level;
            set
            {
                _level = value;
                OnPropertyChanged();
            }
        }

        private LinearPlacementTypeElement _placement;
        public LinearPlacementTypeElement Placement
        {
            get => _placement;
            set
            {
                _placement = value;
                OnPropertyChanged();
            }
        }

        // This is used to execute the command when a button is clicked, with the cell's properties.
        // The JsonIgnore is here to not serialize the property.
        [JsonIgnore]
        public ICommand PlaceCommand { get; }
        [JsonIgnore]
        public ICommand CopyCommand { get; }
        [JsonIgnore]
        public ICommand UpdateCommand { get; }
        [JsonIgnore]
        public ICommand DeleteCommand { get; }
        public Linear()
        {
            PlaceCommand = new RelayCommand(PlacementMethod);
            CopyCommand = new RelayCommand(CopyMethod);
            UpdateCommand = new RelayCommand(UpdateMethod);
            DeleteCommand = new RelayCommand(DeleteMethod);
        }

        private void PlacementMethod()
        {

            var level = DgnHelper.GetAllLevelsFromLibrary()
                .First(element => element.Name == this.Level);

            Log.Write(Placement.ToString());
            /*if (Placement.ToString() == "Linéaire simple")
            {
                Session.Instance.Keyin("ACTIVE LEVEL " + Level);
                Session.Instance.Keyin("PLACE SMARTLINE");
            }
            else*/
                LinearPlacement.LinearPlacementTool(this, Placement);
            
            

            //LineElement line = new LineElement(Session.Instance.GetActiveDgnModel(), Element.,);
            //
        }

        private void CopyMethod()
        {
            SaveLinearToPaste.Copy(this);
        }

        private void DeleteMethod()
        {
            ((ObservableCollection<Linear>)((ElementSelector)ElementSelector.ElementSelectorDockableWindow.Content).LinearControl.ItemsSource).Remove(this);
        }

        private void UpdateMethod()
        {
            AddLinearElement.add = false;
            AddLinearElement.l = this;

            AddLinearElement.ShowWindow();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public override string ToString()
        {
            return $@"Value : {_thicknessOrLength} | Level : {_level} | Placement : {_placement}";
        }
    }
}
