using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.Events;

public class RadialMenu : MonoBehaviour, IDragHandler, IEndDragHandler
{

    [System.Serializable]
    struct RadialMenuElement
    {
        [SerializeField] public string Name;
        [SerializeField] public Sprite Icon;
        [SerializeField] public UnityEvent Callback;
    }

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
    [SerializeField] RectTransform _rotationRect;

    [SerializeField] RadialMenuFraction[] _fractions;

    float _stepAngle;
    int _currentSelected = int.MaxValue;
    bool _isDragging = false;

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
                if (_isDragging)
                    return;

                SelectItem(aux);
                _elements[aux].Callback?.Invoke();
            });
        }

        //Set the _firstSelected item in the list as selected
        SelectItem(_firstSelected);
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
        fraction.Circle.transform.localRotation = Quaternion.Euler(0, 0, -stepAngle / 2f - _gap / 2f + index * stepAngle);
        //fraction.Circle.color = Color.gray;

        //Debug.Log(_fractions[i].CircleRect.rect.height);

        fraction.Icon.transform.localPosition = fraction.Circle.transform.localPosition + Quaternion.AngleAxis((index - 1) * stepAngle, Vector3.forward) * Vector3.up * ((1f - _iconDistance) * fraction.CircleRect.rect.height / 2f);
        fraction.Icon.transform.localRotation = Quaternion.Euler(0, 0, (index - 1) * stepAngle + 90);
        fraction.Icon.transform.localScale = Vector3.one * _iconScale;

        fraction.Icon.sprite = element.Icon;
        fraction.Name.text = element.Name;
    }

    void SelectItem(int index)
    {
        if (_currentSelected == index)
            return;

        if (!_fractions[index].gameObject.activeInHierarchy)
        {
            SelectItem(NormalizeIndex(index + (NormalizeIndex(index - 1) > NormalizeIndex(_currentSelected - 1) ? 1 : -1)));
            return;
        }

        RotateElementParent(-(index - 1) * _stepAngle);

        UpdateFractionsIsSelected(index);

        _currentSelected = index;
    }

    void UpdateFractionsIsSelected(int index)
    {
        for (int j = 0; j < _fractions.Length; j++)
        {
            _fractions[j].SetIsSelected(j == index);
        }
    }

    public void SelectionUp()
    {
        if (!_isDragging)
            SelectItem(NormalizeIndex(_currentSelected + 1));
    }
    public void SelectionDown()
    {
        if (!_isDragging)
            SelectItem(NormalizeIndex(_currentSelected - 1));
    }

    void RotateElementParent(float targetAngle)
    {
        _elementParent.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, targetAngle), 0.3f).SetUpdate(true).SetEase(_rotationEase);
    }

    public void OnDrag(PointerEventData eventData)
    {
        _isDragging = true;

        Vector2 local;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_rotationRect, eventData.position, eventData.pressEventCamera, out local);
        Vector2 localdelta;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_rotationRect, eventData.position - eventData.delta, eventData.pressEventCamera, out localdelta);

        float angleDelta = Vector2.SignedAngle(local, localdelta);

        _elementParent.rotation *= Quaternion.Euler(0, 0, -angleDelta);

        int selected = Mathf.RoundToInt(1f + _elementParent.rotation.eulerAngles.z / -_stepAngle);

        //Normalize the index since the previous math can return negative indexes
        UpdateFractionsIsSelected(NormalizeIndex(selected));
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _isDragging = false;
        
        //Once the draggins is done, calculate which item is closest to the top
        int selected = Mathf.RoundToInt( 1f + _elementParent.rotation.eulerAngles.z / -_stepAngle);

        //Normalize the index since the previous math can return negative indexes
        SelectItem(NormalizeIndex(selected));
    }

    int NormalizeIndex(int index) => (index + _fractions.Length) % _fractions.Length;
}
