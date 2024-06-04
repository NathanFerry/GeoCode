using System.Collections.ObjectModel;
using GeoCode.Model;

namespace GeoCode.ViewModel;

public class CategoriesViewModel
{
    public ObservableCollection<Category> Categories { get; init; }
}