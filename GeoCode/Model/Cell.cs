using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Bentley.UI.Mvvm;
using GeoCode.Cells.Placement;
using GeoCode.Saving;
using Newtonsoft.Json;

#region Documentation

/*
|   This model is used to represent a cell and to easily save it,
|   by serializing the class to a JSON object.
|
|   It implement INotifyPropertyChanged to update the UI when values change.
|   Please refer to WPF documentation as this is not part of the Bentley SDK
|   so you can find many articles and posts to help you.
*/

#endregion

namespace GeoCode.Model
{
    public sealed class Cell : INotifyPropertyChanged
    {
        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
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
        
        private PlacementTypeElement _placement;
        public PlacementTypeElement Placement
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
        public ICommand CopyCommand { get; }

        public Cell()
        {
            PlaceCommand = new RelayCommand(PlacementMethod);
            CopyCommand = new RelayCommand(CopyMethod);
        }

        private void PlacementMethod()
        {
            CellPlacement.PlacementTool(Name, Level, Placement);
        }

        private void CopyMethod()
        {
            SavedCellToPaste.Copy(this);
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
            return $@"Name : {_name} | Level : {_level} | Placement : {_placement}";
        }
    };
}