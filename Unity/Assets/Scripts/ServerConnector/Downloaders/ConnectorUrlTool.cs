using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace ServerConnector.Downloader
{
	public class ConnectorUrlTool : UdonSharpBehaviour
	{
		public VRCUrl ConvertMessageToVRCUrl(int input)
		{
			if (input > urls.Length)
			{
				Debug.LogError("No URL exists for input: " + input);
				return null;
			}
			return urls[input];
		}

		[HideInInspector]
		public VRCUrl[] urls = new VRCUrl[0];
	}

}