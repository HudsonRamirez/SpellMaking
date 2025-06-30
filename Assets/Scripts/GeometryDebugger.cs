using UnityEngine;

public class GeometryDebugger : MonoBehaviour
{

    public DrawSurface drawSurface;
    private StrokeGeometryAnalyzer analyzer = new();

    public GameObject pointMarkerPrefab;
    public float simplifyEpsilon = 10f;

    public void OnAnalyzeButtonClicked()
    {
        foreach (Stroke stroke in drawSurface.committedStrokes)
        {
            if (analyzer.ContainsLine(stroke))
            {
                Debug.Log("Stroke contains straight segment");
            }
            else
            {
                Debug.Log("Stroke does not contain straight segment");
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
