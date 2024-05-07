
using Raylib_cs;
using System.Numerics;

namespace GameEngine.src.helper;

public struct Mouse
{

    // Left mouse button
    public static bool IsLMBPressed() { return Raylib.IsMouseButtonPressed(MouseButton.Left); }
    public static bool IsLMBDown() { return Raylib.IsMouseButtonDown(MouseButton.Left); }
    public static bool IsLMBReleased() { return Raylib.IsMouseButtonReleased(MouseButton.Left); }
    public static bool IsLMBUp() { return Raylib.IsMouseButtonUp(MouseButton.Left); }


    // Middle mouse button
    public static bool IsMMBPressed() { return Raylib.IsMouseButtonPressed(MouseButton.Middle); }
    public static bool IsMMBDown() { return Raylib.IsMouseButtonDown(MouseButton.Middle); }
    public static bool IsMMBUp() { return Raylib.IsMouseButtonUp(MouseButton.Middle); }
    public static bool IsMMBReleased() { return Raylib.IsMouseButtonReleased(MouseButton.Middle); }

    // Right mouse button
    public static bool IsRMBPressed() { return Raylib.IsMouseButtonPressed(MouseButton.Right); }
    public static bool IsRMBDown() { return Raylib.IsMouseButtonDown(MouseButton.Right); }
    public static bool IsRMBUp() { return Raylib.IsMouseButtonUp(MouseButton.Right); }
    public static bool IsRMBReleased() { return Raylib.IsMouseButtonReleased(MouseButton.Right); }

    // Mouse Position
    public static int GetX() { return Raylib.GetMouseX(); }
    public static int GetY() { return Raylib.GetMouseY(); }

    public static Vector2 GetDelta() { return Raylib.GetMouseDelta(); }
    public static Vector2 GetPos() 
    {
        return new Vector2(GetX(), GetY()); 
    }
 
    public static void SetPos(Vector2 pos) 
    { 
        Raylib.SetMousePosition((int)pos.X, (int)pos.Y); 
    }


}