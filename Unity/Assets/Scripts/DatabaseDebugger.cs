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
	        responseField.text = $"Messages Recieved: {read},\n Current Message: {response}";
        }

        //public override void HandleUntrustedError()
        //{
        //    Debug.LogError("Rate limited, please increase delay");
        //    return;
        //}
    }

}