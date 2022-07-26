﻿using System;
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

                    const int MAX_ARRAY_CAPACITY = 8192;

                    PrefabUtility.UnpackPrefabInstanceAndReturnNewOutermostRoots(urlObject.transform.parent.gameObject, PrefabUnpackMode.OutermostRoot);
                    VrcUrlToolManager urlManager = urlObject.GetComponent<VrcUrlToolManager>();
                    for(int i = 0; i < urlManager.urlCollections.Length; i++)
                    {
                        if (urlManager?.urlCollections[i] != null)
                        {
                            VRCUrlTool urlTool = urlManager?.urlCollections[i];
                            DestroyImmediate(urlTool?.gameObject);
                        }
                    }

                    int urlTopLevel = Mathf.CeilToInt(urlsToGenerate / (float)MAX_ARRAY_CAPACITY);
                    urlManager.urlCollections = new VRCUrlTool[urlTopLevel];

                    GameObject urlToolObj = new GameObject("0 - " + (urlsToGenerate).ToString());
                    urlToolObj.transform.parent = urlObject.transform;

                    Undo.RecordObject(urlManager.gameObject, "Add VRCDatabase URLs");
                    
                    for (int topLevel = 0; topLevel < urlTopLevel; topLevel++)
                    {
                        int topLevelMin = (topLevel * MAX_ARRAY_CAPACITY);
                        int topLevelMax = (topLevel + 1) * MAX_ARRAY_CAPACITY <= urlsToGenerate ? (topLevel + 1) * MAX_ARRAY_CAPACITY : topLevelMin + (urlsToGenerate - topLevelMin);
                        VRCUrl[] urls = new VRCUrl[topLevelMax - topLevelMin];
                        //GameObject urlToolObj = new GameObject(topLevelMin.ToString() + " - " + (topLevelMax - 1).ToString());
                        //urlToolObj.transform.parent = urlObject.transform;

                        VRCUrlTool urlTool = urlToolObj.AddComponent<VRCUrlTool>();

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

        public void OnGUI()
        {
            urlObject = (GameObject)EditorGUILayout.ObjectField("URL Object: ", urlObject, typeof(GameObject), true);
            startingURL = EditorGUILayout.TextField("Starting URL: ", startingURL);
            urlsToGenerate = EditorGUILayout.IntField("URLs To Generate: ", urlsToGenerate);

            GenerateURLs(urlObject, urlsToGenerate, startingURL);
        }

    }

}