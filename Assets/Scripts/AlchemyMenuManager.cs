using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class AlchemyMenuManager : MonoBehaviour
{
    [SerializeField] Button _centralButton;
    [SerializeField] Button _backButton;
    [SerializeField] Image _centralButtonIcon;
    [Space]
    [SerializeField] RectTransform _sliderParent;
    [SerializeField] Slider _slider;
    [SerializeField] TMP_InputField _sliderNumber;
    [SerializeField] Button _sliderPlusButton;
    [SerializeField] Button _sliderMinusButton;
    [Space]
    [SerializeField] RectTransform _overlayParent;
    [SerializeField] Transform _overlayLightning;
    [SerializeField] Button _thanksButton;
    [SerializeField] Image _resultImage;
    [SerializeField] TMP_Text _resultText;

    [SerializeField] AlchemyRingMenu[] _ringMenuLayers;

    [Header("Behaviour")]
    [SerializeField] float[] _scaleIncreaseStages;
    [SerializeField] int _mainScaleIndex = 2;

    [Header("Inventory")]
    [SerializeField] CraftComponent[] _startingItems;

    Dictionary<GameItem, int> _inventory = new Dictionary<GameItem, int>();

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
        _overlayParent.gameObject.SetActive(false);
        _overlayLightning.gameObject.SetActive(false);

        ScaleLayers();

        foreach (var item in _startingItems)
        {
            _inventory.Add(item.Item, item.Quantity);
        }


        _ringMenuLayers[currentRingMenuIndex].InitializeSelection();

        
        _sliderPlusButton.interactable = _slider.value < _slider.maxValue;
        _sliderMinusButton.interactable = _slider.value > _slider.minValue;

        _slider.onValueChanged.AddListener(v => {
            _sliderNumber.text = v.ToString();
        });

        _sliderNumber.onValueChanged.AddListener(s => {
            int v = 0;

            if (!int.TryParse(s, out v))
                return;

            v = Mathf.FloorToInt(Mathf.Clamp(v, _slider.minValue, _slider.maxValue));

            _sliderNumber.text = v.ToString();
            _slider.value = v;

            _sliderPlusButton.interactable = v < _slider.maxValue;
            _sliderMinusButton.interactable = v > _slider.minValue;
        });

        _sliderPlusButton.onClick.AddListener(() => {
            if(_slider.value < _slider.maxValue)
                _slider.value = _slider.value + 1;
        });

        _sliderMinusButton.onClick.AddListener(() => {
            if (_slider.value > _slider.minValue)
                _slider.value = _slider.value - 1;
        });

        _sliderNumber.text = _slider.value.ToString();
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

    public void CentralButtonRelease()
    {
        _ringMenuLayers[currentRingMenuIndex].OnRelease();
    }

    public void CentralButtonHold()
    {
        _ringMenuLayers[currentRingMenuIndex].OnHold();
    }

    void ShowOverlay(Sprite image, string name, int quantity)
    {
        _overlayLightning.gameObject.SetActive(true);
        _overlayLightning.localScale = Vector3.one * 100;

        _overlayLightning.DOScale(1000, 1f)
            .OnComplete(() => { 
                _overlayParent.gameObject.SetActive(true);

                _resultImage.sprite = image;

                _resultText.text = name + "\nx" + quantity.ToString();
                _overlayLightning.gameObject.SetActive(false);
            });
    }

    public void HideOverlay()
    {
        _overlayParent.gameObject.SetActive(false);
    }
    Tween craftTween;
    float originalRotation;
    public void AnimateCraft(CraftComponent[] craftComponents, CraftComponent result)
    {
        originalRotation = _ringMenuLayers[currentRingMenuIndex].RadialSubMenu.transform.localRotation.z;
        craftTween = _ringMenuLayers[currentRingMenuIndex].RadialSubMenu.transform.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, originalRotation + 180), 1f)
            .OnComplete(() => {
                CraftRecipe(craftComponents, result);
                _ringMenuLayers[currentRingMenuIndex].RadialSubMenu.transform.localRotation = Quaternion.Euler(0, 0, originalRotation);
            });
    }

    public void KillAnimateCraft()
    {
        if (craftTween == null)
            return;

        craftTween.Kill();
        _ringMenuLayers[currentRingMenuIndex].RadialSubMenu.transform.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, originalRotation), 0.2f);
    }

    public void calculateRecipe(CraftComponent[] craftComponents)
    {
        if (craftComponents == null || craftComponents.Length == 0)
            return;

        int result = int.MaxValue;

        foreach(var item in craftComponents)
        {
            if (!_inventory.ContainsKey(item.Item))
            {
                result = 0;
                break;
            }

            int r = _inventory[item.Item] / item.Quantity;

            result = result > r ? r : result;
        }

        _slider.value = result > 0 ? 1 : 0;
        //_slider.minValue = result > 0 ? 1 : 0;
        _slider.maxValue = result;
    }

    public void CraftRecipe(CraftComponent[] craftComponents, CraftComponent result)
    {
        if (craftComponents == null || craftComponents.Length == 0)
            return;

        int quantity = Mathf.FloorToInt(_slider.value);

        foreach (var item in craftComponents)
        {
            if (_inventory[item.Item] < item.Quantity * quantity)
                return;
        }

        foreach (var item in craftComponents)
        {
            _inventory[item.Item] -= item.Quantity * quantity;
        }

        if (_inventory.ContainsKey(result.Item))
            _inventory[result.Item] += result.Quantity * quantity;
        else
            _inventory.Add(result.Item, result.Quantity * quantity);

        ShowOverlay(result.Item.Icon, result.Item.Name, result.Quantity * quantity);

        calculateRecipe(craftComponents);
    }

    void AnimateSliderParent(float targetPos)
    {
        _sliderParent.DOAnchorPosY(targetPos, 0.3f);
    }

    public void AnimateSliderIn()
    {
        AnimateSliderParent(0f);
    }
    public void AnimateSliderOut()
    {
        AnimateSliderParent(-300f);
    }

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
