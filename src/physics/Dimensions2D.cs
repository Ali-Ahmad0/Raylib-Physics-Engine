using System.Numerics;

namespace GameEngine.src.physics;
public class Dimensions2D
{

    // Shape dimensions for a body
    public readonly float Area;

    public readonly float Radius;
    public readonly float Height;
    public readonly float Width;

    // Constructor 
    public Dimensions2D() { }
    public Dimensions2D(float radius, float area)
    {
        Radius = radius;
        Area = area;
    }

    public Dimensions2D(Vector2 size, float area)
    {
        Width = size.X;
        Height = size.Y;
        Area = area;
    }
}
