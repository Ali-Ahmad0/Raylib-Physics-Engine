using System.Numerics;
using GameEngine.src.physics.body;
using GameEngine.src.physics.collision;

using GameEngine.src.main;

namespace GameEngine.src.world;

internal class WorldPhysics
{
    private static object lockOject = new object();
    private static HashSet<(int, int)> contactPairs = new HashSet<(int, int)>();

    internal static void HandlePhysics(List<PhysicsBody2D> bodies, double delta)
    {
        // Catch exception caused by projectiles
        try
        {
            for (int it = 0; it < 16; it++)
            {
                HandleCollisions(bodies);
                UpdateBodies(bodies, delta);
            }
        }

        catch (Exception)
        { }
    }

    private static void HandleCollisions(List<PhysicsBody2D> bodies)
    {
        contactPairs.Clear();
        CollisionBroadPhase(bodies);
        CollisionNarrowPhase(bodies);
    }

    // Check if 2 bodies may or may not be colliding
    private static void CollisionBroadPhase(List<PhysicsBody2D> bodies)
    {
        for (int i = 0; i < bodies.Count; i++)
        {
            PhysicsBody2D bodyA = bodies[i];

            for (int j = i + 1; j < bodies.Count; j++)
            {
                PhysicsBody2D bodyB = bodies[j];

                if (CollisionDetection.AABBIntersection(bodyA.GetAABB(), bodyB.GetAABB()))
                    contactPairs.Add((i, j));

                else
                {
                    bodyA.ResetCollisionState();
                    bodyB.ResetCollisionState();
                }

            }
        }
    }

    struct State { public List<PhysicsBody2D> bodies; public (int, int) pair; public State(List<PhysicsBody2D> bodies, (int, int) pair) { this.bodies = bodies; this.pair = pair; } }

    // Decide what do to after collision
    private static void ResolvePair(List<PhysicsBody2D> bodies, (int, int) pair)
    {
        PhysicsBody2D bodyA = bodies[pair.Item1];
        PhysicsBody2D bodyB = bodies[pair.Item2];

        Vector2 normal;
        float depth;

        if (CollisionDetection.CheckCollision(bodyA, bodyB, out normal, out depth))
        {
            CollisionHelper.FindContactPoints(bodyA, bodyB, out Vector2 contactP1, out Vector2 contactP2, out int contactCount);
            CollisionManifold contact = new CollisionManifold(bodyA, bodyB, normal, depth, contactP1, contactP2, contactCount);
            if (bodyA is PlayerBody2D || bodyB is PlayerBody2D)
               CollisionResolution.ResolveCollisionBasic(bodyA, bodyB, normal, depth);
            lock (lockOject)
            {
                CollisionResolution.ResolveCollisionAdvanced(in contact);
            }
            SeparateBodies(bodyA, bodyB, normal * depth);
            UpdateCollisionState(bodyA, bodyB, normal);
        }
    }

    // Check if 2 bodies are colliding
    private static void CollisionNarrowPhase(List<PhysicsBody2D> bodies)
    {
        List<Task> tasks = new List<Task>();
        // Sometimes a projectile body might be destroyed in the middle of the loop, so we need to handle the exception
        try
        {
            foreach ((int, int) pair in contactPairs)
            {
                if (Properties.EnableMT)
                    tasks.Add(Task.Factory.StartNew((Object state) => { ResolvePair(((State)state).bodies, ((State)state).pair); }, (Object)(new State(bodies, pair))));
                    //ThreadPool.QueueUserWorkItem(, (Object)(new State(bodies, pair)));
                else
                    ResolvePair(bodies, pair);
            }
            Task.WaitAll(tasks.ToArray());
            tasks.Clear();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
    // Seperate colliding bodies
    private static void SeparateBodies(PhysicsBody2D bodyA, PhysicsBody2D bodyB, Vector2 direction)
    {
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
    private static void UpdateBodies(List<PhysicsBody2D> bodies, double delta)
    {
        foreach (PhysicsBody2D body in bodies)
        {
            if (body is RigidBody2D)
                body.RunComponents(delta);
        }
    }
}


