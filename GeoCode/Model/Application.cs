using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GeoCode.Model;

#region Documentation

/*
|   This model is used to represent the application state at runtime
|   and to easily save it, by serializing the class to a JSON object.
|
|   It implement INotifyPropertyChanged to update the UI when values change.
|   Please refer to WPF documentation as this is not part of the Bentley SDK
|   so you can find many articles and posts to help you.
*/

#endregion

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