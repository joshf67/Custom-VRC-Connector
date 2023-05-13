
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Image;
using Joshf67.ServerConnector.Development;

namespace Joshf67.ServerConnector.Downloader
{

    /// <summary>
    /// This class was generated to solve an error with the Image Downloader where it can read erros from different threads
	/// The solution was to extract the onsuccess/onerror to another script and report back to the main script
	/// <para>
	/// This is no longer needed however does split up the code for downloaders and connectors
	/// </para>
    /// </summary>
    public class ImageDownloaderListener : UdonSharpBehaviour
	{				
		/// <summary>
		/// Stores if a message has recieved a response from a server
		/// </summary>
		[SerializeField]
		public DownloaderMessageStatus DownloaderStatus = DownloaderMessageStatus.Awaiting_Request;
		
		/// <summary>
		/// Image downloader is used purely to decrease response time, no result handling is needed
		/// </summary>
		/// <param name="result"> The response from a server </param>
		public override void OnImageLoadSuccess(IVRCImageDownload result)
		{
			//This shouldn't be possible
			DownloaderStatus = DownloaderMessageStatus.Awaiting_Request;
	
			if (DevelopmentManager.IsImageListenerEnabled(DevelopmentMode.Basic))
				Debug.Log($"Server somehow responded to an image request when it shouldn't");
	
			base.OnImageLoadSuccess(result);
		}
	
		/// <summary>
		/// Image downloader is used purely to decrease response time, no result handling is needed
		/// </summary>
		/// <param name="result"> The response from a server </param>
		public override void OnImageLoadError(IVRCImageDownload result)
		{
			base.OnImageLoadError(result);
			
			DownloaderStatus = DownloaderMessageStatus.Request_Error;
	
			if (DevelopmentManager.IsImageListenerEnabled(DevelopmentMode.Basic))
				Debug.Log($"Error result: ${result.Error} with error code: {result.ErrorMessage}");
		}
	}

}