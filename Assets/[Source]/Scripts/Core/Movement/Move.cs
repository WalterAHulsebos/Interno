using UnityEngine;
using System;
using Sirenix.OdinInspector;
using Sirenix.Utilities;

[Serializable]
public class Move : SerializedScriptableObject
{
        //[BoxGroup("GridSize", ShowLabel = false)]
        //public Vector2Int gridSize = new Vector2Int(20, 20);
        #if UNITY_EDITOR
            [BoxGroup("Chance", ShowLabel = false)]
            [ProgressBar(0, 100, ColorMember = "GetWeightColour")]
        #endif
        [Range(0, 1f)]
        public float weight = 1;
        public bool directConnectionRequired;

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
        public bool[,] footPrint = new bool[25, 25];
    
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
