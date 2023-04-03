using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ServerConnector.Downloader
{
    
	public enum DownloaderMessageStatus
	{
		Server_Error = -99,
		Unexpected_Request = -1,
		
		Awaiting_Request = 0,
		Message_Sent = 1,
		Failed_To_Send = 2,
		Type_Fail = 3,
		User_Not_Logged_In = 4,
		Request_Error = 5
	}

}