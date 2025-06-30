using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements a simple point-cloud based gesture recognizer using resampling,
/// normalization (translation and scaling), and distance comparison.
/// </summary>
public class PointCloudRecognizer
{
    private const int NumResamplePoints = 256;

    /// <summary>
    /// Resamples the input points to a fixed number evenly spaced along the path.
    /// </summary>
    public List<Vector2> Resample(List<Vector2> points)
    {
        float pathLength = PathLength(points);
        float interval = pathLength / (NumResamplePoints - 1);
        float D = 0f;
        List<Vector2> newPoints = new() { points[0] };

        for (int i = 1; i < points.Count; i++)
        {
            float d = Vector2.Distance(points[i - 1], points[i]);
            if (D + d >= interval)
            {
                // Interpolate new point at interval distance
                float qx = points[i - 1].x + (interval - D) / d * (points[i].x - points[i - 1].x);
                float qy = points[i - 1].y + (interval - D) / d * (points[i].y - points[i - 1].y);
                Vector2 q = new(qx, qy);
                newPoints.Add(q);
                points.Insert(i, q);
                D = 0;
            }
            else
            {
                D += d;
            }
        }

        // Ensure last point is included if missing
        while (newPoints.Count < NumResamplePoints)
            newPoints.Add(points[points.Count - 1]);

        return newPoints;
    }

    /// <summary>
    /// Calculates the total length of the path defined by the sequence of points.
    /// </summary>
    private float PathLength(List<Vector2> points)
    {
        float length = 0;
        for (int i = 1; i < points.Count; i++)
            length += Vector2.Distance(points[i - 1], points[i]);
        return length;
    }

    /// <summary>
    /// Translates points so their centroid is at the origin (0,0).
    /// </summary>
    public List<Vector2> TranslateToOrigin(List<Vector2> points)
    {
        Vector2 centroid = Centroid(points);
        List<Vector2> newPoints = new();
        foreach (var p in points)
            newPoints.Add(p - centroid);
        return newPoints;
    }

    /// <summary>
    /// Computes the centroid (average position) of the points.
    /// </summary>
    private Vector2 Centroid(List<Vector2> points)
    {
        float x = 0, y = 0;
        foreach (var p in points)
        {
            x += p.x;
            y += p.y;
        }
        return new Vector2(x / points.Count, y / points.Count);
    }

    /// <summary>
    /// Scales points to fit inside a square of given size while maintaining aspect ratio.
    /// </summary>
    public List<Vector2> ScaleToSquare(List<Vector2> points, float size = 1f)
    {
        Rect bounds = BoundingBox(points);
        float maxDim = Mathf.Max(bounds.width, bounds.height);

        List<Vector2> newPoints = new();
        foreach (var p in points)
        {
            float scaledX = (p.x - bounds.x) / maxDim * size;
            float scaledY = (p.y - bounds.y) / maxDim * size;
            newPoints.Add(new Vector2(scaledX, scaledY));
        }
        return newPoints;
    }


    /// <summary>
    /// Calculates the bounding box enclosing all points.
    /// </summary>
    private Rect BoundingBox(List<Vector2> points)
    {
        float minX = float.MaxValue, maxX = float.MinValue;
        float minY = float.MaxValue, maxY = float.MinValue;

        foreach (var p in points)
        {
            if (p.x < minX) minX = p.x;
            if (p.x > maxX) maxX = p.x;
            if (p.y < minY) minY = p.y;
            if (p.y > maxY) maxY = p.y;
        }

        return new Rect(minX, minY, maxX - minX, maxY - minY);
    }

    /// <summary>
    /// Computes the sum of distances between corresponding points in two point clouds.
    /// </summary>
    public float CloudDistance(List<Vector2> points1, List<Vector2> points2)
    {
        if (points1.Count != points2.Count)
            throw new System.ArgumentException("Point clouds must be the same length.");

        float distance = 0;
        for (int i = 0; i < points1.Count; i++)
            distance += Vector2.Distance(points1[i], points2[i]);
        return distance;
    }

    /// <summary>
    /// Recognizes the input gesture by comparing normalized input points to templates,
    /// returning the best match if below a distance threshold, otherwise null.
    /// </summary>
    public Gesture Recognize(List<Vector2> inputPoints, List<Gesture> templates, float maxDistanceThreshold = 100.0f)
    {
        if (inputPoints == null || inputPoints.Count == 0 || templates == null || templates.Count == 0)
            return null;

        // Normalize input gesture
        var candidate = ScaleToSquare(TranslateToOrigin(Resample(new List<Vector2>(inputPoints))));

        float minDistance = float.MaxValue;
        Gesture bestMatch = null;

        // Compare against each template gesture
        foreach (var template in templates)
        {
            List<Vector2> templatePoints = new();
            foreach (var stroke in template.strokes)
                templatePoints.AddRange(stroke.points);

            var normalizedTemplate = ScaleToSquare(TranslateToOrigin(Resample(new List<Vector2>(templatePoints))));

            float dist = CloudDistance(candidate, normalizedTemplate);
            if (dist < minDistance)
            {
                minDistance = dist;
                bestMatch = template;
            }
            Debug.Log($"Matching {template.name} with distance: {dist}");
        }

        // Return best match if close enough
        
        if (minDistance > maxDistanceThreshold)
            return null;

        return bestMatch;
    }
}
