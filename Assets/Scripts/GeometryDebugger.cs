using System.Collections.Generic;
// using System.Numerics;
using UnityEngine;

public class GeometryDebugger : MonoBehaviour
{

    public DrawSurface drawSurface;
    private StrokeGeometryAnalyzer analyzer = new();

    public GameObject pointMarkerPrefab;
    public float simplifyEpsilon = 10f;

    public void OnAnalyzeButtonClicked()
    {
        // Evaluate each stroke
        foreach (Stroke stroke in drawSurface.committedStrokes)
        {
            // Straight line segment check
            if (analyzer.ContainsLine(stroke))
            {
                Debug.Log("Stroke contains straight segment");
            }
            else
            {
                Debug.Log("Stroke does not contain straight segment");
            }

            // Right angle check
            analyzer.ContainsRightAngle(stroke);

            // Check for self intersections and output to debug how many there are
            List<(Vector2 point, Vector2 dir1, Vector2 dir2)> strokeSelfIntersections = analyzer.GetSelfIntersections(stroke);
            int selfIntersectionCount = strokeSelfIntersections.Count;
            if (selfIntersectionCount > 0)
            {
                Debug.Log($"Stroke self intersects {selfIntersectionCount} times.");

                foreach ((Vector2 point, Vector2 dir1, Vector2 dir2) selfIntersection in strokeSelfIntersections)
                {
                    CreatePointMarker(selfIntersection.point);
                }
            }
        }
    }

    public void OnSimplifyAndVisualizeClicked()
    {
        drawSurface.ClearCanvas();

        foreach (Stroke stroke in drawSurface.committedStrokes)
        {
            // Simplify stroke
            Stroke simplified = analyzer.SimplifyStroke(stroke, simplifyEpsilon);

            // Draw the simplified stroke (e.g., green)
            drawSurface.DrawStroke(simplified, drawSurface.committedLineMaterial, Color.green);

            // Visualize each point
            foreach (Vector2 point in simplified.points)
            {
                CreatePointMarker(point);
            }
        }
    }

    // Creates a visual marker (e.g., small sphere) at a local-space point
    private void CreatePointMarker(Vector2 localPos)
    {
        if (pointMarkerPrefab == null)
        {
            Debug.LogWarning("No point marker prefab assigned.");
            return;
        }

        GameObject marker = Instantiate(pointMarkerPrefab, drawSurface.transform);
        marker.transform.localPosition = new Vector3(localPos.x, localPos.y, -0.4f); // Slight Z offset
        marker.transform.localScale = Vector3.one * 10f; // Adjust as needed
    }

}
