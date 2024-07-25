using GameEngine.src.helper;
using GameEngine.src.physics.body;
using GameEngine.src.physics.collision.shape;
using System.Numerics;

namespace GameEngine.src.physics.collision;

internal struct CollisionHelper
{
    internal static void ProjectCircle(Vector2 centerC, float radius, Vector2 axis, out float min, out float max)
    {
        /*
        Draw a diameter line perpendicular
        to the axis and then get the points
        on the circumference that the line
        intersects
         */
        Vector2 direction = Vector2.Normalize(axis) * radius;

        Vector2 pointA = centerC + direction;
        Vector2 pointB = centerC - direction;

        // Get perpendicular projection of both points onto axis using dot product
        min = Vector2.Dot(pointA, axis);
        max = Vector2.Dot(pointB, axis);

        if (min > max)
        {
            // Swap
            min = min + max;
            max = min - max;
            min = min - max;
        }

    }

    // Find vertex with minimum distance from circle center
    internal static int FindClosestPolygonVertex(Vector2 centerC, Vector2[] vertices)
    {
        int index = -1;
        float min = float.MaxValue; // minimum distance

        // Find minimum distance
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector2 vertex = vertices[i];
            float distance = Vector2.Distance(vertex, centerC);

            if (distance < min)
            {
                min = distance;
                index = i;
            }
        }

        // Return index of vertex with minimum distance
        return index;
    }

    // Project vertices on normal axis based on Seperating Axis Theorem
    internal static void ProjectVertices(Vector2[] vertices, Vector2 axis, out float min, out float max)
    {
        // Minimum and maximum projection
        min = float.PositiveInfinity;
        max = float.NegativeInfinity;

        // Get perpendicular projection of every vertex onto axis using dot product
        foreach (Vector2 vertex in vertices)
        {
            float projection = Vector2.Dot(vertex, axis);

            // Find the minimum and maximum projection for a polygon
            if (projection < min) { min = projection; }
            if (projection > max) { max = projection; }
        }
    }

    // Calculates squared distance between a point p and a line segment defined by two points a and b
    private static void PointSegmentDistance(Vector2 p, Vector2 a, Vector2 b, out float distanceSquared, out Vector2 contact)
    {
        // Vector from a to b
        Vector2 ab = b - a;
        // Vector from a to p
        Vector2 ap = p - a;

        // Projection of ap onto ab
        float projection = Vector2.Dot(ab, ap);

        // Normalized distance from a to the projection of p onto ab
        float d = projection / Vector2.DistanceSquared(ab, Vector2.Zero);

        // If the projection is before the start of the segment, the closest point is a
        if (d <= 0f)
            contact = a;

        // If the projection is beyond the end of the segment, the closest point is b
        else if (d >= 1f)
            contact = b;

        // Otherwise, calculate the closest point using the projection
        else
            contact = a + ab * d;

        // Squared distance between p and the closest point
        distanceSquared = Vector2.DistanceSquared(p, contact);
    }

    // This method finds contact points between two physics bodies
    public static void FindContactPoints(CollisionShape2D bodyA, CollisionShape2D bodyB, out Vector2 cpoint1, out Vector2 cpoint2, out int ccount)
    {
        cpoint1 = Vector2.Zero;
        cpoint2 = Vector2.Zero;
        ccount = 0;

        // Centers of the two bodies
        Vector2 centerA = bodyA.Transform.Translation;
        Vector2 centerB = bodyB.Transform.Translation;

        if (bodyA.Shape == bodyB.Shape)
        {
            // Circle - Circle
            if (bodyA.Shape == ShapeType.Circle)
            {
                float radius = bodyA.Dimensions.Radius;

                // Calculate the contact point on body A
                Vector2 direction = Vector2.Normalize(centerB - centerA);
                cpoint1 = centerA + direction * radius;

                ccount = 1; 
            }

            // Polygon - Polygon
            else 
            {
                // Vertices of both polygons
                Vector2[] verticesA = bodyA.GetTransformedVertices();
                Vector2[] verticesB = bodyB.GetTransformedVertices();

                float minDistanceSquared = float.MaxValue;

                // Loop through all vertices of body A
                for (int i = 0; i < verticesA.Length; i++)
                {
                    Vector2 point = verticesA[i];

                    // Loop through all edges of body B
                    for (int j = 0; j < verticesB.Length; j++)
                    {
                        Vector2 vertexA = verticesB[j];
                        Vector2 vertexB = verticesB[(j + 1) % verticesB.Length];

                        // Calculate the closest point and squared distance between point and edge
                        PointSegmentDistance(point, vertexA, vertexB, out float distanceSquared, out Vector2 cp);

                        // Update the closest contact point
                        if (distanceSquared < minDistanceSquared)
                        {
                            minDistanceSquared = distanceSquared;
                            cpoint1 = cp;
                            ccount = 1;
                        }
                        // Check if the distance is approximately equal to the minimum distance and update the second contact point
                        else if (MathF.Abs(distanceSquared - minDistanceSquared) < 0.0005f &&
                            !(Vector2.DistanceSquared(cp, cpoint1) < 0.0005f * 0.0005f))
                        {
                            cpoint2 = cp;
                            ccount = 2;
                        }
                    }
                }

                // Same process for body B vertices
                for (int i = 0; i < verticesB.Length; i++)
                {
                    Vector2 point = verticesB[i];

                    for (int j = 0; j < verticesA.Length; j++)
                    {
                        Vector2 vertexA = verticesA[j];
                        Vector2 vertexB = verticesA[(j + 1) % verticesA.Length];

                        PointSegmentDistance(point, vertexA, vertexB, out float distanceSquared, out Vector2 cp);

                        if (distanceSquared < minDistanceSquared)
                        {
                            minDistanceSquared = distanceSquared;
                            cpoint1 = cp;
                            ccount = 1;
                        }
                        else if (MathF.Abs(distanceSquared - minDistanceSquared) < 0.0005f &&
                            !(MathF.Abs(cp.X - cpoint1.X) < 0.0005f && MathF.Abs(cp.Y - cpoint1.Y) < 0.0005f))
                        {
                            cpoint2 = cp;
                            ccount = 2;
                        }
                    }
                }
            }
        }

        // Circle - Polygon
        else 
        {
            Vector2 centerC;
            Vector2[] vertices;

            if (bodyA.Shape == ShapeType.Circle)
            {
                centerC = centerA;
                vertices = bodyB.GetTransformedVertices();
            }
            else
            {
                centerC = centerB;
                vertices = bodyA.GetTransformedVertices();
            }

            float minDistanceSquared = float.MaxValue;

            // Loop through all edges of the polygon
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector2 vertexA = vertices[i];
                Vector2 vertexB = vertices[(i + 1) % vertices.Length];

                // Calculate the closest point on the edge to the circle's center
                PointSegmentDistance(centerC, vertexA, vertexB, out float distanceSquared, out Vector2 cp);

                // Update the closest contact point
                if (distanceSquared < minDistanceSquared)
                {
                    minDistanceSquared = distanceSquared;
                    cpoint1 = cp;
                    ccount = 1;
                }
            }
        }
    }

    // Helper method to check if AABB intersects with camera bounds
    internal static bool AABBExceedsBounds(AxisAlignedBoundingBox aabb, CameraBounds bounds)
    {
        return aabb.Max.X < bounds.Min.X ||
               aabb.Max.Y < bounds.Min.Y ||
               aabb.Min.X > bounds.Max.X ||
               aabb.Min.Y > bounds.Max.Y;
    }
}
