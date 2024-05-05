using GameEngine.src.main;
using Raylib_cs;
using System.Numerics;

namespace GameEngine.src.helper;

public struct CameraBounds
{
    public Vector2 Min { get; private set; }
    public Vector2 Max { get; private set; }
    public void Calculate(Camera2D camera)
    {
        // Assuming Properties.ScreenWidth and Properties.ScreenHeight are the dimensions of your screen
        float halfScreenWidth = Properties.ScreenWidth / 2;
        float halfScreenHeight = Properties.ScreenHeight / 2;

        // Calculate the bounds of the camera, accounting for offset
        float minX = halfScreenWidth + (-halfScreenWidth / camera.Zoom + camera.Target.X - camera.Offset.X);
        float maxX = halfScreenWidth + (halfScreenWidth / camera.Zoom + camera.Target.X - camera.Offset.X);
        float minY = halfScreenHeight + (-halfScreenHeight / camera.Zoom + camera.Target.Y - camera.Offset.Y);
        float maxY = halfScreenHeight + (halfScreenHeight / camera.Zoom + camera.Target.Y - camera.Offset.Y);

        Min = new Vector2(minX, minY);
        Max = new Vector2(maxX, maxY);
    }
}