using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


[System.Serializable]
public struct CraftComponent
{
    [SerializeField] public GameItem Item;
    [SerializeField] public int Quantity;
}

[CreateAssetMenu(fileName = "RadialMenuItemElementCollection", menuName = "Radial Menu/Item Element Collection")]
public class RadialMenuItemElementCollection : RadialMenuElementCollection
{
    [SerializeField] CraftComponent _item;
    [SerializeField] public RadialMenuRecipeElementCollection Recipe;

    private void OnValidate()
    {
        if (_elements == null || _elements.Length != 1)
        {
            _elements = new RadialMenuElement[1];
        }

        if (_item.Item == null)
            return;

        _elements[0].Icon = _item.Item.Icon;
        //_elements[0].Name = _item.Item.Name;
    }

    public override void OnCommit(AlchemyRingMenu rm)
    {
        //base.OnCommit(rm);
    }

    public override void OnRelease(AlchemyRingMenu rm)
    {
        rm.manager.KillAnimateCraft();
    }
    public override void OnHold(AlchemyRingMenu rm)
    {
        rm.manager.AnimateCraft(Recipe.Recipe, _item);
    }

    public override void OnFocused(AlchemyRingMenu rm)
    {
        base.OnFocused(rm);
        rm.manager.calculateRecipe(Recipe.Recipe);
        rm.manager.AnimateSliderIn();
    }

    public override void OnShrunk(AlchemyRingMenu rm)
    {
        base.OnShrunk(rm);

        rm.manager.AnimateSliderOut();
    }

    public override void OnSelectItem(AlchemyRingMenu rm)
    {
        if (rm.manager.currentRingMenuIndex != rm.RingIndex)
        {
            rm.RadialSubMenu.ElementCollection = Recipe;
            rm.RadialSubMenu.Interactable = false;
            rm.RadialSubMenu.BuildMenu();
            rm.RadialSubMenu.gameObject.SetActive(true);
            rm.RadialSubMenu.SetItemHoverAble(false);
            rm.manager.GoToRing(rm.RingIndex);
        }
        else
        {
            float originalScale = rm.RadialSubMenu.transform.localScale.x;
            float originalRotation = rm.RadialSubMenu.transform.eulerAngles.z;
            //_radialSubMenu.transform.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, originalRotation + 90), 0.2f);
            rm.RadialSubMenu.transform.DOScale(0.3f, 0.2f).OnComplete(() =>
            {
                rm.RadialSubMenu.ElementCollection = Recipe;
                rm.RadialSubMenu.Interactable = false;
                rm.RadialSubMenu.BuildMenu();
                rm.RadialSubMenu.gameObject.SetActive(true);
                rm.RadialSubMenu.SetItemHoverAble(false);
                rm.RadialSubMenu.transform.DOScale(originalScale, 0.2f);
                //_radialSubMenu.transform.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, originalRotation), 0.2f);
            });

        }

        rm.manager.ChangeButtonIcon(_item.Item.Icon);
    }
}
