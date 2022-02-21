using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RadialMenuRecipeElementCollection", menuName = "Radial Menu/Recipe Element Collection")]
public class RadialMenuRecipeElementCollection : RadialMenuElementCollection
{
    [SerializeField] public CraftComponent[] Recipe;

    private void OnValidate()
    {
        if(_elements == null || _elements.Length != Recipe.Length)
        {
            _elements = new RadialMenuElement[Recipe.Length];
        }

        for (int i = 0; i < Recipe.Length; i++)
        {
            if (Recipe[i].Item == null)
                continue;

            _elements[i].Icon = Recipe[i].Item.Icon;
            _elements[i].Name = Recipe[i].Item.Name;
        }
    }
}
