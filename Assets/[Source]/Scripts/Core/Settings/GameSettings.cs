using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Settings/GameSettings")]
public class GameSettings : ScriptableObject
{
    public GridLayout grid = null;
    
    public Vector3 gridCellSize = new Vector3(1,1,0); 
}
