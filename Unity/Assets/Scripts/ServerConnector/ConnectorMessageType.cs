using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ServerConnector
{
    
	public enum ConnectorMessageType
	{
		Invalid = -1,
        Login = 0,
	    AccountCreation = 1,
		GeneralMessage = 2,
		ModifyItem = 3,
	    AcknowledgeMessage = 14,
        MessageFinished = 15
    }

}