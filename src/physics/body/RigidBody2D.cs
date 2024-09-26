using System.Numerics;
using GameEngine.src.physics.component;
using GameEngine.src.physics.collision.shape;

namespace GameEngine.src.physics.body;

public class RigidBody2D : PhysicsBody2D
{
    // Force applied to the body
    public Vector2 Force { get; internal set; }
    public List<Component> components = new List<Component>() 
    {
        new Motion(), 
        new Gravity() 
    };

    // Constructor
    internal RigidBody2D(Vector2 position, float rotation, Vector2 scale, float density, 
        float restitution, ShapeType shape, float width=0, float height=0, float radius=0)
        
        : base(position, rotation, shape, width, height, radius)
    {
        // Keep restitution in valid range
        restitution = Math.Clamp(restitution, 0.0f, 1.0f);
        float area, mass;

        switch (shape)
        {
            case ShapeType.Box:
                width *= scale.X; height *= scale.Y;

                // Calculate the area for the rigid body
                area = width * height;
                ValidateParameters(area, density);

                // Calculate mass
                mass = (area * density) / 1000;

                MomentOfInertia = (1f / 12) * mass * (width * width + height * height);
                break;

            case ShapeType.Circle:
                radius *= scale.Length();

                // Calculate the area for the rigid body
                area = MathF.PI * radius * radius;
                ValidateParameters(area, density);

                // Calculate mass
                mass = (area * density) / 1000;

                MomentOfInertia = (1f / 2) * mass * (radius * radius);
                break;

            default:
                throw new ArgumentException("[ERROR]: Invalid ShapeType");
        }

        // Create the material for the body
        Material = new Material2D(mass, density, restitution);

        // Initialize velocity and force
        LinVelocity = Vector2.Zero;
        RotVelocity = 0f;
        Force = Vector2.Zero;


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