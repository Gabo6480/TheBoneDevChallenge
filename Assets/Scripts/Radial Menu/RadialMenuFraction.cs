using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadialMenuFraction : MonoBehaviour
{
    public Image Icon;
    public Image Circle;
    public RectTransform CircleRect;
    public Text Name;

    private void OnValidate()
    {
        if (Circle != null)
            CircleRect = Circle.GetComponent<RectTransform>();
    }
}
