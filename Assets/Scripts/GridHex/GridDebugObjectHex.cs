using UnityEngine;
using TMPro;

public class GridDebugObjectHex : MonoBehaviour
{
    [SerializeField] private TextMeshPro textMeshPro;
    private object gridObjectHex;

    // ------------ MONOBEHAVIOUR FUNCTIONS -------------------------------------------------------
    protected virtual void Update()
    {
        textMeshPro.text = gridObjectHex.ToString();
    }

    // ------------ NEW FUNCTIONS -------------------------------------------------------
    public virtual void SetGridObjectHex(object gridObjectHex)
    {
        this.gridObjectHex = gridObjectHex;
    }
}
