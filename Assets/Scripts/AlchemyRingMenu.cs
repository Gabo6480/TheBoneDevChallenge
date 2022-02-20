using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AlchemyRingMenu : MonoBehaviour
{
    [SerializeField] protected RadialMenu _radialMenu;
    public AlchemyMenuManager manager;

    public int RingIndex = 0;

    public abstract void CommitCurrentItem();
    public abstract void SelectCurrentItem();
    public abstract void OnRingFocused();
    public abstract void OnRingShrunk();
    public abstract void OnRingExpand();

    public void InitializeSelection()
    {
        _radialMenu.InitializeSelection();
    }

    public void SelectionUp()
    {
        _radialMenu.SelectionUp();
    }
    public void SelectionDown()
    {
        _radialMenu.SelectionDown();
    }
}
