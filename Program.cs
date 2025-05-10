using System;
using SplashKitSDK;

namespace _7_4HD
{
    public class Program
    {
        public static void Main()
        {
            // window creation
            Window gameWindow = new Window("RobotDodge", 800, 600);
            RobotDodge game = new RobotDodge(gameWindow);

            while (!gameWindow.CloseRequested && !game.Quit)
            {
                // Process all events like 
                SplashKit.ProcessEvents();
                // Handle input (movement and quiting)
                game.HandleInput();
                game.Update();
                game.Draw();
                SplashKit.Delay(20);
            }
        }
    }
}
