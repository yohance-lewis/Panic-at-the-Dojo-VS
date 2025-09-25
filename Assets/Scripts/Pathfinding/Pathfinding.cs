using System.Collections.Generic;
using Utils;
using UnityEngine;
using UnityEngine.Video;

public class Pathfinding : MonoBehaviour
{
    public static Pathfinding Instance { get; private set; }
    [SerializeField] private Transform gridDebugObjectPrefab;
    [SerializeField] private bool createDebugObjects;
    private GridSystemHex<PathNode> gridSystemHex;

    // ------------ MONOBEHAVIOR FUNCTIONS -------------------------------------------------------
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // ------------ NEW FUNCTIONS -------------------------------------------------------
    public void Setup(int q, int r, int s, bool isHex)
    {
        if (isHex)
        {
            gridSystemHex = new GridSystemHexHexagon<PathNode>(q, r, s, (g, hexAxial) => new PathNode(hexAxial));
        }
        else
        {
            gridSystemHex = new GridSystemHexRectangle<PathNode>(q, r, (g, hexAxial) => new PathNode(hexAxial));
        }
        if (createDebugObjects)
        {
            gridSystemHex.CreateDebugObjects(gridDebugObjectPrefab);
        }
    }

    public List<HexAxial> FindPath(HexAxial startHexAxial, HexAxial endHexAxial)
    {
        PriorityQueue<PathNode, int> nodesToVisit = new();
        List<PathNode> visitedPathNodeList = new();

        PathNode startNode = gridSystemHex.GetGridObjectHex(startHexAxial);
        PathNode endNode = gridSystemHex.GetGridObjectHex(endHexAxial);

        foreach (HexAxial hexAxial in gridSystemHex.GetAllHexes())
        {
            PathNode pathNode = gridSystemHex.GetGridObjectHex(hexAxial);

            pathNode.SetGCost(int.MaxValue);
            pathNode.SetHCost(0);
            pathNode.CalculateFCost();
            pathNode.ResetCameFromPathNode();
        }

        startNode.SetGCost(0);
        startNode.SetHCost(HexUtilities.AxialDistance(startHexAxial, endHexAxial));
        startNode.CalculateFCost();

        nodesToVisit.Enqueue(startNode, startNode.GetFCost());
        while (nodesToVisit.Count > 0)
        {
            PathNode currentNode = nodesToVisit.Dequeue();
            HexAxial currentHexAxial = currentNode.GetHexAxial();
            if (currentNode == endNode)
            {
                return CalculatePath(endNode);
            }

            visitedPathNodeList.Add(currentNode);

            foreach (Neighbor neighbor in gridSystemHex.GetNeighbors(currentHexAxial))
            {
                PathNode neighborNode = GetNode(neighbor.neighborHex);
                if (visitedPathNodeList.Contains(neighborNode))
                {
                    continue;
                }

                if (!neighborNode.IsWalkable())
                {
                    visitedPathNodeList.Add(neighborNode);
                    continue;
                }

                int tentativeGCost = currentNode.GetGCost() + neighbor.cost;

                if (tentativeGCost < neighborNode.GetGCost())
                {
                    neighborNode.SetCameFromPathNode(currentNode);
                    neighborNode.SetGCost(tentativeGCost);
                    neighborNode.SetHCost(HexUtilities.AxialDistance(neighbor.neighborHex, endHexAxial));
                    neighborNode.CalculateFCost();

                    nodesToVisit.Enqueue(neighborNode, neighborNode.GetFCost());
                }

            }
        }
        return null;
    }

    private List<HexAxial> CalculatePath(PathNode endNode)
    {
        List<PathNode> pathNodeList = new()
        {
            endNode
        };
        PathNode currentNode = endNode;
        while (currentNode.GetCameFromPathNode() != null)
        {
            pathNodeList.Add(currentNode.GetCameFromPathNode());
            currentNode = currentNode.GetCameFromPathNode();
        }

        pathNodeList.Reverse();

        List<HexAxial> hexAxialList = new();
        foreach (PathNode pathNode in pathNodeList)
        {
            hexAxialList.Add(pathNode.GetHexAxial());
        }
        return hexAxialList;
    }
    
    // ------------ GETTERS -------------------------------------------------------
    public PathNode GetNode(HexAxial hexAxial)
    {
        return gridSystemHex.GetGridObjectHex(hexAxial);
    }

    public PathNode GetNode(int q, int r)
    {
        return gridSystemHex.GetGridObjectHex(new HexAxial(q,r));
    }
}
