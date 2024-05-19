using Raylib_cs;
using System;
using System.IO;
using System.Numerics;
using GameEngine.src.gui;
using GameEngine.src.main;
using GameEngine.src.world;
using Game.res.scenes;

namespace GameEngine.example.scenes.game
{
    public class MainMenu : Global
    {
        string title;
        TextureButton start;
        TextureButton exit;
        Texture2D background; // New background texture

        Texture2D texture0;
        Texture2D texture1;
        Texture2D texture0pressed;
        Texture2D texture1pressed;
        Vector2 center;
        Font customFont;

        internal MainMenu()
        {
            title = "< DUNGEON ESCAPE >";
            Vector2 size = new Vector2(224, 80);
            center = new Vector2(Properties.ScreenWidth / 2, Properties.ScreenHeight / 2);

            Rectangle button0 = new Rectangle(center + new Vector2(-size.X / 2, -size.Y), size);
            Rectangle button1 = new Rectangle(center + new Vector2(-size.X / 2, +size.Y / 2), size);

            // Load background texture
            background = Raylib.LoadTexture(Path.Combine(Properties.ExecutableDirectory, "../../../example/assets/ui/background.png"));

            // Scale background texture to fit the screen
            float scaleX = (float)Properties.ScreenWidth / background.Width;
            float scaleY = (float)Properties.ScreenHeight / background.Height;
            background.Width = (int)(background.Width * Math.Max(scaleX, scaleY));
            background.Height = (int)(background.Height * Math.Max(scaleX, scaleY));

            // load buttons
            texture0 = Raylib.LoadTexture(Path.Combine(Properties.ExecutableDirectory, "../../../example/assets/ui/start.png"));
            texture1 = Raylib.LoadTexture(Path.Combine(Properties.ExecutableDirectory, "../../../example/assets/ui/exit.png"));

            texture0pressed = Raylib.LoadTexture(Path.Combine(Properties.ExecutableDirectory, "../../../example/assets/ui/start_pressed.png"));
            texture1pressed = Raylib.LoadTexture(Path.Combine(Properties.ExecutableDirectory, "../../../example/assets/ui/exit_pressed.png"));

            start = new TextureButton(button0, texture0);
            exit = new TextureButton(button1, texture1);

            // Load the custom font
            customFont = Raylib.LoadFont(Path.Combine(Properties.ExecutableDirectory, "../../../example/assets/font/editundo.ttf"));
        }

        public override void Update(double delta)
        {
            if (start.IsHovered() || exit.IsHovered())
            {
                if (start.IsHovered())
                    start.Texture = texture0pressed;

                if (exit.IsHovered())
                    exit.Texture = texture1pressed;
            }
            else
            {
                start.Texture = texture0;
                exit.Texture = texture1;
            }

            if (start.IsClicked())
                SceneTree.Scene = 5;

            if (exit.IsClicked())
                Properties.ShouldClose = true;

            Draw();
        }

        private void Draw()
        {
            // Draw background image first to cover the entire screen
            Raylib.DrawTextureEx(background, new Vector2(0, 0), 0, 
                Math.Max((float)Properties.ScreenWidth / background.Width, (float)Properties.ScreenHeight / background.Height), Color.White
                );

            // Measure the width of the title text using the custom font
            float titleWidth = Raylib.MeasureTextEx(customFont, title, 60, 1).X;

            // Position the title text in the center
            Vector2 titlePosition = new Vector2(center.X - titleWidth / 2, 128);

            // Draw the main title text in green color
            Raylib.DrawTextEx(customFont, title, titlePosition, 60, 1, Color.DarkPurple);

            // Draw the small text at the bottom
            string byText = "Credits: Ali Ahmad, Saad Khan";
            float byTextWidth = Raylib.MeasureText(byText, 20);
            Vector2 byTextPosition = new Vector2(center.X - byTextWidth / 2, Properties.ScreenHeight - 40);
            Raylib.DrawText(byText, (int)byTextPosition.X, (int)byTextPosition.Y, 20, Color.White);

            start.DrawButton();
            exit.DrawButton();
        }
    }
}
