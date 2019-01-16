using Sirenix.Utilities.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New ViewSet", menuName = "ViewSets/Create ViewSet")]
public class ViewSet
{
    public View[] views;

#if UNITY_EDITOR
    private void DrawRefreshButton()
    {
        if (SirenixEditorGUI.ToolbarButton(EditorIcons.Refresh))
            foreach (View view in views)
                view.ConvertGridToList();
    }
#endif
}
