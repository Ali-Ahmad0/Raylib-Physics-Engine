using System.Numerics;

namespace GameEngine.src.physics.collision.shape;

// Create a box boundary around a shape
public class AxisAlignedBoundingBox
{
    // Min and Max edges of box
    internal readonly Vector2 Min;
    internal readonly Vector2 Max;

    // Constructors
    internal AxisAlignedBoundingBox() { }
    internal AxisAlignedBoundingBox(Vector2 min, Vector2 max)
    {
        Min = min;
        Max = max;
    }

    internal AxisAlignedBoundingBox(float minX, float minY, float maxX, float maxY)
    {
        Min = new Vector2(minX, minY);
        Max = new Vector2(maxX, maxY);
    }
}