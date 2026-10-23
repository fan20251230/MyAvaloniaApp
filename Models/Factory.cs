using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Drawing;
using smartFactoryApp.Models;
using static smartFactoryApp.Models.Parameters;
using static smartFactoryApp.Models.ViewParameters;
using System.Collections.Generic;
using System;


namespace smartFactoryApp.ViewModels;

public class Factory
{
    Robot robot;

    // 当前机器人抓住的积木
    private LegoBrick? dockedBrick = null;
    private List<LegoBrick> freeBricks = new List<LegoBrick>();
    List <LegoBrick> bricks = new List<LegoBrick>();
    ProductionLine productionRed;
    ProductionLine productionBlue;
    ProductionLine productionYellow;
    ProductionLine productionGreen;
    public double RobotX => robot.Location.X;
    public double RobotY => robot.Location.Y;
    public double RobotAngle => robot.Angle;
    

// 计算机器人夹爪中心 TCP 的实际位置
public PointF GetRobotTCP()
{
    // 机器人 Canvas 的左上角
    double x = robot.Location.X;
    double y = robot.Location.Y;

    // 机器人旋转中心
    double centerX = x + 35;
    double centerY = y + 25;

    // TCP 位于机器人上边缘的中点
    double rad = robot.Angle * Math.PI / 180.0;

    double tcpX = centerX + 25 * Math.Sin(rad);
    double tcpY = centerY - 25 * Math.Cos(rad);

    return new PointF((float)tcpX, (float)tcpY);
}

private double GetDistanceToBrick(LegoBrick brick)
{
    // 获取机器人夹爪 TCP
    PointF tcp = GetRobotTCP();

    // Lego 积木中心
    double brickCenterX = brick.Location.X + 6;
    double brickCenterY = brick.Location.Y + 6;

    // 计算两点之间的距离
    double dx = tcp.X - brickCenterX;
    double dy = tcp.Y - brickCenterY;

    return Math.Sqrt(dx * dx + dy * dy);
}


// 寻找机器人 TCP 附近最近的 Lego 积木
private LegoBrick? FindNearestBrick()
{
    LegoBrick? nearestBrick = null;
    double minDistance = 10.0;

    // 搜索四条传送带上的所有积木
    foreach (ProductionLine line in new[]
    {
        productionRed,
        productionBlue,
        productionYellow,
        productionGreen
    })
    {
        foreach (LegoBrick brick in line.Bricks)
        {
            double distance = GetDistanceToBrick(brick);

            if (distance <= minDistance)
            {
                minDistance = distance;
                nearestBrick = brick;
            }
        }
    }

// 搜索已经离开传送带、但当前未被抓取的积木
foreach (LegoBrick brick in freeBricks)
{
    if (brick.IsDocked)
        continue;

    double distance = GetDistanceToBrick(brick);

    if (distance <= minDistance)
    {
        minDistance = distance;
        nearestBrick = brick;
    }
}


    return nearestBrick;
}


// 抓取 TCP 附近的 Lego 积木
public bool Dock()
{
    // 已经抓住一个积木，不能重复抓取
    if (dockedBrick != null)
        return false;

    // 寻找 TCP 附近最近的积木
    LegoBrick? brick = FindNearestBrick();

    // 10 像素范围内没有积木
    if (brick == null)
        return false;

    // 记录抓住的积木
    dockedBrick = brick;
    dockedBrick.IsDocked = true;

    // 从原来的传送带移除，避免释放后再次被传送带移动
productionRed.Bricks.Remove(brick);
productionBlue.Bricks.Remove(brick);
productionYellow.Bricks.Remove(brick);
productionGreen.Bricks.Remove(brick);

if (!freeBricks.Contains(brick))
{
    freeBricks.Add(brick);
}


    UpdateDockedBrick();

    return true;
}

public bool Undock()
{
    if (dockedBrick == null)
        return false;

    dockedBrick.IsDocked = false;
    dockedBrick = null;

    return true;
}
   

// 让抓住的积木跟随机器人 TCP
private void UpdateDockedBrick()
{
    if (dockedBrick == null)
        return;

    // 获取机器人夹爪中心
    PointF tcp = GetRobotTCP();

    // Lego 积木为 12 × 12，中心应位于 TCP
    dockedBrick.Location = new Point(
        (int)Math.Round(tcp.X - 6),
        (int)Math.Round(tcp.Y - 6));

    // 积木跟随机器人旋转
    dockedBrick.Angle = robot.Angle;
}


    public List<LegoBrick> Bricks { get {return bricks;}}
    public ProductionLine ProductionRed => productionRed;
    public ProductionLine ProductionBlue => productionBlue;
    public ProductionLine ProductionYellow => productionYellow;
    public ProductionLine ProductionGreen => productionGreen;



    public IReadOnlyList<CollisionArea> CollisionAreas => collisionAreas;



private List<CollisionArea> collisionAreas = new List<CollisionArea>();



   
    public Factory()
    {
        robot = new Robot(RobotPose.Location, Borders);

    // 暂时保留原来的四个 LegoBrick
        CreateBricks();

    // 创建四条生产线
        productionRed = new ProductionLine(
            new Point(140, 150), 90, LegoColor.red);

        productionBlue = new ProductionLine(
            new Point(490, 150), 90, LegoColor.blue);

        productionYellow = new ProductionLine(
            new Point(100, 450), 0, LegoColor.yellow);

        productionGreen = new ProductionLine(
            new Point(380, 450), 0, LegoColor.green);


// 四条彩色生产线
collisionAreas.Add(new CollisionArea(
    productionRed.Location, 110, 45, productionRed.Angle));

collisionAreas.Add(new CollisionArea(
    productionBlue.Location, 110, 45, productionBlue.Angle));

collisionAreas.Add(new CollisionArea(
    productionYellow.Location, 110, 45, productionYellow.Angle));

collisionAreas.Add(new CollisionArea(
    productionGreen.Location, 110, 45, productionGreen.Angle));

// Recycling Line
collisionAreas.Add(new CollisionArea(
    new Point(240, 280), 110, 45, 0));

// Delivery Line
collisionAreas.Add(new CollisionArea(
    new Point(212, 40), 185, 65, 0));




    }
    private void CreateBricks()
    {    
        Pose pose1 = new Pose(BrickPose.X + 0, BrickPose.Y, BrickPose.Angle);
        Pose pose2 = new Pose(BrickPose.X + 50, BrickPose.Y, BrickPose.Angle);
        Pose pose3 = new Pose(BrickPose.X + 100, BrickPose.Y, BrickPose.Angle);
        Pose pose4 = new Pose(BrickPose.X + 150, BrickPose.Y, BrickPose.Angle);
        /*Pose pose1 = new Pose(50, 200, BrickPose.Angle);
        Pose pose2 = new Pose(100, 250, BrickPose.Angle);
        Pose pose3 = new Pose(150, 250, BrickPose.Angle);
        Pose pose4 = new Pose(200, 250, BrickPose.Angle);*/
            
        bricks.Add(new LegoBrick(pose1, LegoColor.red, LegoSize.small));
        bricks.Add(new LegoBrick(pose2, LegoColor.green, LegoSize.small));
        bricks.Add(new LegoBrick(pose3, LegoColor.blue, LegoSize.small));
        bricks.Add(new LegoBrick(pose4, LegoColor.yellow, LegoSize.small));
    }
    public void RobotBewegen(Direction dir)
    {// 保存移动前的位置
    Point oldPosition = robot.Location;

    // 按原来的方式移动
    robot.Bewegen(dir);

    // 检查移动后是否与传送带碰撞
    if (Kollidiert(
        robot.Location.X,
        robot.Location.Y,
        robot.Angle))
    {
        // 发生碰撞，恢复原来的位置
        robot.Location = oldPosition;
    }
       
    UpdateDockedBrick();

    }
    public void RobotRotateRight()
    {
        // 保存旋转前的角度
    double oldAngle = robot.Angle;

    // 执行旋转
    robot.RotateRight();

    // 检查旋转后是否发生碰撞
    if (Kollidiert(
        robot.Location.X,
        robot.Location.Y,
        robot.Angle))
    {
        // 发生碰撞，恢复原来的角度
        robot.Angle = oldAngle;
    }
    UpdateDockedBrick();

    }

public void RobotRotateLeft()
{
    double oldAngle = robot.Angle;

    robot.RotateLeft();

    if (Kollidiert(
        robot.Location.X,
        robot.Location.Y,
        robot.Angle))
    {
        robot.Angle = oldAngle;
    }
    UpdateDockedBrick();
}

public void RobotResetAngle()
{
    // 保存复位前的角度
    double oldAngle = robot.Angle;

    // 复位到 0°
    robot.ResetAngle();

    // 检查复位后是否发生碰撞
    if (Kollidiert(
        robot.Location.X,
        robot.Location.Y,
        robot.Angle))
    {
        // 如果发生碰撞，恢复原来的角度
        robot.Angle = oldAngle;
    }

    UpdateDockedBrick();
}



    int tickCount = 0;
    public void LegoBricksBewegen()
    {
        bool outward = tickCount < 10;
        tickCount = (tickCount + 1) % 20;
        foreach (LegoBrick brick in Bricks)
        {   
            if (outward)
                brick.Bewegen(Parameters.Direction.right);
            else
               brick.Bewegen(Parameters.Direction.left); 
        }
    }
    public void ProductionLinesBewegen()
    {
        productionRed.Bewegen();
        productionBlue.Bewegen();
        productionYellow.Bewegen();
        productionGreen.Bewegen();
    }

    
// 计算一个矩形旋转后的四个顶点
private static PointF[] GetCorners(
    double x, double y,
    double width, double height,
    double angle,
    double originX, double originY)
{
    double rad = angle * Math.PI / 180.0;
    double cos = Math.Cos(rad);
    double sin = Math.Sin(rad);

    PointF[] corners =
    {
        new PointF((float)x, (float)y),
        new PointF((float)(x + width), (float)y),
        new PointF((float)(x + width), (float)(y + height)),
        new PointF((float)x, (float)(y + height))
    };

    for (int i = 0; i < corners.Length; i++)
    {
        double dx = corners[i].X - originX;
        double dy = corners[i].Y - originY;

        corners[i] = new PointF(
            (float)(originX + dx * cos - dy * sin),
            (float)(originY + dx * sin + dy * cos));
    }

    return corners;
}

// 判断两个旋转矩形是否相交（SAT）
private static bool RectanglesIntersect(
    PointF[] a, PointF[] b)
{
    foreach (PointF[] polygon in new[] { a, b })
    {
        for (int i = 0; i < 4; i++)
        {
            PointF p1 = polygon[i];
            PointF p2 = polygon[(i + 1) % 4];

            double axisX = -(p2.Y - p1.Y);
            double axisY = p2.X - p1.X;

            double minA = double.MaxValue;
            double maxA = double.MinValue;
            double minB = double.MaxValue;
            double maxB = double.MinValue;

            foreach (PointF p in a)
            {
                double projection = p.X * axisX + p.Y * axisY;
                minA = Math.Min(minA, projection);
                maxA = Math.Max(maxA, projection);
            }

            foreach (PointF p in b)
            {
                double projection = p.X * axisX + p.Y * axisY;
                minB = Math.Min(minB, projection);
                maxB = Math.Max(maxB, projection);
            }

            if (maxA <= minB || maxB <= minA)
                return false;
        }
    }

    return true;
}




// 检查机器人是否与六条传送带发生碰撞
private bool Kollidiert(int robotX, int robotY, double robotAngle)
{
    // 蓝框在机器人 Canvas 内的位置和尺寸
    // Canvas: 70 × 50，旋转中心为 (35, 25)
    PointF[] robotCorners = GetCorners(
        robotX + 19,
        robotY - 2,
        30,
        52,
        robotAngle,
        robotX + 35,
        robotY + 25);

    foreach (CollisionArea area in collisionAreas)
    {
        // 传送带围绕图片左上角旋转
        PointF[] conveyorCorners = GetCorners(
            area.Location.X,
            area.Location.Y,
            area.Width,
            area.Height,
            area.Angle,
            area.Location.X,
            area.Location.Y);

        if (RectanglesIntersect(robotCorners, conveyorCorners))
        {
            return true;
        }
    }

    return false;
}



}