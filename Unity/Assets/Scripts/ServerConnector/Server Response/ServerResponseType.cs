using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ServerConnector.Response
{
    
	public enum ServerResponseType
	{
		None = -100,
		Unexpected_Request = -99,
		Type_Fail = -98,
		Failed_To_Parse = -97,
		
		Server_Error = -40,
		
		User_Not_Logged_In = -2,
		Failed = -1,
		Succeeded = 0,
		
		Login_Updated = 1,
		Login_Failed = 2,
		Login_Complete = 3,
		
		Account_Creation_Complete = 4,
		
		Item_Updated = 5,
		Added_Item = 6,
		Removed_Item = 7,
	}

}