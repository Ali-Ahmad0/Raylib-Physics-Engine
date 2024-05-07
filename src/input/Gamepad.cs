using Raylib_cs;
using System.Numerics;

namespace GameEngine.src.helper;

public struct Gamepad
{
    // Dictionary of key bindings
    private static Dictionary<string, GamepadButton> controllerBindings = new Dictionary<string, GamepadButton>()
    {
        // Defualt values, can be modified later
        { "left", GamepadButton.LeftFaceLeft },
        { "right", GamepadButton.LeftFaceRight },
        { "up", GamepadButton.LeftFaceUp },
        { "down", GamepadButton.LeftFaceDown }
    };

    // Method to assign a button to a specific action
    // Creates action if does not exist
    public static void AssignButton(string action, GamepadButton button)
    {
        if (!controllerBindings.ContainsKey(action))
            controllerBindings.Add(action, button);
        else
            controllerBindings[action] = button;
    }

    // Modify an already assigned button for a specific action
    // Creates action if does not exist
    public static void ModifyButton(string action, GamepadButton newButton)
    {
        if (controllerBindings.ContainsKey(action))
            controllerBindings[action] = newButton;
        else
            AssignButton(action, newButton);
    }

    // Removes an already assigned button for a specific action
    // Does nothing if action does not exist
    public static void RemoveButton(string action)
    {
        if (controllerBindings.ContainsKey(action))
            controllerBindings.Remove(action);
        else
            return;
    }

    // Method to get the button assigned to a specific action
    // Returns null if action does not exist
    private static GamepadButton GetButton(string action)
    {
        if (controllerBindings.ContainsKey(action))
            return controllerBindings[action];
        else
            return GamepadButton.Unknown;
    }

    // Check for gamepad button input
    public static bool IsButtonPressed(string action, int player = 0) 
    { 
        return Raylib.IsGamepadButtonPressed(player, GetButton(action)); 
    }
    public static bool IsButtonDown(string action, int player = 0) 
    { 
        return Raylib.IsGamepadButtonDown(player, GetButton(action)); 
    }
    public static bool IsButtonUp(string action, int player = 0)
    {
        return Raylib.IsGamepadButtonUp(player, GetButton(action)); 
    }
    public static bool IsButtonReleased(string action, int player = 0) 
    { 
        return Raylib.IsGamepadButtonReleased(player, GetButton(action)); 
    }

    public static float GetDirection(string actionA, string actionB)
    {
        float neg = IsButtonDown(actionA) ? -1 : 0;
        float pos = IsButtonDown(actionB) ? 1 : 0;

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

    // Get gamepad movement axis 
    public static float GetLeftXAxis(int player = 0)
    {
        return Raylib.GetGamepadAxisMovement(player, GamepadAxis.LeftX);
    }

    public static float GetLeftYAxis(int player = 0)
    {
        return Raylib.GetGamepadAxisMovement(player, GamepadAxis.LeftY);
    }

    public static float GetRightXAxis(int player = 0)
    {
        return Raylib.GetGamepadAxisMovement(player, GamepadAxis.RightX);
    }

    public static float GetRightYAxis(int player = 0)
    {
        return Raylib.GetGamepadAxisMovement(player, GamepadAxis.RightY);
    }

    public static Vector2 GetLeftAxis(int player = 0)
    {
        return new Vector2(GetLeftXAxis(player), GetLeftYAxis(player));
    }

    public static Vector2 GetRightAxis(int player = 0)
    {
        return new Vector2(GetRightXAxis(player), GetRightYAxis(player));
    }

}