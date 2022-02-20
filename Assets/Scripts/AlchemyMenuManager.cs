using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AlchemyMenuManager : MonoBehaviour
{
    [SerializeField] Button _centralButton;
    [SerializeField] Button _backButton;
    [SerializeField] Image _centralButtonIcon;

    [SerializeField] AlchemyRingMenu[] _ringMenuLayers;

    [Header("Behaviour")]
    [SerializeField] float[] _scaleIncreaseStages;
    [SerializeField] int _mainScaleIndex = 2;

    Color _clearTransparent = new Color(1,1,1,0);

    public int currentRingMenuIndex { get; private set; } = 0;

    private void Start()
    {
        for (int i = 0; i < _ringMenuLayers.Length; i++)
        {
            if (_ringMenuLayers[i] == null)
                continue;

            _ringMenuLayers[i].RingIndex = i;
            _ringMenuLayers[i].manager = this;
        }

        _backButton.gameObject.SetActive(false);

        ScaleLayers();

        _ringMenuLayers[currentRingMenuIndex].InitializeSelection();
    }

    public void SelectRadialItem()
    {
        if (_ringMenuLayers[currentRingMenuIndex] != null)
            _ringMenuLayers[currentRingMenuIndex].CommitCurrentItem();
    }
    public void GoToRing(int index)
    {
        if (index == currentRingMenuIndex)
            return;

        if (_ringMenuLayers[currentRingMenuIndex] != null)
        {
            if (index < currentRingMenuIndex)
                _ringMenuLayers[currentRingMenuIndex].OnRingShrunk();
            else if (index > currentRingMenuIndex)
                _ringMenuLayers[currentRingMenuIndex].OnRingExpand();
        }

        currentRingMenuIndex = Mathf.Max(index, 0);

        if (_ringMenuLayers[currentRingMenuIndex] != null)
            _ringMenuLayers[currentRingMenuIndex].OnRingFocused();

        ScaleLayers();

        _backButton.gameObject.SetActive(currentRingMenuIndex > 0);
    }

    //public void GoToRing(AlchemyRingMenu ring)
    //{
    //    int index = -1;
    //
    //    for (int i = 0; i < _ringMenuLayers.Length; i++)
    //    {
    //        if (_ringMenuLayers[i] == ring)
    //        {
    //            index = i;
    //            break;
    //        }
    //    }
    //
    //    if (index != -1)
    //        GoToRing(index);
    //}

    public void PreviousRing()
    {
        //Debug.Log("Back");
        GoToRing(currentRingMenuIndex - 1);
    }

    public void SelectionUp()
    {
        if (_ringMenuLayers[currentRingMenuIndex] != null)
            _ringMenuLayers[currentRingMenuIndex].SelectionUp();
    }
    public void SelectionDown()
    {
        if (_ringMenuLayers[currentRingMenuIndex] != null)
            _ringMenuLayers[currentRingMenuIndex].SelectionDown();
    }

    public void ChangeButtonIcon(Sprite newIcon)
    {
        _centralButtonIcon.DOColor(_clearTransparent, 0.2f)
            .OnComplete(() => {
                _centralButtonIcon.sprite = newIcon;
                if(newIcon != null)
                    _centralButtonIcon.DOColor(Color.white, 0.2f);
            });
    }

    void ScaleLayers()
    {
        for (int i = 0; i < _ringMenuLayers.Length; i++)
        {
            if (_ringMenuLayers[i] == null)
                continue;

            int scaleIndex = Mathf.Clamp(currentRingMenuIndex - i + _mainScaleIndex, 0, _scaleIncreaseStages.Length - 1);

            _ringMenuLayers[i].gameObject.SetActive(scaleIndex >= _mainScaleIndex - 2);

            _ringMenuLayers[i].transform.DOKill(true);
            int aux = i;
            _ringMenuLayers[i].transform.DOScale(_scaleIncreaseStages[scaleIndex], 0.3f).OnComplete(() => {
                    _ringMenuLayers[aux].gameObject.SetActive(scaleIndex >= _mainScaleIndex - 1);
            });
        }
    }
}
