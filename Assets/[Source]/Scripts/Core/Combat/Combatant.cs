using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Combat;
using Core;
using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditorInternal;
using System.Reflection;
using System.Linq;
using Core.Utilities;
using Sirenix.Utilities;
using Core.Jext;
using Core.Inferno;

public abstract class Combatant : Filler, ICombatable, IDamageable
{
    public int CombatOrder { get; protected set; }

    public List<DamageTypes> Immunities { get; set; } = new List<DamageTypes>();

    [InlineEditor]
    public WalkableTiles walkableTiles = null;
    
    public int teamID, health;
    protected int currentHealth;

    public abstract int CompareTo(ICombatable other);
    public abstract void OnActiveCombatant();

    protected override void Awake()
    {
        base.Awake();
        currentHealth = health;
    }

    protected override void NotifyExistence()
    {
        base.NotifyExistence();
        GameManager.instance.CombatManager.Add(this);
    }

    protected override void NotifyDestruction()
    {
        base.NotifyDestruction();
        GameManager.instance.CombatManager.Remove(this);
    }

    protected virtual void EndTurn()
    {
        GameManager.instance.CombatManager.Next();
    }

    public virtual void OnDamageReceived(int damage, DamageTypes damageType)
    {
        if (Immunities.Contains(damageType))
            return;
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, health);

        if (currentHealth > 0)
            return;

        OnDeath();
    }

    protected virtual void OnDeath()
    {
        NotifyDestruction();
    }
    
    protected Vector3Int PositionOnTileGrid
    {
        get
        {
            return GameManager.instance.TileGrid.WorldToCell(transform.position);
        }
    }

    public override bool CanSee<T>(Node other, T[] set)
    {
        Vector2Int convertedPosition;

        foreach (T t in set)
        {
            convertedPosition = t.ConvertGridToMovePosition(other.Position, Node.Position);

            if (t.footPrint[convertedPosition.x, convertedPosition.y])
            {
                if (!t.directConnectionRequired)
                    return true;
                if (GameManager.instance.CanSee(Node.Position, other.Position))
                    return true;
            }
        }

        return false;
    }

    public List<Vector3Int> AvailableDestinationTiles() //TODO: Better naming
    {
        Tilemap tilemap = GameManager.instance.WalkableTilemap.Tilemap;
        return AvailableDestinationTiles(tilemap);
    }
    public List<Vector3Int> AvailableDestinationTiles(Tilemap tilemap) //TODO: Better naming
    {
        Vector3Int combatantPosition = PositionOnTileGrid;

        List<Vector3Int> totalAvailableTiles = new List<Vector3Int>();

        foreach (Move move in MoveSet.moves) //Goes through every single move in the Unit's movelist...
        {
            totalAvailableTiles.AddRange(CheckMove(combatantPosition, move, tilemap)); //And adds all available destination tiles in a list.
        }

        return totalAvailableTiles;
    }

    #if UNITY_EDITOR
    protected void DebugAvailableDestinations() //TODO: Call this on BeginTurn.
    {
        GameManager gameManager = GameManager.instance;

        if (gameManager.WalkableTilemap != null)
        {
            foreach(Vector3Int availableDestination in AvailableDestinationTiles(gameManager.WalkableTilemap.Tilemap))
            {
                var worldPos = gameManager.WalkableTilemap.Tilemap.CellToWorld(availableDestination) + new Vector3(0, .25f, 0);
                
                //Vector2Int adjustedAvailableDestination = gameManager.NodeIndexOnTileGrid(new Vector2Int(AvailableDestination.x, AvailableDestination.y))
                DebugExtension.DebugArrow(worldPos, Vector3.back, Color.green, 0.01f);
                //Gizmos.DrawWireCube(availableDestination, Vector3.one);
            }       
        }
    }
    #endif
    
    private List<Vector3Int> CheckMove(Vector3Int playerPosition, Move move, Tilemap tilemap)
    {
        List<Vector3Int> availableTiles = new List<Vector3Int>();
        
        Vector3Int startingPosition = playerPosition + new Vector3Int(
            -((move.footPrint.GetLength(0)/2)), -((move.footPrint.GetLength(1)/2)), 0);

        Vector3Int checkPosition = startingPosition;
        
        for (int x = 0; x <= move.footPrint.GetLength(0) -1; x++)
        {
            checkPosition.x = startingPosition.x + x;
            
            for (int y = 0; y <= move.footPrint.GetLength(1) -1; y++)
            {
                checkPosition.y = startingPosition.y + y;
                
                if (move.footPrint[x, y] == true)
                {
                    TileBase tile = null;

                    if (CheckCell(checkPosition, tilemap, out tile))
                    {
                        availableTiles.Add(checkPosition);
                    }
                }
            }
        }

        return availableTiles;
    }

    public bool CheckCell(Vector3Int position, Tilemap tilemap, out TileBase tileBase)
    {
        TileBase tile = tilemap.GetTile(position);

        if (tile != null)
        {
            if (walkableTiles.tiles.Contains(tile))
            {
                tileBase = tile;
                return true;
            }
            Debug.Log("walkableTiles does not contain the tile you're checking");
        }
        
        tileBase = null;
        return false;
    }
    
}