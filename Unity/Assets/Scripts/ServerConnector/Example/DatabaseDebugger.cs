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
	public ServerExampleResponse ExampleResponseParser;
	
	public int[] items;
	public Text itemField;
	
	public InputField AddItemField;
	public InputField RemoveItemField;
	
    public int read = 0;

    public void Login()
    {
	    AddMessagesToBuffer(MessagePacker.PackMessageBytesToURL(GenerateLoginHashByte(), ConnectorMessageType.Login, packingMessageBitSize, messageTypeSize));
    }

    public void CreateAccount()
    {
        AddMessagesToBuffer(MessagePacker.PackMessageBytesToURL(GenerateLoginHashByte(), ConnectorMessageType.AccountCreation, packingMessageBitSize, messageTypeSize));
    }
    
	public void AddItem() {
		
		//Debug.Log(float.Parse(AddItemField.text));

		Debug.Log(ByteConverter.ConvertInt16(31424));
		
		////Only allow 8 bit itemIds
		//byte itemId = (byte)float.Parse(AddItemField.text);
		//if (itemId < 0 || itemId > 255) return;
		
		////Set up the options to send to the server's add item function
		////1st bit = adding/removing, next 7 bits = amount to follow
		//byte options = (1 << 1) + 1; //add 1 item
		//byte[] addItemMessage = new [] {options, itemId};
		
		//AddMessagesToBuffer(MessagePacker.PackMessageBytesToURL(addItemMessage, ConnectorMessageType.ModifyItem, packingMessageBitSize));
	}
	
	public void RemoveItem() {
		//Only allow 4 bit indices for removing
		byte itemIndex = (byte)float.Parse(RemoveItemField.text);
		if (itemIndex < 0 || itemIndex > 15) return;
		
		//Set up the options to send to the server's add item function
		//1st bit = adding/removing, next 7 bits = amount to follow
		byte options = (1 << 1) + 0; //remove 1 item
		//CompressedMessage[] removeItemMessage = new CompressedMessage[2];
		//CompressedMessage temp = new CompressedMessage(options);
		//CompressedMessage temp2 = new CompressedMessage(itemIndex);

		object removeItemMessage = MessagePacker.CompressMessage(options);
		AddMessagesToBuffer(MessagePacker.PackMessageBytesToURL(removeItemMessage, ConnectorMessageType.ModifyItem, packingMessageBitSize, messageTypeSize));
	}

    private object GenerateLoginHashByte()
    {
	    string usernameHash = ConvertSHA256ToMessage(ConvertTextToHash(usernameField.text + passwordField.text));
	    Debug.Log(usernameHash);
		return MessagePacker.CompressMessage(ByteConverter.ConvertUTF8String(usernameHash));
    }

    public override void HandleMessage(string response)
    {
        read++;
	    Debug.Log(read);
	    
	    ServerResponseType loginResponse = ServerResponse.GetMessageType(response);
	    switch (loginResponse) {
		    case ServerResponseType.Login_Updated:
			    ExampleResponseParser.Parse(response);
			    responseField.text = $"Recieved a login update message with the response: {ExampleResponseParser.Response.ToString()}";
			    break;
			    
		    case ServerResponseType.Login_Complete:
			    ExampleResponseParser.Parse(response);
			    responseField.text = $"Recieved a login complete message with the response: {ExampleResponseParser.Response.ToString()}";
			    break;
			    
		    case ServerResponseType.Login_Failed:
			    ExampleResponseParser.Parse(response);
			    responseField.text = $"Recieved a login failed message with the response: {ExampleResponseParser.Response.ToString()}";
			    break;
			    
		    case ServerResponseType.Added_Item:
		    	ExampleResponseParser.Parse(response);
			    ArrayUtilities.AddToArray<int>(ref items, (int)ExampleResponseParser.Response);
			    itemField.text = "";
			    for (int a = 0; a < items.Length; a++) {
			    	itemField.text += items[a].ToString() + "\n";
			    }
			    break;
			    
		    case ServerResponseType.Removed_Item:
		    	ExampleResponseParser.Parse(response);
			    ArrayUtilities.RemoveValueFromArrayAtIndex<int>(ref items, (int)ExampleResponseParser.Response);
			    itemField.text = "";
			    for (int a = 0; a < items.Length; a++) {
			    	itemField.text += items[a].ToString() + "\n";
			    }
			    break;
		}
    }
}