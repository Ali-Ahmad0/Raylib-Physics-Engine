using Game.res.scenes;
using GameEngine.src.main;
using Raylib_cs;

namespace Game.main;
internal class Process
{
    // Define percentages for element positions and sizes
    private const float FPS_POSITION_X_PERCENTAGE = 0.9f;
    private const float FPS_POSITION_Y_PERCENTAGE = 0.0375f;

    private float fpsPositionX;
    private float fpsPositionY;

    internal void Start()
    {
        Init(); // Initialize the program
        Loop(); // Process loop

        // Stop at the end of loop
        Stop();
    }

    // Initialization 
    private void Init()
    {
        // Initialize Window
        Raylib.InitWindow(Properties.ScreenWidth, 
            Properties.ScreenHeight, Properties.Title);

        Raylib.SetTargetFPS(60); // Set max FPS
    }

    // Game loop
    private void Loop()
    {
        for (; !Raylib.WindowShouldClose();)
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(new Color(15, 15, 15, 255));

            // Toggle fullscreen
            if (Raylib.IsKeyPressed(KeyboardKey.F) && Properties.AllowToggleFullscreen)
                Raylib.ToggleFullscreen();

            // Calculate initial FPS position based on window size
            fpsPositionX = Raylib.GetScreenWidth() * FPS_POSITION_X_PERCENTAGE;
            fpsPositionY = Raylib.GetScreenHeight() * FPS_POSITION_Y_PERCENTAGE;


            // Show FPS to screen
            if (Properties.DisplayFPS)
            {
                // Draw FPS using calculated position
                Raylib.DrawFPS((int)fpsPositionX, (int)fpsPositionY);
            }

            double delta = Raylib.GetFrameTime() / (256);
            SceneTree.Update(delta);

            Raylib.EndDrawing();
        }
    }

    // End Process (Obviously)
    private static void Stop()
    {
        Raylib.CloseWindow();
    }
}
