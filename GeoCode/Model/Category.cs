using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Bentley.UI.Mvvm;
using GeoCode.UI;
using GeoCode.ViewModel;
using Newtonsoft.Json;

namespace GeoCode.Model;

public sealed class Category : INotifyPropertyChanged
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

    private ObservableCollection<Cell> _cells;

    public ObservableCollection<Cell> Cells
    {
        get => _cells;
        set
        {
            _cells = value;
            OnPropertyChanged();
        }
    }
    
    [JsonIgnore]
    public ICommand Focus { get; }

    public Category()
    {
        Focus = new RelayCommand(SetAsCurrentCategory);
    }

    private void SetAsCurrentCategory()
    {

        ((ElementSelector)ElementSelector.ElementSelectorDockableWindow.Content).CellControl.DataContext =
            new CellViewModel
            {
                Cells = this.Cells
            };
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
}