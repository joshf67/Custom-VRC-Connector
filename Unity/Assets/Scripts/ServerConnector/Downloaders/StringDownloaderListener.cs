
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.StringLoading;

using ServerConnector.Response;

namespace ServerConnector.Downloader
{
	
	//This class is needed as there is an error with the Image Downloader where it can read erros from different threads
	//The solution is to extract the onsuccess/onerror to another script and report back to the main script
	public class StringDownloaderListener : UdonSharpBehaviour
	{		
		[SerializeField]
		private bool DevelopmentMode = false;
		
		[SerializeField]
		//Stores if a message has recieved a response from a server
		public DownloaderMessageStatus DownloaderStatus = DownloaderMessageStatus.Awaiting_Request;
		
		//Stores the last recieved response from a server
		private IVRCStringDownload requestResult;
		public IVRCStringDownload RequestResult {
			get { return requestResult; }
		}
		
		//Handle String Downloader successful message 
		public override void OnStringLoadSuccess(IVRCStringDownload result)
		{
			base.OnStringLoadSuccess(result);
	
			//If the response is that it failed on the server end, retry the messsage until it works
			if (ServerResponse.GetMessageType(result.Result) == ServerResponseType.Failed_To_Parse)
			{
				DownloaderStatus = DownloaderMessageStatus.Failed_To_Send;
				return;
			}
			
			DownloaderStatus = DownloaderMessageStatus.Message_Sent;
	
			requestResult = result;
			if (DevelopmentMode)
				Debug.Log($"String loaded successfully: {result.Result}");
		}
	
		//Handle String Downloader error message
		public override void OnStringLoadError(IVRCStringDownload result)
		{
			base.OnStringLoadError(result);
			DownloaderStatus = DownloaderMessageStatus.Request_Error;
	
			requestResult = result;
			if (DevelopmentMode)
				Debug.Log($"Error result: ${result.Error} with error code: {result.ErrorCode}");
		}
	}

}