using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Combat;
using Sirenix.OdinInspector;
using Core.Jext;
using Core.Managers;

public class Player : Combatant
{    
   public override int CompareTo(ICombatable other)
    {
        return CombatOrder - other.CombatOrder;
    }
    
    public override void OnActiveCombatant()
    {
        List<Combatant> combatants = GameManager.instance.CombatManager.Combatants;

        foreach (Combatant combatant in combatants)
        {
            if (combatant.teamID == teamID)
                continue;
            return;
        }
    }

    private void Start()
    {
        GameManager gameManager = GameManager.instance;

        Vector2Int playerNode = gameManager.TileIndexOnNavGrid(PositionOnTileGrid);
        
        Node = gameManager.NavGrid[playerNode.x, playerNode.y];
    }

    private void Update()
    {
        DebugAvailableDestinations(); //TODO: Only Update this on Moves.
    }

    /// <summary>
    /// Moves the Player to destination (if possible).
    /// </summary>
    /// <param name="destination"></param>
    public virtual void Move(Vector2Int navGridDestination, Vector3 worldDestination)
    {
        GameManager gameManager = GameManager.instance;
        
        List<Node> path = gameManager.GetPath(this, navGridDestination);

        var myPosition = gameManager.TileIndexOnNavGrid(PositionOnTileGrid);
        
        Debug.Log(string.Format("Path.Count from {0} to {1} = {2}", myPosition, navGridDestination, path.Count));
        
        if (path.Count > 0)
        {
            Debug.Log("Path is higher than 0");
            
            Node = gameManager.NavGrid[navGridDestination.x, navGridDestination.y];

            transform.position = worldDestination;
            
            //EndTurn(); //TODO: Functionality.
        }
    }
}
