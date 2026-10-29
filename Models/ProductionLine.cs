using System;
using System.Collections.Generic;
using System.Drawing;

namespace smartFactoryApp.Models;

public class ProductionLine
{
    // 字段（Felder）
    private double angle;
    private List<LegoBrick> bricks = new();
    private int stepsPerDirection = 10;
    private int tickCount = 0;
    private int x;
    private int y;

    // 属性（Eigenschaften）
    public double Angle
    {
        get => angle;
        set => angle = value;
    }

    public List<LegoBrick> Bricks => bricks;

    public Parameters.LegoColor Color { get; set; }

    public Point Location
    {
        get => new Point(x, y);
        set
        {
            x = value.X;
            y = value.Y;
        }
    }

    // 构造函数 1
    public ProductionLine(Point location,
                          double angle,
                          Parameters.LegoColor color)
    {
        Location = location;
        Angle = angle;
        Color = color;
        CreateBricks();
    }

    // 构造函数 2
    public ProductionLine(Pose pose,
                          Parameters.LegoColor color)
        : this(pose.Location, pose.Angle, color)
    {
    }

    // 创建传送带上的积木
    public void CreateBricks()
{
    bricks.Clear();

    double radians = Angle * Math.PI / 180.0;

    double spacing = 24.0;
    double brickSize = 12.0;

    // 以传送带左上角为旋转基准
    // 红蓝积木放在传送带下侧，黄绿放在上侧
    double sideOffset;

    switch (Color)
{
    case Parameters.LegoColor.red:
        sideOffset = 10.0;
        break;

    case Parameters.LegoColor.blue:
        sideOffset = 47.0;
        break;

    case Parameters.LegoColor.yellow:
        sideOffset = -2.0;
        break;

    case Parameters.LegoColor.green:
        sideOffset = -2.0;
        break;

    default:
        sideOffset = 0.0;
        break;
}

    for (int i = 0; i < 4; i++)
    {
        double offset = 10.0 + i * spacing;

        double brickX =
            x + offset * Math.Cos(radians)
              - sideOffset * Math.Sin(radians);

        double brickY =
            y + offset * Math.Sin(radians)
              + sideOffset * Math.Cos(radians);

        Pose pose = new Pose(
            (int)Math.Round(brickX),
            (int)Math.Round(brickY),
            Angle
        );

        bricks.Add(new LegoBrick(
            pose,
            Color,
            Parameters.LegoSize.small
        ));
    }
}
    


    // 交付完成后，在原来的生产位置补充一块积木
public void ReplaceDeliveredBrick(LegoBrick deliveredBrick)
{
    // 保留最初的生产位置、颜色和尺寸
    LegoBrick replacement = new LegoBrick(
        deliveredBrick.LegoPose,
        Color,
        deliveredBrick.Size);

    // 计算传送带当前相对于初始位置的位移
    int forwardSteps = tickCount <= stepsPerDirection
        ? tickCount
        : 2 * stepsPerDirection - tickCount;

    double distance = forwardSteps * 3.0;
    double radians = Angle * Math.PI / 180.0;

    Point initialLocation = replacement.Location;

    replacement.Location = new Point(
        initialLocation.X
            + (int)Math.Round(distance * Math.Cos(radians)),
        initialLocation.Y
            + (int)Math.Round(distance * Math.Sin(radians)));

    bricks.Add(replacement);
}
    
    
    
    
    // 后续实现沿传送带方向运动
    public void Bewegen()
    {
         // 前10次向前移动，后10次向后移动
        bool forward = tickCount < stepsPerDirection;

        tickCount = (tickCount + 1) % (2 * stepsPerDirection);

    // 将角度转换为弧度
        double radians = Angle * Math.PI / 180.0;

    // 每次移动3个单位
        double step = 3.0;

        double dx = step * Math.Cos(radians);
        double dy = step * Math.Sin(radians);

        if (!forward)
        {
            dx = -dx;
            dy = -dy;
        }

        foreach (LegoBrick brick in Bricks)
        {
             // 已被机器人抓住的积木不再随传送带移动
             if (brick.IsDocked)
                continue;

            Point oldLocation = brick.Location;

            brick.Location = new Point(
                (int)Math.Round(oldLocation.X + dx),
                (int)Math.Round(oldLocation.Y + dy)
            );
        }
    }
}