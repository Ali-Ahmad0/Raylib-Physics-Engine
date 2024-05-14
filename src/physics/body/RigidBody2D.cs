using System.Numerics;
using GameEngine.src.physics.component;

namespace GameEngine.src.physics.body;

public class RigidBody2D : PhysicsBody2D
{
    // Force applied to the body
    public Vector2 Force { get; internal set; }
    protected List<Component> components = new List<Component>();

    // Constructor
    internal RigidBody2D(Vector2 position, float rotation, float mass, float density,
        float restitution, ShapeType shape, List<Component> components) : base(position, rotation)
    {
        // Keep restitution in valid range
        restitution = Math.Clamp(restitution, 0.0f, 1.0f);

        // Create the material for the body
        Material = new Material2D(mass, density, restitution);
        Shape = shape;

        // Initialize velocity and force
        LinVelocity = Vector2.Zero;
        RotVelocity = 0f;
        Force = Vector2.Zero;

        // Get components list 
        this.components = components;
    }

    // Apply a force to the physics body
    public override void ApplyForce(Vector2 amount)
    {
        Force = amount;
    }

    // Run all components attached to the physics body in parallel
    internal override void RunComponents(double delta)
    {
        components[0].RunComponent(this, delta);

        if (ApplyGravity)
            components[1].RunComponent(this, delta);
    }
}

public class RigidBox2D : RigidBody2D
{
    // Constructor
    internal RigidBox2D(Vector2 position, float rotation, float mass, float density, float area, float restitution,
        float width, float height, List<Component> components) : base(position, rotation, mass, density, restitution, ShapeType.Box, components)
    {
        // Initialize dimensions and vertices
        Dimensions = new Dimensions2D(new Vector2(width, height), area);
        MapVerticesBox();

        // I = m/12 * (w^2 + h^2)
        MomentOfInertia = (1f / 12) * mass * (width * width + height * height);

    }
}

public class RigidCircle2D : RigidBody2D
{
    // Constructor
    internal RigidCircle2D(Vector2 position, float mass, float density, float area, float restitution,
        float radius, List<Component> components) : base(position, 0f, mass, density, restitution, ShapeType.Circle, components)
    {
        // Initialize dimensions
        Dimensions = new Dimensions2D(radius, area);

        // I = m/2 * r^2
        MomentOfInertia = (1f / 2) * mass * (radius * radius);
    }

}