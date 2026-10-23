using System.Drawing;

namespace smartFactoryApp.Models;

public class CollisionArea
{
    public Point Location { get; set; }

    public double Width { get; set; }

    public double Height { get; set; }

    public double Angle { get; set; }

    public CollisionArea(
        Point location,
        double width,
        double height,
        double angle)
    {
        Location = location;
        Width = width;
        Height = height;
        Angle = angle;
    }
}