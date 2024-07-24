using GameEngine.src.physics.body;
using System.Numerics;

namespace GameEngine.src.physics.component;
public class Gravity : Component
{
    public float G;

    public Gravity() { G = 9.81f; }

    public Gravity(float g) 
    {    
        G = g; 
    }

    public void RunComponent(RigidBody2D body, double delta)
    {
        // Move body downwards if it is midair
        ApplyGravity(body);
    }

    // Calculate and apply gravitational acceleration
    private void ApplyGravity(RigidBody2D body)
    {
        Vector2 gravity = new Vector2(0, G * body.Material.Mass);
        body.ApplyForce(gravity);
    }
}
