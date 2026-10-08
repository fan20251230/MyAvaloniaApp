using Avalonia.Controls;
using Avalonia.Interactivity;
using MyAvaloniaApp.Models;
using DrawingPoint = System.Drawing.Point;
using DrawingRectangle = System.Drawing.Rectangle;
using Avalonia.Media;

namespace MyAvaloniaApp.Views;

public partial class MainWindow : Window
{
    private readonly Robot robot;

    public MainWindow()
    {
        InitializeComponent();

        // 暂按截图设置一个稍微向内留边的活动区域
        // 左上角 (40, 40)，宽 520，高 520
        DrawingRectangle factoryBorders =
            new DrawingRectangle(40, 40, 520, 520);

        robot = new Robot(
            new DrawingPoint(300, 300),
            factoryBorders);

        // 把按钮点击事件连接到对应的方法
        UpButton.Click += MoveUp_Click;
        DownButton.Click += MoveDown_Click;
        LeftButton.Click += MoveLeft_Click;
        RightButton.Click += MoveRight_Click;
        UpLeftButton.Click += MoveUpLeft_Click;
        UpRightButton.Click += MoveUpRight_Click;
        DownLeftButton.Click += MoveDownLeft_Click;
        DownRightButton.Click += MoveDownRight_Click;
        RotateLeftButton.Click += RotateLeft_Click;
        RotateRightButton.Click += RotateRight_Click;


        UpdateRobotPosition();
    }

    private void MoveUp_Click(object? sender, RoutedEventArgs e)
    {
        robot.Bewegen(Parameters.Direction.Up);
        UpdateRobotPosition();
    }

    private void MoveDown_Click(object? sender, RoutedEventArgs e)
    {
        robot.Bewegen(Parameters.Direction.Down);
        UpdateRobotPosition();
    }

    private void MoveLeft_Click(object? sender, RoutedEventArgs e)
    {
        robot.Bewegen(Parameters.Direction.Left);
        UpdateRobotPosition();
    }

    private void MoveRight_Click(object? sender, RoutedEventArgs e)
    {
        robot.Bewegen(Parameters.Direction.Right);
        UpdateRobotPosition();
    }

    private void MoveUpLeft_Click(object? sender, RoutedEventArgs e)
    {
        robot.Bewegen(Parameters.Direction.UpLeft);
        UpdateRobotPosition();
    }

    private void MoveUpRight_Click(object? sender, RoutedEventArgs e)
    {
        robot.Bewegen(Parameters.Direction.UpRight);
        UpdateRobotPosition();
    }

    private void MoveDownLeft_Click(object? sender, RoutedEventArgs e)
    {
        robot.Bewegen(Parameters.Direction.DownLeft);
        UpdateRobotPosition();
    }

    private void MoveDownRight_Click(object? sender, RoutedEventArgs e)
    {
        robot.Bewegen(Parameters.Direction.DownRight);
        UpdateRobotPosition();
    }
    
    private void RotateLeft_Click(object? sender, RoutedEventArgs e)
    {
        robot.Drehen(false);
        UpdateRobotPosition();
    }

    private void RotateRight_Click(object? sender, RoutedEventArgs e)
    {
        robot.Drehen(true);
        UpdateRobotPosition();
    }

    private void UpdateRobotPosition()
    {
        Canvas.SetLeft(robotRectangle, robot.Location.X);
        Canvas.SetTop(robotRectangle, robot.Location.Y);

        robotRectangle.RenderTransform = new RotateTransform
        {
            Angle = robot.Angle
        };
    }
}