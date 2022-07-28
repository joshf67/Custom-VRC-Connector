using UnityEditor;
using UnityEngine;

namespace VRCDatabase.Editors
{
    [CustomEditor(typeof(VrcUrlToolManager))]
    public class VRCUrlToolManagerEditor : Editor
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
            EditorGUILayout.IntField("Total URLs:", serializedObject.FindProperty("urlCount").intValue);
            EditorGUILayout.TextField("URL prefix:", serializedObject.FindProperty("urlPrefix").stringValue);
            EditorGUILayout.Space(5);
            GUI.enabled = true;

            GUILayout.BeginHorizontal();
            {
                Color color = GUI.color;
                GUI.color = Color.red;
                EditorGUILayout.LabelField("Generate URLs", label);
                GUI.color = color;
            }
            GUILayout.EndHorizontal();

            startingURL = EditorGUILayout.TextField("Starting URL: ", startingURL);
            urlsToGenerate = EditorGUILayout.IntField("URLs To Generate: ", urlsToGenerate);

            GenerateVRCUrlsTool.GenerateURLs((target as VrcUrlToolManager).gameObject, urlsToGenerate, startingURL);

            if (GUILayout.Button("Clear URLs")) 
            {
                Undo.RecordObject(target, "Clear VRCDatabase URLs");

                VrcUrlToolManager urlManager = (VrcUrlToolManager)target;
                PrefabUtility.UnpackPrefabInstanceAndReturnNewOutermostRoots(urlManager.transform.parent.gameObject, PrefabUnpackMode.OutermostRoot);

                for(int i = 0; i < urlManager.urlCollections.Length; i++)
                {
                    if (urlManager?.urlCollections[i] != null)
                    {
                        VRCUrlTool urlTool = urlManager?.urlCollections[i];
                        DestroyImmediate(urlTool?.gameObject);
                    }
                }

                urlManager.urlCollections = new VRCUrlTool[0];
                urlManager.urlPrefix = "";
                urlManager.urlCount = 0;
                EditorUtility.SetDirty(target);
            }
        }

    }

}