using GameEngine.src.physics.component;
using GameEngine.src.physics.body;
using Raylib_cs;
using System.Numerics;
using GameEngine.src.physics;
using System.Net.Security;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

namespace GameEngine.src.world;

public abstract class WorldEditor
{
    // Constraints
    private static readonly float MIN_BODY_SIZE = 0.01f * 0.01f;
    private static readonly float MAX_BODY_SIZE = 2048f * 2048f;

    private static readonly float MIN_DENSITY = 0.10f;
    private static readonly float MAX_DENSITY = 22.5f;

    private static List<Component> components = new List<Component>
    {
        new Motion(),
        new Gravity()
    };

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
            case ShapeTypes.Box:

                // Draw the rectangle
                Raylib.DrawRectanglePro(new Rectangle(position, size), new Vector2(width / 2, height / 2), rotation, color);
                break;

            case ShapeTypes.Circle:

                // Draw the circle
                Raylib.DrawCircleV(position, radius, color); ;
                break;

            default: throw new Exception("[ERROR]: Invalid ShapeType");
        }
    }

    // Creates a Circle RigidBody
    protected static void CreateRigidBody(Vector2 position, float rotation, Vector2 scale, float density, 
        float restitution, ShapeTypes shape, out RigidBody2D body2D, float width=0, float height=0, float radius=0)
    {
        body2D = null;
        float area = 0; float mass = 0;

        switch (shape)
        {
            case ShapeTypes.Box:
                width *= scale.X; height *= scale.Y;

                // Calculate the area for the rigid body
                area = width * height;
                ValidateParameters(area, density);

                // Calculate mass
                mass = (area * density) / 1000;
                
                body2D = new RigidBody2D(
                    position, rotation, mass, density, restitution, shape, components, width: width, height: height
                );
                
                break;

            case ShapeTypes.Circle:
                radius *= scale.Length();

                // Calculate the area for the rigid body
                area = MathF.PI * radius * radius;
                ValidateParameters(area, density);
                
                // Calculate mass
                mass = (area * density) / 1000;

                // Create a rigid body 
                body2D = new RigidBody2D(
                    position, 0, mass, density, restitution, shape, components, radius: radius
                );

                break;
        }
    }


    // Creates a Circle StaticBody
    protected static void CreateStaticBody(Vector2 position, float rotation, Vector2 scale, float restitution,
        ShapeTypes shape, out StaticBody2D body2D, float width = 0, float height = 0, float radius = 0)
    {
        body2D = null;

        radius *= scale.Length();
        float area = 0;

        switch (shape)
        {
            case ShapeTypes.Box:
                width *= scale.X; height *= scale.Y;

                // Calculate the area for the rigid body
                area = width * height;
                ValidateParameters(area);

                body2D = new StaticBody2D(
                    position, rotation, restitution, shape, width: width, height: height
                );

                break;

            case ShapeTypes.Circle:
                radius *= scale.Length();

                // Calculate the area for the rigid body
                area = MathF.PI * radius * radius;
                ValidateParameters(area);

                // Create a rigid body 
                body2D = new StaticBody2D(
                    position, 0, restitution, shape, radius: radius
                );

                break;
        }
    }

    // Make a function to make a projectile which is a rigidbody with name as projectile
    protected static void CreateProjectileBody(Vector2 position, Vector2 scale, float density, float restitution,
       float radius, Vector2 velocity, List<PhysicsBody2D> bodies, out RigidBody2D body2D)
    {
        body2D = null;

        radius *= scale.Length();

        // Calculate the area for the rigid body
        float area = MathF.PI * radius * radius;

        ValidateParameters(area, density);

        // For Any Object, Mass = Volume * Denisty
        // Where Volume = Area * Depth in 3D space
        // For 2D plane, we can assume depth to be 1
        // Convert mass into kg
        float mass = (area * density) / 1000;

        // Create a rigid body 
        body2D = new ProjectileBody2D(position, radius, components, velocity, bodies);
    }

    protected static void CreatePlayerBody(Vector2 position, float rotation, float density, 
        float width, float height, out PlayerBody2D body2D)
    {
        body2D = null;

        float area = width * height;

        ValidateParameters(area, density);

        // Create a rigid body 
        body2D = new PlayerBody2D(position, rotation, width, height, components);
    }

    private static void ValidateParameters(float area, float density = 0)
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

    protected static void HandlePhysics(List<PhysicsBody2D> bodies, double delta, Camera2D camera) => WorldPhysics.HandlePhysics(bodies, delta, camera);

    public virtual void Update(double delta) { }

}