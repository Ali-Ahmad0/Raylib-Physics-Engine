﻿namespace GameEngine.src.main;

#pragma warning disable CS8601 // Possible null reference assignment.

public struct Properties
{
    public static int ScreenWidth = 1280;
    public static int ScreenHeight = 960;
    public static string Title = "Game Engine";

    public static bool DisplayFPS = true;
    public static bool ShouldClose = false;

    // Get the directory of the executable
    public static string ExecutableDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);


    public static bool AllowToggleFullscreen = true;

}

