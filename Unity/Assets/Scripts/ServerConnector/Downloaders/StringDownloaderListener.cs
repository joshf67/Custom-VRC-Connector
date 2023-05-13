
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.StringLoading;

using Joshf67.ServerConnector.Server;
using Joshf67.ServerConnector.Development;

namespace Joshf67.ServerConnector.Downloader
{

    /// <summary>
    /// This class was generated because of an issue with the Image Downloader where it can read erros from different threads,
    /// The solution was to extract the onsuccess/onerror to another script and report back to the main script.
    /// <para>
    /// This is no longer needed however does split up the code for downloaders and connectors
    /// </para>
    /// </summary>
    public class StringDownloaderListener : UdonSharpBehaviour
	{		
		
		/// <summary>
		/// Stores if a message has recieved a response from a server
		/// </summary>
		[SerializeField]
		public DownloaderMessageStatus DownloaderStatus = DownloaderMessageStatus.Awaiting_Request;
		
		/// <summary>
		/// Stores the last recieved response from a server incase an error occurs
		/// </summary>
		private IVRCStringDownload requestResult;

		/// <summary>
		/// Public getter for the last recieved response
		/// </summary>
		public IVRCStringDownload RequestResult {
			get { return requestResult; }
		}
		
		/// <summary>
		/// Handle String Downloader successful message 
		/// </summary>
		/// <param name="result"> The response from the server </param>
		public override void OnStringLoadSuccess(IVRCStringDownload result)
		{
			base.OnStringLoadSuccess(result);
			
			if (DevelopmentManager.IsStringListenerEnabled(DevelopmentMode.Basic))
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
	
		/// <summary>
		/// Handle String Downloader error message
		/// </summary>
		/// <param name="result"> The response from the server </param>
		public override void OnStringLoadError(IVRCStringDownload result)
		{
			base.OnStringLoadError(result);
			DownloaderStatus = DownloaderMessageStatus.Request_Error;
	
			requestResult = result;
			if (DevelopmentManager.IsStringListenerEnabled(DevelopmentMode.Basic))
				Debug.Log($"Error result: ${result.Error} with error code: {result.ErrorCode}");
		}
	}

}