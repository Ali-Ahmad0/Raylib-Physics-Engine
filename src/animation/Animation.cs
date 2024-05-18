using GameEngine.src.physics.body;
using Raylib_cs;
using System.Numerics;

public struct Animation
{
    public Texture2D Atlas { get; private set; }
    public int FramesPerSecond { get; private set; }
    private int CurrentFrame; 
    public List<Rectangle> Rectangles { get; private set; }
    public int TotalFrames { get; private set; }

    private static double AnimationStartTime = 0;

    private static PlayerStates prevState = PlayerStates.IDLE;


    public Animation(Texture2D atlas, int framesPerSecond, List<Rectangle> rectangles)
    {
        Atlas = atlas;
        FramesPerSecond = framesPerSecond;
        Rectangles = rectangles;
        TotalFrames = rectangles.Count; // Calculate and store the total number of frames
        CurrentFrame = 0;
    }

    public int GetUpdatedFrame()
    {
        return (int)((Raylib.GetTime() - AnimationStartTime) * FramesPerSecond) % TotalFrames;
    }

    // Plays the animation on a body
    public void Play(PhysicsBody2D body, bool flipH = false, bool flipV = false)
    {
        if (body is PlayerBody2D)
        {
            PlayerBody2D player = (PlayerBody2D)body;
            PlayerStates currentState = player.State;
            if (currentState != prevState)
            {
                AnimationStartTime = Raylib.GetTime();
                prevState = currentState;
                System.Console.WriteLine("State: " + currentState);
            }

        }
        CurrentFrame = GetUpdatedFrame();

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
        return GetUpdatedFrame() >= TotalFrames - 1;
    }
}
