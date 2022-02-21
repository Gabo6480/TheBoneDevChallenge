using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AlchemyRingMenu : MonoBehaviour
{
    [SerializeField] public RadialMenu RadialMenuRef;
    public AlchemyMenuManager manager;

    public int RingIndex = 0;

    [SerializeField] public RadialMenu RadialSubMenu;

    public abstract void CommitCurrentItem();
    public abstract void SelectCurrentItem();
    public abstract void OnRingFocused();
    public abstract void OnRingShrunk();
    public abstract void OnRingExpand();
    public  void OnHold()
    {
        RadialMenuRef.ElementCollection.OnHold(this);
    }
    public  void OnRelease()
    {
        RadialMenuRef.ElementCollection.OnRelease(this);
    }

    public void InitializeSelection()
    {
        RadialMenuRef.InitializeSelection();
    }

    public void SelectionUp()
    {
        RadialMenuRef.SelectionUp();
    }
    public void SelectionDown()
    {
        RadialMenuRef.SelectionDown();
    }
}
