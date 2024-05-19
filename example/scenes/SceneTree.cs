using GameEngine.example.scenes;
using GameEngine.example.scenes.game;
using GameEngine.src.helper;
using GameEngine.src.input;
using GameEngine.src.world;
using Raylib_cs;

namespace Game.res.scenes;

public static class SceneTree
{
    internal static int Scene;
    internal static Global CurrentScene;

    static SceneTree()
    {
        Scene = -1;
        CurrentScene = new InitialScene();

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
            Scene = 0;
        else if (Input.IsKeyPressed("two"))
            Scene = 1;
        else if (Input.IsKeyPressed("three"))
            Scene = 2;
        else if (Input.IsKeyPressed("four"))
            Scene = 3;
        else if (Input.IsKeyPressed("five"))
            Scene = 4;

        else if (Gamepad.IsButtonPressed("r2"))
            Scene = (Scene + 1) % 5; 
        else if (Gamepad.IsButtonPressed("l2"))
            Scene = (Scene - 1 + 4) % 5; 

        switch (Scene)
        {
            case 0:
                if (CurrentScene is not CollisionTest)
                    CurrentScene = new CollisionTest();
                break;

            case 1:
                if (CurrentScene is not ProjectileTest)
                    CurrentScene = new ProjectileTest();
                break;

            case 2:
                if (CurrentScene is not TilemapTest)
                    CurrentScene = new TilemapTest();
                break;

            case 3:
                if (CurrentScene is not PlayerTest)
                    CurrentScene = new PlayerTest();
                break;

            case 4:
                if (CurrentScene is not MainMenu)
                    CurrentScene = new MainMenu();
                break;

            case 5:
                if (CurrentScene is not Level1)
                    CurrentScene = new Level1();
                
                break;

            default:
                if (CurrentScene is not InitialScene)
                {
                    if (Scene > 4)
                        Scene = 0;

                    else if (Scene < 0)
                        Scene = 3;
                }

                break;
        }

        CurrentScene.Update(delta);
    }
  
}