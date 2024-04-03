﻿using PhysicsEngine.src.physics._2D.body;
using Raylib_cs;

namespace PhysicsEngine.src.components;
public class Gravity : Component
{  
    public override void RunComponent(RigidBody2D body)
    {
        // Move body downwards if it is midair
        if (!body.IsOnFloor) { ApplyGravity(body); }
    }

    // Calculate and apply gravitational acceleration
    private void ApplyGravity(RigidBody2D body)
    {
        body.LinVelocity.Y += 9.81f * Raylib.GetFrameTime();
    }
}
