using GameEngine.src.helper;
using GameEngine.src.physics.collision;
using Raylib_cs;
using System.Numerics;

#pragma warning disable CS8618 // Non nullable field must have non null value when exiting constructor.

namespace GameEngine.src.physics.body;
public enum ShapeType
{
    Circle, Box
}

public abstract class PhysicsBody2D
{
    public string Name;
    public ShapeType Shape { get; protected set; }

    // Physical properties of the body
    public Transform2D Transform { get; protected set; }
    public Dimensions2D Dimensions { get ; protected set; }
    public Material2D Material { get; protected set; }

    // Vertices and Bounding Boxes
    protected Vector2[] Vertices;
    protected Vector2[] TransformedVertices;
    protected AxisAlignedBoundingBox AABB;

    protected bool VerticesUpdateRequired;
    protected bool AABBUpdateRequired;

    // Current collision state
    public bool IsOnFloor { get; internal set; }
    public bool IsOnCeiling { get; internal set; }
    public bool IsOnWallR { get; internal set; }
    public bool IsOnWallL { get; internal set; }

    public bool HandleCollision;
    public bool IsColliding { get; internal set; }

    public bool ApplyGravity;

    // Motion attributes
    public Vector2 LinVelocity;
    public float RotVelocity;
    public float MomentOfInertia { get; protected set; }

    // Constructor
    public PhysicsBody2D(Vector2 position, float rotation)
    {
        // Initialize physical properties
        Transform = new Transform2D(position, rotation);

        HandleCollision = true;
        ApplyGravity = true;

        VerticesUpdateRequired = true;
        AABBUpdateRequired = true;
    }

    // Calculate new position of vertices after transformation
    internal Vector2[] GetTransformedVertices()
    {
        // Return if no need to update vertices
        if (!VerticesUpdateRequired)
            return TransformedVertices;

        Vector2 position = Transform.Translation;
        float rotation = MathExtra.Deg2Rad(Transform.Rotation); 

        // Create a transformation matrix
        Matrix3x2 transformationMatrix = Matrix3x2.CreateRotation(rotation) *
                                         Matrix3x2.CreateTranslation(position);

        // Update transformed vertices using the transformation matrix
        for (int i = 0; i < Vertices.Length; i++)
            TransformedVertices[i] = Vector2.Transform(Vertices[i], transformationMatrix);

        VerticesUpdateRequired = false; // Mark vertices as updated
        return TransformedVertices;
    }

    // Calculate new AABB after transformation
    internal AxisAlignedBoundingBox GetAABB()
    {
        // Return if no need to update AABB
        if (!AABBUpdateRequired)
            return AABB;

        // Otherwise create new AABB based on shape
        switch (Shape)
        {
            case ShapeType.Box:
                // Get transformed vertices
                Vector2[] vertices = GetTransformedVertices();

                // Find min and max position of edges using vertices
                float minX = float.PositiveInfinity, minY = float.PositiveInfinity;
                float maxX = float.NegativeInfinity, maxY = float.NegativeInfinity;

                foreach (Vector2 vertex in vertices)
                {
                    minX = Math.Min(minX, vertex.X);
                    minY = Math.Min(minY, vertex.Y);
                    maxX = Math.Max(maxX, vertex.X);
                    maxY = Math.Max(maxY, vertex.Y);
                }

                AABB = new AxisAlignedBoundingBox(minX, minY, maxX, maxY);
                break;

            case ShapeType.Circle:

                // Calculate AABB based on circle radius
                float radius = Dimensions.Radius;
                AABB = new AxisAlignedBoundingBox(Transform.Translation.X - radius, Transform.Translation.Y - radius,
                                                   Transform.Translation.X + radius, Transform.Translation.Y + radius);
                break;

            default:
                throw new Exception("[ERROR]: Invalid ShapeType");
        }

        AABBUpdateRequired = false; // Mark AABB as updated
        return AABB;
    }

    // Map the vertices to a box shape
    protected void MapVerticesBox()
    {
        float width = Dimensions.Width;
        float height = Dimensions.Height;

        // Define vertices of the box shape
        Vertices = new Vector2[]
        {
        new Vector2(-width / 2f, height / 2f),   // Top-left
        new Vector2(width / 2f, height / 2f),    // Top-right
        new Vector2(width / 2f, -height / 2f),   // Bottom-right
        new Vector2(-width / 2f, -height / 2f)   // Bottom-left
        };

        // Initialize TransformedVertices array
        TransformedVertices = new Vector2[Vertices.Length];
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

    public void Translate(Vector2 direction)
    {
        Transform.Translate(direction);
        SetUpdateRequiredTrue(); 
    }

    // Rotate the physics body by the specified angle in radians
    public void Rotate(float angle)
    {
        Transform.Rotate(angle);
        SetUpdateRequiredTrue(); 
    }

    // Method to update vertices and AABB
    private void SetUpdateRequiredTrue()
    {
        VerticesUpdateRequired = true;
        AABBUpdateRequired = true;
    }

    // Methods to be overridden
    internal virtual void RunComponents(double delta) { }
    public virtual void ProjectileHit(PhysicsBody2D body) { }
    public virtual void ApplyForce(Vector2 amount) { }
}

