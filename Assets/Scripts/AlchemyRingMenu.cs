using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AlchemyRingMenu : MonoBehaviour
{
    [SerializeField] protected RadialMenu _radialMenu;

    public abstract void CommitCurrentItem();
}
