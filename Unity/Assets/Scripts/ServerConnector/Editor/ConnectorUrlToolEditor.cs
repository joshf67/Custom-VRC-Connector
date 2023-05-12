using UnityEditor;
using UnityEngine;

using Joshf67.ServerConnector.Downloader;

namespace Joshf67.ServerConnector.Editors
{
	[CustomEditor(typeof(ConnectorUrlTool))]
	public class ConnectorUrlToolEditor : Editor
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
            GUI.enabled = true;
        }

    }

}