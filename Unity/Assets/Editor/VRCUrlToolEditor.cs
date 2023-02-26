using UnityEditor;
using UnityEngine;

namespace VRCDatabase.Editors
{
    [CustomEditor(typeof(VRCUrlTool))]
    public class VRCUrlToolEditor : Editor
    {

        private int urlsToGenerate = 0;
        private string startingURL = "";

        public void OnEnable()
        {
            //if (style == null) style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
        }

        public override void OnInspectorGUI()
        {
            GUIStyle label = GUI.skin.label;
            label.alignment = TextAnchor.MiddleCenter;
            label.fontStyle = FontStyle.Bold;

            GUI.enabled = false;
            EditorGUILayout.IntField("Total URLs:", serializedObject.FindProperty("urls").arraySize);
            //EditorGUILayout.TextField("URL prefix:", serializedObject.FindProperty("urlPrefix").stringValue);
            //EditorGUILayout.Space(5);
            GUI.enabled = true;

            //GUILayout.BeginHorizontal();
            //{
            //    Color color = GUI.color;
            //    GUI.color = Color.red;
            //    EditorGUILayout.LabelField("Generate URLs", label);
            //    GUI.color = color;
            //}
            //GUILayout.EndHorizontal();

            //startingURL = EditorGUILayout.TextField("Starting URL: ", startingURL);
            //urlsToGenerate = EditorGUILayout.IntField("URLs To Generate: ", urlsToGenerate);

            //GenerateVRCUrlsTool.GenerateURLs((target as VRCUrlTool).gameObject, urlsToGenerate, startingURL);

            //if (GUILayout.Button("Clear URLs")) {
            //    Undo.RecordObject(target, "Clear VRCDatabase URLs");
            //    serializedObject.FindProperty("urls").ClearArray();
            //    serializedObject.FindProperty("urlPrefix").stringValue = "";
            //    serializedObject.ApplyModifiedProperties();
            //    EditorUtility.SetDirty(target);
            //    PrefabUtility.RecordPrefabInstancePropertyModifications(target);
            //}
        }

    }

}