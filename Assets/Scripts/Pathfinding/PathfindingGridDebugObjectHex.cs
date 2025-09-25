using TMPro;
using UnityEngine;

public class PathfindingGridDebugObjectHex : GridDebugObjectHex
{
    [SerializeField] private TextMeshPro gCostText;
    [SerializeField] private TextMeshPro hCostText;
    [SerializeField] private TextMeshPro fCostText;

    private PathNode pathNode;
    // ------------ MONOBEHAVIOR FUNCTIONS -------------------------------------------------------
    protected override void Update()
    {
        base.Update();
        gCostText.text = pathNode.GetGCost().ToString();
        hCostText.text = pathNode.GetHCost().ToString();
        fCostText.text = pathNode.GetFCost().ToString();
    }

    // ------------ NEW FUNCTIONS -------------------------------------------------------
    public override void SetGridObjectHex(object gridObjectHex)
    {
        pathNode = (PathNode)gridObjectHex;
        base.SetGridObjectHex(gridObjectHex);
    }
}
