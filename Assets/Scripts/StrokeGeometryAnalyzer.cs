using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Analyze the geometric properties of a single stroke.
/// </summary>
[System.Serializable]
public class StrokeGeometryAnalyzer
{
    // Returns true if the stroke contains a straight line.
    public bool ContainsLine(Stroke stroke, int windowSize = 40, float maxAngleDeviation = 3.0f)
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
    public bool IsArc(Stroke stroke)
    {
        // TODO: Implement circular arc detection
        return false;
    }

    // Returns true if the stroke forms a closed shape.
    public bool IsClosed(Stroke stroke)
    {
        // TODO: Check if start and end points are close enough
        return false;
    }

    // Returns true if the stroke exhibits symmetry across any axis.
    public bool IsSymmetrical(Stroke stroke)
    {
        // TODO: Reflect and compare stroke halves
        return false;
    }

    // Returns true if the stroke has one or more sharp corners.
    public bool HasCorners(Stroke stroke)
    {
        // TODO: Implement corner/inflection point detection
        return false;
    }


    // Returns true if the stroke loops or intersects with itself.

    public bool HasSelfIntersection(Stroke stroke)
    {
        // TODO: Check for self-intersections
        return false;
    }

    // Returns the axis-aligned bounding box aspect ratio.
    public float GetAspectRatio(Stroke stroke)
    {
        // TODO: Calculate bounding box and return width / height
        return 1f;
    }

    // Returns the overall direction of the stroke in degrees (0–360).
    public float GetAverageDirection(Stroke stroke)
    {
        // TODO: Compute net vector and angle
        return 0f;
    }

    // Returns the signed area enclosed by the stroke (0 if not closed).

    public float GetEnclosedArea(Stroke stroke)
    {
        // TODO: Use shoelace formula if stroke is closed
        return 0f;
    }

    // Returns true if the stroke has a wave or zig-zag pattern.
    public bool IsWavy(Stroke stroke)
    {
        // TODO: Analyze curvature oscillation
        return false;
    }

    // --- RDP Simplification Helper ---

    /// <summary>
    /// Simplifies a stroke using the Ramer–Douglas–Peucker algorithm.
    /// </summary>
    /// <param name="stroke">The input stroke to simplify.</param>
    /// <param name="epsilon">Tolerance: higher = more aggressive simplification.</param>
    /// <returns>A new simplified Stroke.</returns>
    public Stroke SimplifyStroke(Stroke stroke, float epsilon)
    {
        if (stroke == null || stroke.points == null || stroke.points.Count < 3)
            return stroke;

        List<Vector2> simplifiedPoints = RamerDouglasPeucker(stroke.points, epsilon);
        return new Stroke { points = simplifiedPoints };
    }

    private List<Vector2> RamerDouglasPeucker(List<Vector2> points, float epsilon)
    {
        int startIndex = 0;
        int endIndex = points.Count - 1;

        float maxDistance = 0f;
        int indexFarthest = 0;

        for (int i = startIndex + 1; i < endIndex; i++)
        {
            float distance = PerpendicularDistance(points[i], points[startIndex], points[endIndex]);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                indexFarthest = i;
            }
        }

        if (maxDistance > epsilon)
        {
            var left = RamerDouglasPeucker(points.GetRange(startIndex, indexFarthest - startIndex + 1), epsilon);
            var right = RamerDouglasPeucker(points.GetRange(indexFarthest, endIndex - indexFarthest + 1), epsilon);

            left.RemoveAt(left.Count - 1); // Avoid duplicate
            left.AddRange(right);
            return left;
        }
        else
        {
            return new List<Vector2> { points[startIndex], points[endIndex] };
        }
    }

    private float PerpendicularDistance(Vector2 point, Vector2 lineStart, Vector2 lineEnd)
    {
        if (lineStart == lineEnd)
            return Vector2.Distance(point, lineStart);

        Vector2 dir = lineEnd - lineStart;
        Vector2 projected = lineStart + Vector2.Dot(point - lineStart, dir) / dir.sqrMagnitude * dir;
        return Vector2.Distance(point, projected);
    }

}
