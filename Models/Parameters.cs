namespace MyAvaloniaApp.Models;

public static class Parameters
{
    // 每次移动的距离
    private const int step = 10;

    // 其他类通过这个属性读取步长
    public static int Step => step;
    private const int rotationStep = 15;

    public static int RotationStep => rotationStep;

    // 机器人可以移动的方向
    public enum Direction
    {
        Stop,
        Up,
        Down,
        Left,
        Right,
        UpLeft,
        UpRight,
        DownLeft,
        DownRight

    }
}