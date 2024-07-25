using System.Numerics;
using GameEngine.src.physics.collision.shape;

namespace GameEngine.src.physics.body;

#pragma warning disable CS8618 // Non nullable field must have non null value when exiting constructor.

public abstract class PhysicsBody2D : BaseObject
{
    public string Name;
    
    public Material2D Material { get; protected set; }
    public CollisionShape2D CollisionShape { get; protected set; }


    // Current collision state
    public bool IsOnFloor { get; internal set; }
    public bool IsOnCeiling { get; internal set; }
    public bool IsOnWallR { get; internal set; }
    public bool IsOnWallL { get; internal set; }

    public bool HandleCollision;
    public bool IsColliding { get; internal set; }

    // Motion attributes
    public Vector2 LinVelocity;
    public float RotVelocity;
    public float MomentOfInertia { get; protected set; }

    // Constructor
    public PhysicsBody2D(Vector2 position, float rotation, ShapeType shape, float width = 0, float height = 0, float radius=0) 
        : base(position, rotation)
    {       
        HandleCollision = true;

        CollisionShape = new CollisionShape2D(position, rotation, shape, width, height, radius);
    }

    internal void ResetCollisionState()
    {
        // Reset all collision-related properties to false 
        IsColliding = false;

        IsOnCeiling = false;
        IsOnFloor = false;
        IsOnWallL = false;
        IsOnWallR = false;
    }

    public override void Translate(Vector2 direction)
    {
        base.Translate(direction);
        CollisionShape.Translate(direction); 
    }

    public override void Rotate(float angle)
    {
        base.Rotate(angle);
        CollisionShape.Rotate(angle); 
    }

    // Methods to be overridden
    internal virtual void RunComponents(double delta) { }
    public virtual void ProjectileHit(PhysicsBody2D body) { }
    public virtual void ApplyForce(Vector2 amount) { }
}