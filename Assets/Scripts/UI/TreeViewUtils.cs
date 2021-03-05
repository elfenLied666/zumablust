using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

static class TreeViewUtils
{
    [MenuItem("TreeViewUtility/Toggle Animation")]
    static void ToggleAnimation()
    {
        const string prefKey = "TreeViewExpansionAnimation";
        bool newValue = !EditorPrefs.GetBool(prefKey, true);
        EditorPrefs.SetBool(prefKey, newValue);
        InternalEditorUtility.RequestScriptReload();
        Debug.Log("TreeView animation is now " + (newValue ? "enabled" : "disabled"));
    }
}
