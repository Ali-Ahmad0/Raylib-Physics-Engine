using Raylib_cs;
using System.Numerics;

namespace GameEngine.src.input;
public struct Input
{
    // Dictionary of key bindings
    private static Dictionary<string, KeyboardKey> keyBindings = new Dictionary<string, KeyboardKey>()
    {
        // Defualt values, can be modified later
        { "left", KeyboardKey.Left },
        { "right", KeyboardKey.Right },
        { "up", KeyboardKey.Up },
        { "down", KeyboardKey.Down }
    };
    
    // Method to assign a key to a specific action
    // Creates action if does not exist
    public static void AssignKey(string action, KeyboardKey key)
    {
        if (!keyBindings.ContainsKey(action))
            keyBindings.Add(action, key);

        else keyBindings[action] = key;
    }

    // Modify an already assigned key for a specific action
    // Creates action if does not exist
    public static void ModifyKey(string action, KeyboardKey newKey)
    {
        if (keyBindings.ContainsKey(action))
            keyBindings[action] = newKey;

        else AssignKey(action, newKey);
    }

    // Removes an already assigned key for a specific action
    // Does nothing if action does not exist
    public static void RemoveKey(string action)
    {
        if (keyBindings.ContainsKey(action))
            keyBindings.Remove(action);

        else return;
    }

    // Method to get the key assigned to a specific action
    // Returns null if action does not exist
    private static KeyboardKey GetKey(string action)
    {
        if (keyBindings.ContainsKey(action))
            return keyBindings[action];

        else return KeyboardKey.Null;
    }
    
    // Check for keyboard input
    public static bool IsKeyPressed(string action) { return Raylib.IsKeyPressed(GetKey(action)); }
    public static bool IsKeyPressedAgain(string action) { return Raylib.IsKeyPressedRepeat(GetKey(action)); }
    public static bool IsKeyDown(string action) { return Raylib.IsKeyDown(GetKey(action)); }
    public static bool IsKeyUp(string action) { return Raylib.IsKeyUp(GetKey(action)); }
    public static bool IsKeyReleased(string action) { return Raylib.IsKeyReleased(GetKey(action)); }

    // Returns a direction based on 2 inputs
    public static float GetDirection(string actionA, string actionB)
    {
        float neg = IsKeyDown(actionA) ? -1 : 0;
        float pos = IsKeyDown(actionB) ? 1 : 0;

        return neg + pos;    
    }

    // Returns a vector based on 4 inputs
    public static Vector2 GetVector(string actionA, string actionB, string actionC, string actionD) 
    { 
        float x = GetDirection(actionA, actionB);
        float y = GetDirection(actionC, actionD);

        Vector2 direction = new Vector2(x, y);
        Vector2.Normalize(direction);

        return direction;
    }
}

