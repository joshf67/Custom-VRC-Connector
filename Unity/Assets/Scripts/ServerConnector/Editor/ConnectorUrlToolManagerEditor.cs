using UnityEditor;
using UnityEngine;

using ServerConnector.Downloader;

namespace ServerConnector.Editors
{
	[CustomEditor(typeof(ConnectorUrlToolManager))]
	public class ConnectorUrlToolManagerEditor : Editor
    {

        private int urlsToGenerate = 0;
        private string startingURL = "";

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
            
	        if (GUILayout.Button("Populate 20 bit URL count")) {
	        	urlsToGenerate = Mathf.RoundToInt(Mathf.Pow(2, 20));
	        }

	        GenerateConnectorUrlsTool.GenerateURLs((target as ConnectorUrlToolManager).gameObject, urlsToGenerate, startingURL);

            if (GUILayout.Button("Clear URLs")) 
            {
	            Undo.RecordObject(target, "Clear Connector URLs");

                ConnectorUrlToolManager urlManager = (ConnectorUrlToolManager)target;
                PrefabUtility.UnpackPrefabInstanceAndReturnNewOutermostRoots(urlManager.transform.parent.gameObject, PrefabUnpackMode.OutermostRoot);

                for(int i = 0; i < urlManager.urlCollections.Length; i++)
                {
                    if (urlManager?.urlCollections[i] != null)
                    {
	                    ConnectorUrlTool urlTool = urlManager?.urlCollections[i];
                        DestroyImmediate(urlTool?.gameObject);
                    }
                }

                urlManager.urlCollections = new ConnectorUrlTool[0];
                urlManager.urlPrefix = "";
                urlManager.urlCount = 0;
                EditorUtility.SetDirty(target);
            }
        }

    }

}