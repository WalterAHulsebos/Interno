using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Core.Utilities;
using Combat;

public class GameManager : PersistentSingleton<GameManager>
{
    public CombatManager combatManager;
    public Node[,] grid;

    public void SetupLevel(Node[,] grid)
    {
        this.grid = grid;
    }
}
