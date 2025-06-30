using System.Collections.Generic;
using UnityEngine;

public class GestureRecognizer : MonoBehaviour
{
    public DrawSurface drawSurface;
    public List<Gesture> templates = new();      // Your gesture templates
    public SpellBuilder spellBuilder;            // Reference to spell manager
    private PointCloudRecognizer recognizer = new PointCloudRecognizer();

    private void Awake()
    {
        InitializeTemplates();
    }


    public Gesture Recognize()
    {

        if (drawSurface.completedStrokes.Count == 0)
        {
            Debug.Log("No drawing is pending recognition.");
            return null;
        }

        Stroke lastStroke = drawSurface.completedStrokes[^1];   // For now should only ever be one completed stroke

        if (lastStroke == null || lastStroke.points.Count == 0)
        {
            Debug.Log($"currentStrokePoints empty!");
            return null;
        }

        // Recognize gesture from templates
        Gesture recognizedGesture = recognizer.Recognize(lastStroke.points, templates);


        return recognizedGesture;
    }

    public void OnRegisterButtonClicked()
    {

        Gesture recognized = Recognize();
        if (recognized != null)
        {
            Debug.Log("Gesture recognized: " + recognized.name);
        }
        else
        {
            Debug.Log("No gesture recognized.");
        }
    }


    public void InitializeTemplates()
    {
        // TODO: PUT TEMPLATES HERE
        
    }



}

