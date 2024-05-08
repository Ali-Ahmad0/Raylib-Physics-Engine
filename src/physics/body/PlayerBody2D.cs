using GameEngine.src.physics.component;
using Raylib_cs;
using System.Numerics;
using GameEngine.src.input;
using GameEngine.src.helper;

namespace GameEngine.src.physics.body;

public enum PlayerStates
{
    IDLE,
    WALK,
    JUMP,
    FALL,
    CROUCH,
    DIE
}

public class PlayerBody2D : RigidBox2D
{
    internal PlayerStates State { get; set; }

    private Animation[] animations;
    public PlayerBody2D(Vector2 position, float rotation, float width, float height, List<Component> components) :
        base(position, rotation, 0.985f * width * height, 0.985f, width * height, 0f, width, height, components) 
    {
        // Initialize the player
        State = PlayerStates.IDLE;
        animations = new Animation[6];

        // Initialize the player animations
        createAnimations();
    }

    public void UseDefaultMotion(double delta)
    {
        MovePlayer(delta);
        Jump();
    }

    // Default Player Motion logic (optional)
    private float maxSpeed = 6000;
    private float acceleration = 500;

    private float landDeceleration = 750;
    private float airDeceleration  = 300;

    private const int JUMP_BUFFER_TIME = 15;
    private const int CAYOTE_JUMP_TIME = 10;

    private int jumpBufferCounter = 0;
    private int cayoteJumpCounter = 0;

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
        }

        else
        {
            LinVelocity.X = IsOnFloor ? MathExtra.MoveToward(LinVelocity.X, 0, landDeceleration * (float)delta)
            : MathExtra.MoveToward(LinVelocity.X, 0, airDeceleration * (float)delta);
        }

        LinVelocity.X = (float)Math.Clamp(LinVelocity.X, -maxSpeed * delta, maxSpeed * delta);
        
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

    public void DrawPlayer()
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

            case PlayerStates.CROUCH:
                currAnimation = animations[4];
                break;

            case PlayerStates.DIE:
                currAnimation = animations[5];
                break;
            default:
                break;
        }


        Rectangle dest = new Rectangle(Transform.Translation.X, Transform.Translation.Y, Dimensions.Height, Dimensions.Height);
        Vector2 origin = new Vector2(Dimensions.Height / 2.75f, Dimensions.Height / 2);
        
        int index = (int)(Raylib.GetTime() * currAnimation.framesPerSecond) % currAnimation.rectangles.Count;
        Raylib.DrawTexturePro(currAnimation.atlas, currAnimation.rectangles[index], dest, origin, 0, Color.White);
    }

    // Animations for the default player character
    private void createAnimations()
    {
        // Implement the new AddAnimation method
        string path = "C:/Users/saadk/Desktop/NUST/Semester 2/Object Oriented Programming/End Semester Project/sprites/Hero Knight/Sprites/";
        AddAnimation(PlayerStates.IDLE, path + "_Idle.png", 1, 10, new Rectangle(0, 40, 40, 40));
        AddAnimation(PlayerStates.WALK, path + "Run.png", 10, 8, new Rectangle(0, 0, 180, 180));
    }

    public void AddAnimation(PlayerStates state, string path, int framesPerSecond, int numberOfSprite, Rectangle spriteSize)
    {
        List<Rectangle> rectangles = new List<Rectangle>();
        for (int i = 0; i < numberOfSprite; i++)
        {
            rectangles.Add(new Rectangle(spriteSize.X + (i * 3 + 1) * spriteSize.Width, spriteSize.Y, spriteSize.Width, spriteSize.Height));
        }
        Animation anim = new Animation(Raylib.LoadTexture(path), framesPerSecond, rectangles);
        animations[(int)state] = anim;
    }
}

