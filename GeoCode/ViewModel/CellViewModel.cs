using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Bentley.UI.Mvvm;
using GeoCode.Model;
using GeoCode.Cells.Placement;

namespace GeoCode.ViewModel;

public class CellViewModel
{
    public ObservableCollection<Cell> Cells { get; init; }
}