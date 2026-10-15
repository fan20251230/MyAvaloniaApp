using Avalonia.Controls;
using Avalonia.Interactivity;
using smartFactoryApp.ViewModels;

namespace smartFactoryApp.Views;

public partial class MainWindow : Window
{
    
    public MainWindow()
    {
        InitializeComponent();
    }
    private MainViewModel? ViewModel => DataContext as MainViewModel;
    private void MoveUp_Click(object sender, RoutedEventArgs e)
    {
        ViewModel?.MoveUp();
    }
    private void MoveDown_Click(object sender, RoutedEventArgs e)
    {
        ViewModel?.MoveDown();
    }
    private void MoveLeft_Click(object sender, RoutedEventArgs e)
    {
        ViewModel?.MoveLeft();
    }
    private void MoveRight_Click(object sender, RoutedEventArgs e)
    {
        ViewModel?.MoveRight();
    }
    private void MoveUpLeft_Click(
        object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
        {
            vm.MoveUpLeft();
        }
    }

    private void MoveUpRight_Click(
        object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
        {
            vm.MoveUpRight();
        }
    }

    private void MoveDownLeft_Click(
        object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
        {
            vm.MoveDownLeft();
        }
    }

    private void MoveDownRight_Click(
        object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
        {
            vm.MoveDownRight();
        }
    }
    private void RotateRight_Click(object? sender,
    Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
        {
            vm.RotateRight();
        }
    }

    private void RotateLeft_Click(object? sender,
    Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
        {
            vm.RotateLeft();
        }
    }


    private void ResetAngle_Click(object? sender,
    Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
        {
            vm.ResetAngle();
        }
    }
    private void Start_Click(object? sender, RoutedEventArgs e)
    {
        ViewModel?.StartProduction();
    }

    private void Stop_Click(object? sender, RoutedEventArgs e)
   {
      ViewModel?.StopProduction();
   }

}