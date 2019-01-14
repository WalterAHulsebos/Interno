using UnityEngine;
using System;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Collections.Generic;

[Serializable]
public class Move : SerializedScriptableObject
{
    //[BoxGroup("GridSize", ShowLabel = false)]
    //public Vector2Int gridSize = new Vector2Int(20, 20);
    #if UNITY_EDITOR
        [BoxGroup("Weight", ShowLabel = false)]
        [ProgressBar(0, 100, ColorMember = "GetWeightColour")]
    #endif
    [Range(0, 1f)]
    public float weight = 1;
    public bool directConnectionRequired;

    #region Conversion
    public List<Vector2Int> movePositions;

    public virtual void ConvertGridToList()
    {
        movePositions = new List<Vector2Int>();
        Vector2Int size = new Vector2Int(footPrint.GetLength(0), footPrint.GetLength(1));

        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
                if(footPrint[x, y])
                    movePositions.Add(new Vector2Int(x, y));
    }
    #endregion

    #if UNITY_EDITOR
    private Color GetWeightColour(float value)
        {
            return Color.Lerp(Color.green, Color.red, Mathf.Pow(value / 100f, 2));
        }
    #endif
    
    #if UNITY_EDITOR
        [BoxGroup("Footprint", ShowLabel = false)]
        [TableMatrix(DrawElementMethod = "DrawColouredCell", SquareCells = true, HideRowIndices = false,
        HideColumnIndices = false, ResizableColumns = false)]
    #endif
    public bool[,] footPrint = new bool[25, 25]
    {
        {
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
        },
        {
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
        },
        {
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
        },
        {
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
        },
        {
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
        },
        {
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
        },
        {
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
        },
        {
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
        },
        {
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
        },
        {
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
        },
        {
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
        },
        {
            false, false, false, false, false, false, false, false, false, false, false, false, true, false, false,
            false, false, false, false, false, false, false, false, false, false,
        },
        {
            false, false, false, false, false, false, false, false, false, false, false, true, false, true, false,
            false, false, false, false, false, false, false, false, false, false,
        },
        {
            false, false, false, false, false, false, false, false, false, false, false, false, true, false, false,
            false, false, false, false, false, false, false, false, false, false,
        },
        {
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
        },
        {
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
        },
        {
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
        },
        {
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
        },
        {
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
        },
        {
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
        },
        {
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
        },
        {
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
        },
        {
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
        },
        {
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
        },
        {
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false,
        }
    }; //default 
    
    #if UNITY_EDITOR
        private static bool DrawColouredCell(Rect rect, bool toggled)
        {
            Event currentEvent = Event.current;

            bool paintToggle = !toggled;
        
            if ((currentEvent.type == EventType.MouseDrag || currentEvent.type == EventType.MouseDown) && (currentEvent.button == 0 && currentEvent.isMouse) &&
                rect.Contains(Event.current.mousePosition))
            {
                toggled = paintToggle;
                GUI.changed = true;
                Event.current.Use();
            }
        
            UnityEditor.EditorGUI.DrawRect(rect.Padding(1), toggled ? new Color(1f,.5f,0f) : Color.grey);
        
            return toggled;
        }
    #endif
}
