using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace GeoCode.Model;

public class Application : INotifyPropertyChanged
{
    private string _ptTopo;
    public string PtTopo
    {
        get => _ptTopo;
        set
        {
            _ptTopo = value;
            OnPropertyChanged();
        }
    }

    private string _levelTopo;
    public string LevelTopo
    {
        get => _levelTopo;
        set
        {
            _levelTopo = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<Category> _categories;
    public ObservableCollection<Category> Categories
    {
        get => _categories;
        set
        {
            _categories = value;
            OnPropertyChanged();
        }
    }
    
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    public override string ToString()
    {
        return $@"PTTopo : {PtTopo} | LevelTopo : {LevelTopo} | Categories : {Categories}";
    }
}