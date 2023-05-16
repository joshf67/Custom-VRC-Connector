using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace Joshf67.ServerConnector.Downloader
{
	/// <summary>
	/// Used to store all of the sub URL containers to enable better VRC world build speed
	/// </summary>
	public class ConnectorUrlToolManager : UdonSharpBehaviour
	{        
		/// <summary>
		/// The starting URL prefix for messages
		/// </summary>
		[HideInInspector]
		public string urlPrefix = "";

		/// <summary>
		/// The total URLs currently stored/generated
		/// </summary>
		[HideInInspector]
		public int urlCount = 0;

		/// <summary>
		/// Contains every URL container assosiated with this manager
		/// </summary>
		[HideInInspector]
		public ConnectorUrlTool[] urlCollections = new ConnectorUrlTool[0];

		/// <summary>
		/// Converts an int into the correct URL container index and requests the URL from it
		/// </summary>
		/// <param name="input"> The VRCUrl index to look for </param>
		/// <returns> A VRCUrl with the input message </returns>
		public VRCUrl ConvertMessageToVRCUrl(int input) 
		{
			int inputInt = Mathf.FloorToInt(input / 8192f);
			if (inputInt > urlCollections.Length)
			{
				Debug.LogError("No URL exists for input: " + input);
				return null;
			}

			if (urlCollections[inputInt] == null)
			{
				Debug.LogError("No URL exists for input: " + input);
				return null;
			}

			return urlCollections[inputInt].ConvertMessageToVRCUrl((input - (inputInt * 8192)));
		}
	}

}