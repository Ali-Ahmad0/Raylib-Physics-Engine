using System.Numerics;
using GameEngine.src.physics.body;
using GameEngine.src.physics.collision;
using GameEngine.src.main;
using Raylib_cs;
using GameEngine.src.helper;
using System.Runtime.InteropServices;

namespace GameEngine.src.world;

internal struct WorldPhysics
{
    private static object lockOject = new object();
    private static HashSet<(int, int)> contactPairs = new HashSet<(int, int)>();

    internal static void HandlePhysics(List<PhysicsBody2D> bodies, double delta, Camera2D camera)
    {
        CameraBounds bounds = new CameraBounds();
        bounds.Calculate(camera);

        delta /= 256;

        // Catch exception caused by projectiles
        try
        {
            for (int it = 0; it < 16; it++)
            {
                HandleCollisions(bodies, bounds);
                UpdateBodies(bodies, delta, bounds);
            }
        }

        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private static void HandleCollisions(List<PhysicsBody2D> bodies, CameraBounds bounds)
    {
        contactPairs.Clear();
        CollisionBroadPhase(bodies, bounds);
        CollisionNarrowPhase(bodies, bounds);
    }

    // Check if 2 bodies may or may not be colliding
    private static void CollisionBroadPhase(List<PhysicsBody2D> bodies, CameraBounds bounds)
    {

        for (int i = 0; i < bodies.Count; i++)
        {
            PhysicsBody2D bodyA = bodies[i];

            for (int j = i + 1; j < bodies.Count; j++)
            {
                PhysicsBody2D bodyB = bodies[j];

                // Check if either body exceeds the camera bounds
                if (CollisionHelper.AABBExceedsBounds(bodyA.CollisionShape.GetAABB(), bounds) ||
                    CollisionHelper.AABBExceedsBounds(bodyB.CollisionShape.GetAABB(), bounds))
                {
                    continue;
                }

                if (CollisionDetection.AABBIntersection(bodyA.CollisionShape.GetAABB(), bodyB.CollisionShape.GetAABB()))
                    contactPairs.Add((i, j));

                else
                {
                    bodyA.ResetCollisionState();
                    bodyB.ResetCollisionState();
                }

            }
        }
    }


    // Check if 2 bodies are colliding
    private static void CollisionNarrowPhase(List<PhysicsBody2D> bodies, CameraBounds bounds)
    {
        List<Task> tasks = new List<Task>();
        // Sometimes a projectile body might be destroyed in the middle of the loop, so we need to handle the exception
      
        foreach ((int, int) pair in contactPairs)
        { 
            ResolvePair(bodies, pair, bounds); 
        }

        Task.WaitAll(tasks.ToArray());    
        tasks.Clear();

    }

    // Decide what do to after collision
    private static void ResolvePair(List<PhysicsBody2D> bodies, (int, int) pair, CameraBounds bounds)
    {
        PhysicsBody2D bodyA = bodies[pair.Item1];
        PhysicsBody2D bodyB = bodies[pair.Item2];

        Vector2 normal;
        float depth;

        if (CollisionDetection.CheckCollision(bodyA.CollisionShape, bodyB.CollisionShape, out normal, out depth))
        {
            CollisionHelper.FindContactPoints(bodyA.CollisionShape, bodyB.CollisionShape, out Vector2 contactP1, out Vector2 contactP2, out int contactCount);
            CollisionManifold contact = new CollisionManifold(bodyA, bodyB, normal, depth, contactP1, contactP2, contactCount);

            if (bodyA is CharacterBody2D || bodyB is CharacterBody2D)
                CollisionResolution.ResolveCollisionBasic(bodyA, bodyB, normal, depth);

            bodyA.IsColliding = true;
            bodyB.IsColliding = true;

            lock (lockOject)
            {
                if (bodyA.HandleCollision && bodyB.HandleCollision)
                    CollisionResolution.ResolveCollisionAdvanced(in contact);
            }

            SeparateBodies(bodyA, bodyB, normal * depth, bounds);
            UpdateCollisionState(bodyA, bodyB, normal);
        }
    }

    // Seperate colliding bodies
    private static void SeparateBodies(PhysicsBody2D bodyA, PhysicsBody2D bodyB, Vector2 direction, CameraBounds bounds)
    {
        // Check if either body exceeds the camera bounds
        if (CollisionHelper.AABBExceedsBounds(bodyA.CollisionShape.GetAABB(), bounds) ||
            CollisionHelper.AABBExceedsBounds(bodyB.CollisionShape.GetAABB(), bounds))
            return;

        if (!(bodyA.HandleCollision && bodyB.HandleCollision))
            return;

        if (bodyA is ProjectileBody2D || bodyB is ProjectileBody2D)
        {
            return;
        }

        if (bodyA is StaticBody2D)
            bodyB.Translate(direction);

        else if (bodyB is StaticBody2D)
            bodyA.Translate(-direction);

        else
        {
            bodyA.Translate(-direction / 2f);
            bodyB.Translate(direction / 2f);
        }
    }

    // Set collision states for bodies
    private static void UpdateCollisionState(PhysicsBody2D bodyA, PhysicsBody2D bodyB, Vector2 normal)
    {
        bodyA.IsOnCeiling = normal.Y < -0.5f;
        bodyA.IsOnFloor = normal.Y > 0.5f;

        bodyB.IsOnCeiling = bodyA.IsOnFloor;
        bodyB.IsOnFloor = bodyA.IsOnCeiling;

        bodyA.IsOnWallL = normal.X < -0.5f;
        bodyA.IsOnWallR = normal.X > 0.5f;

        bodyB.IsOnWallL = bodyA.IsOnWallR;
        bodyB.IsOnWallR = bodyA.IsOnWallL;
    }

    // Run body components
    private static void UpdateBodies(List<PhysicsBody2D> bodies, double delta, CameraBounds bounds)
    {
        foreach (PhysicsBody2D body in bodies)
        {
            if (CollisionHelper.AABBExceedsBounds(body.CollisionShape.GetAABB(), bounds))
                continue;

            if (body is RigidBody2D)
                
                body.RunComponents(delta);
        }
    }
}


