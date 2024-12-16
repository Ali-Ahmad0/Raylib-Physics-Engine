using GameEngine.src.world;
using Raylib_cs;
using System.Numerics;
using GameEngine.src.physics.body;
using GameEngine.src.input;
using GameEngine.src.helper;
using GameEngine.src.physics.collision.shape;
namespace Game.res.scenes;

public class CollisionTest : World
{
    // Member variables
    private List<PhysicsBody2D> bodies;
    private Camera2D camera;
    private List<Color> colors;

    // Constructor for initialization
    internal CollisionTest()
    {
        colors = new List<Color>
        {
            Color.RayWhite,
            Color.Blue,
            Color.Red,
            Color.Green,
            Color.Gold
        };

        bodies = new List<PhysicsBody2D>();

        Gamepad.AssignButton("l1", GamepadButton.LeftTrigger1);
        Gamepad.AssignButton("r1", GamepadButton.RightTrigger1);

        Raylib.HideCursor();

        // Create floor
        StaticBody2D slope1 = new StaticBody2D(new Vector2(300, 380), 10f, Vector2.One, 0.5f, ShapeType.Box, width: 360f, height: 80f);
        StaticBody2D slope2 = new StaticBody2D(new Vector2(900, 320), -15f, Vector2.One, 0.5f, ShapeType.Box, width: 400f, height: 80f);
        StaticBody2D floor = new StaticBody2D(new Vector2(640, 900), 0f, Vector2.One, 0.5f, ShapeType.Box, width:1200f, height:100f);
        
        bodies.Add(floor);
        bodies.Add(slope1);
        bodies.Add(slope2);

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

        // Random scaling
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
            RigidBody2D circle = new RigidBody2D(Mouse.GetPos(), 0, scaleCir, 1f, 0.5f, ShapeType.Circle, radius: 32f);
            bodies.Add(circle);

        }

        else if (Mouse.IsLMBPressed() || Gamepad.IsButtonPressed("l1")) 
        {
            // Create box rigid body
            RigidBody2D box = new RigidBody2D(Mouse.GetPos(), 0, scaleBox, 1f, 0.5f, ShapeType.Box, width:64f, height:64f);
            bodies.Add(box);
        } 

        // Update and draw each body
        for (int i = 0; i < bodies.Count; i++) 
        {
            if (bodies[i] is RigidBody2D)
            {
                DrawCollisionShapes(bodies[i].CollisionShape, 
                    linVelocityToColor(bodies[i].LinVelocity.Length()));
            }
            else
            {
                DrawCollisionShapes(bodies[i].CollisionShape, colors[i % 5]);
            }   
        }

        // Cursor icon
        Raylib.DrawText("< >", (int)cursorPos.X, (int)cursorPos.Y, 32, Color.Green);
    }

    private float linVelocityToHue(float linVelocity)
    {
        float maxLinVelocity = 0.75f;
        linVelocity = Math.Clamp(linVelocity, 0f, maxLinVelocity);
        float linToMaxRatio = linVelocity / maxLinVelocity;

        float hue = 200.0f - (200.0f * linToMaxRatio);
        return hue;
    }

    private Color linVelocityToColor(float linVelocity) 
    {
        float h = linVelocityToHue(linVelocity);
        float s = 0.9f;
        float v = 0.9f;

        // Convert HSV to RGB
        float c = v * s; // Chroma
        float x = c * (1 - Math.Abs((h / 60) % 2 - 1));
        float m = v - c;

        float rPrime, gPrime, bPrime;

        if (h < 60)
        {
            rPrime = c;
            gPrime = x;
            bPrime = 0;
        }
        else if (h < 120)
        {
            rPrime = x;
            gPrime = c;
            bPrime = 0;
        }
        else if (h < 180)
        {
            rPrime = 0;
            gPrime = c;
            bPrime = x;
        }
        else if (h < 240)
        {
            rPrime = 0;
            gPrime = x;
            bPrime = c;
        }
        else if (h < 300)
        {
            rPrime = x;
            gPrime = 0;
            bPrime = c;
        }
        else
        {
            rPrime = c;
            gPrime = 0;
            bPrime = x;
        }

        // Convert to RGB scale (0-255)
        int r = (int)((rPrime + m) * 255);
        int g = (int)((gPrime + m) * 255);
        int b = (int)((bPrime + m) * 255);

        Color color = new Color(r, g, b, 255);
        return color;
    }
}
