using GameEngine.example.scenes;
using GameEngine.example.scenes.game;
using GameEngine.src.helper;
using GameEngine.src.input;
using GameEngine.src.world;
using Raylib_cs;

namespace Game.res.scenes;

public static class SceneTree
{
    internal static int scene;
    internal static Global currentScene;

    static SceneTree()
    {
        scene = -1;
        currentScene = new InitialScene();

        Input.AssignKey("one", KeyboardKey.One);
        Input.AssignKey("two", KeyboardKey.Two);
        Input.AssignKey("three", KeyboardKey.Three);
        Input.AssignKey("four", KeyboardKey.Four);
        Input.AssignKey("five", KeyboardKey.Five);

        Gamepad.AssignButton("l2", GamepadButton.LeftTrigger2);
        Gamepad.AssignButton("r2", GamepadButton.RightTrigger2);
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
        else if (Input.IsKeyPressed("five"))
            scene = 4;

        else if (Gamepad.IsButtonPressed("r2"))
            scene = (scene + 1) % 5; 
        else if (Gamepad.IsButtonPressed("l2"))
            scene = (scene - 1 + 4) % 5; 

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

            case 4:
                if (currentScene is not MainMenu)
                    currentScene = new MainMenu();
                break;

            default:
                if (currentScene is not InitialScene)
                {
                    if (scene > 4)
                        scene = 0;

                    else if (scene < 0)
                        scene = 3;
                }

                break;
        }

        currentScene.Update(delta);
    }
  
}