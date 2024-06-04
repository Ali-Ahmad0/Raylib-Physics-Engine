using GameEngine.src.physics.body;
using Raylib_cs;
using System.Numerics;

public struct Animation
{
    public Texture2D Atlas { get; private set; } // The texture atlas containing the animation frames
    public int FramesPerSecond { get; private set; } // The number of frames to display per second
    private int CurrentFrame; // The index of the current frame being displayed
    public List<Rectangle> Rectangles { get; private set; } // The list of rectangles representing each frame in the atlas
    public int TotalFrames { get; private set; } // The total number of frames in the animation

    private static double animationStartTime = 0; // The start time of the animation

    private static PlayerStates prevState = PlayerStates.IDLE; // The previous state of the player

    private static float time; // The elapsed time since the animation started

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
        return (int)((Raylib.GetTime() - animationStartTime) * FramesPerSecond) % TotalFrames; // Calculate the current frame based on the elapsed time
    }

    // Plays the animation on a body
    public void Play(PhysicsBody2D body, bool flipH = false, bool flipV = false)
    {
        PlayerStates test = PlayerStates.IDLE;
        if (body is PlayerBody2D)
        {
            PlayerBody2D player = (PlayerBody2D)body;
            PlayerStates currentState = player.State;
            test = currentState;
            if (currentState != prevState)
            {
                animationStartTime = Raylib.GetTime(); // Reset the animation start time when the player state changes
                prevState = currentState;
            }
        }
        CurrentFrame = GetUpdatedFrame(); // Update the current frame based on the elapsed time

        float frameSize = body.Dimensions.Height; // Calculate the size of each frame

        Rectangle source = Rectangles[CurrentFrame]; // Get the source rectangle for the current frame
        Rectangle dest = new Rectangle(
            body.Transform.Translation.X - (frameSize * ((source.Width / source.Height) - 1) / 2), body.Transform.Translation.Y,
            frameSize * source.Width / source.Height, frameSize
            ); // Calculate the destination rectangle for drawing the frame

        Vector2 origin = new Vector2(frameSize / 2.75f, frameSize / 2f); // Calculate the origin for rotation

        if (flipH)
        {
            source.Width *= -1; // Flip the frame horizontally
            origin.X = body.Dimensions.Height - origin.X; // Adjust the origin when flipped horizontally
        }

        Raylib.DrawTexturePro(Atlas, source, dest, origin, 0, Color.White); // Draw the frame using Raylib

    }

    // Returns if an animation is completed or not
    public bool Completed()
    {
        // Check if time for animation has completed
        time += Raylib.GetFrameTime(); // Update the elapsed time
        if (time >= (float)TotalFrames / FramesPerSecond) // Check if the elapsed time exceeds the total animation time
        {
            time = 0; // Reset the elapsed time
            return true; // Animation is completed
        }

        return false; // Animation is not completed
    }
}
