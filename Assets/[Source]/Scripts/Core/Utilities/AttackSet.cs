using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.Utilities.Editor;

[Serializable]
[CreateAssetMenu(fileName = "New AttackSet", menuName = "AttackSets/Create AttackSet")]
public class AttackSet
{
    public List<Attack> attacks = new List<Attack>();

#if UNITY_EDITOR
    private void DrawRefreshButton()
    {
        if (SirenixEditorGUI.ToolbarButton(EditorIcons.Refresh))
            foreach (Attack attack in attacks)
                attack.ConvertGridToList();
    }
#endif
}
