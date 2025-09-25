using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using DG.Tweening;
using System;
using UnityEngine.InputSystem.Interactions;
using Unity.Mathematics;
using System.Linq;
using System.Collections.Generic;
using Point = System.Tuple<float, float>;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; set; }
    public static event EventHandler<float> OnZoomChanged;
    private Vector2 delta;
    private Vector2 wasd;
    private bool isMovingMouse;

    private float mouseMoveSpeed;
    private float keyboardMoveSpeed;
    private float rotationDirection;
    private bool isActionCameraActive = false;
    private bool isTopDownCameraActive = false;
    [SerializeField] private CinemachineCamera controlledCamera;
    [SerializeField] private CinemachineFollow cinemachineFollow;
    [SerializeField] private CinemachineCamera topDownCamera;
    [SerializeField] private CinemachineThirdPersonFollow cinemachineThirdPersonFollow;
    [SerializeField] private float keyboardMovebaseSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float mouseMoveBaseSpeed;
    [SerializeField] private float lowerZoomLimit = 1f;
    [SerializeField] private float upperZoomLimit = 10f;
    [SerializeField] private float zoomSpeed = 0.5f;
    [SerializeField] private int rotationAmount;
    [SerializeField] private float rotationZoom = 4.5f;
    private bool isCameraMoving = false;
    private const float defaultZoom = 3.5f;
    private float defaultYFollow;
    private const float MINYFOLLOW = 4f;
    private const float MAXYFOLLOW = 20f;
    [SerializeField] private GameObject topDownCameraGameObject;

    private readonly float2[] controlPoints = { new(0), new(0, 0.5f), new(1, -0.25f), new(1) };

    private Unit hoveredUnit;

    // ------------ INPUTSYSTEM FUNCTIONS -------------------------------------------------------
    public void OnLook(InputAction.CallbackContext context)
    {
        if (isActionCameraActive) { return; }

        delta = context.ReadValue<Vector2>();
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (isActionCameraActive) { return; }

        Vector3 newPosition = MouseWorld.GetPostionAtCenterOfCurrentHex();
        if (context.interaction is MultiTapInteraction)
        {
            if (!context.performed) { return; }

            CenterCameraOnPosition(newPosition);
            ResetZoom();

        }
        else if (context.interaction is TapInteraction)
        {
            if (!context.performed) { return; }

            CenterCameraOnPosition(newPosition);
        }
        else if (context.interaction is HoldInteraction)
        {
            isMovingMouse = context.started || context.performed;
        }

    }

    public void OnKeyboardMove(InputAction.CallbackContext context)
    {
        if (isActionCameraActive) { return; }

        wasd = context.ReadValue<Vector2>();
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        if (isActionCameraActive) { return; }

        rotationDirection = context.ReadValue<float>();
    }

    public void OnCenterCamera(InputAction.CallbackContext context)
    {
        if (isActionCameraActive) { return; }
        if (!context.started) { return; }

        TurnSystem.Phase currentPhase = TurnSystem.Instance.GetCurrentPhase();
        switch (currentPhase)
        {
            default:
            case TurnSystem.Phase.UNITSELECTION:
                if (hoveredUnit != null) { CenterCameraOnUnit(hoveredUnit); }
                break;
            case TurnSystem.Phase.PLAY:
                CenterCameraOnUnit(UnitActionSystem.Instance.GetSelectedUnit());
                break;
        }
    }


    public void OnZoom(InputAction.CallbackContext context)
    {
        if (isActionCameraActive) { return; }

        float zoomDirection = context.ReadValue<Vector2>().y;
        float newSize = Math.Clamp(controlledCamera.Lens.OrthographicSize - zoomDirection * zoomSpeed, lowerZoomLimit, upperZoomLimit);
        controlledCamera.Lens.OrthographicSize = newSize;
        topDownCamera.Lens.OrthographicSize = newSize;

        if (controlledCamera.Lens.OrthographicSize <= rotationZoom)
        {
            float t = (controlledCamera.Lens.OrthographicSize - lowerZoomLimit) / (rotationZoom - lowerZoomLimit);
            float scalingFactor = BezierLerp(controlPoints, t);
            float newYFollow = MINYFOLLOW + scalingFactor * (MAXYFOLLOW - MINYFOLLOW);
            float yFollowChange = Math.Abs(newYFollow - cinemachineFollow.FollowOffset.y);
            DOTween.To(() => cinemachineFollow.FollowOffset.y, x => cinemachineFollow.FollowOffset.y = x, newYFollow, yFollowChange * Time.deltaTime);
        }
        else
        {
            cinemachineFollow.FollowOffset.y = MAXYFOLLOW;
        }

        OnZoomChanged?.Invoke(this, controlledCamera.Lens.OrthographicSize);
    }

    public void OnCameraToggle(InputAction.CallbackContext context)
    {
        if (isActionCameraActive) { return; }
        if (!context.started) { return; }

        isTopDownCameraActive = !isTopDownCameraActive;
        topDownCameraGameObject.SetActive(isTopDownCameraActive);
    }

    // ------------ MONOBEHAVIOUR FUNCTIONS -------------------------------------------------------
    private void LateUpdate()
    {
        if (isCameraMoving) { return; }
        keyboardMoveSpeed = keyboardMovebaseSpeed * (controlledCamera.Lens.OrthographicSize / 10);
        mouseMoveSpeed = mouseMoveBaseSpeed * (controlledCamera.Lens.OrthographicSize / 10);
        if (isMovingMouse)
        {
            Vector3 position = transform.right * (delta.x * mouseMoveSpeed);
            position += transform.forward * (delta.y * mouseMoveSpeed);
            transform.position += position * Time.deltaTime;

        }
        else
        {
            Vector3 moveVector = transform.forward * wasd.y + transform.right * wasd.x;
            transform.position += keyboardMoveSpeed * Time.deltaTime * moveVector;
        }
        transform.eulerAngles += rotationSpeed * Time.deltaTime * rotationDirection * Vector3.up;
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        float t = (defaultZoom - lowerZoomLimit) / (rotationZoom - lowerZoomLimit);

        float scalingFactor = BezierLerp(controlPoints, t);
        defaultYFollow = 4 + scalingFactor * 16;
    }

    private void Start()
    {
        BaseAction.OnAnyActionStarted += BaseAction_OnAnyActionStarted;
        BaseAction.OnAnyActionCompleted += BaseAction_OnAnyActionCompleted;
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
        TurnSystem.Instance.OnPhaseChanged += TurnSystem_OnPhaseChanged;
        UnitButtonUI.OnHover += UnitButtonUI_OnHover;
    }

    // ------------ NEW FUNCTIONS -------------------------------------------------------
    private void CenterCameraOnUnit(Unit unit, float duration = 0.5f, bool doZoom = true)
    {
        CenterCameraOnPosition(LevelGridHex.Instance.GetWorldPosition(unit.GetHexAxial()), duration);
        if (doZoom) { ResetZoom(); }
    }

    private void CenterCameraOnPosition(Vector3 position, float duration = 0.5f)
    {
        isCameraMoving = true;
        transform.DOMove(position, duration).SetEase(Ease.OutCubic).OnComplete(() => isCameraMoving = false);

    }

    private void ResetZoom()
    {
        isCameraMoving = true;
        float zoomTime = 0.25f;
        DOTween.To(() => controlledCamera.Lens.OrthographicSize, x => controlledCamera.Lens.OrthographicSize = x, defaultZoom, zoomTime).SetEase(Ease.OutCubic).OnComplete(() => isCameraMoving = false);
        DOTween.To(() => topDownCamera.Lens.OrthographicSize, x => topDownCamera.Lens.OrthographicSize = x, defaultZoom, zoomTime).SetEase(Ease.OutCubic);
        DOTween.To(() => cinemachineFollow.FollowOffset.y, x => cinemachineFollow.FollowOffset.y = x, defaultYFollow, zoomTime);
        OnZoomChanged?.Invoke(this, defaultZoom);
    }

    private void CenterCameraOnUnits(List<Unit> currentTeamList)
    {
        Vector3 cameraPosition = new();
        foreach (Unit unit in currentTeamList)
        {
            cameraPosition += unit.GetWorldPosition();
        }
        cameraPosition /= currentTeamList.Count;

        float longestDistance = 0f;
        for (int i = 0; i < currentTeamList.Count - 1; i++)
        {
            for (int j = i; j < currentTeamList.Count; j++)
            {
                float testDistance = Vector3.Distance(currentTeamList[i].GetWorldPosition(), currentTeamList[j].GetWorldPosition());
                if (testDistance > longestDistance)
                {
                    longestDistance = testDistance;
                }
            }
        }

        CenterCameraOnPosition(cameraPosition);
        float newSize = Math.Clamp(lowerZoomLimit + longestDistance / LevelGridHex.Instance.GetMaxDsitance() * (upperZoomLimit - lowerZoomLimit), lowerZoomLimit, upperZoomLimit);
        DOTween.To(() => controlledCamera.Lens.OrthographicSize, x => controlledCamera.Lens.OrthographicSize = x, newSize, 0.25f).SetEase(Ease.OutCubic);
        DOTween.To(() => topDownCamera.Lens.OrthographicSize, x => topDownCamera.Lens.OrthographicSize = x, newSize, 0.25f).SetEase(Ease.OutCubic);

        float t = (newSize - lowerZoomLimit) / (rotationZoom - lowerZoomLimit);
        float scalingFactor = BezierLerp(controlPoints, t);
        if (newSize <= rotationZoom)
        {
            float newYFollow = MINYFOLLOW + scalingFactor * (MAXYFOLLOW - MINYFOLLOW);
            DOTween.To(() => cinemachineFollow.FollowOffset.y, x => cinemachineFollow.FollowOffset.y = x, newYFollow, 0.25f);
        }
        else
        {
            DOTween.To(() => cinemachineFollow.FollowOffset.y, x => cinemachineFollow.FollowOffset.y = x, MAXYFOLLOW, 0.25f);
        }

        OnZoomChanged?.Invoke(this, newSize);
    }

    // ------------ MATH UTILS -------------------------------------------------------
    private float BezierLerp(float2[] controlPoints, float t)
    {
        int n = controlPoints.Count();
        if (n == 0)
        {
            throw new Exception("List of control points is empty");
        }
        float y = 0;

        for (int j = 0; j < n; j++)
        {
            y += MathF.Pow(1 - t, n - 1 - j) * Mathf.Pow(t, j) * nCr(n - 1, j) * controlPoints[j].y;
        }
        return y;
    }
    public int nCr(int n, int r) //will probably move all of the math things to a utils file
    {
        if (r > n) { return 0; }
        if (r == 0 || r == n) { return 1; }

        return nCr(n - 1, r - 1) + nCr(n - 1, r);
    }
    // ------------ EVENT LISTENERS -------------------------------------------------------
    private void BaseAction_OnAnyActionStarted(object sender, EventArgs e)
    {
        switch (sender)
        {
            case OffensiveAction:
                isActionCameraActive = true;
                break;
        }
    }

    private void BaseAction_OnAnyActionCompleted(object sender, EventArgs e)
    {
        switch (sender)
        {
            case OffensiveAction:
                isActionCameraActive = false;
                break;
        }
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e)
    {
        CenterCameraOnUnit(UnitActionSystem.Instance.GetSelectedUnit(), 0.75f);
    }

    private void TurnSystem_OnPhaseChanged(TurnSystem.Phase currentPhase)
    {
        switch (currentPhase)
        {
            case TurnSystem.Phase.UNITSELECTION:
                List<Unit> currentTeamList = TurnSystem.Instance.GetCurrentTeam().GetAliveUnits();
                CenterCameraOnUnits(currentTeamList);
                hoveredUnit = null;
                break;
        }
    }

    private void UnitButtonUI_OnHover(Unit unit)
    {
        if (hoveredUnit == null || hoveredUnit != unit)
        {
            hoveredUnit = unit;
            CenterCameraOnUnit(unit, doZoom: false);
        }
    }

    // ------------ GETTERS -------------------------------------------------------
    public bool IsTopDownCameraActive()
    {
        return isTopDownCameraActive;
    }
    public float GetDefaultZoom()
    {
        return defaultZoom;
    }
}