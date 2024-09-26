using GameEngine.src.physics.component;
using GameEngine.src.physics.body;
using GameEngine.src.physics.collision.shape;
using Raylib_cs;
using System.Numerics;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

namespace GameEngine.src.world;

public abstract class World
{
    // Render the shape for a physics body
    protected static void DrawCollisionShapes(CollisionShape2D collision, Color color)
    {
        // Get world transform and shape
        Vector2 position = collision.Transform.Translation;
        float rotation = collision.Transform.Rotation;

        float width = collision.Dimensions.Width;
        float height = collision.Dimensions.Height;
        float radius = collision.Dimensions.Radius;
        Vector2 size = new Vector2(width, height);

        // Use the raylib draw methods to render the shape for an object

        switch (collision.Shape)
        {
            case ShapeType.Box:

                // Draw the rectangle
                Raylib.DrawRectanglePro(new Rectangle(position, size), new Vector2(width / 2, height / 2), rotation, color);
                break;

            case ShapeType.Circle:

                // Draw the circle
                Raylib.DrawCircleV(position, radius, color); ;
                break;

            default: throw new Exception("[ERROR]: Invalid ShapeType");
        }
    }

    protected static void HandlePhysics(List<PhysicsBody2D> bodies, double delta, Camera2D camera) => Physics.HandlePhysics(bodies, delta, camera);

    public virtual void Update(double delta) { }

}