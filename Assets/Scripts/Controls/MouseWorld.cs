using UnityEngine;
using UnityEngine.InputSystem;

public class MouseWorld : MonoBehaviour
{
    private static MouseWorld Instance;
    [SerializeField] private LayerMask mousePlaneLayerMask;

    // ------------ MONOBEHAVIOUR FUNCTIONS -------------------------------------------------------
    private void Awake()
    {
        Instance = this;
    }
    void Update()
    {
        transform.position = MouseWorld.GetPosition();
    }

    // ------------ GETTERS -------------------------------------------------------
    public static Vector3 GetPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, Instance.mousePlaneLayerMask);
        return raycastHit.point;
    }
    public static HexAxial GetHexAxial()
    {
        return LevelGridHex.Instance.GetHexAxial(MouseWorld.GetPosition());
    }

    public static Vector3 GetPostionAtCenterOfCurrentHex()
    {
        return LevelGridHex.Instance.GetWorldPosition(MouseWorld.GetHexAxial());
    }
}
