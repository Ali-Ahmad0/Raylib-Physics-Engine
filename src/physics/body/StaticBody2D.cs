using System.Numerics;
using GameEngine.src.physics.collision.shape;

namespace GameEngine.src.physics.body;


public class StaticBody2D : PhysicsBody2D
{
    // Constructor
    internal StaticBody2D(Vector2 position, float rotation, Vector2 scale, float restitution, ShapeType shape, 
        float width=0, float height=0, float radius=0) 
        
        : base(position, rotation, shape, width, height, radius)
    {
        float area;
        // Keep restitution in valid range
        restitution = Math.Clamp(restitution, 0.0f, 1.0f);

        switch (shape)
        {
            case ShapeType.Box:
                width *= scale.X; height *= scale.Y;

                // Calculate the area for the rigid body
                area = width * height;
                ValidateParameters(area);

                break;

            case ShapeType.Circle:
                radius *= scale.Length();

                // Calculate the area for the rigid body
                area = MathF.PI * radius * radius;
                ValidateParameters(area);

                break;
        }

        // Create the material for the body
        // Density = Mass, since Mass = Infinity
        Material = new Material2D(float.PositiveInfinity, float.PositiveInfinity, restitution);

        // Moment of Inertia = Infinity (for static bodies)
        MomentOfInertia = float.PositiveInfinity;
    }
}
