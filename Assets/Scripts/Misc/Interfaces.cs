using System;
using System.Collections.Generic;
using UnityEngine;

public interface ICanBeDamaged
{
    void Damage(int damageAmount);

    HexAxial GetHexAxial();

    Vector3 GetWorldPosition();
}

public interface IClickableAction
{
    void OnClick();
}

public interface IHasNeighbors
{
    List<Neighbor> GetNeighbors();
    void AddNeighbor(HexAxial hexAxial);
    void RemoveNeighbor(HexAxial hexAxial);

}
