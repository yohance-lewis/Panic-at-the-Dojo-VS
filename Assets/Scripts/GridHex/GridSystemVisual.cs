using System;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{
    [SerializeField] private Material defaultWallMaterial;
    [SerializeField] private Material targetedWallMaterial;
    public static GridSystemVisual Instance { get; private set; }
    [Serializable]
    public struct GridVisualTypeMaterial
    {
        public GridVisualType gridVisualType;
        public Material material;
    }
    public enum GridVisualType
    {
        White,
        Blue,
        Red,
        RedSoft,
        Green,
        GreenSoft
    }
    [SerializeField] private Transform gridSystemVisualPrefab;
    [SerializeField] private List<GridVisualTypeMaterial> gridVisualTypeMaterialList;
    private Dictionary<GridVisualType, Material> gridVisualTypeMaterialDict = new();
    private Dictionary<HexAxial, GridSystemVisualSingle> gridSystemVisualSingleDict;
    private List<HexAxial> allObstacles;
    private HexAxial previousHex = new(0, 0);
    private List<HexAxial> tentativePath;

    // ------------ MONOBEHAVIOUR FUNCTIONS -------------------------------------------------------
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        foreach (GridVisualTypeMaterial gridVisualTypeMaterial in gridVisualTypeMaterialList)
        {
            gridVisualTypeMaterialDict.Add(gridVisualTypeMaterial.gridVisualType, gridVisualTypeMaterial.material);
        }

        allObstacles = new();
        tentativePath = new();
    }
    private void Start()
    {
        gridSystemVisualSingleDict = new Dictionary<HexAxial, GridSystemVisualSingle>();
        Dictionary<HexAxial, GridObjectHex>.KeyCollection allHexes = LevelGridHex.Instance.GetAllHexes();

        foreach (HexAxial hexAxial in allHexes)
        {
            Transform gridSystemVisualSingleTransform = Instantiate(gridSystemVisualPrefab, LevelGridHex.Instance.GetWorldPosition(hexAxial), Quaternion.identity);

            gridSystemVisualSingleDict.Add(hexAxial, gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>());

            if (LevelGridHex.Instance.HasObstacleOnHexAxial(hexAxial))
            {
                allObstacles.Add(hexAxial);
            }
        }
        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
        TurnSystem.Instance.OnPhaseChanged += TurnSystem_OnPhaseChanged;
        HideAllHexAxials();
    }

    private void Update()
    {
        if(UnitActionSystem.Instance.IsBusy()){ return; }
        HexAxial currentHex = LevelGridHex.Instance.GetHexAxial(MouseWorld.GetPosition());
        switch (TurnSystem.Instance.GetCurrentPhase())
        {
            case TurnSystem.Phase.TEAMPLACEMENT:
                List<HexAxial> validPlacements = TurnSystem.Instance.GetValidPlacements();
                if (currentHex != previousHex)
                {
                    HideAllHexAxials();
                    previousHex = currentHex;
                    if (validPlacements.Contains(currentHex))
                    {
                        gridSystemVisualSingleDict[currentHex].Show(GetGridVisualTypeMaterial(GridVisualType.Green));
                    }
                    else if(LevelGridHex.Instance.IsValidHexAxial(currentHex))
                    {
                        gridSystemVisualSingleDict[currentHex].Show(GetGridVisualTypeMaterial(GridVisualType.Red));
                    }
                }
                break;

            case TurnSystem.Phase.PLAY:
                if (UnitActionSystem.Instance.GetSelectedAction() is MoveAction)
                {
                    List<HexAxial> valid = UnitActionSystem.Instance.GetSelectedAction().GetValidActionHexAxialList();
                    if (currentHex != previousHex)
                    {
                        previousHex = currentHex;
                        HexAxial unitHex = UnitActionSystem.Instance.GetSelectedUnit().GetHexAxial();
                        foreach (HexAxial hexAxial in tentativePath)
                        {
                            if (!LevelGridHex.Instance.HasAnyUnitOnHexAxial(hexAxial))
                            {
                                gridSystemVisualSingleDict[hexAxial].Show(GetGridVisualTypeMaterial(GridVisualType.GreenSoft));
                            }
                            else
                            {
                                gridSystemVisualSingleDict[hexAxial].Hide();
                            }

                        }

                        if (!valid.Contains(currentHex))
                        {
                            Vector3 startPoint = LevelGridHex.Instance.GetWorldPosition(currentHex);
                            Vector3 endPoint = LevelGridHex.Instance.GetWorldPosition(unitHex);

                            int distance = HexUtilities.AxialDistance(currentHex, unitHex);
                            for (int i = 0; i <= distance; i++)
                            {
                                Vector3 lerp = Vector3.Lerp(startPoint, endPoint, (float)i / distance);
                                HexAxial hexLerp = LevelGridHex.Instance.GetHexAxial(lerp);
                                if (valid.Contains(hexLerp))
                                {
                                    currentHex = hexLerp;
                                    break;
                                }

                            }
                        }
                        if (currentHex != unitHex && valid.Contains(currentHex))
                        {
                            tentativePath = Pathfinding.Instance.FindPath(UnitActionSystem.Instance.GetSelectedUnit().GetHexAxial(), currentHex);
                            ShowHexAxialList(tentativePath, GridVisualType.Green);
                        }
                        else
                        {
                            tentativePath.Clear();
                        }
                    }
                }
                break;
        }
    }

    // ------------ NEW FUNCTIONS -------------------------------------------------------
    public void UpdateGridVisual()
    {
        HideAllHexAxials();
        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();
        GridVisualType gridVisualType;
        List<HexAxial> validActionHexAxialList = selectedAction.GetValidActionHexAxialList();
        bool isOffensive = false;
        switch (selectedAction)
        {

            default:
            case MoveAction:
                gridVisualType = GridVisualType.GreenSoft;
                break;
            case SpinAction:
                gridVisualType = GridVisualType.Blue;
                break;
            case OffensiveAction offensiveAction:
                gridVisualType = GridVisualType.Red;
                ShowHexAxialList(offensiveAction.GetRangeHexAxialList(), GridVisualType.RedSoft);
                isOffensive = true;
                break;
        }
        ShowHexAxialList(
                validActionHexAxialList, gridVisualType, isOffensive
        );
    }

    public void HideAllHexAxials()
    {
        foreach (GridSystemVisualSingle gridSystemVisualSingle in gridSystemVisualSingleDict.Values)
        {
            gridSystemVisualSingle.Hide();
        }

        foreach (HexAxial hexAxial in allObstacles)
        {
            Obstacle obstacle = LevelGridHex.Instance.GetObstacleAtHexAxial(hexAxial);
            if (obstacle != null)
            {
                obstacle.SetGlow(false);
            }
        }
    }

    public void ShowHexAxialList(List<HexAxial> hexAxialList, GridVisualType gridVisualType, bool isShoot = false)
    {
        foreach (HexAxial hexAxial in hexAxialList)
        {
            gridSystemVisualSingleDict[hexAxial].Show(GetGridVisualTypeMaterial(gridVisualType));

            if (!isShoot) { continue; }
            
            if (allObstacles.Contains(hexAxial))
            {
                Obstacle obstacle = LevelGridHex.Instance.GetObstacleAtHexAxial(hexAxial);
                if (obstacle != null){obstacle.SetGlow(true);}
            }  
        }
    }

    // ------------ GETTERS -------------------------------------------------------
    private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType)
    {
        return gridVisualTypeMaterialDict[gridVisualType];
    }

    public List<HexAxial> GetAllObstacles()
    {
        return allObstacles;
    }
    
    // ------------ EVENT LISTENERS -------------------------------------------------------
    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private void TurnSystem_OnPhaseChanged(TurnSystem.Phase currentPhase)
    {
        switch (currentPhase)
        {
            case TurnSystem.Phase.UNITSELECTION:
                HideAllHexAxials();
                break;
            
            case TurnSystem.Phase.END:
                HideAllHexAxials();
                break;
        }
    }
}
