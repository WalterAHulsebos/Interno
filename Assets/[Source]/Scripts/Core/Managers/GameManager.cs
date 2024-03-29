﻿using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEditorInternal;

using Core.Pathfinding;
using Core.Utilities;
using Core.Combat;
using Core.Jext;
using Core.Inferno;
using Core;

public class GameManager : PersistentSingleton<GameManager>
{
    /// <summary>
    /// Used to determine turn order. Combatants will automatically be added and removed from this object.
    /// </summary>
    public CombatManager<Combatant> CombatManager { get; private set; }
    
    /// <summary>
    ///  Use this to get paths between positions.
    /// </summary>
    public Pathfinding Pathfinding { get; private set; }

    /// <summary>
    /// The grid used for pathfinding.
    /// </summary>
    public Node[,] NavGrid { get; private set; }

    /// <summary>
    /// Used to determine cache sizes.
    /// </summary>
    [SerializeField]
    private int maxCombatants = 0, maxFiller = 0;

    #region Loading Level
    [SerializeField]
    private RectTransform loadScreen = null, mainMenu = null, ingameMenu = null;
    [SerializeField]
    private Image loadBar = null;
    [SerializeField]
    private Button startButton = null;
    private AsyncOperation asyncLoad;
    #endregion

    #region Level Data
    
    /// <summary>
    ///  All the filler in the current level. Filler will be automatically added and removed.
    /// </summary>
    [NonSerialized] public List<Filler> filler;
    
    public GridLayout TileGrid { get; private set; } //TODO: I don't know if this belongs here, move somewhere else maybe?
    
    [NonSerialized] public List<TileMapComponent> tileMapComponents = new List<TileMapComponent>();
    
    /// <summary>
    /// A TileMapComponent contains a Tilemap, TilemapCollider2D, TilemapRenderer and it's SortingLayerIndex 
    /// </summary>
    public class TileMapComponent : IComparable<TileMapComponent>
    {
        public Tilemap Tilemap { get; set; }
            
        public TilemapCollider2D Collider2D { get; set; }
            
        public TilemapRenderer Renderer { get; set; }
    
        public int SortingLayerIndex
        {
            get
            {
                if (Renderer != null)
                {
                    return SortingLayers.FindIndex(i => i == Renderer.sortingLayerName);
                }
                else
                {
                    Debug.LogError("No Renderer Component on this TileMap");
                    return 32; //return index higher than possible for SortingLayer (will always be rendered on top)
                }
            }
        }
            
        /// <summary>
        /// Compares TileMap Layers against each other.
        /// </summary>
        public int CompareToLayer(TileMapComponent other)
        {
            if (other == null) return 1;
                
            if (other.SortingLayerIndex < SortingLayerIndex) return 1;
    
            if (other.SortingLayerIndex == SortingLayerIndex) return 0;
    
            return -1;
        }
            
        /// <summary>
        /// Compares TileMap Sorting Orders against each other.
        /// </summary>
        public int CompareTo(TileMapComponent other)
        {
            if (other == null) return 1;
                
            if (other.Renderer.sortingOrder < Renderer.sortingOrder) return 1;
    
            if (other.Renderer.sortingOrder == Renderer.sortingOrder) return 0;
    
            return -1;
        }
    }

    /// <summary>
    /// The TilemapComponent that is walkable.
    /// </summary>
    public TileMapComponent WalkableTilemap { get; private set; }
    
    private static List<string> SortingLayers
    {
        get
        {
            PropertyInfo sortingLayersProperty = typeof(InternalEditorUtility).GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
            return ((string[])sortingLayersProperty.GetValue(null, new object[0])).ToList();
        }
    }
    
    #endregion

    #region Editor
    
    private bool debugging = false, displayGizmos = true;
    [ContextMenu("Toggle Debugging")]
    void ToggleDebugging()
    {
        Debug.Log(string.Format("Debugging set to: {0}", Convert.ToString(!debugging)));
        debugging = !debugging;
    }
    
    [ContextMenu("Toggle Gizmos")]
    void ToggleGizmos()
    {
        Debug.Log(string.Format("Gizmos set to: {0}", Convert.ToString(!displayGizmos)));
        displayGizmos = !displayGizmos;
    }
    
    #endregion

    private List<Node> nodeCache;

    protected override void Awake()
    {
        base.Awake();
        CombatManager = new CombatManager<Combatant>(maxCombatants, false);
        filler = new List<Filler>(maxFiller);
    }

    /// <summary>
    /// Set level data, initialize pathfinding and node cache.
    /// </summary>
    /// <param name="grid"></param>
    //public void SetupLevel(Node[,] grid)
    public void SetupLevel()
    {
        if (FindObjectOfType<GridLayout>())
        {
            TileGrid = FindObjectOfType<GridLayout>();
        } //Get the levels' gridLayout
        
        TilemapCollider2D[] tilemapColliders = TileGrid.GetComponentsInChildren<TilemapCollider2D>();
        foreach (TilemapCollider2D tilemapCollider in tilemapColliders)
        {
            TilemapRenderer tilemapRenderer = tilemapCollider.GetComponent<TilemapRenderer>(); //Find the attached renderer.
            Tilemap tilemap = tilemapCollider.GetComponent<Tilemap>(); //Find the attached tilemap.

            if (tilemapRenderer != null && tilemap != null)
            {
                TileMapComponent thisTileMapComponent = new TileMapComponent()
                {
                    Tilemap = tilemap,
                    Collider2D = tilemapCollider,
                    Renderer = tilemapRenderer
                };

                tileMapComponents.Add(thisTileMapComponent);

                if (tilemapRenderer.sortingLayerName == "Default")
                {
                    WalkableTilemap = thisTileMapComponent;

                    NavGrid = GenerateGrid(thisTileMapComponent.Tilemap);
                    
                    NavGrid.Init();
                }
            }
        }

        //MethodInfo method = GetType().GetMethod(DebugNavGrid);
        
        InvokeRepeating("DebugNavGrid",0 , 10f); //TODO: Replace this so it only updates on a new turn.
        
        Pathfinding = new Pathfinding(NavGrid);
        nodeCache = new List<Node>(NavGrid.GetLength(0) * NavGrid.GetLength(1));
    }

    private void OnLevelWasLoaded(int level)
    {
        // Reset objects
        CombatManager.Clear();
        filler.Clear();

        // Update UI
        bool b = SceneManager.GetActiveScene().name.Contains("Level_");
        mainMenu.gameObject.SetActive(!b);
        ingameMenu.gameObject.SetActive(b);
    }

    #region Load Level
    public void LoadLevel(int index)
    {
        StartCoroutine(LoadLevelAsync(index));
    }

    public void ReloadLevel()
    {
        LoadLevel(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator LoadLevelAsync(int index)
    {
        asyncLoad = SceneManager.LoadSceneAsync(string.Format("{0}{1}", "Level_", index));
        asyncLoad.allowSceneActivation = false;
        loadScreen.gameObject.SetActive(true);
        startButton.gameObject.SetActive(false);

        while (!asyncLoad.isDone)
        {
            loadBar.fillAmount = asyncLoad.progress + .1f;
            yield return null;
        }

        startButton.gameObject.SetActive(true);
    }

    /// <summary>
    /// Called from a button after a level is loaded
    /// </summary>
    public void AllowSceneActivation()
    {
        asyncLoad.allowSceneActivation = true;
        loadScreen.gameObject.SetActive(false);
    }
    #endregion

    // Not the perfect place for this function, but did it for the cache. Might make a separate script for this.
    /// <summary>
    /// Used for objects to check if an object can see amother object
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public bool CanSee(Vector2Int from, Vector2Int to)
    {
        nodeCache.Clear();
        NavGrid.GetLine(nodeCache, from, to);
        nodeCache.RemoveAt(0);
        nodeCache.RemoveAt(nodeCache.Count - 1);
        return !nodeCache.IsLineInterrupted();
    }

    public bool CanWalk(IMoveable<Node> moveable, Vector2Int from, Vector2Int to)
    {
        nodeCache.Clear();
        NavGrid.GetLine(nodeCache, from, to);
        nodeCache.RemoveAt(0);
        nodeCache.RemoveAt(nodeCache.Count - 1);
        return nodeCache.IsLineWalkable(moveable);
    }

    /// <summary>
    /// Use A* to get a path to target destination
    /// </summary>
    /// <param name="combatant"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public List<Node> GetPath(Combatant combatant, Vector2Int to)
    {
        nodeCache.Clear();
        Pathfinding.GetPath(combatant, to, nodeCache);
        return nodeCache;
    }

    /// <summary>
    /// Convert a LayoutGrid to Pathfinding Grid.
    /// </summary>
    private Node[,] GenerateGrid(Tilemap tilemap)
    {
        tilemap.CompressBounds(); //Recalculates this tilemap's bounds.

        BoundsInt cellBounds = tilemap.cellBounds;   
        
        Vector2Int nodesLength = new Vector2Int(
            Mathf.Abs(cellBounds.xMax) + Mathf.Abs(cellBounds.xMin), 
            Mathf.Abs(cellBounds.yMax) + Mathf.Abs(cellBounds.yMin));  //Get bounds size
        
        Node[,] nodes = new Node[nodesLength.x, nodesLength.y];
        
        //tilemap.cellBounds.allPositionsWithin.GetEnumerator(); ??

        int numberOfTiles = 0;
        
        foreach (Vector3Int position in cellBounds.allPositionsWithin)
        {
            numberOfTiles++;

            Vector2Int gridIndex = TileIndexOnNavGrid(cellBounds, position); 
            
            nodes[gridIndex.x, gridIndex.y] = new Node(){Walkable = tilemap.HasTile(position)};
        }

        Debug.Log(string.Format("NumberofTiles = {0}", numberOfTiles));
        
        return nodes;
    }

    #if UNITY_EDITOR
    private void DebugNavGrid()
    {
        Tilemap tilemap = WalkableTilemap.Tilemap;
        
        foreach (Vector3Int position in tilemap.cellBounds.allPositionsWithin)
        {
            Vector3 worldPosition = tilemap.CellToWorld(position) + new Vector3(0, .25f, 0);

            if ((displayGizmos) && tilemap.HasTile(position))
            {
                DebugExtension.DebugArrow(worldPosition, Vector3.back, Color.blue, 10f);
            }
            else
            {
                DebugExtension.DebugArrow(worldPosition, Vector3.back, Color.red, 10f);
            }
        }
    }
    #endif
    
    /// <summary>
    /// Gets a tiles' index on NavGrid within bounds
    /// </summary>
    /// <param name="tilePosition"></param>
    /// <returns></returns>
    public Vector2Int TileIndexOnNavGrid(Vector3Int tilePosition)
    {
        if (WalkableTilemap != null)
        {
            return TileIndexOnNavGrid(WalkableTilemap.Tilemap.cellBounds, tilePosition);
        }
        else
        {
            foreach (TileMapComponent tileMapComponent in tileMapComponents)
            {
                string sortingLayerName = tileMapComponent.Renderer.sortingLayerName;

                if (sortingLayerName == "Default")
                {
                    return TileIndexOnNavGrid(tileMapComponent.Tilemap.cellBounds, tilePosition);
                }
            }
        }

        Debug.LogError("No Tilemap with sortingLayer 'Default' in scene");
        return Vector2Int.zero;
    }
    
    /// <summary>
    /// Gets a tiles' index on NavGrid within bounds
    /// </summary>
    /// <param name="bounds"></param>
    /// <param name="tilePosition"></param>
    /// <returns></returns>
    public Vector2Int TileIndexOnNavGrid(BoundsInt bounds, Vector3Int tilePosition)
    {
        Vector3Int gridIndex = tilePosition - bounds.min;
        
        if(debugging){Debug.Log(string.Format("TilePosition = {0} , NavGridIndex = {1}", tilePosition, gridIndex));}

        return new Vector2Int(gridIndex.x, gridIndex.y);
    }
   
    
    /// <summary>
    /// Gets a nodes' index on TileGrid within bounds
    /// </summary>
    /// <param name="nodePosition"></param>
    /// <returns></returns>
    public Vector3Int NodeIndexOnTileGrid(Vector2Int nodePosition)
    {
        if (WalkableTilemap != null)
        {
            return NodeIndexOnTileGrid(WalkableTilemap.Tilemap.cellBounds, nodePosition);
        }
        else
        {
            foreach (TileMapComponent tileMapComponent in tileMapComponents)
            {
                string sortingLayerName = tileMapComponent.Renderer.sortingLayerName;

                if (sortingLayerName == "Default")
                {
                    return NodeIndexOnTileGrid(tileMapComponent.Tilemap.cellBounds, nodePosition);
                }
            }
        }

        Debug.LogError("No Tilemap with sortingLayer 'Default' in scene");
        return Vector3Int.zero;
    }
    
    /// <summary>
    /// Gets a nodes' index on TileGrid within bounds
    /// </summary>
    /// <param name="bounds"></param>
    /// <param name="nodePosition"></param>
    /// <returns></returns>
    public Vector3Int NodeIndexOnTileGrid(BoundsInt bounds, Vector2Int nodePosition)
    {
        Vector3Int adjustedNodePosition = new Vector3Int(nodePosition.x, nodePosition.y, 0);
        
        Vector3Int tileGridIndex = adjustedNodePosition + bounds.min;
        
        if(debugging){Debug.Log(string.Format("NodePosition = {0} , TileGridIndex = {1}", nodePosition, tileGridIndex));}

        return tileGridIndex;
    }
}