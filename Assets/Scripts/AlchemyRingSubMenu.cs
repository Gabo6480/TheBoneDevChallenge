using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AlchemyRingSubMenu : AlchemyRingMenu
{
    [SerializeField] RadialMenu _radialSubMenu;
    [SerializeField] RadialMenuElementCollection[] _subMenuElementCollection;

    [Header("Appereance")]
    [Range(0, 1)] [SerializeField] float selfIconDistance = 0.1f;
    [Range(0f, 0.5f)] [SerializeField] float selfCircleCutout = .333f;
    [Space]
    [Range(0, 1)] [SerializeField] float subIconDistance = 0.1f;
    [Range(0f, 0.5f)] [SerializeField] float subCircleCutout = .28f;

    private void OnValidate()
    {
        if (_radialMenu == null)
            _radialMenu = GetComponent<RadialMenu>();

        if (_subMenuElementCollection == null)
            _subMenuElementCollection = new RadialMenuElementCollection[_radialMenu.elementCount];
        else if(_subMenuElementCollection.Length != _radialMenu.elementCount)
        {
            var aux = _subMenuElementCollection;
            _subMenuElementCollection = new RadialMenuElementCollection[_radialMenu.elementCount];

            for (int i = 0; i < Mathf.Min(_subMenuElementCollection.Length, aux.Length); i++)
                _subMenuElementCollection[i] = aux[i];
        }

    }
    public override void CommitCurrentItem()
    {
        manager.GoToRing(RingIndex + 1);
    }
    public override void SelectCurrentItem()
    {
        if (manager.currentRingMenuIndex != RingIndex)
        {
            _radialSubMenu.ElementCollection = _subMenuElementCollection[_radialMenu.currentSelected];
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
                _radialSubMenu.ElementCollection = _subMenuElementCollection[_radialMenu.currentSelected];
                _radialSubMenu.BuildMenu();
                _radialSubMenu.transform.DOScale(originalScale, 0.2f);
                //_radialSubMenu.transform.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, originalRotation), 0.2f);
            });

        }

        manager.ChangeButtonIcon(_radialMenu.currentFraction.Icon.sprite);
    }

    public override void OnRingFocused()
    {
        _radialMenu.Interactable = true;

        DOTween.To(() => _radialMenu.IconDistance, x => _radialMenu.IconDistance = x, selfIconDistance, 0.2f);
        DOTween.To(() => _radialMenu.CircleCutout, x => _radialMenu.CircleCutout = x, selfCircleCutout, 0.2f)
            .OnUpdate(() => _radialMenu.UpdateFractionAppereance())
            .OnComplete(() => _radialMenu.UpdateFractionAppereance());
        
        DOTween.To(() => _radialSubMenu.IconDistance, x => _radialSubMenu.IconDistance = x, subIconDistance, 0.2f);
        DOTween.To(() => _radialSubMenu.CircleCutout, x => _radialSubMenu.CircleCutout = x, subCircleCutout, 0.2f)
            .OnUpdate(() => _radialSubMenu.UpdateFractionAppereance())
            .OnComplete(() => _radialSubMenu.UpdateFractionAppereance());


        InitializeSelection();
        manager.ChangeButtonIcon(_radialMenu.currentFraction.Icon.sprite);
    }

    public override void OnRingShrunk()
    {
        //_radialMenu.Interactable = false;
        _radialMenu.DiselectCurrentFracction();
    }

    public override void OnRingExpand()
    {
        _radialMenu.Interactable = false;
    }
}
