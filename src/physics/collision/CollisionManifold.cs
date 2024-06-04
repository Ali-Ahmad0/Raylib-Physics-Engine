using GameEngine.src.physics.body;
using System.Numerics;

namespace GameEngine.src.physics.collision;

// Information about colliding bodies
internal readonly struct CollisionManifold
{
    // 2 colliding bodies
    internal readonly PhysicsBody2D BodyA;
    internal readonly PhysicsBody2D BodyB;

    // Collision normal and depth
    internal readonly Vector2 Normal;
    internal readonly float Depth;

    // Contact points on collision
    internal readonly Vector2 ContactP1;
    internal readonly Vector2 ContactP2;

    // Number of contact points (Max 2)
    internal readonly int ContactCount;

    // Constructor
    internal CollisionManifold(PhysicsBody2D bodyA, PhysicsBody2D bodyB, Vector2 normal, float depth, 
        Vector2 contactP1, Vector2 contactP2, int contactCount)
    {
        BodyA = bodyA;
        BodyB = bodyB;

        Normal = normal;
        Depth = depth;

        ContactP1 = contactP1;
        ContactP2 = contactP2;

        ContactCount = Math.Clamp(contactCount, 0, 2); ;
    }
}