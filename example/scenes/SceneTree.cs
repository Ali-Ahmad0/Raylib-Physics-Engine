using GameEngine.example.scenes;
using GameEngine.example.scenes.game;
using GameEngine.src.helper;
using GameEngine.src.input;
using GameEngine.src.world;
using Raylib_cs;

namespace Game.res.scenes
{
    public static class SceneTree
    {
        internal static int Scene;
        internal static RootScene CurrentScene;

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
            if (Input.IsKeyPressed("one")) Scene = 0;
            else if (Input.IsKeyPressed("two")) Scene = 1;
            else if (Input.IsKeyPressed("three")) Scene = 2;
            else if (Input.IsKeyPressed("four")) Scene = 3;
            else if (Input.IsKeyPressed("five")) Scene = 4;
            else if (Gamepad.IsButtonPressed("r2")) Scene = (Scene + 1) % 5;
            else if (Gamepad.IsButtonPressed("l2")) Scene = (Scene - 1 + 5) % 5;

            RootScene newScene = Scene switch
            {
                0 => CurrentScene is not CollisionTest ? new CollisionTest() : CurrentScene,
                1 => CurrentScene is not ProjectileTest ? new ProjectileTest() : CurrentScene,
                2 => CurrentScene is not TilemapTest ? new TilemapTest() : CurrentScene,
                3 => CurrentScene is not PlayerTest ? new PlayerTest() : CurrentScene,
                4 => CurrentScene is not MainMenu ? new MainMenu() : CurrentScene,
                5 => CurrentScene is not Level1 ? new Level1() : CurrentScene,
                6 => CurrentScene is not Level2 ? new Level2() : CurrentScene,
                7 => CurrentScene is not Level3 ? new Level3() : CurrentScene,
                _ => CurrentScene
            };

            if (newScene != CurrentScene)
                CurrentScene = newScene;
            
            CurrentScene.Update(delta);
        }
    }
}
