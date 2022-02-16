using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RadialMenu : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] RadialMenuElement[] _elements;
    [SerializeField] RadialMenuFraction _radialMenuFractionPrefab;
    [SerializeField] int _firstSelected = 0;

    [Header("Appearance")]
    [SerializeField] float _gap = 1f;
    [Range(0, 1)] [SerializeField] float _iconDistance = 0.1f;
    [SerializeField] float _iconScale = 1f;

    [Header("Animation")]
    [SerializeField] AnimationCurve _rotationEase;

    [Header("References")]
    [SerializeField] Transform _elementParent;

    [SerializeField] RadialMenuFraction[] _fractions;

    float _stepAngle;
    private void Start()
    {
        //BuildMenu();
        _stepAngle = 360f / _elements.Length;

        for (int i = 0; i < _fractions.Length; i++)
        {
            //It's important to use an auxiliary variable that gets created on the loop
            //This allows the callback have access to the value from memory
            int aux = i;

            _fractions[i].Button.onClick.AddListener(() => {
                //Debug.Log(aux);
                RotateElementParent(-(aux - 1) * _stepAngle);

                for (int j = 0; j < _fractions.Length; j++)
                {
                    _fractions[j].SetIsSelected(j == aux);
                }
            });
        }

        //Set the _firstSelected item in the list as selected
        RotateElementParent(-(_firstSelected - 1) * _stepAngle);
        _fractions[_firstSelected].SetIsSelected(true);
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
        var frac = GetComponentsInChildren<RadialMenuFraction>();
        foreach (var f in frac)
        {
                if(f != null)
                    DestroyImmediate(f.gameObject);
        }

        _fractions = new RadialMenuFraction[_elements.Length];

        var stepAngle = 360f / _elements.Length;

        for (int i = 0; i < _elements.Length; i++)
        {
            _fractions[i] = Instantiate(_radialMenuFractionPrefab, _elementParent);

            SetFractionAppereance(_fractions[i], _elements[i], i, stepAngle);
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
        //fraction.Circle.color = Color.gray;

        //Debug.Log(_fractions[i].CircleRect.rect.height);

        fraction.Icon.transform.localPosition = fraction.Circle.transform.localPosition + Quaternion.AngleAxis((index - 1) * stepAngle, Vector3.forward) * Vector3.up * ((1f - _iconDistance) * fraction.CircleRect.rect.height / 2f);
        fraction.Icon.transform.localRotation = Quaternion.Euler(0, 0, (index - 1) * stepAngle + 90);
        fraction.Icon.transform.localScale = Vector3.one * _iconScale;

        //temp
        fraction.Name.text = index.ToString();

        if (element == null)
            return;

        fraction.Icon.sprite = element.Icon;
        //fraction.Name.text = element.Name;
    }

    void RotateElementParent(float targetAngle)
    {
        _elementParent.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, targetAngle), 0.3f).SetUpdate(true).SetEase(_rotationEase);
    }
}
