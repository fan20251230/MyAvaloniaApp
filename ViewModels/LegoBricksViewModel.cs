using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using smartFactoryApp.Models;
using Avalonia.Media;

namespace smartFactoryApp.ViewModels;
public partial class LegoBrickViewModel : ViewModelBase
{
    private LegoBrick brick;
    public LegoBrick Model => brick;
    [ObservableProperty] private double x;
    [ObservableProperty] private double y;
    [ObservableProperty] private double angle;
    public IBrush Color => brick.Color switch
    {
       Parameters.LegoColor.red => Brushes.Red,
       Parameters.LegoColor.blue => Brushes.Blue,
       Parameters.LegoColor.yellow => Brushes.Yellow,
       Parameters.LegoColor.green => Brushes.Green,
       _ => Brushes.Gray
    };

    public LegoBrickViewModel(LegoBrick brick)
    {
        this.brick = brick;
        Refresh();
    }
    public void Refresh()
    {
        X = brick.Location.X;
        Y = brick.Location.Y;
        Angle = brick.Angle;
    }
}
