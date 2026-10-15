using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Drawing;
using smartFactoryApp.Models;
using static smartFactoryApp.Models.Parameters;
using static smartFactoryApp.Models.ViewParameters;
using System.Collections.Generic;

namespace smartFactoryApp.ViewModels;

public class Factory
{
    Robot robot;
    List <LegoBrick> bricks = new List<LegoBrick>();
    ProductionLine productionRed;
    ProductionLine productionBlue;
    ProductionLine productionYellow;
    ProductionLine productionGreen;
    public double RobotX => robot.Location.X;
    public double RobotY => robot.Location.Y;
    public double RobotAngle => robot.Angle;
    public List<LegoBrick> Bricks { get {return bricks;}}
    public ProductionLine ProductionRed => productionRed;
    public ProductionLine ProductionBlue => productionBlue;
    public ProductionLine ProductionYellow => productionYellow;
    public ProductionLine ProductionGreen => productionGreen;
   
    public Factory()
    {
        robot = new Robot(RobotPose.Location, Borders);

    // 暂时保留原来的四个 LegoBrick
        CreateBricks();

    // 创建四条生产线
        productionRed = new ProductionLine(
            new Point(60, 120), -45, LegoColor.red);

        productionBlue = new ProductionLine(
            new Point(480, 60), 45, LegoColor.blue);

        productionYellow = new ProductionLine(
            new Point(80, 460), 45, LegoColor.yellow);

        productionGreen = new ProductionLine(
            new Point(460, 520), -45, LegoColor.green);
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
    {
        robot.Bewegen(dir);
    }
    public void RobotRotateRight()
    {
        robot.RotateRight();
    }

    public void RobotRotateLeft()
    {
        robot.RotateLeft();
    }

    public void RobotResetAngle()
    {
        robot.ResetAngle();
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
}