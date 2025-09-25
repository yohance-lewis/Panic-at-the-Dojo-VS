using System;
using Unity.Mathematics;

[Serializable]
public struct HexAxial : IEquatable<HexAxial>
{
    public int q;
    public int r;

    // ------------ CONSTRUCTOR -------------------------------------------------------
    public HexAxial(int q, int r)
    {
        this.q = q;
        this.r = r;
    }

    // ------------ TOSTRING -------------------------------------------------------
    public override readonly string ToString()
    {
        return "q: " + q + "; r: " + r;
    }

    // ------------ EQUIVALENCY -------------------------------------------------------
    public static bool operator ==(HexAxial a, HexAxial b)
    {
        return a.q == b.q && a.r == b.r;
    }

    public static bool operator !=(HexAxial a, HexAxial b)
    {
        return !(a == b);
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(q, r);
    }

    public override readonly bool Equals(object obj)
    {
        return obj is HexAxial position && q == position.q && r == position.r;
    }

    public readonly bool Equals(HexAxial other)
    {
        return this == other;
    }

    // ------------ ADDITION/SUBTRACTION -------------------------------------------------------
    public static HexAxial operator +(HexAxial a, HexAxial b)
    {
        return new(a.q + b.q, a.r + b.r);
    }

    public static HexAxial operator -(HexAxial a, HexAxial b)
    {
        return new(a.q - b.q, a.r - b.r);
    }

    public static HexAxial operator *(HexAxial a, int b)
    {
        return new(a.q * b, a.r * b);
    }
}

public struct HexFrac : IEquatable<HexFrac>
{
    public float q;
    public float r;
    public float s;

    // ------------ CONSTRUCTOR -------------------------------------------------------
    public HexFrac(float q, float r, float s)
    {
        this.q = q;
        this.r = r;
        this.s = s;
    }

    // ------------ TOSTRING -------------------------------------------------------
    public override readonly string ToString()
    {
        return "q: " + q + "; r: " + r;
    }

    // ------------ EQUIVALENCY -------------------------------------------------------
    public static bool operator ==(HexFrac a, HexFrac b)
    {
        return a.q == b.q && a.r == b.r && a.s == b.s;
    }

    public static bool operator !=(HexFrac a, HexFrac b)
    {
        return !(a == b);
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(q, r);
    }

    public override readonly bool Equals(object obj)
    {
        return obj is HexFrac position && q == position.q && r == position.r && s == position.s;
    }

    public readonly bool Equals(HexFrac other)
    {
        return this == other;
    }

    // ------------ ADDITION/SUBTRACTION -------------------------------------------------------
    public static HexFrac operator +(HexFrac a, HexFrac b)
    {
        return new(a.q + b.q, a.r + b.r, a.s + b.s);
    }

    public static HexFrac operator -(HexFrac a, HexFrac b)
    {
        return new(a.q - b.q, a.r - b.r, a.s - b.s);
    }

    public static HexFrac operator *(HexFrac a, int b)
    {
        return new(a.q * b, a.r * b, a.s * b);
    }
}

public struct HexCube : IEquatable<HexCube>
{
    public int q;
    public int r;
    public int s;

    // ------------ CONSTRUCTOR -------------------------------------------------------
    public HexCube(int q, int r, int s)
    {
        this.q = q;
        this.r = r;
        this.s = s;
    }

    // ------------ TOSTRING -------------------------------------------------------
    public override readonly string ToString()
    {
        return "q: " + q+ "; r: " + r+ "; s: " + s;
    }

    // ------------ EQUIVALENCY -------------------------------------------------------
    public static bool operator ==(HexCube a, HexCube b)
    {
        return a.q== b.q && a.r== b.r && a.s== b.s;
    }

    public static bool operator !=(HexCube a, HexCube b)
    {
        return !(a == b);
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(q, r);
    }

    public override readonly bool Equals(object obj)
    {
        return obj is HexCube position && q== position.q&& r== position.r && s == position.s;
    }

    public readonly bool Equals(HexCube other)
    {
        return this == other;
    }

    // ------------ ADDITION/SUBTRACTION -------------------------------------------------------
    public static HexCube operator +(HexCube a, HexCube b)
    {
        return new(a.q+ b.q, a.r+ b.r, a.s + b.s);
    }

    public static HexCube operator -(HexCube a, HexCube b)
    {
        return new(a.q- b.q, a.r- b.r, a.s + b.s);
    }
}