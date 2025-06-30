using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages creation and layering of a spell composed strokes.
/// </summary>
public class SpellBuilder : MonoBehaviour
{
    public Spell currentSpell = new();


    // Creates a new spell from a single set of strokes and returns it.
    public Spell BuildNewSpell(List<Stroke> strokes, string gestureName = "Unnamed")
    {
        currentSpell.Clear();
        AddLayerToSpell(strokes, gestureName);
        Debug.Log($"New spell built with initial layer: {gestureName}");

        return currentSpell;
    }

    // Adds a new layer to the current spell using provided strokes.
    public void AddLayerToSpell(List<Stroke> strokes, string gestureName = "Unnamed")
    {
        int index = currentSpell.layers.Count;
        SpellLayer newLayer = new SpellLayer(strokes);
        currentSpell.AddLayer(newLayer);
        Debug.Log($"Layer '{gestureName}' added. Total layers: {currentSpell.layers.Count}");
    }

    /// Clears the current spell and all its layers.
    public void ClearSpell()
    {
        currentSpell.Clear();
        Debug.Log("Spell cleared.");
    }

    /// Draws the full spell (all layers) onto the draw surface.
    public void DrawSpell(DrawSurface drawSurface)
    {
        drawSurface.ClearCanvas();

        Debug.Log($"Drawing spell with {currentSpell.layers.Count} layers.");

        foreach (var layer in currentSpell.layers)
        {
            foreach (var stroke in layer.strokes)
            {
                drawSurface.CreateNewLine(); // Add color argument if needed
                drawSurface.UpdateLineRenderer(stroke.points);
            }
        }

        Debug.Log("Full spell drawn.");
    }
}
