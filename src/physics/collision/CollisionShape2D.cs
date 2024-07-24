﻿using GameEngine.src.helper;
using GameEngine.src.physics.collision;
using System.Numerics;

namespace GameEngine.src.physics;

#pragma warning disable CS8618 // Non nullable field must have non null value when exiting constructor.

public enum ShapeTypes
{
    Circle, Box
}

public class CollisionShape2D : BaseObject
{
    public ShapeTypes Shape;

    public Dimensions2D Dimensions;

    protected Vector2[] Vertices;
    protected Vector2[] TransformedVertices;
    protected AxisAlignedBoundingBox AABB;

    protected bool VerticesUpdateRequired;
    protected bool AABBUpdateRequired;

    public CollisionShape2D(Vector2 position, float rotation, ShapeTypes shape, float width=0, float height=0, float radius=0) 
        : base(position, rotation)
    {
        VerticesUpdateRequired = true;
        AABBUpdateRequired = true;

        switch (shape)
        {
            case ShapeTypes.Box:
                Dimensions = new Dimensions2D(new Vector2(width, height));
                MapVerticesBox();
                break;

            case ShapeTypes.Circle:
                Dimensions = new Dimensions2D(radius);
                break;

            default:
                throw new ArgumentException("[ERROR]: Invalid ShapeType");
        }

        Shape = shape;
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
            case ShapeTypes.Box:
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

            case ShapeTypes.Circle:

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

    public override void Translate(Vector2 direction) 
    {
        base.Translate(direction);
        SetUpdateRequiredTrue();
    }

    public override void Rotate(float angle)
    {
        base.Rotate(angle);
        SetUpdateRequiredTrue();
    }

    // Method to update vertices and AABB
    private void SetUpdateRequiredTrue()
    {
        VerticesUpdateRequired = true;
        AABBUpdateRequired = true;
    }
}