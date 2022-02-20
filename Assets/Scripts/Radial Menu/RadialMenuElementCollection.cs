using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RadialMenuElement
{
    [SerializeField] public bool Inactive;
    [SerializeField] public string Name;
    [SerializeField] public Sprite Icon;
    [SerializeField] public RadialMenuElementCollection SubElement;
}

[CreateAssetMenu(fileName = "RadialMenuElementCollection", menuName = "Radial Menu/Element Collection")]
public class RadialMenuElementCollection : ScriptableObject
{
    [SerializeField] RadialMenuElement[] _elements;
    [SerializeField] RadialMenuFraction _radialMenuFractionPrefab;
    [SerializeField] int _firstSelected = 0;

    public RadialMenuElement[] Elements { get { return _elements; } }
    public RadialMenuFraction RadialMenuFractionPrefab { get { return _radialMenuFractionPrefab; } }
    public int FirstSelected { get { return _firstSelected; } }
}
