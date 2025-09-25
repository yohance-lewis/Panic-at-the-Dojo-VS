using UnityEngine;

public struct Neighbor
{
    public HexAxial neighborHex;
    public int cost;

    public Neighbor(HexAxial neighborHex, int cost)
    {
        this.neighborHex = neighborHex;
        this.cost = cost;
    } 
}
