using System;
using System.Collections.Generic;
using SplashKitSDK;
using _7_4HD;

public class RobotDodge
{
    private Window _gameWindow;
    private Player _player;
    private List<Robot> _robots;
    private List<Bullet> _bullets;
    private int _score;
    // New field for the highest score
    private int _highestScore; 
    private SplashKitSDK.Timer _timer;
    private bool _isPaused;
    private GameState _state;
    // File to store the highest score
    private const string HighScoreFile = "highest_score.txt"; 
    private Music _backgroundMusic;

    public RobotDodge(Window gameWindow)
    {
        _gameWindow = gameWindow;
        _player = new Player(_gameWindow);
        _robots = new List<Robot>();
        _bullets = new List<Bullet>();
        _timer = new SplashKitSDK.Timer("Game Timer");
        _timer.Start();
        _isPaused = false;
        _state = GameState.Running;
        // Initialize the highest score
        _highestScore = 0; 
        // Load the highest score from the file
        LoadHighestScore(); 
        _backgroundMusic = SplashKit.LoadMusic("Background", "background_music.mp3");
        // Loop indefinitely
        SplashKit.PlayMusic(_backgroundMusic, -1); 
    }

    public bool Quit
    {
        get { return _player.Quit; }
    }

    public void HandleInput()
    {
        // Restart game on R key press
        if (SplashKit.KeyTyped(KeyCode.RKey)) 
        {
            RestartGame();
            return;
        }

        if (_state == GameState.GameOver) return; 
        if (_isPaused || _state == GameState.GameOver)
        {
            SplashKit.PauseMusic();
        }
        else
        {
            SplashKit.ResumeMusic();
        }


        if (SplashKit.KeyTyped(KeyCode.PKey))
        {
            if (_state == GameState.Running)
            {
                _isPaused = true;
                _state = GameState.Paused;
                _timer.Pause();
            }
            else if (_state == GameState.Paused)
            {
                _isPaused = false;
                _state = GameState.Running;
                _timer.Resume();
            }
        }

        if (!_isPaused)
        {
            _player.HandleInput();
            if (SplashKit.KeyDown(KeyCode.BKey))
            {
                Vector2D direction = new Vector2D();

                if (SplashKit.KeyDown(KeyCode.RightKey)) direction = SplashKit.VectorTo(1, 0);
                else if (SplashKit.KeyDown(KeyCode.LeftKey)) direction = SplashKit.VectorTo(-1, 0);
                else if (SplashKit.KeyDown(KeyCode.UpKey)) direction = SplashKit.VectorTo(0, -1);
                else if (SplashKit.KeyDown(KeyCode.DownKey)) direction = SplashKit.VectorTo(0, 1);

                if (direction.X != 0 || direction.Y != 0)
                {
                    Point2D startPoint = SplashKit.PointAt(_player.X + _player.Width / 2, _player.Y + _player.Height / 2);
                    _bullets.Add(new Bullet(startPoint.X, startPoint.Y, direction));
                }
            }
        }
    }


    public void Update()
    {
        if (_state != GameState.Running) return;

        if (_player.Lives <= 0)
        {
            if (_score > _highestScore)
            {
                _highestScore = _score;
                // Save the new highest score to the file
                SaveHighestScore(); 
            }

            _state = GameState.GameOver;
            SplashKit.PauseMusic();
            _timer.Stop();
            return;
        }

        _player.StayOnWindow(_gameWindow);

        foreach (Robot robot in _robots)
        {
            robot.Update();
        }

        foreach (Bullet bullet in _bullets)
        {
            bullet.Update();
        }

        _bullets.RemoveAll(bullet => bullet.IsOffscreen(_gameWindow));
        _robots.RemoveAll(robot => robot.IsOffscreen(_gameWindow));

        CheckBulletCollisions();
        CheckPlayerCollisions();

        SpawnRobot();
        _score = (int)(_timer.Ticks / 1000);
    }

    public void Draw()
    {
        if (_state == GameState.GameOver)
        {
            _gameWindow.Clear(Color.Yellow);
            SplashKit.DrawText("GAME OVER", Color.Red, "Arial-Bold", 72, _gameWindow.Width / 2 - 150, _gameWindow.Height / 2 - 50);
            SplashKit.DrawText($"FINAL SCORE: {_score}", Color.Black, "Arial-Bold", 36, _gameWindow.Width / 2 - 100, _gameWindow.Height / 2 + 20);
            SplashKit.DrawText($"HIGHEST SCORE: {_highestScore}", Color.Black, "Arial-Bold", 36, _gameWindow.Width / 2 - 120, _gameWindow.Height / 2 + 60);
            SplashKit.DrawText("PRESS R TO RESTART", Color.Green, "Arial-Bold", 36, _gameWindow.Width / 2 - 120, _gameWindow.Height / 2 + 100);
            _gameWindow.Refresh();
            return;
        }

        if (_isPaused)
        {
            _gameWindow.Clear(Color.Yellow);
            SplashKit.DrawText("GAME PAUSED", Color.Red, "Arial-Bold", 72, _gameWindow.Width / 2 - 150, _gameWindow.Height / 2 - 50);
            SplashKit.DrawText("PRESS R TO RESTART", Color.Green, "Arial-Bold", 36, _gameWindow.Width / 2 - 120, _gameWindow.Height / 2 + 20);
            _gameWindow.Refresh();
            return;
        }

        _gameWindow.Clear(Color.White);

        foreach (Robot robot in _robots)
        {
            robot.Draw();
        }

        foreach (Bullet bullet in _bullets)
        {
            bullet.Draw();
        }

        SplashKit.DrawText($"SCORE: {_score}", Color.Black, "Arial-Bold", 36, 10, 40);
        SplashKit.DrawText($"HIGHEST SCORE: {_highestScore}", Color.Black, "Arial-Bold", 36, 10, 80);

        _player.Draw();
        _player.DrawLives(_gameWindow);

        _gameWindow.Refresh();
    }

    private void CheckBulletCollisions()
    {
        List<Robot> robotsToRemove = new List<Robot>();
        List<Bullet> bulletsToRemove = new List<Bullet>();

        foreach (Bullet bullet in _bullets)
        {
            foreach (Robot robot in _robots)
            {
                if (bullet.CollidedWith(robot))
                {
                    robotsToRemove.Add(robot);
                    bulletsToRemove.Add(bullet);
                    break;
                }
            }
        }

        foreach (Robot robot in robotsToRemove)
        {
            _robots.Remove(robot);
        }

        foreach (Bullet bullet in bulletsToRemove)
        {
            _bullets.Remove(bullet);
        }
    }

    private void CheckPlayerCollisions()
    {
        foreach (Robot robot in _robots)
        {
            if (_player.CollidedWith(robot))
            {
                _robots.Remove(robot);
                _player.LoseLife();
                break;
            }
        }
    }

    private void SpawnRobot()
    {
        // 3% chance to spawn any robot
        if (SplashKit.Rnd(100) < 3) 
        {
            // 10% chance for the boss within spawn events
            if (SplashKit.Rnd(10) == 0) 
            {
                // Spawn Boss
                _robots.Add(new Boss(_gameWindow, _player)); 
            }
            else if (SplashKit.Rnd(2) == 0)
            {
                // Spawn Boxy
                _robots.Add(new Boxy(_gameWindow, _player)); 
            }
            else
            {
                // Spawn Roundy
                _robots.Add(new Roundy(_gameWindow, _player)); 
            }
        }
    }

    private void SaveHighestScore()
    {
        // Write the highest score to the file
        System.IO.File.WriteAllText(HighScoreFile, _highestScore.ToString());
    }

    private void LoadHighestScore()
    {
        // Check if the file exists
        if (System.IO.File.Exists(HighScoreFile))
        {
            string fileContent = System.IO.File.ReadAllText(HighScoreFile);
            if (int.TryParse(fileContent, out int savedScore))
            {
                _highestScore = savedScore;
            }
        }
        else
        {
            // Default to 0 if file doesn't exist
            _highestScore = 0; 
        }
    }

    private void RestartGame()
    {
        // Reset player
        _player = new Player(_gameWindow); 
        // Clear all robots
        _robots.Clear();
        // Clear all bullets
        _bullets.Clear();
        // Reset score
        _score = 0;
        // Reset timer 
        _timer.Reset(); 
        // Start the timer
        _timer.Start(); 
        // Reset the game state to running
        _state = GameState.Running; 
    }


    public enum GameState
    {
        Running,
        Paused,
        GameOver
    }

}
