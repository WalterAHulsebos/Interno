using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using Core.Utilities;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;

[Serializable]
[CreateAssetMenu(fileName = "New MoveSet", menuName = "MoveSets/Create MoveSet")]
public class MoveSet : SerializedScriptableObject
{
    #if UNITY_EDITOR
    [Required]
    [InlineEditor]
    //[HideLabel]
    [ListDrawerSettings(Expanded = true, ShowItemCount = false, CustomAddFunction = "AddMove")]
    #endif
    public Move[] moves;
    
    private Move AddMove()
    {
        Move moveHolder = null;
        
        ScriptableObjectCreator.ShowDialog<Move>("Assets/[Source]/Prefabs/Moves",obj =>
        {
            moveHolder = obj;
        });
        
        return moveHolder;
    }
}