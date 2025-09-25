using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Most hex methods derived from: https://www.redblobgames.com/grids/hexagons/
public partial class HexUtilities : MonoBehaviour
{
    public enum Direction
    {
        NORTHWEST,
        NORTHEAST,
        EAST,
        SOUTHEST,
        SOUTHWEST,
        WEST,
    }

    public static Dictionary<Direction, HexAxial> neighbors = new()
    {
        [Direction.NORTHWEST] = new HexAxial(0, 1),
        [Direction.NORTHEAST] = new HexAxial(1, 1),
        [Direction.EAST] = new HexAxial(1, 0),
        [Direction.SOUTHEST] = new HexAxial(0, -1),
        [Direction.SOUTHWEST] = new HexAxial(-1, -1),
        [Direction.WEST] = new HexAxial(-1, 0)
    };

    public static HexCube AxialToCube(HexAxial hexAxial)
    {
        return new HexCube(hexAxial.q, hexAxial.r, -hexAxial.q - hexAxial.r);
    }

    public static HexAxial CubeToAxial(HexCube hexCube)
    {
        return new HexAxial(hexCube.q, hexCube.r);
    }

    public static HexCube CubeRound(HexFrac hexFrac)
    {

        int q = Mathf.RoundToInt(hexFrac.q);
        int r = Mathf.RoundToInt(hexFrac.r);
        int s = Mathf.RoundToInt(hexFrac.s);

        float q_diff = Mathf.Abs(q - hexFrac.q);
        float r_diff = Mathf.Abs(r - hexFrac.r);
        float s_diff = Mathf.Abs(s - hexFrac.s);

        if (q_diff > r_diff && q_diff > s_diff)
        { q = -r - s; }
        else if (r_diff > s_diff)
        { r = -q - s; }
        else
        { s = -q - r; }


        return new HexCube(-q, r, s);
    }

    public static HexAxial AxialRound(HexFrac hexFrac)
    {
        return CubeToAxial(CubeRound(hexFrac));
    }

    public static int AxialDistance(HexAxial a, HexAxial b)
    {
        return (Math.Abs(-a.q + b.q) + Math.Abs(-a.q + a.r + b.q - b.r) + Math.Abs(a.r - b.r)) / 2;
    }
}
