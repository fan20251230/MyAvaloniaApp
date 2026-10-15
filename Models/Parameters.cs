namespace smartFactoryApp.Models;
public class Parameters
{
    public enum Direction
    {
        stop = 0,
        up = 1,
        down = 2,
        left = 3,
        right = 4,  
        upLeft = 5,
        upRight = 6,
        downLeft = 7,
        downRight = 8    
    }
    public enum LegoColor
    {
        yellow = 0,
        blue = 1,
        red = 2,
        green = 3,
    }
    public enum LegoSize
    {
        large = 0,
        small = 1,
    }
    public const int bricksPerLine = 4;
    public const double rotationStep = 10;
    public const int timerInteval = 50; //Timer-Intervall in Millisekunden
    public const int step = 3;
}