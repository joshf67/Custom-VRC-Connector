using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ServerConnector.Downloader
{
    
	public enum DownloaderMessageStatus
	{
		Awaiting_Request = 0,
		Message_Sent = 1,
		Failed_To_Send = 2,
		Request_Error = 3
	}

}