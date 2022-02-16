using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialMenu : MonoBehaviour
{
    [SerializeField] RadialMenuElement[] _elements;
    [SerializeField] RadialMenuFraction _radialMenuFractionPrefab;

    [Header("Appearance")]
    [SerializeField] float _gap = 1f;
    [Range(0, 1)] [SerializeField] float _iconDistance = 0.1f;
    [SerializeField] float _iconScale = 1f;

    protected RadialMenuFraction[] _fractions;

    [Header("References")]
    [SerializeField] Transform _mask;

    private void Start()
    {
        BuildMenu();
    }

    private void OnValidate()
    {
        var stepAngle = 360f / _elements.Length;

        if (_fractions != null && _elements.Length == _fractions.Length)
            for (int i = 0; i < _elements.Length; i++)
        {
            if(_fractions[i] != null)
                SetFractionAppereance(_fractions[i], _elements[i], i, stepAngle);
        }
    }

    [ContextMenu("Build Menu")]
    void BuildMenu()
    {
        if(_fractions != null)
        foreach (var f in _fractions)
        {
                if(f != null)
                    DestroyImmediate(f.gameObject);
        }

        _fractions = new RadialMenuFraction[_elements.Length];

        var stepAngle = 360f / _elements.Length;

        Debug.Log(stepAngle);

        for (int i = 0; i < _elements.Length; i++)
        {
            _fractions[i] = Instantiate(_radialMenuFractionPrefab, transform);

            SetFractionAppereance(_fractions[i], _elements[i], i, stepAngle);

            _fractions[i].transform.SetParent(_mask, true);
        }
    }

    void SetFractionAppereance(RadialMenuFraction fraction, RadialMenuElement element, int index, float stepAngle)
    {
        //Make sure the object is properly transformed
        fraction.transform.localPosition = Vector3.zero;
        fraction.transform.localRotation = Quaternion.identity;

        fraction.Circle.fillAmount = 1f / _elements.Length - _gap / 360f;
        fraction.Circle.transform.localPosition = Vector3.zero;
        fraction.Circle.transform.localRotation = Quaternion.Euler(0, 0, -stepAngle / 2f + _gap / 2f + index * stepAngle);
        fraction.Circle.color = Color.gray;

        //Debug.Log(_fractions[i].CircleRect.rect.height);

        fraction.Icon.transform.localPosition = fraction.Circle.transform.localPosition + Quaternion.AngleAxis((index - 1) * stepAngle, Vector3.forward) * Vector3.up * ((1f - _iconDistance) * fraction.CircleRect.rect.height / 2f);
        fraction.Icon.transform.localRotation = Quaternion.Euler(0, 0, (index - 1) * stepAngle + 90);
        fraction.Icon.transform.localScale = Vector3.one * _iconScale;


        if (element == null)
            return;

        fraction.Icon.sprite = element.Icon;
        fraction.Name.text = element.Name;
    }
}
