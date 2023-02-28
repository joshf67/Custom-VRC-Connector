
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
namespace VRCDatabase
{
    
	public abstract class ServerResponse : UdonSharpBehaviour
	{
		//Store the type of this response into an easily accessible place to not have to cast in the future
		private ServerResponseType _type = ServerResponseType.None;
		public ServerResponseType Type {
			get { return _type; }
		}
		
		//Traverse the XML response and find the type of the message
		public static ServerResponseType GetMessageType(string message) {
			//Ensure the XML is valid
			object root = UdonXML.LoadXml(message);
			if (root == null) return ServerResponseType.None;
			
			//Ensure the XML has a valid response
			object type = UdonXML.GetChildNode(root, 0);
			if (type == null) return ServerResponseType.None;
			
			object typeValue = UdonXML.GetNodeValue(type);
			if (typeValue == null) return ServerResponseType.None;
			return (ServerResponseType)typeValue;
		}
		
		public bool Parse(string message) {
			//Ensure Type is reset when a parse begins
			_type = ServerResponseType.None;
			
			//Try to parse the response into XML
			object root = UdonXML.LoadXml(message);
			if (root == null) return false;
			
			//Store the type of the response for use in the future
			_type = ServerResponse.GetMessageType(message);
			if (_type == ServerResponseType.None) return false;
			
			//Ensure the XML has a valid response
			object response = UdonXML.GetChildNode(root, 1);
			if (response == null) return false;
			
			return ParseResponse(response);
		}
		
		protected abstract bool ParseResponse(object response);
	}

}