using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Core.Pathfinding;
using Core.Utilities;
using Core.MenuHolder;
using Combat;

public class GameManager : PersistentSingleton<GameManager>
{
    public CombatManager CombatManager { get; private set; }
    public Pathfinding Pathfinding { get; private set; }
    public Node[,] Grid { get; private set; }

    [SerializeField]
    private int maxCombatants;

    protected override void Awake()
    {
        base.Awake();
        CombatManager = new CombatManager(maxCombatants, false);
    }

    public void SetupLevel(Node[,] grid)
    {
        Grid = grid;
        Pathfinding = new Pathfinding(grid);
    }

    private void OnLevelWasLoaded(int level)
    {
        CombatManager.Clear();
    }
}
