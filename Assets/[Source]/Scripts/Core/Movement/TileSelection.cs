using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditorInternal;
using System.Reflection;
using System.Linq;
//using Core;

namespace MyNamespace
{
    public class TileSelection : MonoBehaviour
    {    
        //TODO: Only Execute when playing (not in menu)
        
        #region Variables    
        [SerializeField] private GameObject selectionOutline;
        
        private Camera selectionCamera;

        private GridLayout grid;
    
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
    
        private void Start()
        {            
            tileMapComponents = new List<TileMapComponent>(); //Reset Tilemap Components list.
            
            if (selectionCamera == null)
            {
                selectionCamera = Camera.main ? Camera.main : FindObjectOfType<Camera>();
            } //Get Selection Camera

            if (FindObjectOfType<GridLayout>())
            {
                grid = FindObjectOfType<GridLayout>();
            }
    
            TilemapCollider2D[] tilemapColliders = FindObjectsOfType<TilemapCollider2D>(); //Only collect tilemaps that have colliders
    
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

            bool canSelectTile = false;
            
            foreach (var tileMapComponent in tileMapComponents)
            {
                TileBase tile;
                //if (Input.GetMouseButtonDown(0))             //TODO: Replace this.

                if (TilemapCast(tileMapComponent.Tilemap, out tile))
                {
                    string sortingLayerName = tileMapComponent.Renderer.sortingLayerName;

                    switch (sortingLayerName)
                    {
                        case "Default":
                            canSelectTile = true;
                            
                            SelectWalkable(grid.CellToWorld(GridMousePosition()) + new Vector3(0, .5f, 0));

                            break;
                        case "Interactable":
                            canSelectTile = true;

                            SelectInteractable();

                            break;
                    }
                }
            }
            
            selectionOutline.SetActive(canSelectTile);
        }
    
        private void SelectWalkable(Vector3 cellPosition)
        {            
            selectionOutline.transform.position = cellPosition;

            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Floerp");                                
            }
            
            //TODO: Actual Behaviour.
        }

        private void SelectInteractable()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Flaarp");                                
            }
            
            //TODO: Actual Behaviour
        }

        private Vector3Int GridMousePosition()
        {
            return grid.WorldToCell(selectionCamera.ScreenToWorldPoint(Input.mousePosition));
        }
    
        private bool TilemapCast(Tilemap tilemap, out TileBase tileBase)
        {
            TileBase tile = tilemap.GetTile(GridMousePosition());

            if (tile != null)
            {
                tileBase = tile;
                return true;
            }
            else
            {
                tileBase = null;
                return false;
            }
        }
    }
}