using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRCDatabase
{
    
    public enum DatabaseMessageTypes
    {
        Login = 0,
	    AccountCreation = 1,
	    GeneralMessage = 2,
	    AcknowledgeMessage = 14,
        MessageFinished = 15
    }

}