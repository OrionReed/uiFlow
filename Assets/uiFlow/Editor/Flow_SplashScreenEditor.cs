using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(Flow_SplashScreen))]
public class Flow_SplashScreenEditor : Editor
{
    private ReorderableList list;
    private void OnEnable()
    {
        list = new ReorderableList(serializedObject,
                serializedObject.FindProperty("Screens"),
                                   true, true, true, true);

        list.drawElementCallback =
    (Rect rect, int index, bool isActive, bool isFocused) =>
    {
        var element = list.serializedProperty.GetArrayElementAtIndex(index);
        rect.y += 2;


        EditorGUI.PropertyField(
            new Rect(rect.x, rect.y, 50, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("Type"), GUIContent.none);
            
        EditorGUI.PropertyField(new Rect(rect.x - 5 + 60, rect.y, rect.width - 150 - 30, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("Image"), GUIContent.none);
        //EditorGUI.PropertyField(new Rect(rect.x - 5 + 60, rect.y, rect.width - 150 - 30, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("Video"), GUIContent.none);

        EditorGUI.PropertyField(
                new Rect(rect.x - 70 + rect.width - 30, rect.y, 30, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("In"), GUIContent.none);
        EditorGUI.PropertyField(
            new Rect(rect.x - 35 + rect.width - 30, rect.y, 30, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("Sustain"), GUIContent.none);
        EditorGUI.PropertyField(
            new Rect(rect.x + rect.width - 30, rect.y, 30, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("Out"), GUIContent.none);
    };
        //LABELS
        list.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Screens");
        };
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        serializedObject.Update();
        list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}
