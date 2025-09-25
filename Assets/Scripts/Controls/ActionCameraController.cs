using System;
using Unity.Cinemachine;
using UnityEngine;

public class ActionCameraController : MonoBehaviour
{
    public static event EventHandler<float> OnActionCameraToggled;
    [SerializeField] private GameObject actionCameraGameObject;
    [SerializeField] private GameObject topDownActionCameraGameObject;
    [SerializeField] private Transform actionCameraController;
    private CinemachineCamera actionCinemachineCamera;
    private CinemachineCamera topDownActionCinemachineCamera;
    private float previousCameraSize;
    private const float LOWERZOOMLIMIT = 2f;
    private const float UPPERZOOMLIMIT = 10f;

    // ------------ MONOBEHAVIOUR FUNCTIONS -------------------------------------------------------
    private void Awake()
    {
        actionCinemachineCamera = actionCameraGameObject.GetComponent<CinemachineCamera>();
        topDownActionCinemachineCamera = topDownActionCameraGameObject.GetComponent<CinemachineCamera>();
    }
    private void Start()
    {
        BaseAction.OnAnyActionStarted += BaseAction_OnAnyActionStarted;
        BaseAction.OnAnyActionCompleted += BaseAction_OnAnyActionCompleted;
    }
    // ------------ NEW FUNCTIONS -------------------------------------------------------
    private void ShowActionCamera()
    {
        if (CameraController.Instance.IsTopDownCameraActive())
        {
            topDownActionCameraGameObject.SetActive(true);
        }
        else
        {
            actionCameraGameObject.SetActive(true);
        }
        OnActionCameraToggled?.Invoke(this, Camera.main.orthographicSize);
    }

    private void HideActionCamera()
    {
        topDownActionCameraGameObject.SetActive(false);
        actionCameraGameObject.SetActive(false);
        OnActionCameraToggled?.Invoke(this, previousCameraSize);
    }

    private void SetCameraPostion(OffensiveAction offensiveAction)
    {
        Unit shooterUnit = offensiveAction.GetUnit();
        ICanBeDamaged targetUnit = offensiveAction.GetTarget();
        Vector3 targetPosition;

        switch (offensiveAction)
        {
            case PushAction pushAction:
                targetPosition = LevelGridHex.Instance.GetWorldPosition(pushAction.GetLastInPath());
                break;
            default:
                targetPosition = targetUnit.GetWorldPosition();
                break;
        }
        Vector3 cameraTarget = (targetPosition + shooterUnit.GetWorldPosition()) / 2f;

        Vector3 perpendicularVector = cameraTarget - shooterUnit.GetWorldPosition();


        float distance = Vector3.Distance(targetPosition, shooterUnit.GetWorldPosition());
        float scalingFactor = (distance - LOWERZOOMLIMIT) / LevelGridHex.Instance.GetMaxDsitance();
        actionCameraController.position = cameraTarget;
        actionCameraController.transform.right = perpendicularVector;
        actionCinemachineCamera.Lens.OrthographicSize = LOWERZOOMLIMIT + scalingFactor * (UPPERZOOMLIMIT - LOWERZOOMLIMIT);
        topDownActionCinemachineCamera.Lens.OrthographicSize = actionCinemachineCamera.Lens.OrthographicSize;
    }
    // ------------ NEW FUNCTIONS -------------------------------------------------------
    private void BaseAction_OnAnyActionStarted(object sender, EventArgs e)
    {
        switch (sender)
        {
            case OffensiveAction offensiveAction:
                previousCameraSize = Camera.main.orthographicSize;
                SetCameraPostion(offensiveAction);
                ShowActionCamera();
                break;
        }
    }

    private void BaseAction_OnAnyActionCompleted(object sender, EventArgs e)
    {
        switch (sender)
        {
            case OffensiveAction offensiveAction:
                HideActionCamera();
                break;
        }
    }
}
