using System;
using UnityEngine;
using UnityEditor;
using VRC.SDKBase;

using Joshf67.ServerConnector.Downloader;

#if UNITY_EDITOR
namespace Joshf67.ServerConnector.Editors
{

    /// <summary>
    /// Displays editor UI to help with creating the server connection URLs
    /// </summary>
    public class GenerateConnectorUrlsTool : EditorWindow
    {

        private GameObject urlObject;
        private int urlsToGenerate = 0;
        private string startingURL = "";

        /// <summary>
        /// Open the editor window
        /// </summary>
	    [MenuItem("Server Connector/Generate URLs")]
        public static void ShowWindow()
        {
	        GetWindow<GenerateConnectorUrlsTool>("Generate Connector Urls Tool");
        }

        /// <summary>
        /// Close the editor window
        /// </summary>
        public static void HideWindow()
        {
            GetWindow<GenerateConnectorUrlsTool>().Close();
        }

        /// <summary>
        /// Displays editor UI to help with creating the server connection URLs
        /// </summary>
        /// <param name="urlObject"> The object to add the URL children to </param>
        /// <param name="urlsToGenerate"> The amount of URLs to generate </param>
        /// <param name="startingURL"> The initial URL parameter to be prepended to the URL </param>
        public static void GenerateURLs(GameObject urlObject, int urlsToGenerate, string startingURL)
        {
            if (urlObject != null && urlsToGenerate != 0 && startingURL != "")
            {
                EditorGUILayout.Space(20, true);

                if (GUILayout.Button("Generate URLs"))
                {

                    const int MAX_ARRAY_CAPACITY = 8192;

                    PrefabUtility.UnpackPrefabInstanceAndReturnNewOutermostRoots(urlObject.transform.parent.gameObject, PrefabUnpackMode.OutermostRoot);
	                ConnectorUrlToolManager urlManager = urlObject.GetComponent<ConnectorUrlToolManager>();
                    for(int i = 0; i < urlManager.urlCollections.Length; i++)
                    {
                        if (urlManager?.urlCollections[i] != null)
                        {
	                        ConnectorUrlTool urlTool = urlManager?.urlCollections[i];
                            DestroyImmediate(urlTool?.gameObject);
                        }
                    }

                    int urlTopLevel = Mathf.CeilToInt(urlsToGenerate / (float)MAX_ARRAY_CAPACITY);
	                urlManager.urlCollections = new ConnectorUrlTool[urlTopLevel];

                    GameObject urlToolObj = new GameObject("0 - " + (urlsToGenerate).ToString());
                    urlToolObj.transform.parent = urlObject.transform;

                    Undo.RecordObject(urlManager.gameObject, "Add Connector URLs");
                    
                    for (int topLevel = 0; topLevel < urlTopLevel; topLevel++)
                    {
                        int topLevelMin = (topLevel * MAX_ARRAY_CAPACITY);
                        int topLevelMax = (topLevel + 1) * MAX_ARRAY_CAPACITY <= urlsToGenerate ? (topLevel + 1) * MAX_ARRAY_CAPACITY : topLevelMin + (urlsToGenerate - topLevelMin);
                        VRCUrl[] urls = new VRCUrl[topLevelMax - topLevelMin];

	                    ConnectorUrlTool urlTool = urlToolObj.AddComponent<ConnectorUrlTool>();

                        for (int i = 0; i < urls.Length; i++)
                        {
                            urls[i] = new VRCUrl(startingURL + Convert.ToString(topLevelMin + i, 16));
                        }

                        urlTool.urls = urls;
                        urlManager.urlCollections[topLevel] = urlTool;
                        EditorUtility.SetDirty(urlTool);
                    }

                    urlManager.urlCount = urlsToGenerate;
                    urlManager.urlPrefix = startingURL;
                    EditorUtility.SetDirty(urlManager);
                }
            }
        }

        /// <summary>
        /// Displays editor UI to help with creating the server connection URLs
        /// </summary>
        public void OnGUI()
        {
            urlObject = (GameObject)EditorGUILayout.ObjectField("URL Object: ", urlObject, typeof(GameObject), true);
            startingURL = EditorGUILayout.TextField("Starting URL: ", startingURL);
	        urlsToGenerate = EditorGUILayout.IntField("URLs To Generate: ", urlsToGenerate);
            
	        if (GUILayout.Button("Populate 21 bit URL count")) {
	        	urlsToGenerate = Mathf.RoundToInt(Mathf.Pow(2, 21));
	        }
	        
            GenerateURLs(urlObject, urlsToGenerate, startingURL);
        }

    }

}
#endif