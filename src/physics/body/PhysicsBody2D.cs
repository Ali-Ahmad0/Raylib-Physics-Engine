using System.Numerics;
using GameEngine.src.physics.collision.shape;

namespace GameEngine.src.physics.body;

#pragma warning disable CS8618 // Non nullable field must have non null value when exiting constructor.

public abstract class PhysicsBody2D : BaseObject
{
    // Constraints
    protected static readonly float MIN_BODY_SIZE = 0.01f * 0.01f;
    protected static readonly float MAX_BODY_SIZE = 2048f * 2048f;

    protected static readonly float MIN_DENSITY = 0.10f;
    protected static readonly float MAX_DENSITY = 22.5f;

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

    protected static void ValidateParameters(float area, float density = 0)
    {
        string errorMessage = string.Empty;

        if (area < MIN_BODY_SIZE || area > MAX_BODY_SIZE)
            errorMessage = area < MIN_BODY_SIZE ? $"[ERROR]: Body area is too small, Minimum Area: {MIN_BODY_SIZE}" :
                           $"[ERROR]: Body area is too large, Maximum Area: {MAX_BODY_SIZE}";

        if (density != 0 && (density < MIN_DENSITY || density > MAX_DENSITY))
            errorMessage += errorMessage != string.Empty ? Environment.NewLine : string.Empty +
                            (density < MIN_DENSITY ? $"[ERROR]: Body density is too low, Minimum Density: {MIN_DENSITY}" :
                                                     $"[ERROR]: Body density is too high, Maximum Density: {MAX_DENSITY}");

        // Throw exception with error message
        if (!string.IsNullOrEmpty(errorMessage))
            throw new Exception(errorMessage);

        else return;
    }

    // Methods to be overridden
    internal virtual void RunComponents(double delta) { }
    public virtual void ProjectileHit(PhysicsBody2D body) { }
    public virtual void ApplyForce(Vector2 amount) { }
}