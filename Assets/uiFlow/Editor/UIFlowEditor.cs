using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(UIFlow))]
public class UIFlowEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        UIFlow myScript = (UIFlow)target;
        if (GUILayout.Button("Open Editor"))
        {
            myScript.OpenInterfaceIO();
        }
    }

}