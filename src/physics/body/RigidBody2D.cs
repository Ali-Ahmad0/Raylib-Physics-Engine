using System.Numerics;
using GameEngine.src.physics.component;

namespace GameEngine.src.physics.body;

public class RigidBody2D : PhysicsBody2D
{
    // Force applied to the body
    public Vector2 Force { get; internal set; }
    public List<Component> components = new List<Component>() { new Motion(), new Gravity() };

    // Constructor
    internal RigidBody2D(Vector2 position, float rotation, float mass, float density, 
        float restitution, ShapeTypes shape, float width=0, float height=0, float radius=0)
        
        : base(position, rotation, shape, width, height, radius)
    {
        // Keep restitution in valid range
        restitution = Math.Clamp(restitution, 0.0f, 1.0f);

        // Create the material for the body
        Material = new Material2D(mass, density, restitution);

        // Initialize velocity and force
        LinVelocity = Vector2.Zero;
        RotVelocity = 0f;
        Force = Vector2.Zero;

        switch (shape)
        {
            case ShapeTypes.Box:
                MomentOfInertia = (1f / 12) * mass * (width * width + height * height);              
                break;

            case ShapeTypes.Circle:
                MomentOfInertia = (1f / 2) * mass * (radius * radius);
                break;

            default:
                throw new ArgumentException("[ERROR]: Invalid ShapeType");
        }
    }

    // Apply a force to the physics body
    public override void ApplyForce(Vector2 amount)
    {
        Force = amount;
    }

    // Run all components attached to the physics body in parallel
    internal override void RunComponents(double delta)
    {
        foreach (Component component in components)
        {
            component.RunComponent(this, delta);
        }
    }
}