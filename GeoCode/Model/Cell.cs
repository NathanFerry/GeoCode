using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Windows.Input;
using Bentley.MstnPlatformNET;
using Bentley.UI.Mvvm;
using GeoCode.Cells.Placement;
using Newtonsoft.Json;

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

        [JsonIgnore]
        public ICommand PlaceCommand { get; }

        public Cell()
        {
            PlaceCommand = new RelayCommand(PlacementMethod);
        }

        private void PlacementMethod()
        {
            CellPlacement.PlacementTool(Name, Level, Placement);
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