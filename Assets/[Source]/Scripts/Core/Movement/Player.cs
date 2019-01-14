using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Combat;
using Sirenix.OdinInspector;
using Core.Jext;
using Core.Managers;

public class Player : Combatant
{
    [InlineEditor]
    public WalkableTiles walkableTiles = null;

    public Vector3Int PositionOnGrid
    {
        get
        {
            return SelectionManager.instance.grid.WorldToCell(transform.position);
            
            //TODO: Replace SelectionManager with GameManger.SelectionManager.
        }
    }
    
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

        Vector2Int playerNode = gameManager.TileIndexOnNavGrid(PositionOnGrid);
        
        Debug.Log(playerNode);
        
        Node = gameManager.NavGrid[playerNode.x, playerNode.y];

    }

    //public virtual void Convert
    
    /// <summary>
    /// Moves the Player to destination.
    /// </summary>
    /// <param name="destination"></param>
    public virtual void Move(Vector2Int navGridDestination, Vector3Int worldDestination)
    {
        //Vector2Int pathDestination = new Vector2Int(destination.x, destination.y); 

        GameManager gameManager = GameManager.instance;
        
        Debug.Log(string.Format("Destination = {0}", navGridDestination));
        
        List<Node> path = gameManager.GetPath(this, navGridDestination);

        Debug.Log(string.Format("Path.Count = {0}", path.Count));
        
        if (path.Count > 0)
        {
            Debug.Log("De haarbal rolt");

            Node = gameManager.NavGrid[navGridDestination.x, navGridDestination.y]; 

            transform.position = worldDestination; 
            
            EndTurn();   
        }
    }
}
