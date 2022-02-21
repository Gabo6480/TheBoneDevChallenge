using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AlchemyRingSubMenu : AlchemyRingMenu
{
    [SerializeField] RadialMenu _radialSubMenu;

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
        if (manager.currentRingMenuIndex != RingIndex)
        {
            _radialSubMenu.ElementCollection = RadialMenuRef.ElementCollection.Elements[RadialMenuRef.currentSelected].SubElement;
            _radialSubMenu.BuildMenu();
            manager.GoToRing(RingIndex);
        }
        else
        {
            float originalScale = _radialSubMenu.transform.localScale.x;
            float originalRotation = _radialSubMenu.transform.eulerAngles.z;
            //_radialSubMenu.transform.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, originalRotation + 90), 0.2f);
            _radialSubMenu.transform.DOScale(0.3f, 0.2f).OnComplete(() =>
            {
                _radialSubMenu.ElementCollection = RadialMenuRef.ElementCollection.Elements[RadialMenuRef.currentSelected].SubElement;
                _radialSubMenu.BuildMenu();
                _radialSubMenu.transform.DOScale(originalScale, 0.2f);
                //_radialSubMenu.transform.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, originalRotation), 0.2f);
            });

        }

        manager.ChangeButtonIcon(RadialMenuRef.currentFraction.Icon.sprite);
    }

    public override void OnRingFocused()
    {
        RadialMenuRef.Interactable = true;

        InitializeSelection();

        DOTween.To(() => RadialMenuRef.IconDistance, x => RadialMenuRef.IconDistance = x, selfIconDistance, 0.2f);
        DOTween.To(() => RadialMenuRef.CircleCutout, x => RadialMenuRef.CircleCutout = x, selfCircleCutout, 0.2f)
            .OnUpdate(() => RadialMenuRef.UpdateFractionAppereance())
            .OnComplete(() => RadialMenuRef.UpdateFractionAppereance());
        
        DOTween.To(() => _radialSubMenu.IconDistance, x => _radialSubMenu.IconDistance = x, subIconDistance, 0.2f);
        DOTween.To(() => _radialSubMenu.CircleCutout, x => _radialSubMenu.CircleCutout = x, subCircleCutout, 0.2f)
            .OnUpdate(() => _radialSubMenu.UpdateFractionAppereance())
            .OnComplete(() => _radialSubMenu.UpdateFractionAppereance());

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
