using System;
using System.Drawing;

namespace MyAvaloniaApp.Models;

public class Robot
{
    // 机器人的左上角位置
    private Point location;

    // 允许机器人活动的工厂区域
    private Rectangle borders;

    // 与界面中 Rectangle 的尺寸保持一致
    private const int width = 60;
    private const int height = 40;

    // 当前旋转角度
    private int angle = 0;

    public int Angle => angle;

    // 预留能容纳机器人所有旋转角度的空间
    private const int safetyRadius = 37;

    // 对外提供只读属性
    public Point Location => location;
    public Rectangle Borders => borders;

    // 构造函数 1：从工厂区域左上角开始
    public Robot(Rectangle factoryBorders)
        : this(
            new Point(factoryBorders.Left, factoryBorders.Top),
            factoryBorders)
    {
    }

    // 构造函数 2：指定初始位置和工厂区域
    public Robot(Point initialLocation, Rectangle factoryBorders)
    {
        borders = factoryBorders;
        location = initialLocation;
    }

    // 根据方向移动机器人
    public void Bewegen(Parameters.Direction direction)
    {
        int nextX = location.X;
        int nextY = location.Y;

        switch (direction)
        {
            case Parameters.Direction.Up:
                nextY -= Parameters.Step;
                break;

            case Parameters.Direction.Down:
                nextY += Parameters.Step;
                break;

            case Parameters.Direction.Left:
                nextX -= Parameters.Step;
                break;

            case Parameters.Direction.Right:
                nextX += Parameters.Step;
                break;

            case Parameters.Direction.UpLeft:
                nextX -= Parameters.Step;
                nextY -= Parameters.Step;
                break;

            case Parameters.Direction.UpRight:
                nextX += Parameters.Step;
                nextY -= Parameters.Step;
                break;

            case Parameters.Direction.DownLeft:
                nextX -= Parameters.Step;
                nextY += Parameters.Step;
                break;

            case Parameters.Direction.DownRight:
                nextX += Parameters.Step;
                nextY += Parameters.Step;
                break;

            case Parameters.Direction.Stop:
                break;
        }

        // 限制位置，确保整个机器人都在工厂区域内
        nextX = Math.Clamp(
            nextX,
            borders.Left + safetyRadius - width / 2,
            borders.Right - safetyRadius - width / 2);

        nextY = Math.Clamp(
            nextY,
            borders.Top + safetyRadius - height / 2,
            borders.Bottom - safetyRadius - height / 2);

        // 保存移动后的新位置
        location = new Point(nextX, nextY);
    }

    public void Drehen(bool clockwise)
    {
        if (clockwise)
        {
            angle += Parameters.RotationStep;
        }
        else
        {
            angle -= Parameters.RotationStep;
        }


    

    // 把角度统一到 0～359 度
    angle = (angle % 360 + 360) % 360;
    }
}