using SplashKitSDK;
using _7_4HD;

public class Bullet
{
    private double X { get; set; }
    private double Y { get; set; }
    private Vector2D Velocity { get; set; }
    private const int Speed = 5;

    public Bullet(double startX, double startY, Vector2D direction)
    {
        X = startX;
        Y = startY;
        Velocity = SplashKit.VectorMultiply(direction, Speed);
    }

    public void Update()
    {
        X += Velocity.X;
        Y += Velocity.Y;
    }

    public void Draw()
    {
        SplashKit.FillCircle(Color.Blue, X, Y, 5);
    }

    public bool IsOffscreen(Window gameWindow)
    {
        return (X < 0 || X > gameWindow.Width || Y < 0 || Y > gameWindow.Height);
    }

    public bool CollidedWith(Robot robot)
    {
        return SplashKit.CirclesIntersect(SplashKit.CircleAt(X, Y, 5), robot.CollisionCircle);
    }
}
