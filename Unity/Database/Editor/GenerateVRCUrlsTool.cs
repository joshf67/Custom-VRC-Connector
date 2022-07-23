using System;
using UnityEngine;
using UnityEditor;
using VRC.SDKBase;

namespace VRCDatabase.Editors
{

    public class GenerateVRCUrlsTool : EditorWindow
    {

        private GameObject urlObject;
        private int urlsToGenerate = 0;
        private string startingURL = "";

        [MenuItem("VRC Database/Generate URLs")]
        public static void ShowWindow()
        {
            GetWindow<GenerateVRCUrlsTool>("Generate VRCUrls Tool");
        }

        public static void HideWindow()
        {
            GetWindow<GenerateVRCUrlsTool>().Close();
        }

        public static void GenerateURLs(GameObject urlObject, int urlsToGenerate, string startingURL)
        {
            if (urlObject != null && urlsToGenerate != 0 && startingURL != "")
            {
                EditorGUILayout.Space(20, true);

                if (GUILayout.Button("Generate URLs"))
                {
                    VRCUrlTool objTool = urlObject.GetComponent<VRCUrlTool>();
                    if (objTool == null) objTool = urlObject.AddComponent<VRCUrlTool>();

                    VRCUrl[] urls = new VRCUrl[urlsToGenerate];

                    for (int i = 0; i < urlsToGenerate; i++)
                    {
                        urls[i] = new VRCUrl(startingURL + Convert.ToString(i, 16));
                    }

                    Undo.RecordObject(objTool, "Modify VRCDatabase URLs");
                    objTool.urls = urls;
                    objTool.urlPrefix = startingURL;
                    EditorUtility.SetDirty(objTool);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(objTool);
                }
            }
        }

        public void OnGUI()
        {
            urlObject = (GameObject)EditorGUILayout.ObjectField("URL Object: ", urlObject, typeof(GameObject), true);
            startingURL = EditorGUILayout.TextField("Starting URL: ", startingURL);
            urlsToGenerate = EditorGUILayout.IntField("URLs To Generate: ", urlsToGenerate);

            GenerateURLs(urlObject, urlsToGenerate, startingURL);
        }

    }

}