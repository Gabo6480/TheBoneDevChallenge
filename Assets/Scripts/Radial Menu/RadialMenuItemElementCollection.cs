using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RadialMenuItemElementCollection", menuName = "Radial Menu/Item Element Collection")]
public class RadialMenuItemElementCollection : RadialMenuElementCollection
{
    public override void OnCommit(AlchemyRingMenu rm)
    {
        Debug.Log("Fart");
    }

    public override void OnFocused(AlchemyRingMenu rm)
    {
        base.OnFocused(rm);

        rm.manager.AnimateSliderIn();
    }

    public override void OnShrunk(AlchemyRingMenu rm)
    {
        base.OnShrunk(rm);

        rm.manager.AnimateSliderOut();
    }
}
