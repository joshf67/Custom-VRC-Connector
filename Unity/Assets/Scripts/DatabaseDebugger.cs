using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

namespace VRCDatabase
{

    public class DatabaseDebugger : DatabaseBase
    {
        public InputField usernameField;
	    public InputField passwordField;
	    public Text responseField;
	    public ServerLoginResponse ConvertToManager;
        public int read = 0;

        public void Login()
        {
	        string usernameHash = ConvertSHA256ToMessage(ConvertTextToHash(usernameField.text + passwordField.text));
            byte[] usernameBytes = new byte[usernameHash.Length];

            for (int i = 0; i < usernameHash.Length; i++)
            {
                usernameBytes[i] = (byte)usernameHash[i];
            }

	        AddMessagesToBuffer(MessagePacker.PackMessageBytes(usernameBytes, DatabaseMessageTypes.Login, packingMessageBitSize));
        }
        
	    public void CreateAccount()
	    {
		    string usernameHash = ConvertSHA256ToMessage(ConvertTextToHash(usernameField.text + passwordField.text));
		    byte[] usernameBytes = new byte[usernameHash.Length];

		    for (int i = 0; i < usernameHash.Length; i++)
		    {
			    usernameBytes[i] = (byte)usernameHash[i];
		    }

		    AddMessagesToBuffer(MessagePacker.PackMessageBytes(usernameBytes, DatabaseMessageTypes.AccountCreation, packingMessageBitSize));
	    }

	    public override void HandleMessage(string response)
        {
            read++;
	        Debug.Log(read);
	        if (ServerResponse.GetMessageType(response) == ServerResponseType.Login_Updated) {
	        	ConvertToManager.Parse(response);
		        responseField.text = $"Recieved a login update message with the response: {ConvertToManager.Response.ToString()}";
	        } else if (ServerResponse.GetMessageType(response) == ServerResponseType.Login_Complete) {
	        	ConvertToManager.Parse(response);
		        responseField.text = $"Recieved a login complete message with the response: {ConvertToManager.Response.ToString()}";
	        }
        }

        //public override void HandleUntrustedError()
        //{
        //    Debug.LogError("Rate limited, please increase delay");
        //    return;
        //}
    }

}