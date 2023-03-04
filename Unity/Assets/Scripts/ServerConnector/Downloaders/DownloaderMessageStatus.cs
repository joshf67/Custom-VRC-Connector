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
		Type_Fail = 3,
		Unexpected_Request = 4,
		Request_Error = 5
	}

}