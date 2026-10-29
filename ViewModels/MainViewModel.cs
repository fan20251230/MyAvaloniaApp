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
using System.Collections.ObjectModel;

namespace smartFactoryApp.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    Factory factory;
    DispatcherTimer timer;

    // 机器人连续运动专用定时器
    private DispatcherTimer robotTimer;

    // false = Step 模式，true = Move 模式
    private bool continuousMode = false;

    // 当前运动方向
    private Direction currentDirection = Direction.stop;

    // 当前旋转方向：-1 左转，1 右转，0 不旋转
    private int rotationDirection = 0;

    public bool ContinuousMode
    {
        get => continuousMode;
        set
        {
            if (continuousMode == value)
                return;

            continuousMode = value;
            StopRobot();
            OnPropertyChanged(nameof(ContinuousMode));
        }
    }


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

    
    public ObservableCollection<LegoBrickViewModel> BricksView { get; }
    = new ObservableCollection<LegoBrickViewModel>();
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
        robotTimer = new DispatcherTimer();
        robotTimer.Interval = TimeSpan.FromMilliseconds(100);
        robotTimer.Tick += RobotTimer_Tick;
        
    }
    private void Timer_Tick(object? sender, EventArgs e)
{
    factory.ProductionLinesBewegen();

    factory.DeliveryBricksBewegen();

    factory.RecycleBricksBewegen();

    NotifyLegoBricksPoseChanged();
}
    public void MoveUp()
    {
        ExecuteMovement(Direction.up);  
    }
    public void MoveDown()
    {
        ExecuteMovement(Direction.down);
    }
    public void MoveLeft()
    {
        ExecuteMovement(Direction.left);
        
    }
    public void MoveRight()
    {
        ExecuteMovement(Direction.right);
        
    }
    public void MoveUpLeft()
    {
        ExecuteMovement(Direction.upLeft);
        
    }

    public void MoveUpRight()
    {
        ExecuteMovement(Direction.upRight);
        
    }

    public void MoveDownLeft()
    {
        ExecuteMovement(Direction.downLeft);
        
    }

    public void MoveDownRight()
    {
        ExecuteMovement(Direction.downRight);
               
    }
    public void RotateRight()
    {
        ExecuteRotation(1);
        
    }

    public void RotateLeft()
    {
        ExecuteRotation(-1);
        
    }

    public void ResetAngle()
    {
        StopRobot();
        factory.RobotResetAngle();
        NotifyRobotPoseChanged();
    }
    private void NotifyRobotPoseChanged()
    {
        OnPropertyChanged(nameof(RobotX));
        OnPropertyChanged(nameof(RobotY));
        OnPropertyChanged(nameof(RobotAngle));

         // 同时刷新 Lego 积木的位置和角度
        NotifyLegoBricksPoseChanged();
    }
private void NotifyLegoBricksPoseChanged()
{
    // 先同步新增和删除
    SyncBricksView();

    // 再刷新坐标和角度
    foreach (LegoBrickViewModel brick in BricksView)
        brick.Refresh();
}

    public void StartProduction()
    {
        timer.Start();
    }

    public void StopProduction()
    {
        timer.Stop();
    }


    public void Dock()
{
    StopRobot();

    if (factory.Dock())
    {
        NotifyLegoBricksPoseChanged();
    }
}

   

public void Undock()
{
    StopRobot();

    UndockResult result = factory.Undock();

    switch (result)
    {
        case UndockResult.Success:
            UndockInfo = "Brick delivered.";
            NotifyLegoBricksPoseChanged();
            break;

        case UndockResult.NoBrick:
            UndockInfo = "No brick docked: pick up a brick first.";
            break;

        case UndockResult.NotOnDeliveryLine:
            UndockInfo =
                "Not on the delivery line: move the brick there first.";
            break;
    }
}



    private void RobotTimer_Tick(object? sender, EventArgs e)
{
    double oldX = RobotX;
    double oldY = RobotY;
    double oldAngle = RobotAngle;

    if (rotationDirection == 1)
    {
        factory.RobotRotateRight();
    }
    else if (rotationDirection == -1)
    {
        factory.RobotRotateLeft();
    }
    else if (currentDirection != Direction.stop)
    {
        factory.RobotBewegen(currentDirection);
    }

    NotifyRobotPoseChanged();

    // 如果尝试移动，但位置没有变化，说明可能被边界阻止
    if (currentDirection != Direction.stop &&
        oldX == RobotX &&
        oldY == RobotY)
    {
        StopRobot();
    }
}

    private void ExecuteMovement(Direction direction)
    {
        if (ContinuousMode)
        {
            currentDirection = direction;
            rotationDirection = 0;
            robotTimer.Start();
        }
        else
        {
            StopRobot();
            factory.RobotBewegen(direction);
            NotifyRobotPoseChanged();
        }
    }


    private void ExecuteRotation(int direction)
    {
        if (ContinuousMode)
        {
             currentDirection = Direction.stop;
             rotationDirection = direction;
             robotTimer.Start();
        }
        else
        {
            StopRobot();

            if (direction == 1)
                factory.RobotRotateRight();
            else
                factory.RobotRotateLeft();

            NotifyRobotPoseChanged();
        }
    }

    public void StopRobot()
    {
            robotTimer.Stop();
            currentDirection = Direction.stop;
            rotationDirection = 0;
    }


    private string undockInfo = "";

    public string UndockInfo
    {
        get => undockInfo;
        set
        {
            if (undockInfo == value)
                return;

            undockInfo = value;
            OnPropertyChanged(nameof(UndockInfo));
        }
    }



// 同步工厂中的积木与界面中的积木
private void SyncBricksView()
{
    HashSet<LegoBrick> activeBricks =
        new HashSet<LegoBrick>(factory.AlleBricks());

    // 删除已经不在工厂中的积木显示
    for (int i = BricksView.Count - 1; i >= 0; i--)
    {
        if (!activeBricks.Contains(BricksView[i].Model))
            BricksView.RemoveAt(i);
    }

    HashSet<LegoBrick> displayedBricks = new();

    foreach (LegoBrickViewModel viewModel in BricksView)
        displayedBricks.Add(viewModel.Model);

    // 为新生产的积木增加显示
    foreach (LegoBrick brick in activeBricks)
    {
        if (!displayedBricks.Contains(brick))
            BricksView.Add(new LegoBrickViewModel(brick));
    }
}



}


