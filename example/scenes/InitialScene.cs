using GameEngine.src.main;
using GameEngine.src.world;
using Raylib_cs;

namespace GameEngine.example.scenes;

public class InitialScene : Global
{
    string heading;
    string subheading;

    internal InitialScene()
    {
        heading = "-Game Engine Test-";
        subheading = "\t\t\t[Press 1 to 5 to view example scenes]\n\n\n" +
                    "[Or Press L2/R2 on Gamepad to switch scenes]";
    }

    public override void Update(double delta)
    {
        Draw();
    }

    private void Draw()
    { 
        // Calculate the width of the text      
        int headingWidth = Raylib.MeasureText(heading, 64);
        int subheadingWidth = Raylib.MeasureText(subheading, 32);

        // Calculate the X-coordinates
        int headingX = Properties.ScreenWidth / 2 - headingWidth / 2;    
        int subheadingX = Properties.ScreenWidth / 2 - subheadingWidth / 2;

        // Draw the text centered on the X-axis            
        Raylib.DrawText(heading, headingX, Properties.ScreenHeight / 2 - 256, 64, Color.Green);
        Raylib.DrawText(subheading, subheadingX, Properties.ScreenHeight / 2, 32, Color.White);
        

    }
}