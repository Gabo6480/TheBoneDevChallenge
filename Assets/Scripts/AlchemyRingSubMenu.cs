using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AlchemyRingSubMenu : AlchemyRingMenu
{
    [Header("Appereance")]
    [Range(0, 1)] [SerializeField] float selfIconDistance = 0.1f;
    [Range(0f, 0.5f)] [SerializeField] float selfCircleCutout = .333f;
    [Space]
    [Range(0, 1)] [SerializeField] float subIconDistance = 0.1f;
    [Range(0f, 0.5f)] [SerializeField] float subCircleCutout = .28f;

    private void OnValidate()
    {
        if (RadialMenuRef == null)
            RadialMenuRef = GetComponent<RadialMenu>();
    }
    public override void CommitCurrentItem()
    {
        RadialMenuRef.ElementCollection.OnCommit(this);
    }
    public override void SelectCurrentItem()
    {
        RadialMenuRef.ElementCollection.OnSelectItem(this);
    }

    public override void OnRingFocused()
    {
        RadialMenuRef.Interactable = true;

        InitializeSelection();

        DOTween.To(() => RadialMenuRef.IconDistance, x => RadialMenuRef.IconDistance = x, selfIconDistance, 0.2f);
        DOTween.To(() => RadialMenuRef.CircleCutout, x => RadialMenuRef.CircleCutout = x, selfCircleCutout, 0.2f)
            .OnUpdate(() => RadialMenuRef.UpdateFractionAppereance())
            .OnComplete(() => RadialMenuRef.UpdateFractionAppereance());
        
        DOTween.To(() => RadialSubMenu.IconDistance, x => RadialSubMenu.IconDistance = x, subIconDistance, 0.2f);
        DOTween.To(() => RadialSubMenu.CircleCutout, x => RadialSubMenu.CircleCutout = x, subCircleCutout, 0.2f)
            .OnUpdate(() => RadialSubMenu.UpdateFractionAppereance())
            .OnComplete(() => RadialSubMenu.UpdateFractionAppereance());

        RadialMenuRef.ElementCollection.OnFocused(this);
    }

    public override void OnRingShrunk()
    {
        //_radialMenu.Interactable = false;
        RadialMenuRef.ElementCollection.OnShrunk(this);
    }

    public override void OnRingExpand()
    {
        RadialMenuRef.Interactable = false;
    }
}
