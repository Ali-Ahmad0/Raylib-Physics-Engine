using Raylib_cs;

namespace GameEngine.src.helper;

public struct Animation
{
    public Texture2D Atlas { get; private set; }
    public int FramesPerSecond { get; private set; }
    public List<Rectangle> Rectangles { get; private set; }

    public Animation(Texture2D atlas, int framesPerSecond, List<Rectangle> rectangles)
    {
        Atlas = atlas;
        FramesPerSecond = framesPerSecond;
        Rectangles = rectangles;
    }
}