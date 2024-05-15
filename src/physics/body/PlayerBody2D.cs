using GameEngine.src.physics.component;
using Raylib_cs;
using System.Numerics;
using GameEngine.src.input;
using GameEngine.src.helper;
using GameEngine.src.main;

namespace GameEngine.src.physics.body;

public enum PlayerStates
{
    IDLE, WALK, JUMP, FALL, CROUCH_IDLE, CROUCH_WALK, DIE
}

public class PlayerBody2D : RigidBox2D
{
    internal PlayerStates State { get; set; }

    private List<Animation> animations;
    public PlayerBody2D(Vector2 position, float rotation, float width, float height, List<Component> components) :
        base(position, rotation, 0.985f * width * height, 0.985f, width * height, 0f, width, height, components) 
    {
        // Initialize the player
        State = PlayerStates.IDLE;
        animations = new List<Animation>();
        flipH = false;

        // Initialize the player animations
        createAnimations();
    }

    public void UseDefaultPlayer(double delta)
    {
        MovePlayer(delta);
        Crouch();
        Jump();

        DrawPlayer();
    }

    // Default Player Motion logic (optional)
    private float maxSpeed = 5000;
    private float acceleration = 500;

    private float landDeceleration = 750;
    private float airDeceleration  = 300;

    private const int JUMP_BUFFER_TIME = 15;
    private const int CAYOTE_JUMP_TIME = 10;

    private int jumpBufferCounter = 0;
    private int cayoteJumpCounter = 0;

    private bool flipH;

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
            LinVelocity.Y = -0.35f;

            jumpBufferCounter = 0;
            cayoteJumpCounter = 0;
        }

    }

    private void DrawPlayer()
    {
        Animation currAnimation = animations[0];

        switch (State)
        {
            case PlayerStates.IDLE:
                currAnimation = animations[0];
                break;

            case PlayerStates.WALK:
                currAnimation = animations[1];
                break;

            case PlayerStates.JUMP:
                currAnimation = animations[2];
                break;

            case PlayerStates.FALL:
                currAnimation = animations[3];
                break;

            case PlayerStates.CROUCH_IDLE:
                currAnimation = animations[4];
                break;

            case PlayerStates.CROUCH_WALK:
                currAnimation = animations[5];
                break;

            default:
                break;
        }

        currAnimation.Play(this, flipH);
    }

    // Animations for the default player character
    private void createAnimations()
    {
        // Construct the relative path from the executable's directory to the assets folder
        string relativePath = "../../../example/assets/player";

        // Combine the executable directory with the relative path to get the full path to the assets folder
        string path = Path.GetFullPath(Path.Combine(Properties.ExecutableDirectory, relativePath)) + "/";
        Rectangle size = new Rectangle(0, 40, 40, 40);

        AddAnimation(path + "_Idle.png", 10, 10, size);
        AddAnimation(path + "_Run.png", 12, 10, size);
        AddAnimation(path + "_Jump.png", 12, 3, size);
        AddAnimation(path + "_Fall.png", 12, 3, size);
        AddAnimation(path + "_Crouch.png", 1, 1, size);
        AddAnimation(path + "_CrouchWalk.png", 10, 8, size);
    }

    public void AddAnimation(string path, int framesPerSecond, int numberOfSprite, Rectangle spriteSize)
    {
        List<Rectangle> rectangles = new List<Rectangle>();
        for (int i = 0; i < numberOfSprite; i++)
        {
            rectangles.Add(new Rectangle(spriteSize.X + (i * 3 + 1) * spriteSize.Width, spriteSize.Y, spriteSize.Width, spriteSize.Height));
        }
        Animation anim = new Animation(Raylib.LoadTexture(path), framesPerSecond, rectangles);
        animations.Add(anim);
        
    }
}

