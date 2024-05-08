using GameEngine.src.helper;
using GameEngine.src.input;
using GameEngine.src.world;
using Raylib_cs;

namespace GameEngine.res.scenes;

public static class SceneTree
{
    internal static int scene;
    internal static World2D currentScene;

    static SceneTree()
    {
        scene = 0;
        currentScene = new CollisionTest();

        Input.AssignKey("one", KeyboardKey.One);
        Input.AssignKey("two", KeyboardKey.Two);
        Input.AssignKey("three", KeyboardKey.Three);
        Input.AssignKey("four", KeyboardKey.Four);

        Input.AssignKey("jump", KeyboardKey.Space);

        Gamepad.AssignButton("l2", GamepadButton.LeftTrigger2);
        Gamepad.AssignButton("r2", GamepadButton.RightTrigger2);

        Gamepad.AssignButton("jump", GamepadButton.RightFaceDown);
    }

    public static void Update(double delta)
    {
        if (Input.IsKeyPressed("one"))
            scene = 0;
        else if (Input.IsKeyPressed("two"))
            scene = 1;
        else if (Input.IsKeyPressed("three"))
            scene = 2;
        else if (Input.IsKeyPressed("four"))
            scene = 3;
        else if (Gamepad.IsButtonPressed("r2"))
            scene = (scene + 1) % 4; // 4 represents the total number of scenes
        else if (Gamepad.IsButtonPressed("l2"))
            scene = (scene - 1 + 4) % 4; // 4 represents the total number of scenes

        switch (scene)
        {
            case 0:
                if (currentScene is not CollisionTest)
                    currentScene = new CollisionTest();
                break;

            case 1:
                if (currentScene is not ProjectileTest)
                    currentScene = new ProjectileTest();
                break;

            case 2:
                if (currentScene is not TilemapTest)
                    currentScene = new TilemapTest();
                break;

            case 3:
                if (currentScene is not PlayerTest)
                    currentScene = new PlayerTest();
                break;

            default:
                if (scene > 3)
                    scene = 0;

                else if (scene < 0)
                    scene = 3;
                break;
        }

        currentScene.Update(delta);
    }
  
}