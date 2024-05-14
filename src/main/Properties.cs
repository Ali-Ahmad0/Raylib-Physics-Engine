namespace GameEngine.src.main;
public struct Properties
{
    public static int ScreenWidth = 1280;
    public static int ScreenHeight = 960;
    public static string Title = "Game Engine";

    public static bool DisplayFPS = true;
    public static bool EnableMT = false; // Added by Osman
    
    // Get the directory of the executable
    public static string ExecutableDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

    public static bool AllowToggleFullscreen = true;

}

