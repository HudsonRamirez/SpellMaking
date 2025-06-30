using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stroke
{
    public List<Vector2> points = new List<Vector2>();
}

[System.Serializable]
public class Gesture
{
    public string name;
    public List<Stroke> strokes = new List<Stroke>();

    public Gesture(string name, List<Stroke> strokes)
    {
        this.name = name;
        this.strokes = strokes;
    }
}
