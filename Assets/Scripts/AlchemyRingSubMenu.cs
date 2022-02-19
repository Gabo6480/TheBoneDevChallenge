using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlchemyRingSubMenu : AlchemyRingMenu
{
    [SerializeField] RadialMenu _radialSubMenu;
    [SerializeField] RadialMenuElementCollection[] _subMenuElementCollection;

    private void OnValidate()
    {
        if(_subMenuElementCollection == null)
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
        _radialSubMenu.ElementCollection = _subMenuElementCollection[_radialMenu.currentSelected];
        _radialSubMenu.BuildMenu();
    }
}
