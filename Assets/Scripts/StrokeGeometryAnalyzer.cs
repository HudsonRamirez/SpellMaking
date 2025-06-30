using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Analyze the geometric properties of a single stroke.
/// </summary>
public class StrokeGeometryAnalyzer: MonoBehaviour
{
    // Returns true if the stroke contains a straight line.
    public static bool ContainsLine(Stroke stroke, int windowSize = 20, float maxAngleDeviation = 10f)
    {
        List<Vector2> points = stroke.points;
        if (points == null || points.Count < windowSize)
            return false;

        float maxAngleRad = maxAngleDeviation * Mathf.Deg2Rad;

        for (int i = 0; i <= points.Count - windowSize; i++)
        {
            Vector2 start = points[i];
            Vector2 end = points[i + windowSize - 1];
            Vector2 segmentDir = (end - start).normalized;

            bool isStraight = true;
            for (int j = i + 1; j < i + windowSize - 1; j++)
            {
                Vector2 currentDir = (points[j] - start).normalized;
                float angle = Vector2.Angle(segmentDir, currentDir) * Mathf.Deg2Rad;

                if (angle > maxAngleRad)
                {
                    isStraight = false;
                    break;
                }
            }

            if (isStraight)
                return true;
        }

        return false;
    }

    // Returns true if the stroke approximates a circular or elliptical arc.
    public static bool IsArc(Stroke stroke)
    {
        // TODO: Implement circular arc detection
        return false;
    }

    // Returns true if the stroke forms a closed shape.
    public static bool IsClosed(Stroke stroke)
    {
        // TODO: Check if start and end points are close enough
        return false;
    }

    // Returns true if the stroke exhibits symmetry across any axis.
    public static bool IsSymmetrical(Stroke stroke)
    {
        // TODO: Reflect and compare stroke halves
        return false;
    }

    // Returns true if the stroke has one or more sharp corners.
    public static bool HasCorners(Stroke stroke)
    {
        // TODO: Implement corner/inflection point detection
        return false;
    }


    // Returns true if the stroke loops or intersects with itself.

    public static bool HasSelfIntersection(Stroke stroke)
    {
        // TODO: Check for self-intersections
        return false;
    }

    // Returns the axis-aligned bounding box aspect ratio.
    public static float GetAspectRatio(Stroke stroke)
    {
        // TODO: Calculate bounding box and return width / height
        return 1f;
    }

    // Returns the overall direction of the stroke in degrees (0–360).
    public static float GetAverageDirection(Stroke stroke)
    {
        // TODO: Compute net vector and angle
        return 0f;
    }

    // Returns the signed area enclosed by the stroke (0 if not closed).

    public static float GetEnclosedArea(Stroke stroke)
    {
        // TODO: Use shoelace formula if stroke is closed
        return 0f;
    }

    // Returns true if the stroke has a wave or zig-zag pattern.
    public static bool IsWavy(Stroke stroke)
    {
        // TODO: Analyze curvature oscillation
        return false;
    }
}
