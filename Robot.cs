using System;
using SplashKitSDK;
using _7_4HD;

namespace _7_4HD
{
    public abstract class Robot
    {
        protected double X { get; set; }
        protected double Y { get; set; }
        protected Color MainColor { get; set; }
        protected Vector2D Velocity { get; set; }

        public virtual int Width
        {
            get { return 50; }
        }
        public virtual int Height
        {
            get { return 50; }
        }

        public Robot(Window gameWindow, Player player)
        {
            // Randomize starting position offscreen
            Random rnd = new Random();
            if (rnd.Next(2) == 0)
            {
                // Left or right
                X = (rnd.Next(2) == 0) ? -Width : gameWindow.Width;
                Y = SplashKit.Rnd(gameWindow.Height);
            }
            else
            {
                // Top or bottom
                Y = (rnd.Next(2) == 0) ? -Height : gameWindow.Height;
                X = SplashKit.Rnd(gameWindow.Width);
            }

            // Calculate velocity toward the player
            Point2D fromPt = new Point2D() { X = X, Y = Y };
            Point2D toPt = new Point2D() { X = player.X, Y = player.Y };
            Vector2D dir = SplashKit.UnitVector(SplashKit.VectorPointToPoint(fromPt, toPt));
            Velocity = SplashKit.VectorMultiply(dir, 2); // Speed of 2 units

            // Assign random color
            MainColor = Color.RandomRGB(200);
        }

        public void Update()
        {
            X += Velocity.X;
            Y += Velocity.Y;
        }

        public bool IsOffscreen(Window gameWindow)
        {
            return (X < -Width || X > gameWindow.Width || Y < -Height || Y > gameWindow.Height);
        }

        public Circle CollisionCircle
        {
            get { return SplashKit.CircleAt(X + Width / 2, Y + Height / 2, 20); }
        }

        public abstract void Draw();
    }

    // Boxy Robot Class
    public class Boxy : Robot
    {
        public Boxy(Window gameWindow, Player player) : base(gameWindow, player) { }

        public override void Draw()
        {
            // Body
            SplashKit.FillRectangle(Color.Gray, X, Y, Width, Height);

            // Eyes
            SplashKit.FillRectangle(MainColor, X + 12, Y + 10, 10, 10); // Left eye
            SplashKit.FillRectangle(MainColor, X + 27, Y + 10, 10, 10); // Right eye

            // Mouth
            SplashKit.FillRectangle(MainColor, X + 12, Y + 30, 25, 10);
            SplashKit.FillRectangle(Color.White, X + 14, Y + 32, 21, 6); // Inner mouth
        }
    }

    // Roundy Robot Class
    public class Roundy : Robot
    {
        public Roundy(Window gameWindow, Player player) : base(gameWindow, player) { }

        public override void Draw()
        {
            double leftX = X + 17, midX = X + 25, rightX = X + 33;
            double midY = Y + 25, eyeY = Y + 20, mouthY = Y + 35;

            // Body
            SplashKit.FillCircle(Color.White, midX, midY, 25);
            SplashKit.DrawCircle(Color.Gray, midX, midY, 25);

            // Eyes
            SplashKit.FillCircle(MainColor, leftX, eyeY, 5);
            SplashKit.FillCircle(MainColor, rightX, eyeY, 5);

            // Mouth
            SplashKit.DrawLine(Color.Black, leftX, mouthY, rightX, mouthY);
        }
    }
    //boss robot, new creation
    public class Boss : Robot
    {
        private Bitmap _bossBitmap;

        public Boss(Window gameWindow, Player player) : base(gameWindow, player)
        {
            // Load the boss image
            _bossBitmap = SplashKit.LoadBitmap("Boss", "Boss.png");
            
            if (_bossBitmap == null)
            {
                throw new Exception("Boss bitmap could not be loaded.");
            }

            Point2D fromPt = new Point2D() { X = X, Y = Y };
            Point2D toPt = new Point2D() { X = player.X, Y = player.Y };
            Vector2D dir = SplashKit.UnitVector(SplashKit.VectorPointToPoint(fromPt, toPt));
            // Boss moves slower than other robots
            Velocity = SplashKit.VectorMultiply(dir, 0.5);
        }

        public override void Draw()
        {
            // Draw the boss using the bitmap
            SplashKit.DrawBitmap(_bossBitmap, X, Y);
        }

        public override int Width
        {
            get { return _bossBitmap != null ? _bossBitmap.Width : 0; }
        }

        public override int Height
        {
            get { return _bossBitmap != null ? _bossBitmap.Height : 0; }
        }
    }

}
