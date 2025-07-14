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

    public bool ContainsRightAngle(Stroke stroke, float angleTolerance = 5f)
    {
        int rightAngleCount = 0;

        Stroke simpleStroke = SimplifyStroke(stroke, 10f);

        if (simpleStroke.points.Count < 3)
        {
            Debug.Log("Not enough points to evaluate right angles.");
            return false;
        }

        for (int i = 1; i < simpleStroke.points.Count - 1; i++)
        {
            Vector2 A = simpleStroke.points[i - 1];
            Vector2 B = simpleStroke.points[i];
            Vector2 C = simpleStroke.points[i + 1];

            Vector2 AB = (A - B).normalized;
            Vector2 CB = (C - B).normalized;

            float dot = Vector2.Dot(AB, CB); // Cosine of the angle
            float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

            if (Mathf.Abs(angle - 90f) <= angleTolerance)
            {
                rightAngleCount++;
            }
        }

        // Check self-intersections
        var intersections = GetSelfIntersections(stroke);
        foreach (var (point, dir1, dir2) in intersections)
        {
            float angle = Mathf.Acos(Vector2.Dot(dir1, dir2)) * Mathf.Rad2Deg;

            if (Mathf.Abs(angle - 90f) <= angleTolerance)
            {
                rightAngleCount++;
            }
        }

        Debug.Log($"Right angles found: {rightAngleCount}");

        return rightAngleCount > 0;
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

    
    public List<(Vector2 point, Vector2 dir1, Vector2 dir2)> GetSelfIntersections(Stroke stroke)
    {
        var intersections = new List<(Vector2, Vector2, Vector2)>();
        var pts = stroke.points;

        for (int i = 0; i < pts.Count - 1; i++)
        {
            Vector2 p1 = pts[i];
            Vector2 p2 = pts[i + 1];

            for (int j = i + 2; j < pts.Count - 1; j++)
            {
                // Skip adjacent segments
                if (j == i + 1) continue;

                Vector2 p3 = pts[j];
                Vector2 p4 = pts[j + 1];

                if (LineSegmentsIntersect(p1, p2, p3, p4, out Vector2 intersection))
                {
                    Vector2 dir1 = (p2 - p1).normalized;
                    Vector2 dir2 = (p4 - p3).normalized;
                    intersections.Add((intersection, dir1, dir2));
                }
            }
        }

        return intersections;
    }

    private bool LineSegmentsIntersect(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, out Vector2 intersection)
    {
        intersection = Vector2.zero;

        float d = (p1.x - p2.x) * (p3.y - p4.y) - 
                (p1.y - p2.y) * (p3.x - p4.x);

        if (Mathf.Abs(d) < Mathf.Epsilon)
            return false; // Lines are parallel or coincident

        float pre = (p1.x * p2.y - p1.y * p2.x);
        float post = (p3.x * p4.y - p3.y * p4.x);
        float x = (pre * (p3.x - p4.x) - (p1.x - p2.x) * post) / d;
        float y = (pre * (p3.y - p4.y) - (p1.y - p2.y) * post) / d;
        intersection = new Vector2(x, y);

        // Check if the intersection is within both segments
        if (x < Mathf.Min(p1.x, p2.x) - Mathf.Epsilon || x > Mathf.Max(p1.x, p2.x) + Mathf.Epsilon ||
            x < Mathf.Min(p3.x, p4.x) - Mathf.Epsilon || x > Mathf.Max(p3.x, p4.x) + Mathf.Epsilon ||
            y < Mathf.Min(p1.y, p2.y) - Mathf.Epsilon || y > Mathf.Max(p1.y, p2.y) + Mathf.Epsilon ||
            y < Mathf.Min(p3.y, p4.y) - Mathf.Epsilon || y > Mathf.Max(p3.y, p4.y) + Mathf.Epsilon)
            return false;

        return true;
    }



}
