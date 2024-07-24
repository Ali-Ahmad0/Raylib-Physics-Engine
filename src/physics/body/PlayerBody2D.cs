using GameEngine.src.physics.component;
using Raylib_cs;
using System.Numerics;
using GameEngine.src.input;
using GameEngine.src.helper;
using GameEngine.src.main;

namespace GameEngine.src.physics.body;

internal enum PlayerStates
{
    IDLE, WALK, JUMP, FALL, CROUCH_IDLE, CROUCH_WALK, ATTACK, CROUCH_ATTACK, DIE
}

public class PlayerBody2D : RigidBody2D
{
    internal PlayerStates State { get; set; }

    // Default Player Motion logic (optional)
    private float maxSpeed = 5000;
    private float acceleration = 500;

    private float landDeceleration = 750;
    private float airDeceleration = 300;

    private const int JUMP_BUFFER_TIME = 15;
    private const int CAYOTE_JUMP_TIME = 10;

    private int jumpBufferCounter = 0;
    private int cayoteJumpCounter = 0;

    private int attackCounter = 0;

    private bool flipH;

    private List<Animation> Animations;

    public PlayerBody2D(Vector2 position, float rotation, float width, float height, List<Component> components) :
        base(position, rotation, 0.985f * width * height, 0.985f, 0f, ShapeTypes.Box, components, width:width, height:height) 
    {
        // Initialize the player
        State = PlayerStates.IDLE;
        flipH = false;

        Animations = new List<Animation>();

        // Initialize the player animations
        CreateAnimations();

        Input.AssignKey("jump", KeyboardKey.Space);
        Input.AssignKey("crouch", KeyboardKey.LeftControl);
        Input.AssignKey("attack", KeyboardKey.Z);

        Gamepad.AssignButton("jump", GamepadButton.RightFaceDown);
        Gamepad.AssignButton("crouch", GamepadButton.RightFaceRight);
        Gamepad.AssignButton("attack", GamepadButton.RightTrigger2);
    }

    public void UseDefaultPlayer(double delta)
    {
        if (State != PlayerStates.ATTACK && State != PlayerStates.CROUCH_ATTACK) 
        {
            MovePlayer(delta);
            Jump();
            Crouch();
        }

        Attack();
        DrawPlayer();
    }

    private void MovePlayer(double delta)
    {
        float keyboardDirection = 0f;
        float gamepadDirection = 0f;

        // Check keyboard input
        keyboardDirection = Input.GetDirection("left", "right");

        // Check gamepad input if connected
        if (Raylib.IsGamepadAvailable(0))
        {
            gamepadDirection = Gamepad.GetDirection("left", "right");
        }

        // Use gamepad direction only if keyboard direction is not providing input
        float direction = keyboardDirection != 0f ? keyboardDirection : gamepadDirection;

        if (direction != 0)
        {
            LinVelocity.X += acceleration * direction * (float)delta;
            State = PlayerStates.WALK;

            flipH = direction < 0;
        }

        else
        {
            LinVelocity.X = IsOnFloor ? MathExtra.MoveToward(LinVelocity.X, 0, landDeceleration * (float)delta)
            : MathExtra.MoveToward(LinVelocity.X, 0, airDeceleration * (float)delta);

            State = PlayerStates.IDLE;
        }

        LinVelocity.X = (float)Math.Clamp(LinVelocity.X, -maxSpeed * delta, maxSpeed * delta);
        
    }

    // Crouching
    private void Crouch()
    {
        if ((Input.IsKeyDown("crouch") || Gamepad.IsButtonDown("crouch")) && IsOnFloor)
        {
            maxSpeed = 2000;
            State = LinVelocity.X == 0 ? PlayerStates.CROUCH_IDLE : PlayerStates.CROUCH_WALK;
        }

        else
        {
            maxSpeed = 5000;
        }
    }

    private void Jump()
    {
        if (!IsOnFloor)
        {
            // Start cayote timer when jumped
            if (cayoteJumpCounter > 0)
            {
                cayoteJumpCounter--;
            }

            State = LinVelocity.Y < 0 ? PlayerStates.JUMP : PlayerStates.FALL;

        }

        // Set cayote timer when player is on floor
        else
        {
            cayoteJumpCounter = CAYOTE_JUMP_TIME;

        }

        if ((Input.IsKeyPressed("jump") || Gamepad.IsButtonPressed("jump")))
        {
            jumpBufferCounter = JUMP_BUFFER_TIME;
        }

        // Start the jump buffer timer
        if (jumpBufferCounter > 0)
        {
            jumpBufferCounter--;
        }

        // Check valid condition for jump
        if (jumpBufferCounter > 0 && cayoteJumpCounter > 0)
        {
            LinVelocity.Y = -0.425f;

            jumpBufferCounter = 0;
            cayoteJumpCounter = 0;
        }

    }

    private void Attack()
    {
        if (IsOnFloor && !(State == PlayerStates.ATTACK || State == PlayerStates.CROUCH_ATTACK))
        {
            if (Input.IsKeyPressed("attack") || Gamepad.IsButtonPressed("attack"))
            {
                LinVelocity.X = 0;

                if (State is PlayerStates.CROUCH_IDLE || State is PlayerStates.CROUCH_WALK)
                    State = PlayerStates.CROUCH_ATTACK;

                else 
                {
                    attackCounter = (attackCounter + 1) % 2;
                    State = PlayerStates.ATTACK;
                }
            }
        }
    }

    private void DrawPlayer()
    {
        // Draw the player based on the current state
        Animation currAnimation = Animations[0];

        switch (State)
        {
            case PlayerStates.IDLE:
                currAnimation = Animations[0];
                break;

            case PlayerStates.WALK:
                currAnimation = Animations[1];
                break;

            case PlayerStates.JUMP:
                currAnimation = Animations[2];
                break;

            case PlayerStates.FALL:
                currAnimation = Animations[3];
                break;

            case PlayerStates.CROUCH_IDLE:
                currAnimation = Animations[4];
                break;

            case PlayerStates.CROUCH_WALK:
                currAnimation = Animations[5];
                break;

            case PlayerStates.ATTACK:
                if (attackCounter == 1)
                    currAnimation = Animations[6];

                else
                    currAnimation = Animations[7];

                if (Completed(currAnimation))
                {
                    State = PlayerStates.IDLE;
                }
                    
                break;

            case PlayerStates.CROUCH_ATTACK:
                currAnimation = Animations[8];

                if (Completed(currAnimation))
                {
                    State = PlayerStates.IDLE;
                }

                break;

            default:
                break;
        }

        Play(currAnimation, flipH);
    }

    // Animations for the default player character

    private static double animationStartTime;
    private PlayerStates prevState = PlayerStates.IDLE;

    private int GetUpdatedFrame(Animation animation)
    {
        return (int)((Raylib.GetTime() - animationStartTime) * animation.FramesPerSecond) % animation.TotalFrames;
    }

    // Play the animation
    private void Play(Animation animation, bool flipH)
    {
        PlayerStates currentState = State;
        if (currentState != prevState)
        {
            animationStartTime = Raylib.GetTime();
            prevState = currentState;
        }

        animation.CurrentFrame = GetUpdatedFrame(animation);

        float frameSize = CollisionShape.Dimensions.Height;

        Rectangle source = animation.Rectangles[animation.CurrentFrame];
        Rectangle dest = new Rectangle(
            Transform.Translation.X - (frameSize * ((source.Width / source.Height) - 1) / 2), Transform.Translation.Y,
            frameSize * source.Width / source.Height, frameSize
            );

        Vector2 origin = new Vector2(frameSize / 2.75f, frameSize / 2f);

        if (flipH)
        {
            source.Width *= -1;
            origin.X = CollisionShape.Dimensions.Height - origin.X; // Adjusting the origin when flipped horizontally
        }

        Raylib.DrawTexturePro(animation.Atlas, source, dest, origin, 0, Color.White);
    }

    private bool Completed(Animation animation) 
    {
        if (GetUpdatedFrame(animation) >= animation.TotalFrames - 1)
            return true;

        return false;
    }

    private void CreateAnimations()
    {
        // Construct the relative path from the executable's directory to the assets folder
        string relativePath = "../../../example/assets/player";

        // Combine the executable directory with the relative path to get the full path to the assets folder
        string path = Path.GetFullPath(Path.Combine(Properties.ExecutableDirectory, relativePath)) + "/";
        Rectangle size = new Rectangle(0, 40, 120, 40);

        AddAnimation(path + "_Idle.png", 10, 10, size);
        AddAnimation(path + "_Run.png", 12, 10, size);
        AddAnimation(path + "_Jump.png", 12, 3, size);
        AddAnimation(path + "_Fall.png", 12, 3, size);
        AddAnimation(path + "_Crouch.png", 1, 1, size);
        AddAnimation(path + "_CrouchWalk.png", 10, 8, size);

        AddAnimation(path + "_Attack.png", 10, 4, new Rectangle(0, 40, 120, 40));
        AddAnimation(path + "_Attack2.png", 16, 6, new Rectangle(0, 40, 120, 40));
        AddAnimation(path + "_CrouchAttack.png", 10, 4, new Rectangle(0, 40, 120, 40));
    }

    private void AddAnimation(string path, int framesPerSecond, int numberOfSprite, Rectangle spriteSize)
    {
        // Create a list of rectangles to store the sprite sheet
        List<Rectangle> rectangles = new List<Rectangle>();
        for (int i = 0; i < numberOfSprite; i++)
        {
            rectangles.Add(new Rectangle(spriteSize.X + (i) * spriteSize.Width, spriteSize.Y, spriteSize.Width, spriteSize.Height));
        }
        Animation anim = new Animation(Raylib.LoadTexture(path), framesPerSecond, rectangles);
        Animations.Add(anim);
        
    }
}

internal struct Animation
{
    internal Texture2D Atlas { get; private set; }
    internal int FramesPerSecond { get; private set; }
    internal int CurrentFrame;
    internal List<Rectangle> Rectangles { get; private set; }
    internal int TotalFrames { get; private set; }

    public Animation(Texture2D atlas, int framesPerSecond, List<Rectangle> rectangles)
    {
        Atlas = atlas;
        FramesPerSecond = framesPerSecond;
        Rectangles = rectangles;
        TotalFrames = rectangles.Count; // Calculate and store the total number of frames
        CurrentFrame = 0;
    }
}

