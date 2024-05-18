using GameEngine.src.physics.component;
using GameEngine.src.physics.body;
using Raylib_cs;
using System.Numerics;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

namespace GameEngine.src.world;

internal struct Creation
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
    internal static void DrawCollisionShapes(PhysicsBody2D body, Color color)
    {
        // Get world transform and shape
        Vector2 position = body.Transform.Translation;
        float rotation = body.Transform.Rotation;

        float width = body.Dimensions.Width;
        float height = body.Dimensions.Height;
        float radius = body.Dimensions.Radius;
        Vector2 size = new Vector2(width, height);

        // Use the raylib draw methods to render the shape for an object

        switch (body.Shape)
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

    internal static void DrawBodySprite(PhysicsBody2D body, Texture2D sprite)
    {

    }

    // Creates a Circle RigidBody
    internal static void CreateRigidBody(Vector2 position, Vector2 scale, float density, float restitution,
        float radius, out RigidBody2D body2D)
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
        body2D = new RigidCircle2D(position, mass, density, area, restitution, radius, components);
    }

    // Creates a Box RigidBody
    internal static void CreateRigidBody(Vector2 position, float rotation, Vector2 scale, float density, float restitution,
    float width, float height, out RigidBody2D body2D)
    {

        body2D = null;

        width *= scale.X; height *= scale.Y;

        // Calculate the area for the rigid body
        float area = width * height;

        ValidateParameters(area, density);

        // For Any Object, Mass = Volume * Denisty
        // Where Volume = Area * Depth in 3D space
        // For 2D plane, we can assume depth to be 1
        // Convert mass into kg
        float mass = (area * density) / 1000;

        // Create a rigid body 
        body2D = new RigidBox2D(position, rotation, mass, density, area, restitution, width, height, components);

    }

    // Creates a Circle StaticBody
    internal static void CreateStaticBody(Vector2 position, Vector2 scale, float restitution,
    float radius, out StaticBody2D body2D)
    {
        body2D = null;

        radius *= scale.Length();

        // Calculate the area for the static body
        float area = MathF.PI * radius * radius;

        ValidateParameters(area);

        // Create a static body
        body2D = new StaticCircle2D(position, restitution, area, radius);

    }

    // Creates a Box StaticBody
    internal static void CreateStaticBody(Vector2 position, float rotation, Vector2 scale, float restitution,
    float width, float height, out StaticBody2D body2D)
    {
        body2D = null;

        width *= scale.X; height *= scale.Y;

        // Calculate the area for the static body
        float area = width * height;

        ValidateParameters(area);

        // Create a static body
        body2D = new StaticBox2D(position, rotation, area, restitution, width, height);
    }


    // Make a function to make a projectile which is a rigidbody with name as projectile
    public static void CreateProjectileBody(Vector2 position, Vector2 scale, float density, float restitution,
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
        body2D = new ProjectileBody2D(position, area, radius, components, velocity, bodies);
    }

    internal static void CreatePlayerBody(Vector2 position, float rotation, float density, 
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

}