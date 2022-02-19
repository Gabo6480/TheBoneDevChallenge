using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlchemyMenuManager : MonoBehaviour
{
    [SerializeField] Button _centralButton;
    [SerializeField] Image _centralButtonIcon;

    [Header("Rings")]
    [SerializeField] AlchemyRingMenu _outterRingMenu;

    [Header("Behaviour")]
    [SerializeField] float _scaleIncrease = 0.55f;


    AlchemyRingMenu _currentRingMenu;

    public void SelectRadialItem()
    {
        Debug.Log("Hello");
        _outterRingMenu.CommitCurrentItem();
    }
}
