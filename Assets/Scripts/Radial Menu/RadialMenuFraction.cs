using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class RadialMenuFraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image Icon;
    public Image Circle;
    public RectTransform CircleRect;
    public Text Name;
    public Button Button;

    [Header("Appereance")]
    [SerializeField] Color _selectedColor = Color.white;
    [SerializeField] Color _unSelectedColor = Color.gray;
    [SerializeField] float _selectedTargetScale = 1.05f;

    float _originalCircleScale = 1;

    bool _isSelected = false;

    Tween hoverColorTween;
    Tween hoverScaleTween;

    Tween selectedColorTween;
    Tween selectedScaleTween;

    private void Start()
    {
        Circle.color = _unSelectedColor;

        _originalCircleScale = Circle.transform.localScale.x;
    }

    private void OnValidate()
    {
        if (Circle != null)
            CircleRect = Circle.GetComponent<RectTransform>();
    }

    public void SetIsSelected(bool value)
    {
        if (_isSelected == value)
            return;

        _isSelected = value;

        selectedColorTween.Kill();
        selectedScaleTween.Kill();

        if (_isSelected)
        {
            selectedColorTween = Circle.DOColor(_selectedColor, 0.2f).SetDelay(0.1f);
            selectedScaleTween = transform.DOScale(Vector3.one *  _selectedTargetScale, 0.2f).SetDelay(0.1f);
        }
        else
        {
            selectedColorTween = Circle.DOColor(_unSelectedColor, 0.2f);
            selectedScaleTween = transform.DOScale(Vector3.one * _originalCircleScale, 0.2f);
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_isSelected)
            return;

        hoverColorTween.Kill();
        hoverScaleTween.Kill();

        hoverColorTween = Circle.DOColor(Color.Lerp(_selectedColor, _unSelectedColor, 0.5f), 0.2f);
        hoverScaleTween = transform.DOScale(Vector3.one * Mathf.Lerp(_selectedTargetScale, _originalCircleScale, 0.5f), 0.2f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(_isSelected)
            return;

        hoverColorTween.Kill();
        hoverScaleTween.Kill();

        hoverColorTween = Circle.DOColor(_unSelectedColor, 0.1f);
        hoverScaleTween = transform.DOScale(Vector3.one * _originalCircleScale, 0.2f);
    }
}
