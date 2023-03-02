using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

using ServerConnector;
using ServerConnector.Response;

public class DatabaseDebugger : Connector
{
    public InputField usernameField;
    public InputField passwordField;
    public Text responseField;
    public ServerLoginResponse ConvertToManager;
    public int read = 0;

    public void Login()
    {
        AddMessagesToBuffer(MessagePacker.PackMessageBytes(GenerateLoginHashByte(), ConnectorMessageType.Login, packingMessageBitSize));
    }

    public void CreateAccount()
    {
        AddMessagesToBuffer(MessagePacker.PackMessageBytes(GenerateLoginHashByte(), ConnectorMessageType.AccountCreation, packingMessageBitSize));
    }

    private byte[] GenerateLoginHashByte()
    {
        string usernameHash = ConvertSHA256ToMessage(ConvertTextToHash(usernameField.text + passwordField.text));
        byte[] usernameBytes = new byte[usernameHash.Length];

        for (int i = 0; i < usernameHash.Length; i++)
        {
            usernameBytes[i] = (byte)usernameHash[i];
        }

        return usernameBytes;
    }

    public override void HandleMessage(string response)
    {
        read++;
        Debug.Log(read);
        if (ServerResponse.GetMessageType(response) == ServerResponseType.Login_Updated)
        {
            ConvertToManager.Parse(response);
            responseField.text = $"Recieved a login update message with the response: {ConvertToManager.Response.ToString()}";
        }
        else if (ServerResponse.GetMessageType(response) == ServerResponseType.Login_Complete)
        {
            ConvertToManager.Parse(response);
            responseField.text = $"Recieved a login complete message with the response: {ConvertToManager.Response.ToString()}";
        }
    }
}