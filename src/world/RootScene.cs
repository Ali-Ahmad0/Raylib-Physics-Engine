using Raylib_cs;
using System.Numerics;
using GameEngine.src.physics.body;

namespace GameEngine.src.world;
public abstract class RootScene
{
    protected static void DrawCollisionShapes(PhysicsBody2D body, Color color) => WorldCreation.DrawCollisionShapes(body, color);
    protected static void CreateRigidBody(Vector2 position, Vector2 scale, float density, float restitution,
        float radius, out RigidBody2D body2D) => WorldCreation.CreateRigidBody(position, scale, density, restitution, radius, out body2D);

    protected static void CreateRigidBody(Vector2 position, float rotation, Vector2 scale, float density, float restitution,
        float width, float height, out RigidBody2D body2D) => WorldCreation.CreateRigidBody(position, rotation, scale, density, restitution, width, height, out body2D);

    protected static void CreateStaticBody(Vector2 position, Vector2 scale, float restitution,
        float radius, out StaticBody2D body2D) => WorldCreation.CreateStaticBody(position, scale, restitution, radius, out body2D);

    protected static void CreateStaticBody(Vector2 position, float rotation, Vector2 scale, float restitution,
        float width, float height, out StaticBody2D body2D) => WorldCreation.CreateStaticBody(position, rotation, scale, restitution, width, height, out body2D);

    protected static void CreateProjectileBody(Vector2 position, Vector2 scale, float density, float restitution,
       float radius, Vector2 velocity, List<PhysicsBody2D> bodies, out RigidBody2D body2D) => WorldCreation.CreateProjectileBody(position, scale, density, restitution, radius, velocity, bodies, out body2D);

    protected static void CreatePlayerBody(Vector2 position, float rotation, float density,
        float width, float height, out PlayerBody2D body2D) => WorldCreation.CreatePlayerBody(position, rotation, density, width, height, out body2D);

    protected static void HandlePhysics(List<PhysicsBody2D> bodies, double delta, Camera2D camera) => WorldPhysics.HandlePhysics(bodies, delta, camera);

    public abstract void Update(double delta);

}
