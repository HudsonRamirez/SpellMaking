using System;
using System.Collections.Generic;
using UnityEngine;

public class GestureSaveManager : MonoBehaviour
{
    public DrawSurface drawSurface;  
    public GestureRecognizer gestureRecognizer; 
    /// <summary>
    /// Saves the current stroke from the recognizer as a new gesture template.
    /// </summary>
    public void SaveCurrentGesture()
    {
        if (drawSurface == null)
        {
            Debug.LogError("Draw Surface reference is missing.");
            return;
        }

        if (drawSurface.committedStrokes.Count == 0)
        {
            Debug.LogError("No stroke on canvas.");
            return;
        }

        var recognizer = new PointCloudRecognizer();

        List<Stroke> savedStrokes = new List<Stroke>();

        foreach (Stroke stroke in drawSurface.committedStrokes)     // This will break things if more than one committed stroke exists!
        {
            // Normalize points
            List<Vector2> normalizedPoints = recognizer.ScaleToSquare(
                recognizer.TranslateToOrigin(
                    recognizer.Resample(new List<Vector2>(stroke.points))
                )
            );

            Stroke savedStroke = new Stroke { points = normalizedPoints };
            savedStrokes.Add(savedStroke);
        }

        string gestureName = $"Template {gestureRecognizer.templates.Count}";
        
        Gesture newGesture = new Gesture(gestureName, savedStrokes);

        gestureRecognizer.templates.Add(newGesture);
        Debug.Log($"Gesture '{gestureName}' saved as a new template.");
    }
}
