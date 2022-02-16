using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RadialMenuFraction : MonoBehaviour
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
        _isSelected = value;

        Circle.DOColor(_isSelected ? _selectedColor : _unSelectedColor, 0.2f);

        Circle.transform.DOScale(Vector3.one * (_isSelected ? _selectedTargetScale : _originalCircleScale), 0.2f);
    }
}
