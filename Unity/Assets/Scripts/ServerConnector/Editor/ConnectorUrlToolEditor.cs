using UnityEditor;
using UnityEngine;

using Joshf67.ServerConnector.Downloader;

namespace Joshf67.ServerConnector.Editors
{
    /// <summary>
    /// Displays editor UI to help with creating the server connection URLs
    /// </summary>
	[CustomEditor(typeof(ConnectorUrlTool))]
	public class ConnectorUrlToolEditor : Editor
    {

        private int urlsToGenerate = 0;
        private string startingURL = "";

        /// <summary>
        /// Render the URL count for this URLTool
        /// </summary>
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