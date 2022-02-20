using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.Events;

public class RadialMenu : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [Header("Settings")]
    public RadialMenuElementCollection ElementCollection;
    public bool Interactable = true;

    [Header("Appearance")]
    public float Gap = 1f;
    [Range(0, 1)] public float IconDistance = 0.1f;
    public float IconScale = 1f;
    [Range(0f, 0.5f)] public float CircleCutout = 0.25f;

    [Header("Animation")]
    [SerializeField] AnimationCurve _rotationEase;

    [Header("References")]
    [SerializeField] Transform _elementParent;
    [SerializeField] RectTransform _rotationRect;

    [SerializeField] RadialMenuFraction[] _fractions;

    [SerializeField] UnityEvent<int> _onValueChange;

    float _stepAngle;
    public int currentSelected { get; private set; } = -1;
    public int elementCount { get { return ElementCollection.Elements.Length; } }
    public int activeElementCount { get {
            int i = 0;
            foreach(var e in ElementCollection.Elements)
            {
                if (!e.Inactive)
                    i++;
            }
            return i; 
        } }
    public RadialMenuFraction currentFraction { get { return _fractions[currentSelected]; } }
    bool _isDragging = false;

    private void Awake()
    {
        //BuildMenu();
        _stepAngle = 360f / ElementCollection.Elements.Length;

        for (int i = 0; i < _fractions.Length; i++)
        {
            if (ElementCollection.Elements[i].Inactive)
                continue;

            //It's important to use an auxiliary variable that gets created on the loop
            //This allows the callback have access to the value from memory
            int aux = i;

            _fractions[i].Button.onClick.AddListener(() => {
                //Debug.Log(aux);
                if (_isDragging || !Interactable)
                    return;

                SelectItem(aux);
                //_elementCollection.Elements[aux].Callback?.Invoke();
            });
        }

        //Set the _firstSelected item in the list as selected
        //SelectItem(ElementCollection.FirstSelected);
    }

    private void OnValidate()
    {
        if (ElementCollection == null || Application.isPlaying)
            return;

        UpdateFractionAppereance();
    }

    [ContextMenu("Build Menu")]
    public void BuildMenu()
    {
        var frac = GetComponentsInChildren<RadialMenuFraction>(true);
        foreach (var f in frac)
        {
                if(f != null)
            {
                f.Button.onClick.RemoveAllListeners();

                if (!Application.isPlaying)
                    DestroyImmediate(f.gameObject);
                else
                {
                    PoolingManager.Instance.Despawn(f.gameObject);
                }
            }
        }

        _fractions = new RadialMenuFraction[ElementCollection.Elements.Length];

        var stepAngle = 360f / ElementCollection.Elements.Length;
        _stepAngle = stepAngle;

        _elementParent.rotation = Quaternion.identity;

        for (int i = 0; i < ElementCollection.Elements.Length; i++)
        {
            if(!Application.isPlaying)
                _fractions[i] = Instantiate(ElementCollection.RadialMenuFractionPrefab, transform);
            else
            {
                _fractions[i] = PoolingManager.Instance.Spawn(ElementCollection.RadialMenuFractionPrefab.gameObject).GetComponent<RadialMenuFraction>();
                _fractions[i].transform.SetParent(transform, false);
                var rt = ((RectTransform)_fractions[i].transform);
                rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, ((RectTransform)transform).rect.width);
                rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, ((RectTransform)transform).rect.height);
                _fractions[i].transform.localScale = Vector3.one;
            }

            SetFractionAppereance(_fractions[i], ElementCollection.Elements[i], i, stepAngle);

            _fractions[i].transform.SetParent(_elementParent, true);

            if (ElementCollection.Elements[i].Inactive)
                continue;

            //It's important to use an auxiliary variable that gets created on the loop
            //This allows the callback have access to the value from memory
            int aux = i;

            _fractions[i].Button.onClick.AddListener(() => {
                //Debug.Log(aux);
                if (_isDragging || !Interactable)
                    return;

                SelectItem(aux);
                //_elementCollection.Elements[aux].Callback?.Invoke();
            });
        }

        if(currentSelected != -1)
            RotateElementParent(-(currentSelected - 1) * _stepAngle);
    }

    void SetFractionAppereance(RadialMenuFraction fraction, RadialMenuElement element, int index, float stepAngle)
    {
        //Make sure the object is properly transformed
        fraction.transform.localPosition = Vector3.zero;
        fraction.transform.localRotation = Quaternion.identity;

        fraction.Circle.fillAmount = 1f / ElementCollection.Elements.Length - Gap / 360f;
        fraction.Circle.transform.localPosition = Vector3.zero;
        fraction.Circle.transform.localRotation = Quaternion.Euler(0, 0, -stepAngle / 2f - Gap / 2f + index * stepAngle);
        //fraction.Circle.color = Color.gray;

        //Debug.Log(_fractions[i].CircleRect.rect.height);

        fraction.Icon.transform.localPosition = fraction.Circle.transform.localPosition + Quaternion.AngleAxis((index - 1) * stepAngle, Vector3.forward) * Vector3.up * ((1f - IconDistance) * fraction.CircleRect.rect.height / 2f);
        fraction.Icon.transform.localRotation = Quaternion.Euler(0, 0, (index - 1) * stepAngle);
        fraction.Icon.transform.localScale = Vector3.one * IconScale;

        fraction.Icon.sprite = element.Icon;
        fraction.Icon.enabled = element.Icon != null;


        fraction.Name.text = element.Name;

        fraction.CircleCutout = CircleCutout;

        fraction.gameObject.SetActive(!element.Inactive);
    }

    public void UpdateFractionAppereance()
    {
        var stepAngle = 360f / ElementCollection.Elements.Length;

        if (_fractions != null && ElementCollection.Elements.Length == _fractions.Length)
            for (int i = 0; i < ElementCollection.Elements.Length; i++)
            {
                if (_fractions[i] != null)
                    SetFractionAppereance(_fractions[i], ElementCollection.Elements[i], i, stepAngle);
            }
    }

    public void InitializeSelection()
    {
        if(currentSelected == -1)
            SelectItem(ElementCollection.FirstSelected);
        else
            SelectItem(currentSelected);
    }

    void SelectItem(int index)
    {
        index = NormalizeIndex(index);

        if (currentSelected == index && (_fractions[index].IsSelected || !_fractions[index].gameObject.activeInHierarchy))
            return;

        if (!_fractions[index].gameObject.activeInHierarchy)
        {
            SelectItem(NormalizeIndex(index + (NormalizeIndex(index - 1) > NormalizeIndex(currentSelected - 1) ? 1 : -1)));
            return;
        }

        RotateElementParent(-(index - 1) * _stepAngle);

        UpdateFractionsIsSelected(index);

        currentSelected = index;

        _onValueChange?.Invoke(index);
    }

    void UpdateFractionsIsSelected(int index)
    {
        for (int j = 0; j < _fractions.Length; j++)
        {
            _fractions[j].SetIsSelected(j == index);
            //_fractions[j].SetMaskAble(j != index);
        }
    }
    public void DiselectCurrentFracction()
    {
        _fractions[currentSelected].SetIsSelected(false);
    }

    void SetItemHoverAble(bool value)
    {
        for (int j = 0; j < _fractions.Length; j++)
        {
            _fractions[j].HoverAble = value;

            if (!value)
                _fractions[j].SetIsSelected(false);
            else
                SelectItem(currentSelected);
        }
    }

    public void SelectionUp()
    {
        if (!_isDragging)
            SelectItem(NormalizeIndex(currentSelected + 1));
    }
    public void SelectionDown()
    {
        if (!_isDragging)
            SelectItem(NormalizeIndex(currentSelected - 1));
    }

    [ContextMenu("Deactivate Item Selection Animation")]
    public void DeactivateItemSelectionAnimation()
    {
        SetItemHoverAble(false);
    }

    void RotateElementParent(float targetAngle)
    {
        if(_elementParent.eulerAngles.z + 360f % 360f != targetAngle + 360f % 360f)
            _elementParent.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, targetAngle), 0.4f).SetUpdate(true).SetEase(_rotationEase);
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
