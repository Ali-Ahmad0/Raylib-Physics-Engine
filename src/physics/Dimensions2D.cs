using System.Numerics;

namespace GameEngine.src.physics;
public class Dimensions2D
{

    // Shape dimensions for a body
    public readonly float Area;

    public float Radius;
    public float Height;
    public float Width;

    // Constructor 
    public Dimensions2D() { }
    public Dimensions2D(float radius)
    {
        Radius = radius;
        Area = radius * radius;
    }

    public Dimensions2D(Vector2 size)
    {
        Width = size.X;
        Height = size.Y;

        Area = size.X * size.Y;
    }
}
