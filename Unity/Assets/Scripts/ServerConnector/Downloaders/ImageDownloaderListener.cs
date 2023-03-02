
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Image;

namespace ServerConnector.Downloader
{
	
	//This class is needed as there is an error with the Image Downloader where it can read erros from different threads
	//The solution is to extract the onsuccess/onerror to another script and report back to the main script
	public class ImageDownloaderListener : UdonSharpBehaviour
	{		
		[SerializeField]
		private bool DevelopmentMode = false;
		
		[SerializeField]
		//Stores if a message has recieved a response from a server
		public DownloaderMessageStatus DownloaderStatus = DownloaderMessageStatus.Awaiting_Request;
		
		//Image downloader is used purely to decrease response time, no result handling is needed
		public override void OnImageLoadSuccess(IVRCImageDownload result)
		{
			//This shouldn't be possible
			DownloaderStatus = DownloaderMessageStatus.Awaiting_Request;
	
			if (DevelopmentMode)
				Debug.Log($"Server somehow responded to an image request when it shouldn't");
	
			base.OnImageLoadSuccess(result);
		}
	
		//Image downloader is used purely to decrease response time, no result handling is needed
		public override void OnImageLoadError(IVRCImageDownload result)
		{
			base.OnImageLoadError(result);
			
			DownloaderStatus = DownloaderMessageStatus.Request_Error;
	
			if (DevelopmentMode)
				Debug.Log($"Error result: ${result.Error} with error code: {result.ErrorMessage}");
		}
	}

}