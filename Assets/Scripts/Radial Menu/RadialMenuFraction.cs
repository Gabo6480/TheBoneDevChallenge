using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;
using Coffee.UISoftMask;

public class RadialMenuFraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPoolable
{
    public Image Icon;
    public Image Circle;
    public RectTransform CircleRect;
    public TMP_Text Name;
    public Button Button;

    [SerializeField] [Range(0f, 0.5f)] float _circleCutout = 0.25f;

    public float CircleCutout { 
        get { return _circleCutout; }
        set {
            _circleCutout = value;
            if (_circleMaterial != null)
            {
                //Debug.Log("Update Circle Cutout: " + value);

                if(gameObject.activeInHierarchy && _circleSoftMaskable.modifiedMaterial != null)
                    _circleSoftMaskable.modifiedMaterial.SetFloat("_Cutout_Radius", _circleCutout);
                else
                    _circleMaterial.SetFloat("_Cutout_Radius", _circleCutout);

                Circle.material = _circleMaterial;

                //Circle.SetMaterialDirty();
            }
        }
    }

    [HideInInspector]
    public bool HoverAble = true;

    [Header("Appereance")]
    [SerializeField] Color _selectedColorMod = Color.white;
    [SerializeField] Color _unselectedColorMod = Color.gray;
    [SerializeField] float _selectedTargetScale = 1.05f;
    [Space]
    [SerializeField] SoftMaskable _iconSoftMaskable;
    [SerializeField] SoftMaskable _circleSoftMaskable;
    [SerializeField] SoftMaskable _nameSoftMaskable;


    float _originalCircleScale = 1;

    public bool IsSelected { get; private set; } = false;

    Tween _hoverCircleColorTween;
    Tween _hoverIconColorTween;
    Tween _hoverNameColorTween;
    Tween _hoverScaleTween;

    Tween _selectedCircleColorTween;
    Tween _selectedIconColorTween;
    Tween _selectedNameColorTween;
    Tween _selectedScaleTween;

    Color _originalCircleColor;
    Color _originalIconColor;
    Color _originalNameColor;

    Color _selectedCircleColor;
    Color _unselectedCircleColor;

    Color _selectedIconColor;
    Color _unselectedIconColor;

    Color _selectedNameColor;
    Color _unselectedNameColor;

    Material _circleMaterial;

    public void OnSpawned()
    {
        Circle.color = _unselectedCircleColor;
        Icon.color = _unselectedIconColor;
        Name.color = _unselectedNameColor;
        transform.localScale = Vector3.one * _originalCircleScale;

        _circleMaterial.SetFloat("_Cutout_Radius", _circleCutout);

        IsSelected = false;
        HoverAble = true;
        SetMaskAble(true);
    }
    private void Awake()
    {
        _originalCircleColor = Circle.color;
        _originalIconColor = Icon.color;
        _originalNameColor = Name.color;

        _selectedCircleColor = _originalCircleColor * _selectedColorMod;
        _unselectedCircleColor = _originalCircleColor * _unselectedColorMod;

        _selectedIconColor = _originalIconColor * _selectedColorMod;
        _unselectedIconColor = _originalIconColor * _unselectedColorMod;

        _selectedNameColor = _originalNameColor * _selectedColorMod;
        _unselectedNameColor = _originalNameColor * _unselectedColorMod;

        _originalCircleScale = Circle.transform.localScale.x;

        _circleMaterial = Instantiate(Circle.material);
        Circle.material = _circleMaterial;

        OnSpawned();
    }

    private void OnValidate()
    {
        if (Circle != null)
        {
            CircleRect = Circle.GetComponent<RectTransform>();
            if(_circleMaterial != null)
                _circleMaterial.SetFloat("_Cutout_Radius", _circleCutout);
            _circleSoftMaskable = Circle.GetComponent<SoftMaskable>();
        }

        if (Icon != null)
            _iconSoftMaskable = Icon.GetComponent<SoftMaskable>();

        if (Name != null)
            _nameSoftMaskable = Name.GetComponent<SoftMaskable>();
    }

    public void SetIsSelected(bool value)
    {
        if (IsSelected == value || !HoverAble)
            return;

        IsSelected = value;

        _selectedCircleColorTween.Kill();
        _selectedIconColorTween.Kill();
        _selectedNameColorTween.Kill();
        _selectedScaleTween.Kill();

        if (IsSelected)
        {
            _selectedCircleColorTween = Circle.DOColor(_selectedCircleColor, 0.3f).SetDelay(0.1f);
            _selectedIconColorTween = Icon.DOColor(_selectedIconColor, 0.3f).SetDelay(0.1f);
            _selectedNameColorTween = Name.DOColor(_selectedNameColor, 0.3f).SetDelay(0.1f);
            _selectedScaleTween = transform.DOScale(Vector3.one *  _selectedTargetScale, 0.3f).SetDelay(0.1f);
        }
        else
        {
            _selectedCircleColorTween = Circle.DOColor(_unselectedCircleColor, 0.2f);
            _selectedIconColorTween = Icon.DOColor(_unselectedIconColor, 0.2f);
            _selectedNameColorTween = Name.DOColor(_unselectedNameColor, 0.2f);
            _selectedScaleTween = transform.DOScale(Vector3.one * _originalCircleScale, 0.2f);
        }

    }

    public void SetMaskAble(bool value)
    {
        _circleSoftMaskable.enabled = value;
        _iconSoftMaskable.enabled = value;
        _nameSoftMaskable.enabled = value;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (IsSelected || !HoverAble)
            return;

        _hoverCircleColorTween.Kill();
        _hoverIconColorTween.Kill();
        _hoverNameColorTween.Kill();
        _hoverScaleTween.Kill();

        _hoverCircleColorTween = Circle.DOColor(Color.Lerp(_selectedCircleColor, _unselectedCircleColor, 0.5f), 0.3f);
        _hoverIconColorTween = Icon.DOColor(Color.Lerp(_selectedIconColor, _unselectedIconColor, 0.5f), 0.3f);
        _hoverNameColorTween = Name.DOColor(Color.Lerp(_selectedNameColor, _unselectedNameColor, 0.5f), 0.3f);
        _hoverScaleTween = transform.DOScale(Vector3.one * Mathf.Lerp(_selectedTargetScale, _originalCircleScale, 0.5f), 0.3f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(IsSelected || !HoverAble)
            return;

        _hoverCircleColorTween.Kill();
        _hoverIconColorTween.Kill();
        _hoverNameColorTween.Kill();
        _hoverScaleTween.Kill();

        _hoverCircleColorTween = Circle.DOColor(_unselectedCircleColor, 0.2f);
        _hoverIconColorTween = Icon.DOColor(_unselectedIconColor, 0.2f);
        _hoverNameColorTween = Name.DOColor(_unselectedNameColor, 0.2f);
        _hoverScaleTween = transform.DOScale(Vector3.one * _originalCircleScale, 0.3f);
    }

    private void OnDisable()
    {
        Circle.DOKill();
        Icon.DOKill();
        Name.DOKill();
        transform.DOKill();
    }

    public string getName()
    {
        return "RadialMenuFraction";
    }
}
