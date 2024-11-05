using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Palette : MonoBehaviour
{
    public Color itemHighlighted = new Color(1, 0.8f, 0.5f, 0.6f);
    public Color frameEmpty = Color.white;
    public Color frameOccupied = new Color(0.5f, 0.5f, 0.5f, 1);
    public Color framePointed = new Color(1, 0.8f, 0.5f, 1);
    public Color framePressed = new Color(0.7f, 0.6f, 0.4f, 1);
}
