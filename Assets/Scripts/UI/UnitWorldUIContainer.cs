using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class UnitWorldUIContainer : MonoBehaviour
{
    [SerializeField] private Unit unit;
    [SerializeField] private TextMeshProUGUI actionPointsText;
    [SerializeField] private RectTransform actionPointsRectTransform;

    [SerializeField] private RectTransform healthBarRectTransform;
    [SerializeField] private Outline healthBarOutline;
    private RectTransform containerRectTransform;
    private float defaultFontSize;
    private Vector2 defaultActionPointsTextPosition;
    private Vector2 defaultHealthBarScale;
    private Vector2 defaultHealthBarPosition;
    private Vector2 defaultHealthBarOutlineScale;
    private float lowerZoomLimit = 1.5f;
    private float upperZoomLimit = 16f;
    private float minScale = 0.4f;

    // ------------ MONOBEHAVIOR FUNCTIONS -------------------------------------------------------
    private void Awake()
    {
        containerRectTransform = GetComponent<RectTransform>();
        defaultFontSize = actionPointsText.fontSize;
        defaultActionPointsTextPosition = actionPointsRectTransform.anchoredPosition;
        defaultHealthBarScale = healthBarRectTransform.sizeDelta;
        defaultHealthBarPosition = healthBarRectTransform.anchoredPosition;
        defaultHealthBarOutlineScale = healthBarOutline.effectDistance;
    }

    private void Start()
    {
        CameraController.OnZoomChanged += CameraController_OnZoomChanged;
        ActionCameraController.OnActionCameraToggled += CameraManager_OnActionCameraToggled;
        CorrectSize(Camera.main.orthographicSize);
    }
    private void Update()
    {
        Vector3 swag = Camera.main.WorldToScreenPoint(unit.GetWorldPosition() + Vector3.up * 2);
        if (CameraController.Instance.IsTopDownCameraActive())
        {
            swag.y = swag.y + 20;
        }

        containerRectTransform.position = swag;
    }

    void OnDestroy()
    {
        CameraController.OnZoomChanged -= CameraController_OnZoomChanged;
        ActionCameraController.OnActionCameraToggled -= CameraManager_OnActionCameraToggled;
    }

    private void CorrectSize(float cameraSize)
    {
        float scalingFactor = 0;
        if (cameraSize <= CameraController.Instance.GetDefaultZoom())
        {
            scalingFactor = 1 + (CameraController.Instance.GetDefaultZoom() - cameraSize) / (CameraController.Instance.GetDefaultZoom() - lowerZoomLimit);
        }
        else if (cameraSize > CameraController.Instance.GetDefaultZoom())
        {
            scalingFactor = Mathf.Max(1 - (cameraSize - CameraController.Instance.GetDefaultZoom()) / (upperZoomLimit - CameraController.Instance.GetDefaultZoom()), minScale);
        }
        actionPointsText.fontSize = defaultFontSize * scalingFactor;
        actionPointsRectTransform.anchoredPosition = defaultActionPointsTextPosition * scalingFactor;
        healthBarRectTransform.sizeDelta = defaultHealthBarScale * scalingFactor;
        healthBarRectTransform.anchoredPosition = defaultHealthBarPosition * scalingFactor;
        healthBarOutline.effectDistance = defaultHealthBarOutlineScale * scalingFactor;
    }

    // ------------ NEW FUNCTIONS -------------------------------------------------------
    private void CorrectSizeTween(float cameraSize)
    {
        if (actionPointsText == null || actionPointsRectTransform == null || healthBarRectTransform == null)
        {
            return;
        }
        float scalingFactor = 0;
        if (cameraSize <= CameraController.Instance.GetDefaultZoom())
        {
            scalingFactor = 1 + (CameraController.Instance.GetDefaultZoom() - cameraSize) / (CameraController.Instance.GetDefaultZoom() - lowerZoomLimit);
        }
        else if (cameraSize > CameraController.Instance.GetDefaultZoom())
        {
            scalingFactor = Mathf.Max(1 - (cameraSize - CameraController.Instance.GetDefaultZoom()) / (upperZoomLimit - CameraController.Instance.GetDefaultZoom()), minScale);
        }
        DOTween.To(() => actionPointsText.fontSize, x => actionPointsText.fontSize = x, defaultFontSize * scalingFactor, 0.2f);
        DOTween.To(() => actionPointsRectTransform.anchoredPosition, x => actionPointsRectTransform.anchoredPosition = x, defaultActionPointsTextPosition * scalingFactor, 0.2f);
        DOTween.To(() => healthBarRectTransform.sizeDelta, x => healthBarRectTransform.sizeDelta = x, defaultHealthBarScale * scalingFactor, 0.2f);
        DOTween.To(() => healthBarRectTransform.anchoredPosition, x => healthBarRectTransform.anchoredPosition = x, defaultHealthBarPosition * scalingFactor, 0.2f);
        DOTween.To(() => healthBarOutline.effectDistance, x => healthBarOutline.effectDistance = x, defaultHealthBarOutlineScale * scalingFactor, 0.2f);
    }

    // ------------ EVENT LISTENERS -------------------------------------------------------
    private void CameraController_OnZoomChanged(object sender, float cameraSize)
    {
        CorrectSize(cameraSize);
    }
    private void CameraManager_OnActionCameraToggled(object sender, float cameraSize)
    {
        CorrectSizeTween(cameraSize);
    }
}
