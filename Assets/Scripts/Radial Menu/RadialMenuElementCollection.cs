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
    
    [HideInInspector]
    public int currentSelected = -1;

    public virtual void OnCommit(AlchemyRingMenu rm)
    {
        rm.manager.GoToRing(rm.RingIndex + 1);
    }
    public virtual void OnFocused(AlchemyRingMenu rm)
    {
        if (rm.RadialMenuRef.currentFraction != null)
            rm.manager.ChangeButtonIcon(rm.RadialMenuRef.currentFraction.Icon.sprite);
    }
    public virtual void OnShrunk(AlchemyRingMenu rm)
    {
        rm.RadialMenuRef.DiselectCurrentFracction();
    }
}
