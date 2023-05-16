using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace Joshf67.ServerConnector.Downloader
{
	/// <summary>
	/// Used to store all of the URLs to connect to a server
	/// </summary>
	public class ConnectorUrlTool : UdonSharpBehaviour
	{
		/// <summary>
		/// Convert an input into the correct URL object
		/// </summary>
		/// <param name="input"> The URL/message to send </param>
		/// <returns> A VRC URL with the message encoded </returns>
		public VRCUrl ConvertMessageToVRCUrl(int input)
		{
			if (input > urls.Length)
			{
				Debug.LogError("No URL exists for input: " + input);
				return null;
			}
			return urls[input];
		}

		/// <summary>
		/// Stores any of the URLs on this object
		/// </summary>
		[HideInInspector]
		public VRCUrl[] urls = new VRCUrl[0];
	}

}