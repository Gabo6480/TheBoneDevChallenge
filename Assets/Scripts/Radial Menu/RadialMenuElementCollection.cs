using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
    [SerializeField] protected RadialMenuElement[] _elements;
    [SerializeField] RadialMenuFraction _radialMenuFractionPrefab;
    [SerializeField] int _firstSelected = 0;

    public virtual RadialMenuElement[] Elements { get { return _elements; } }
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
    public virtual void OnSelectItem(AlchemyRingMenu rm)
    {
        if (rm.manager.currentRingMenuIndex != rm.RingIndex)
        {
            rm.RadialSubMenu.ElementCollection = rm.RadialMenuRef.ElementCollection.Elements[rm.RadialMenuRef.currentSelected].SubElement;
            rm.RadialSubMenu.Interactable = true;
            rm.RadialSubMenu.BuildMenu();
            rm.RadialSubMenu.SetItemHoverAble(true);
            rm.manager.GoToRing(rm.RingIndex);
        }
        else
        {
            float originalScale = rm.RadialSubMenu.transform.localScale.x;
            float originalRotation = rm.RadialSubMenu.transform.eulerAngles.z;
            //_radialSubMenu.transform.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, originalRotation + 90), 0.2f);
            rm.RadialSubMenu.transform.DOScale(0.3f, 0.2f).OnComplete(() =>
            {
                rm.RadialSubMenu.ElementCollection = rm.RadialMenuRef.ElementCollection.Elements[rm.RadialMenuRef.currentSelected].SubElement;
                rm.RadialSubMenu.Interactable = true;
                rm.RadialSubMenu.BuildMenu();
                rm.RadialSubMenu.SetItemHoverAble(true);
                rm.RadialSubMenu.transform.DOScale(originalScale, 0.2f);
                //_radialSubMenu.transform.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, originalRotation), 0.2f);
            });

        }

        rm.manager.ChangeButtonIcon(rm.RadialMenuRef.currentFraction.Icon.sprite);
    }
}
