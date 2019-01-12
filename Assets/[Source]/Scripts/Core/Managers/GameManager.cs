using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Core.Utilities;
using Combat;
using Movement;

public class GameManager : PersistentSingleton<GameManager>
{
    public CombatManager combatManager;
    public Plotter<Combatant, Node> movement;
    public Node[,] grid;

    public void SetupLevel(Node[,] grid)
    {
        this.grid = grid;
        movement = new Plotter<Combatant, Node>(grid);
    }
}
