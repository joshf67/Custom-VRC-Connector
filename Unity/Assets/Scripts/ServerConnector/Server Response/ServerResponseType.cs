﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ServerConnector.Response
{
    
	public enum ServerResponseType
	{
		None = -100,
		Failed_To_Parse = -2,
		Failed = -1,
		Succeeded = 0,
		Login_Updated = 1,
		Login_Failed = 2,
		Login_Complete = 3,
		Account_Creation_Complete = 4,
	}

}