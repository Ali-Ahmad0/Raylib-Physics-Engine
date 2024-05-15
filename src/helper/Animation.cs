using GameEngine.src.physics.body;
using Raylib_cs;
using System.Numerics;

public struct Animation
{
    public Texture2D Atlas { get; private set; }
    public int FramesPerSecond { get; private set; }
    public int CurrentFrame { get; private set; }
    public List<Rectangle> Rectangles { get; private set; }

    public Animation(Texture2D atlas, int framesPerSecond, List<Rectangle> rectangles)
    {
        Atlas = atlas;
        FramesPerSecond = framesPerSecond;
        Rectangles = rectangles;
        CurrentFrame = 0; // Initialize the current frame index to 0
    }

    // Plays the animation on a body
    public void Play(PhysicsBody2D body, bool flipH = false, bool flipV = false)
    {
        CurrentFrame = (int)(Raylib.GetTime() * FramesPerSecond) % Rectangles.Count;

        float frameSize = body.Dimensions.Height;

        Rectangle source = Rectangles[CurrentFrame];
        Rectangle dest = new Rectangle(
            body.Transform.Translation.X, body.Transform.Translation.Y,
            frameSize, frameSize
            );

        Vector2 origin = new Vector2(frameSize / 2.75f, frameSize / 2f);

        if (flipH)
        {
            source.Width *= -1;
            origin.X = body.Dimensions.Height - origin.X; // Adjusting the origin when flipped horizontally
        }

        Raylib.DrawTexturePro(Atlas, source, dest, origin, 0, Color.White);
    }

    // Returns if an animation is completed or not
    public bool Completed()
    {
        return CurrentFrame == Rectangles.Count - 1;
    }
}
