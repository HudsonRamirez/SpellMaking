using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

/// <summary>
/// Handles freehand drawing input on a UI panel. Stores multiple strokes,
/// each made of 2D points, and renders them using LineRenderer.
/// </summary>
public class DrawSurface : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private RectTransform drawArea;                 // The panel to draw on (should have RectTransform)
    private Stroke currentStroke;                   // Current stroke being drawn
    public List<Stroke> completedStrokes = new();   // List of all strokes for this drawing
    public List<Stroke> committedStrokes = new();   // List of strokes committed to the canvas
    private LineRenderer currentLine;               // LineRenderer for the active stroke
    public Material lineMaterial;                   // Material for drawing lines
    public Material committedLineMaterial;          // Material for rendering comitted lines
    public float lineWidth = 5f;                    // Width of stroke lines

    private bool isDrawing = false;

    private void Awake()
    {
        drawArea = GetComponent<RectTransform>();
    }

    // Begin a new stroke when the pointer is pressed down
    public void OnPointerDown(PointerEventData eventData)
    {
        ClearCanvas();
        foreach (Stroke stroke in committedStrokes)
        {
            DrawStroke(stroke, committedLineMaterial);
        }
        isDrawing = true;
        currentStroke = new Stroke();
        completedStrokes.Add(currentStroke);

        CreateNewLine();
        AddPoint(eventData);
    }

    // Continue drawing as the pointer moves
    public void OnDrag(PointerEventData eventData)
    {
        if (isDrawing)
            AddPoint(eventData);
    }

    // Finish the stroke when pointer is released
    public void OnPointerUp(PointerEventData eventData)
    {
        isDrawing = false;
        Debug.Log($"Stroke complete. Total points: {currentStroke.points.Count}");
        currentLine = null;
    }

    // Add a new point to the current stroke (if inside panel)
    private void AddPoint(PointerEventData eventData)
    {
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(drawArea, eventData.position, eventData.pressEventCamera, out localPoint))
        {
            if (!IsPointInsidePanel(localPoint))
                return; // Skip point if it's outside the panel

            currentStroke.points.Add(localPoint);
            UpdateLineRenderer(currentStroke.points);
        }
    }

    // Check if point is inside panel bounds (in local space)
    private bool IsPointInsidePanel(Vector2 localPoint)
    {
        return drawArea.rect.Contains(localPoint);
    }

    // Create a new GameObject with LineRenderer for the stroke
    public void CreateNewLine()
    {
        GameObject lineObj = new GameObject("StrokeLine");
        lineObj.transform.SetParent(transform);
        lineObj.transform.localPosition = Vector3.zero;
        lineObj.transform.localRotation = Quaternion.identity;
        lineObj.transform.localScale = Vector3.one;

        currentLine = lineObj.AddComponent<LineRenderer>();
        currentLine.material = lineMaterial;
        currentLine.startWidth = lineWidth;
        currentLine.endWidth = lineWidth;
        currentLine.positionCount = 0;
        currentLine.useWorldSpace = false;
        currentLine.numCapVertices = 8;
    }

    // Redraw the current stroke as a line
    public void UpdateLineRenderer(List<Vector2> points)
    {
        if (currentLine == null) return;

        currentLine.positionCount = points.Count;
        for (int i = 0; i < points.Count; i++)
        {
            currentLine.SetPosition(i, new Vector3(points[i].x, points[i].y, -0.5f)); // Slight Z offset to render above panel
        }
    }

    // Remove line renderers and wipe stroke data
    public void ClearStrokes()
    {
        // Remove all line renderers (stroke visuals)
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Clear stroke data
        completedStrokes.Clear();
        committedStrokes.Clear();
        currentStroke = null;
        currentLine = null;

        Debug.Log("Canvas cleared.");
    }

    // Remove line renderers and wipe completed strokes data
    public void ClearCanvas()
    {
        completedStrokes.Clear();
        // Remove all line renderers (stroke visuals)
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void OnCommitButtonClicked()
    {
        committedStrokes.Add(currentStroke);
        ClearCanvas();

        foreach (Stroke stroke in committedStrokes)
        {
            DrawStroke(stroke, committedLineMaterial);
        }

    }

    // Draws a given stroke on the canvas using a specified material.
    // Optionally sets the line color if provided.
    public void DrawStroke(Stroke stroke, Material material, Color? overrideColor = null)
    {
        if (stroke == null || stroke.points.Count == 0)
        {
            Debug.LogWarning("Cannot draw null or empty stroke.");
            return;
        }

        GameObject lineObj = new GameObject("RenderedStroke");
        lineObj.transform.SetParent(transform);
        lineObj.transform.localPosition = Vector3.zero;
        lineObj.transform.localRotation = Quaternion.identity;
        lineObj.transform.localScale = Vector3.one;

        LineRenderer line = lineObj.AddComponent<LineRenderer>();
        line.material = new Material(material); // clone the material to avoid modifying the original
        if (overrideColor.HasValue)
        {
            line.material.color = overrideColor.Value;
        }

        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.positionCount = stroke.points.Count;
        line.useWorldSpace = false;
        line.numCapVertices = 8;

        for (int i = 0; i < stroke.points.Count; i++)
        {
            line.SetPosition(i, new Vector3(stroke.points[i].x, stroke.points[i].y, -0.5f));
        }
    }

    
    

}
