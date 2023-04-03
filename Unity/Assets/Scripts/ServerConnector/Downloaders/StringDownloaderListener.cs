
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
			
			if (DevelopmentMode)
				Debug.Log($"String loaded successfully: {result.Result}");
			
			switch(ServerResponse.GetMessageType(result.Result)) {
				case ServerResponseType.Unexpected_Request:
					//If the response is that the server wasnt expecting a message, not sure when this would ever be valid
					DownloaderStatus = DownloaderMessageStatus.Unexpected_Request;
					return;
				case ServerResponseType.Failed_To_Parse:
					//If the response is that it failed on the server end
					DownloaderStatus = DownloaderMessageStatus.Failed_To_Send;
					return;
				case ServerResponseType.Type_Fail:
					//If the response is that the server doesn't handle this type
					DownloaderStatus = DownloaderMessageStatus.Type_Fail;
					return;
				case ServerResponseType.User_Not_Logged_In:
					//If the response is that the user isn't logged in
					DownloaderStatus = DownloaderMessageStatus.User_Not_Logged_In;
					return;
				case ServerResponseType.Server_Error:
					//If the response is that the server has run into an issue
					DownloaderStatus = DownloaderMessageStatus.Server_Error;
					return;
				default:
					DownloaderStatus = DownloaderMessageStatus.Message_Sent;
	
					requestResult = result;
					return;
			}
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