using System;
using System.Numerics;

namespace GameEngine.src.helper;

public static class MathExtra
{
    public static float Deg2Rad(float angle)
    {
        return angle * MathF.PI / 180f;
    }

    public static float Rad2Deg(float angle) 
    { 
        return angle * 180f / MathF.PI;
    }

    public static float MoveToward(float current, float target, float maxDelta)
    {
        if (MathF.Abs(target - current) <= maxDelta)
        {
            return target;
        }
        else
        {
            return current + MathF.Sign(target - current) * maxDelta;
        }
    }

    public static float Cross(Vector2 a, Vector2 b)
    {
        // cz = ax * by − ay * bx
        return a.X * b.Y - a.Y * b.X;
    }

}