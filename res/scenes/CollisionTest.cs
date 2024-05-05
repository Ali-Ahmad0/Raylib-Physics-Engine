using GameEngine.src.world;
using Raylib_cs;
using System.Numerics;
using GameEngine.src.physics.body;
using GameEngine.src.input;
using GameEngine.src.helper;

namespace GameEngine.res.scenes;

public class CollisionTest : World2D
{
    // Member variables
    private List<PhysicsBody2D> bodies;
    private List<Color> colors;


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
    }

    public override void Update(double delta)
    {
        //// Create a camera centered at the middle of the screen
        //Camera2D camera = new Camera2D(Vector2.Zero, Vector2.Zero, 0, 1f);

        //if (bodies.Count > 1)
        //{
        //    camera.Target = bodies[1].Transform.Translation;
        //    camera.Offset = new Vector2(640, 480);
        //}

        // Begin 2D mode with the camera
        //Raylib.BeginMode2D(camera);

        float keyboardRotation = Input.GetDirection("left", "right") / 5;
        float gamepadRotation = 0f;

        // Check gamepad input if connected
        if (Raylib.IsGamepadAvailable(0))
        {
            gamepadRotation = Gamepad.GetRightXAxis() / 5;
        }

        // Use gamepad rotation only if keyboard rotation is not providing input
        float rotation = keyboardRotation != 0f ? keyboardRotation : gamepadRotation;

        // Draw
        Draw();

        bodies[0].Rotate(rotation);

        // End 2D mode
        //Raylib.EndMode2D();

        // Handle physics outside the 2D mode
        HandlePhysics(bodies, delta);
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

        // Ensure bodies are created (call once or in Ready)
        if (bodies.Count == 0) { 
            CreateStaticBody(new Vector2(640, 900), 0f, Vector2.One, 0.5f, 1200f, 100f, out StaticBody2D staticBody);
            bodies.Add(staticBody);

        }

        // Random
        Random random = new Random();
        float xBox = (float)(random.NextDouble() * (1.2 - 1) + 1);
        float yBox = (float)(random.NextDouble() * (1.2 - 1) + 1);

        float sCir = (float)(random.NextDouble() * (1 - 0.8) + 0.8);

        Vector2 scaleBox = new Vector2(xBox, yBox);
        Vector2 scaleCir = new Vector2(sCir, sCir);

        // Create bodies (testing)
        if (Mouse.IsRMBDown() || Gamepad.IsButtonPressed("r1")) {
            
            // Create circle rigid body
            CreateRigidBody(Mouse.GetPos(), scaleCir, 1f, 0.5f, 32f, out RigidBody2D rigidBody);
            bodies.Add(rigidBody);

            Console.WriteLine(bodies.Count);
        }

        else if (Mouse.IsLMBPressed() || Gamepad.IsButtonPressed("l1")) {

            // Create box rigid body
            CreateRigidBody(Raylib.GetMousePosition(), 0f, scaleBox, 1f, 0.5f, 64f, 64f, out RigidBody2D rigidBody);
            bodies.Add(rigidBody);

        } 

        // Update and draw each body
        for (int i = 0; i < bodies.Count; i++) {
            DrawCollisionShapes(bodies[i], colors[i % 5]);
        }

        // Cursor icon
        Raylib.DrawText("<>", (int)cursorPos.X, (int)cursorPos.Y, 32, Color.Green);
    }
}
