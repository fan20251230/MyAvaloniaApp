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

    // 工厂的物流管理对象
    public Logistics Logistics { get; } = new Logistics();

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
    private readonly Random deliveryRandom = new Random();

    // 最近一次有效交付的质检结果
    public bool LastDeliveryAccepted { get; private set; }

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
    // 只允许抓取等待状态的积木
    if (brick.IsDocked ||
        brick.Status != BrickStatus.Ready)
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

    // 取走回收积木后，释放它占用的等待位置
    brick.RecyclePlatz = -1;

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

// 判断机器人夹爪是否到达交付线附近
private bool AufDeliveryLinie()
{
    PointF tcp = GetRobotTCP();

    // 交付线的位置和尺寸，与现有碰撞区域一致
    RectangleF deliveryArea = new RectangleF(
        212, 40, 185, 65);

    // 暂设 12 像素的交接容差
    // 因为碰撞保护会阻止机器人进入交付线内部
    deliveryArea.Inflate(12, 12);

    return deliveryArea.Contains(tcp);
}

private int FreierRecyclePlatz()
{
    for (int slot = 0; slot < 4; slot++)
    {
        bool occupied = false;

        foreach (LegoBrick brick in freeBricks)
        {
            if (brick.RecyclePlatz == slot)
            {
                occupied = true;
                break;
            }
        }

        if (!occupied)
            return slot;
    }

    // 四个位置都被占用
    return -1;
}


// 将不合格积木运回中央回收线
// 回收出口的积木直接转入中央维修传送带
public void RecycleBricksBewegen()
{
    foreach (LegoBrick brick in freeBricks)
    {
        if (brick.IsDocked)
            continue;

        if (brick.Status != BrickStatus.AtRecycle)
            continue;

        int slot = FreierRecyclePlatz();

        // 没有空位，继续在回收出口等待
        if (slot == -1)
            continue;

        brick.RecyclePlatz = slot;

        // 中央传送带：
        // 左上角 (240, 280)，尺寸 110 × 45
        // 将积木完整放在传送带内部的上沿
        brick.Location = new Point(
            250 + slot * 24,
            280);

        brick.Angle = 0;

        // 维修完成，允许重新抓取
        brick.Status = BrickStatus.Ready;
        // 机器人空手时，根据新到达的回收积木更新计划
        if (dockedBrick == null)
        {
            UpdatePickUpPlan();
        }
    }
}


// 每次定时器触发，让运输中的积木移动一小段
public void DeliveryBricksBewegen()
{
    const int speed = 3;
    const int customerTargetX = 455;
    const int recycleTargetX = 142;

    List<LegoBrick> deliveredBricks = new();

    foreach (LegoBrick brick in freeBricks)
    {
        if (brick.IsDocked)
            continue;

        if (brick.Status == BrickStatus.ToCustomer)
        {
            int newX = Math.Min(
                brick.Location.X + speed,
                customerTargetX);

            brick.Location = new Point(
                newX, brick.Location.Y);

            if (newX == customerTargetX)
                brick.Status = BrickStatus.AtCustomer;
        }
        else if (brick.Status == BrickStatus.ToRecycle)
        {
            int newX = Math.Max(
                brick.Location.X - speed,
                recycleTargetX);

            brick.Location = new Point(
                newX, brick.Location.Y);

            if (newX == recycleTargetX)
                brick.Status = BrickStatus.AtRecycle;
        }

        if (brick.Status == BrickStatus.AtCustomer)
            deliveredBricks.Add(brick);
    }

    // 遍历结束后，再移除交付完成的积木
    foreach (LegoBrick brick in deliveredBricks)
    {
        freeBricks.Remove(brick);
        ReplaceDeliveredBrick(brick);
    }
}




// 释放机器人携带的积木
public UndockResult Undock()
{
    if (dockedBrick == null)
        return UndockResult.NoBrick;

    if (!AufDeliveryLinie())
        return UndockResult.NotOnDeliveryLine;

    LegoBrick brick = dockedBrick;

    // 放到交付线下方白色运输带的中央
    // 积木尺寸为 12 × 12
    brick.Location = new Point(298, 84);
    brick.Angle = 0;
    brick.IsDocked = false;

    // 每次有效交付独立随机判断：50%通过，50%回收
    LastDeliveryAccepted = deliveryRandom.Next(2) == 0;

    brick.Status = LastDeliveryAccepted
        ? BrickStatus.ToCustomer
        : BrickStatus.ToRecycle;

    // 质检通过、送给客户时才计入订单
    if (brick.Status == BrickStatus.ToCustomer)
    {
        Logistics.BrickDelivered(brick.Color);
    }

    dockedBrick = null;

    // 根据剩余需求重新选择取货目标
    UpdatePickUpPlan();

    return UndockResult.Success;
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

    // 是否已经设置过生产线
public bool ProductionLinesAreSetup { get; private set; } = false;

// 点击 Setup Production Lines 时调用
public void SetupProductionLines()
{
    // 避免重复生成
    if (ProductionLinesAreSetup)
        return;

    productionRed.CreateBricks();
    productionBlue.CreateBricks();
    productionYellow.CreateBricks();
    productionGreen.CreateBricks();

    ProductionLinesAreSetup = true;
}


// 更新下一次取货计划
public void UpdatePickUpPlan()
{
    Logistics.PlanNextPickUp(
        new[]
        {
            productionRed,
            productionBlue,
            productionYellow,
            productionGreen
        },
        freeBricks);
}

// 获取当前任务提示
public string GetTaskInfo()
{
    if (!Logistics.HasOrder)
        return "No order yet: click \"Order\"";

    if (Logistics.IsOrderComplete)
        return $"Order complete: all {Logistics.TotalOrdered} bricks delivered";

    int step = Logistics.TotalDelivered + 1;
    int total = Logistics.TotalOrdered;

    if (dockedBrick != null)
    {
        return $"Step {step}/{total}: Bring the " +
               $"{dockedBrick.Color} brick to delivery";
    }

    LegoBrick? target = Logistics.CurrentBrick;

    if (target == null)
        return $"Step {step}/{total}: Waiting for an available brick";

    return $"Step {step}/{total}: Pick {target.Color} brick " +
           $"from {Logistics.PickStation} and bring to delivery";
}


// 携带积木，定位到交付区前方
public bool GoToDelivery()
{
    if (dockedBrick == null)
        return false;

    robot.Angle = 0;
    robot.Location = new Point(263, 130);

    // 同时更新抓住的积木位置
    UpdateDockedBrick();

    return true;
}

public bool GoToPickUp()
{
    // 携带积木时，应先去交付
    if (dockedBrick != null)
        return false;

    UpdatePickUpPlan();

    LegoBrick? target = Logistics.CurrentBrick;

    if (target == null)
        return false;

    // 目标积木的中心
    double targetX = target.Location.X + 6;
    double targetY = target.Location.Y + 6;

    // 定位后，夹爪距离目标中心24像素
    const double approachDistance = 24;

    double newAngle;
    double robotX;
    double robotY;

    if (productionRed.Bricks.Contains(target))
    {
        // 红色线：从右边接近，夹爪朝左
        newAngle = 270;
        robotX = targetX + approachDistance - 10;
        robotY = targetY - 25;
    }
    else if (productionBlue.Bricks.Contains(target))
    {
        // 蓝色线：从左边接近，夹爪朝右
        newAngle = 90;
        robotX = targetX - approachDistance - 60;
        robotY = targetY - 25;
    }
    else
    {
        // 黄色线、绿色线和中央回收线：
        // 从上方接近，夹爪朝下
        newAngle = 180;
        robotX = targetX - 35;
        robotY = targetY - approachDistance - 50;
    }

    int newX = (int)Math.Round(robotX);
    int newY = (int)Math.Round(robotY);

    // 定位位置不能与传送带发生碰撞
    if (Kollidiert(newX, newY, newAngle))
        return false;

    robot.Angle = newAngle;
    robot.Location = new Point(newX, newY);

    return true;
}


public IEnumerable<LegoBrick> AlleBricks()
{
    foreach (ProductionLine line in new[]
    {
        productionRed,
        productionBlue,
        productionYellow,
        productionGreen
    })
    {
        foreach (LegoBrick brick in line.Bricks)
            yield return brick;
    }

    foreach (LegoBrick brick in freeBricks)
        yield return brick;
}



// 通知积木最初所属的生产线补充
private void ReplaceDeliveredBrick(LegoBrick brick)
{
    brick.OriginLine?.ReplaceDeliveredBrick(brick);

    if (dockedBrick == null)
    {
        UpdatePickUpPlan();
    }
}



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

// 左侧：送往回收的运输区域
collisionAreas.Add(new CollisionArea(
    new Point(88, 40), 120, 65, 0));

// 右侧：送往客户的运输区域
collisionAreas.Add(new CollisionArea(
    new Point(401, 40), 120, 65, 0));



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
    // 空手时，尝试自动抓取夹爪附近的积木
    if (dockedBrick == null)
    {
        Dock();
    }

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

// 从当前生产线库存中随机选8块，生成可完成的订单
public bool CreateAvailableOrder()
{
    List<LegoColor> availableColors = new();

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
            availableColors.Add(brick.Color);
        }
    }

    const int orderSize = 8;

    if (availableColors.Count < orderSize)
        return false;

    int yellow = 0;
    int blue = 0;
    int red = 0;
    int green = 0;

    for (int i = 0; i < orderSize; i++)
    {
        int index = Random.Shared.Next(availableColors.Count);
        LegoColor color = availableColors[index];

        // 只从临时列表移除，实际积木保持不动
        availableColors.RemoveAt(index);

        switch (color)
        {
            case LegoColor.yellow:
                yellow++;
                break;

            case LegoColor.blue:
                blue++;
                break;

            case LegoColor.red:
                red++;
                break;

            case LegoColor.green:
                green++;
                break;
        }
    }

    Logistics.CreateOrder(yellow, blue, red, green);

    return true;
}

}