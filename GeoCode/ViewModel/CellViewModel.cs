using System.Collections.ObjectModel;
using GeoCode.Model;

namespace GeoCode.ViewModel;

public class CellViewModel
{
    public ObservableCollection<Cell> Cells { get; init; }
}