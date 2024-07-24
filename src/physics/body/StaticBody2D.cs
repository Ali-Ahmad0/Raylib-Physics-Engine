using System.Numerics;

namespace GameEngine.src.physics.body;

public class StaticBody2D : PhysicsBody2D
{
    // Constructor
    internal StaticBody2D(Vector2 position, float rotation, float restitution, ShapeTypes shape, 
        float width=0, float height=0, float radius=0) 
        
        : base(position, rotation, shape, width, height, radius)
    {
        // Keep restitution in valid range
        restitution = Math.Clamp(restitution, 0.0f, 1.0f);

        // Create the material for the body
        // Density = Mass, since Mass = Infinity
        Material = new Material2D(float.PositiveInfinity, float.PositiveInfinity, restitution);

        // Moment of Inertia = Infinity (for static bodies)
        MomentOfInertia = float.PositiveInfinity;
    }
}
