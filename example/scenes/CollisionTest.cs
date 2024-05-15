using GameEngine.src.world;
using Raylib_cs;
using System.Numerics;
using GameEngine.src.physics.body;
using GameEngine.src.input;
using GameEngine.src.helper;

namespace Game.res.scenes;

public class CollisionTest : World2D
{
    // Member variables
    private List<PhysicsBody2D> bodies;
    private List<Color> colors;

    private Camera2D camera;


    // Constructor for initialization
    internal CollisionTest()
    {
        colors = new List<Color>() { 
            Color.White, 
            Color.Red,
            Color.Green,
            Color.Blue,
            Color.Gold
        };

        bodies = new List<PhysicsBody2D>();

        Gamepad.AssignButton("l1", GamepadButton.LeftTrigger1);
        Gamepad.AssignButton("r1", GamepadButton.RightTrigger1);

        Raylib.HideCursor();

        // Create floor
        CreateStaticBody(new Vector2(640, 900), 0f, Vector2.One, 0.5f, 1200f, 100f, out StaticBody2D staticBody);
        bodies.Add(staticBody);

        // Create a camera centered at the middle of the screen
        camera = new Camera2D(Vector2.Zero, Vector2.Zero, 0, 1f);
    }

    public override void Update(double delta)
    {
        // Begin 2D mode with the camera
        Raylib.BeginMode2D(camera);

        float keyboardRotation = Input.GetDirection("left", "right") / 4;
        float gamepadRotation = 0f;

        // Check gamepad input if connected
        if (Raylib.IsGamepadAvailable(0))
        {
            gamepadRotation = Gamepad.GetRightXAxis() / 4;
        }

        // Use gamepad rotation only if keyboard rotation is not providing input
        float rotation = keyboardRotation != 0f ? keyboardRotation : gamepadRotation;

        bodies[0].Rotate(rotation);

        // Draw
        Draw();

        // End 2D mode
        Raylib.EndMode2D();

        // Handle physics outside the 2D mode
        HandlePhysics(bodies, delta, camera);
    }

    // Draw
    private void Draw()
    {
        // Set cursor position (works with controller)
        Vector2 cursorPos = Mouse.GetPos();
        Vector2 leftAxis = Gamepad.GetLeftAxis();

        if (leftAxis.LengthSquared() > 0.025)
        {
            cursorPos += leftAxis * 10;
        }

        Mouse.SetPos(cursorPos);

        // Scene title
        Raylib.DrawText("Collision Test", 20, 20, 32, Color.Green);

        // Random
        Random random = new Random();
        float xBox = (float)(random.NextDouble() * (1.3 - 0.9) + 0.9);
        float yBox = (float)(random.NextDouble() * (1.3 - 0.9) + 0.9);

        float sCir = (float)(random.NextDouble() * (1 - 0.7) + 0.7);

        Vector2 scaleBox = new Vector2(xBox, yBox);
        Vector2 scaleCir = new Vector2(sCir, sCir);

        // Create bodies (testing)
        if (Mouse.IsRMBPressed() || Gamepad.IsButtonPressed("r1")) 
        {
            
            // Create circle rigid body
            CreateRigidBody(Mouse.GetPos(), scaleCir, 1f, 0.5f, 32f, out RigidBody2D rigidBody);
            bodies.Add(rigidBody);

        }

        else if (Mouse.IsLMBPressed() || Gamepad.IsButtonPressed("l1")) 
        {

            // Create box rigid body
            CreateRigidBody(Mouse.GetPos(), 0f, scaleBox, 1f, 0.5f, 64f, 64f, out RigidBody2D rigidBody);
            bodies.Add(rigidBody);

        } 

        // Update and draw each body
        for (int i = 0; i < bodies.Count; i++) 
        {
            DrawCollisionShapes(bodies[i], colors[i % 5]);
        }

        // Cursor icon
        Raylib.DrawText("< >", (int)cursorPos.X, (int)cursorPos.Y, 32, Color.Green);
    }
}
