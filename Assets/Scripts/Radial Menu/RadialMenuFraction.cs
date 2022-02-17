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

        if (_isSelected)
        {
            Circle.DOColor(_selectedColor, 0.2f).SetDelay(0.1f);
            transform.DOScale(Vector3.one *  _selectedTargetScale, 0.2f).SetDelay(0.1f);
        }
        else
        {
            Circle.DOColor(_unSelectedColor, 0.2f);
            transform.DOScale(Vector3.one * _originalCircleScale, 0.2f).SetDelay(0.1f);
        }

    }
}
