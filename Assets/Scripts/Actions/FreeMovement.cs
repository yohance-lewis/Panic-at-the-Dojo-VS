// using System;
// using System.Collections.Generic;
// using UnityEngine;

// public class FreeMovement : BaseAction
// {
//     public event EventHandler OnStartMoving;
//     public event EventHandler OnStopMoving;
//     private List<Vector3> positionList;
//     private int currentPositionIndex;
//     [SerializeField] private int maxMoveDistance;
//     private readonly float stoppingDistance = 0.03f;
//     private readonly float rotateSpeed = 10f;
//     private float moveSpeed = 4f;

//     // ------------ MONOBEHAVIOUR FUNCTIONS -------------------------------------------------------
//     protected override void Awake()
//     {
//         base.Awake();
//         actionName = "move";
//     }

//     private void Update()
//     {
//         if (!isActive)
//         {
//             return;
//         }
//         Vector3 targetPosition = positionList[currentPositionIndex];
//         Vector3 moveDirection = (targetPosition - transform.position).normalized;

//         transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);

//         if (Vector3.Distance(targetPosition, transform.position) > stoppingDistance) // handles movement
//         {
//             transform.position += moveSpeed * Time.deltaTime * moveDirection;
//         }
//         else
//         {
//             currentPositionIndex++;
//             unit.RemoveSpeedTokens();
//             if (currentPositionIndex >= positionList.Count)
//             {
//                 OnStopMoving?.Invoke(this, EventArgs.Empty);
//                 ActionComplete();
//             }
//         }
//     }

//     // ------------ ABSTRACT FUNCTIONS -------------------------------------------------------
//     public override void TakeAction(HexAxial hexAxial, Action onActionComplete)
//     {
//         List<HexAxial> pathHexAxialList = Pathfinding.Instance.FindPath(unit.GetHexAxial(), hexAxial);
//         currentPositionIndex = 0;
//         positionList = new();

//         foreach(HexAxial pathHexAxial in pathHexAxialList)
//         {
//             positionList.Add(LevelGridHex.Instance.GetWorldPosition(pathHexAxial));
//         }
        

//         OnStartMoving?.Invoke(this, EventArgs.Empty);

//         ActionStart(onActionComplete);
//     }

//     public override List<HexAxial> GetValidActionHexAxialList()
//     {
//         maxMoveDistance = unit.GetSpeedTokens();
//         List<HexAxial> validHexAxialList = new();
//         BFS(validHexAxialList);
//         return validHexAxialList;
//     }
    
//     protected override void CheckValiditySearchability(HexAxial hexAxial, HexAxial unitHexAxial, int newLevel,Queue<HexAxial> q, List<HexAxial> validHexAxialList, Dictionary<HexAxial, int> levelDict)
//     {
//         if (!LevelGridHex.Instance.IsValidHexAxial(hexAxial)
//             || unitHexAxial == hexAxial
//             || !Pathfinding.Instance.GetNode(hexAxial).IsWalkable()
//             )
//         {
//             return;
//         }

//         validHexAxialList.Add(hexAxial);

//         if (newLevel < maxMoveDistance)
//         {
//             q.Enqueue(hexAxial);
//             levelDict.Add(hexAxial, newLevel);
//         }
//     }
// }
