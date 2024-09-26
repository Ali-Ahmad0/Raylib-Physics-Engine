using GameEngine.src.helper;
using GameEngine.src.main;
using GameEngine.src.physics.body;
using GameEngine.src.world;
using Raylib_cs;
using System.Numerics;

namespace Game.res.scenes;

public class ProjectileTest : World
{
    private List<PhysicsBody2D> bodies;
    private List<Color> colors;
    private Vector2 spawnPosition;

    private Camera2D camera;

    internal ProjectileTest()
    {
        colors = new List<Color>() {
            Color.White,
            Color.Red,
            Color.Green,
            Color.Blue,
            Color.Gold
        };

        bodies = new List<PhysicsBody2D>();
        spawnPosition = new Vector2(Properties.ScreenWidth / 2, Properties.ScreenHeight / 2);

        Gamepad.AssignButton("x", GamepadButton.RightFaceDown);

        Raylib.HideCursor();

        // Create a camera centered at the middle of the screen
        camera = new Camera2D(Vector2.Zero, Vector2.Zero, 0, 1f);
    }

    public override void Update(double delta)
    {
        Raylib.BeginMode2D(camera);

        Draw();

        Raylib.EndMode2D();

        HandlePhysics(bodies, delta, camera);
    }

    private void Draw()
    {
        Vector2 cursorPos = Mouse.GetPos();
        Vector2 leftAxis = Gamepad.GetLeftAxis();

        if (leftAxis.LengthSquared() > 0.025)
        {
            cursorPos += leftAxis * 10;
        }

        Mouse.SetPos(cursorPos);

        Raylib.DrawText("Projectile Test", 20, 20, 32, Color.Green);

        if (Mouse.IsLMBPressed() || Gamepad.IsButtonPressed("x"))
        {
            Vector2 velocity = (Mouse.GetPos() - spawnPosition);
            velocity /= 128;

            // Create projectile
            //CreateProjectileBody(spawnPosition, Vector2.One, 1f, 0.5f, 16f, velocity * 0.2f, bodies, out RigidBody2D body);
            ProjectileBody2D projectile = new ProjectileBody2D(spawnPosition, Vector2.One, 16f, velocity * 0.2f, bodies);
            bodies.Add(projectile);

        }

        // Update and draw each body
        for (int i = 0; i < bodies.Count; i++)
        {
            DrawCollisionShapes(bodies[i].CollisionShape, colors[i % 5]);
        }

        Raylib.DrawText("< >", (int)cursorPos.X, (int)cursorPos.Y, 32, Color.Green);
    }



}