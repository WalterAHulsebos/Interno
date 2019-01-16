using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditorInternal;
using System.Reflection;
using System.Linq;
using Core.Utilities;
using Sirenix.Utilities;

namespace Core.Managers
{
    public class SelectionManager : PersistentSingleton<SelectionManager>
    {    
        //TODO: Only Execute when playing (not in menu)
        //TODO: Summaries
        
        #region Variables    
        
        [SerializeField] private GameObject selectionOutlinePrefab = null;
        private GameObject selectionOutline = null;

        private Player player = null;

        private bool canSelectTile = false;
        
        [NonSerialized] public Camera selectionCamera;

        [NonSerialized] public GridLayout grid;
    
        private List<TileMapComponent> tileMapComponents = new List<TileMapComponent>();
        public class TileMapComponent : IComparable<TileMapComponent>
        {
            public Tilemap Tilemap { get; set; }
            
            public TilemapCollider2D Collider2D { get; set; }
            
            public TilemapRenderer Renderer { get; set; }
    
            private int SortingLayerIndex
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
    
        private static List<string> SortingLayers
        {
            get
            {
                PropertyInfo sortingLayersProperty = typeof(InternalEditorUtility).GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
                return ((string[])sortingLayersProperty.GetValue(null, new object[0])).ToList();
            }
        }
        
        #endregion

        private void Start() //TODO: Remove this and call Initialize from the GameManager
        {
            Initialize();
        }

        private void Initialize()
        {    
            tileMapComponents = new List<TileMapComponent>(); //Reset Tilemap Components list.
            
            if (selectionCamera == null)
            {
                selectionCamera = Camera.main ? Camera.main : FindObjectOfType<Camera>();
            } //Get Selection Camera

            if (FindObjectOfType<GridLayout>())
            {
                grid = FindObjectOfType<GridLayout>();
            } //Get the level's grid

            if (selectionOutline == null)
            {
                selectionOutline = Instantiate(selectionOutlinePrefab, transform);
            }

            if (player == null)
            {
                player = FindObjectOfType<Player>();

                if (player == null)
                {
                    Debug.LogError("No Player in scene");
                }
            }

            TilemapCollider2D[] tilemapColliders = grid.GetComponentsInChildren<TilemapCollider2D>(); 
            //FindObjectsOfType<TilemapCollider2D>(); //Only collect tilemaps that have colliders
    
            foreach (TilemapCollider2D tilemapCollider in tilemapColliders)
            {
                TilemapRenderer tilemapRenderer = tilemapCollider.GetComponent<TilemapRenderer>(); //Find the attached renderer.

                Tilemap tilemap = tilemapCollider.GetComponent<Tilemap>(); //Find the attached tilemap.

                if (tilemapRenderer != null)
                {
                    tileMapComponents.Add(new TileMapComponent()
                    {
                        Tilemap = tilemap,
                        Collider2D = tilemapCollider,
                        Renderer = tilemapRenderer
                    });
                }
            }
        }
    
        private void Update()
        {
            tileMapComponents.Sort();

            canSelectTile = false;
            
            SelectionCheck();
            
            selectionOutline.SetActive(canSelectTile);
        }

        /// <summary>
        /// Checks if you can select a tile at your current mouse Position. If you can it activates behaviours based on the tile type.
        /// </summary>
        private void SelectionCheck()
        {
            foreach (TileMapComponent tileMapComponent in tileMapComponents) //Checks each tilemap for a hit.
            {
                Tilemap tilemap = tileMapComponent.Tilemap;
                TileBase tile;   
                if (TilemapCast(tilemap, out tile))
                {
                    string sortingLayerName = tileMapComponent.Renderer.sortingLayerName;

                    switch (sortingLayerName)
                    {
                        case "Default":
                            canSelectTile = true;
                            
                            //SelectWalkable(tilemap);
                            SelectWalkable();

                            break;
                        case "Interactable":
                            canSelectTile = true;

                            SelectInteractable();

                            break;
                    }
                }
            }
        }
    
        private bool TilemapCast(Tilemap tilemap, out TileBase tileBase)
        {
            Vector3Int position = GridMousePosition();

            return player.CheckCell(position, tilemap, out tileBase);
        }
        
        private void SelectWalkable()
        {
            Tilemap tilemap = GameManager.instance.WalkableTilemap.Tilemap;
            
            SelectWalkable(tilemap);
        }
        
        private void SelectWalkable(Tilemap tilemap)
        {            
            Vector3Int gridMousePosition = GridMousePosition();
            
            Vector3 cellPosition = grid.CellToWorld(gridMousePosition) + new Vector3(0, .5f, 0);
            selectionOutline.transform.position = cellPosition;
            
            if (Input.GetMouseButtonDown(0))
            {
                List<Vector3Int> availableDestinationTiles = player.AvailableDestinationTiles(tilemap);
                
                if (availableDestinationTiles.Contains(gridMousePosition))
                {
                    Debug.Log("AvailableDestinationTiles contains GridMousePosition!");
                    
                    BoundsInt cellBounds = tilemap.cellBounds;

                    Vector2Int navGridDestination = GameManager.instance.TileIndexOnNavGrid(cellBounds, gridMousePosition);
                    
                    player.Move(navGridDestination, tilemap.CellToWorld(gridMousePosition) + new Vector3(0, .25f, 0)); //cellPosition?
                }
                else
                {                    
                    Debug.Log("AvailableDestinationTiles does NOT contain GridMousePosition!");
                }

            }
        }
        

        private void SelectInteractable()
        {
            Vector3Int gridMousePosition = GridMousePosition();
            
            Vector3 cellPosition = grid.CellToWorld(gridMousePosition) + new Vector3(0, .5f, 0);
            selectionOutline.transform.position = cellPosition;

            if (Input.GetMouseButtonDown(0))
            {
                
            }

            //TODO: Actual Behaviour
        }

        private Vector3Int GridMousePosition()
        {
            return grid.WorldToCell(selectionCamera.ScreenToWorldPoint(Input.mousePosition));
        }
    }
}