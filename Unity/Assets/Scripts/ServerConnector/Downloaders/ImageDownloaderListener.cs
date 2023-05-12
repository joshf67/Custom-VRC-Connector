
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Image;
using Joshf67.ServerConnector.Development;

namespace Joshf67.ServerConnector.Downloader
{
	
	//This class is needed as there is an error with the Image Downloader where it can read erros from different threads
	//The solution is to extract the onsuccess/onerror to another script and report back to the main script
	public class ImageDownloaderListener : UdonSharpBehaviour
	{				
		[SerializeField]
		//Stores if a message has recieved a response from a server
		public DownloaderMessageStatus DownloaderStatus = DownloaderMessageStatus.Awaiting_Request;
		
		//Image downloader is used purely to decrease response time, no result handling is needed
		public override void OnImageLoadSuccess(IVRCImageDownload result)
		{
			//This shouldn't be possible
			DownloaderStatus = DownloaderMessageStatus.Awaiting_Request;
	
			if (DevelopmentManager.IsImageListenerEnabled(DevelopmentMode.Basic))
				Debug.Log($"Server somehow responded to an image request when it shouldn't");
	
			base.OnImageLoadSuccess(result);
		}
	
		//Image downloader is used purely to decrease response time, no result handling is needed
		public override void OnImageLoadError(IVRCImageDownload result)
		{
			base.OnImageLoadError(result);
			
			DownloaderStatus = DownloaderMessageStatus.Request_Error;
	
			if (DevelopmentManager.IsImageListenerEnabled(DevelopmentMode.Basic))
				Debug.Log($"Error result: ${result.Error} with error code: {result.ErrorMessage}");
		}
	}

}