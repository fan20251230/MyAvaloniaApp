using System.Drawing;
namespace smartFactoryApp.Models;

public class Robot : Beweger
{
    public Robot(Point location, Rectangle borders) : base(location, borders)
    {
        Location = location;
        Borders = borders;
    }
    public Robot(Point location) : base(location)
    {
        Location = location;
    }
    public override void Bewegen(Parameters.Direction direction)
    {
        base.Bewegen(direction);
    }
     // 顺时针旋转
    public void RotateRight()
    {
        Angle = (Angle + Parameters.rotationStep) % 360;
    }

    // 逆时针旋转
    public void RotateLeft()
    {
        Angle = (Angle - Parameters.rotationStep + 360) % 360;
    }

    // 恢复初始角度
    public void ResetAngle()
    {
        Angle = 0;
    }
}

