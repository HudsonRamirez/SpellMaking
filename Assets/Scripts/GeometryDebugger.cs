using UnityEngine;

public class GeometryDebugger : MonoBehaviour
{

    public DrawSurface drawSurface;
    private StrokeGeometryAnalyzer analyzer = new();

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

}
