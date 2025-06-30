using System.Collections.Generic;
using UnityEngine;

public class Spell
{
    public List<SpellLayer> layers = new();         // List of the layers of the spell
    public List<string> globalModifiers = new();    // List of modifiers which apply to the whole spell

    public void AddLayer(SpellLayer layer) => layers.Add(layer);
    public void Clear()
    {
        layers.Clear();
        globalModifiers.Clear();
    }
}

public class SpellLayer
{
    public List<Stroke> strokes = new();    // List of strokes contained in this layer (always one for now)
    public List<string> modifiers = new();  // List of modifiers contained within this one layer


    public SpellLayer(List<Stroke> strokes)
    {
        this.strokes = strokes;
    }
}
