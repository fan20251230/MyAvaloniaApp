using System;
using System.Drawing;
using Avalonia.Win32;

namespace smartFactoryApp.Models
{
    public class Beweger
    {
         private Point location;
         private double angle;
        private Rectangle borders;

    public Point Location { 
        get { return location; } 
        set { location = value; } 
    }
    public double Angle {get {return angle;} set {angle = value;}}
    public Rectangle Borders { get => borders; set => borders = value; }

    public Beweger(Point location, Rectangle borders)
    {
        //this.location = location;
        //this.borders = borders;
        Location = location;
        Borders = borders;
    }
    public Beweger(Point location)
    {
        Location = location;
    }
    public Beweger(Pose pose)
    {
        Location = pose.Location;
        angle = pose.Angle;
        Borders = ViewParameters.Borders;
    }
    public virtual void Bewegen(Parameters.Direction direction)
    {
        double dx = 0;
        double dy = 0;
        double step = Parameters.step;

        // 1. 机器人局部坐标系中的移动方向
        switch (direction)
        {
            case Parameters.Direction.up:
                dy = -step;
                break;

            case Parameters.Direction.down:
                dy = step;
                break;

            case Parameters.Direction.left:
                dx = -step;
                break;

            case Parameters.Direction.right:
                dx = step;
                break;

            case Parameters.Direction.upLeft:
                dx = -step;
                dy = -step;
                break;

            case Parameters.Direction.upRight:
                dx = step;
                dy = -step;
                break;

            case Parameters.Direction.downLeft:
                dx = -step;
                dy = step;
                break;

            case Parameters.Direction.downRight:
                dx = step;
                dy = step;
                break;

            default:
                return;
        }

     // 2. 对角线移动时保持总步长为 3
        if (dx != 0 && dy != 0)
        {
            dx /= Math.Sqrt(2);
            dy /= Math.Sqrt(2);
        }

    // 3. 将角度转换为弧度
        double radians = Angle * Math.PI / 180.0;

    // 4. 局部坐标转换为世界坐标
        double rotatedDx =
            dx * Math.Cos(radians) - dy * Math.Sin(radians);

        double rotatedDy =
            dx * Math.Sin(radians) + dy * Math.Cos(radians);

    // 5. 计算机器人新的位置
        double newX = Location.X + rotatedDx;
        double newY = Location.Y + rotatedDy;

    if (newX >= Borders.Left &&
    newX <= Borders.Right &&
    newY >= Borders.Top &&
    newY <= Borders.Bottom)
{
    Location = new Point(
        (int)Math.Round(newX),
        (int)Math.Round(newY));
}
    }
    }
}