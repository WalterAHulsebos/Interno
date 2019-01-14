using UnityEngine;
using System;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine.Tilemaps;

[Serializable]
[CreateAssetMenu(fileName = "New WalkableTiles", menuName = "WalkableTiles")]
public class WalkableTiles : SerializedScriptableObject
{
#if UNITY_EDITOR
    [Required]
    [InlineEditor]
    [ListDrawerSettings(Expanded = true, ShowItemCount = false)]
#endif
    public Tile[] walkableTiles;
}