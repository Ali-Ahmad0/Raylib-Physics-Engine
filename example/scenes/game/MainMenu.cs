using Raylib_cs;
using System.Numerics;
using GameEngine.src.gui;
using GameEngine.src.main;
using GameEngine.src.world;
using Game.res.scenes;

namespace GameEngine.example.scenes.game;

public class MainMenu : Global
{
    string title;
 
    SimpleButton start;
    SimpleButton exit;

    Vector2 center;

    internal MainMenu() 
    {
        title = "-DUNGEON ESCAPE-";
        Vector2 size = new Vector2(256, 96);
        center = new Vector2(Properties.ScreenWidth / 2, Properties.ScreenHeight / 2);

        Rectangle button0 = new Rectangle(center + new Vector2(-size.X / 2, -size.Y), size);
        Rectangle button1 = new Rectangle(center + new Vector2(-size.X / 2, +size.Y), size);

        start = new SimpleButton(button0, Color.SkyBlue, "Start", 32);
        exit = new SimpleButton(button1, Color.Red, "Exit", 32);
    }

    public override void Update(double delta)
    {
        if (start.IsHovered() || exit.IsHovered())
        {
            if (start.IsHovered())
                start.ButtonColor = Color.White;

            if (exit.IsHovered())
                exit.ButtonColor = Color.White;

        }

        else
        {
            start.ButtonColor = Color.SkyBlue;
            exit.ButtonColor = Color.Red;
        }

        if (start.IsClicked())
            SceneTree.scene = 3;

        if (exit.IsClicked())
            Raylib.CloseWindow();
        

        Draw();
    }

    private void Draw()
    {
        int width = Raylib.MeasureText(title, 64);
        Raylib.DrawText(title, (int)center.X - width / 2, (int)center.Y - 304, 64, Color.Green);

        start.DrawButton();
        exit.DrawButton();
    }

}