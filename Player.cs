using System;
using SplashKitSDK;
using System.Runtime.InteropServices;
using _7_4HD;

public class Player
{
    private Bitmap _playerBitmap;
    public double X { get; private set; }
    public double Y { get; private set; }
    public bool Quit { get; private set; }
    public int Lives { get; private set; }
    private const int SPEED = 5;

    public int Width
    {
        get { return _playerBitmap.Width; }
    }
    public int Height
    {
        get { return _playerBitmap.Height; }
    }

    public Player(Window gameWindow)
    {
        // Load the image to the Bitmap field
        _playerBitmap = SplashKit.LoadBitmap("Player", "Player.png");

        // syntax for having the player at the center of the screen.
        X = (gameWindow.Width - _playerBitmap.Width) / 2;
        Y = (gameWindow.Height - _playerBitmap.Height) / 2;

        // Setting Quit to false initially
        Quit = false;
        Lives = 5;
    }

    //Movement in all 4 directions.
    public void HandleInput()
    {
        if (SplashKit.KeyDown(KeyCode.UpKey))
        {
            Y -= SPEED;
        }
        if (SplashKit.KeyDown(KeyCode.DownKey))
        {
            Y += SPEED;
        }
        if (SplashKit.KeyDown(KeyCode.LeftKey))
        {
            X -= SPEED;
        }
        if (SplashKit.KeyDown(KeyCode.RightKey))
        {
            X += SPEED;
        }

        // Check if Escape key is pressed to quit, storing default value to false
        if (SplashKit.KeyDown(KeyCode.EscapeKey))
        {
            Quit = true;
        }
    }

    // Method to ensure player stays within window
    public void StayOnWindow(Window gameWindow)
    {
        const int GAP = 10;

        // Checking on all 4 sides to see if the gap is proper and the image is not moving out of the window.
        //Please suggest if there is a better approach to it
        if (X < GAP)
        {
            X = GAP;
        }

        if (X + Width > gameWindow.Width - GAP)
        {
            X = gameWindow.Width - Width - GAP;
        }

        if (Y < GAP)
        {
            Y = GAP;
        }

        if (Y + Height > gameWindow.Height - GAP)
        {
            Y = gameWindow.Height - Height - GAP;
        }
    }
    //draw method
    public void Draw()
    {
        SplashKit.DrawBitmap(_playerBitmap, X, Y);
    }

    public bool CollidedWith(Robot robot)
    {
        // Use the bitmap's collision method to check if it overlaps with the robot's collision circle
        return _playerBitmap.CircleCollision(X, Y, robot.CollisionCircle);
    }

    //method to loose a life
    public void LoseLife()
    {
        if (Lives > 0) Lives--;
    }
    public void DrawLives(Window gameWindow)
    {
        // Radius of the circles
        const int RADIUS = 10;    
        // Gap between circles
        const int GAP = 30;       
        // Start from the right, leave some margin
        int startX = gameWindow.Width - (Lives * GAP) - 20; 
        // Fixed position near the top
        int startY = 20;         

        for (int i = 0; i < Lives; i++)
        {
            SplashKit.FillCircle(Color.Red, startX + i * GAP, startY, RADIUS);
        }
    }

}
