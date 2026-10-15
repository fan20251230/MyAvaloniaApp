using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Drawing;
using smartFactoryApp.Models;
using static smartFactoryApp.Models.Parameters;
using Avalonia.Controls.Converters;
using System.Collections.Generic;
using Avalonia.Threading;
using System;

namespace smartFactoryApp.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    Factory factory;
    DispatcherTimer timer;
    public double RobotX => factory.RobotX;
    public double RobotY => factory.RobotY;
    public double RobotAngle => factory.RobotAngle;

     // 红色生产线
    public double ProductionRedX => factory.ProductionRed.Location.X;
    public double ProductionRedY => factory.ProductionRed.Location.Y;
    public double ProductionRedAngle => factory.ProductionRed.Angle;

    // 蓝色生产线
    public double ProductionBlueX => factory.ProductionBlue.Location.X;
    public double ProductionBlueY => factory.ProductionBlue.Location.Y;
    public double ProductionBlueAngle => factory.ProductionBlue.Angle;

    // 黄色生产线
    public double ProductionYellowX => factory.ProductionYellow.Location.X;
    public double ProductionYellowY => factory.ProductionYellow.Location.Y;
    public double ProductionYellowAngle => factory.ProductionYellow.Angle;

    // 绿色生产线
    public double ProductionGreenX => factory.ProductionGreen.Location.X;
    public double ProductionGreenY => factory.ProductionGreen.Location.Y;
    public double ProductionGreenAngle => factory.ProductionGreen.Angle;

    
    public List<LegoBrickViewModel> BricksView { get; } = new List<LegoBrickViewModel>();
    public MainViewModel()
    {
        factory = new Factory();
        foreach (LegoBrick brick in factory.ProductionRed.Bricks)
        {
            BricksView.Add(new LegoBrickViewModel(brick));
        }

        foreach (LegoBrick brick in factory.ProductionBlue.Bricks)
        {
            BricksView.Add(new LegoBrickViewModel(brick));
        }

        foreach (LegoBrick brick in factory.ProductionYellow.Bricks)
       {
            BricksView.Add(new LegoBrickViewModel(brick));
       }

       foreach (LegoBrick brick in factory.ProductionGreen.Bricks)
       {
            BricksView.Add(new LegoBrickViewModel(brick));
       }
        timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromMilliseconds(timerInteval);
        timer.Tick += Timer_Tick; 
        
    }
    private void Timer_Tick(object? sender, EventArgs e)
    {
       
        factory.ProductionLinesBewegen();
        NotifyLegoBricksPoseChanged();
    }
    public void MoveUp()
    {
        factory.RobotBewegen(Direction.up);
        NotifyRobotPoseChanged();
    }
    public void MoveDown()
    {
        factory.RobotBewegen(Direction.down);
        NotifyRobotPoseChanged();
    }
    public void MoveLeft()
    {
        factory.RobotBewegen(Direction.left);
        NotifyRobotPoseChanged();
    }
    public void MoveRight()
    {
        factory.RobotBewegen(Direction.right);
        NotifyRobotPoseChanged();
    }
    public void MoveUpLeft()
    {
        factory.RobotBewegen(Direction.upLeft);
        NotifyRobotPoseChanged();
    }

    public void MoveUpRight()
    {
        factory.RobotBewegen(Direction.upRight);
        NotifyRobotPoseChanged();
    }

    public void MoveDownLeft()
    {
        factory.RobotBewegen(Direction.downLeft);
        NotifyRobotPoseChanged();
    }

    public void MoveDownRight()
    {
        factory.RobotBewegen(Direction.downRight);
         NotifyRobotPoseChanged();
    }
    public void RotateRight()
    {
        factory.RobotRotateRight();
        NotifyRobotPoseChanged();
    }

    public void RotateLeft()
    {
        factory.RobotRotateLeft();
        NotifyRobotPoseChanged();
    }

    public void ResetAngle()
    {
        factory.RobotResetAngle();
        NotifyRobotPoseChanged();
    }
    private void NotifyRobotPoseChanged()
    {
        OnPropertyChanged(nameof(RobotX));
        OnPropertyChanged(nameof(RobotY));
        OnPropertyChanged(nameof(RobotAngle));
    }
    private void NotifyLegoBricksPoseChanged()
    {
        foreach (var brick in BricksView)
        {
            brick.Refresh();
        }
    }

    public void StartProduction()
    {
        timer.Start();
    }

    public void StopProduction()
    {
        timer.Stop();
    }
}
