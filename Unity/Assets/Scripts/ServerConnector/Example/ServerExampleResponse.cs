
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ServerConnector.Response
{
	
	public class ServerExampleResponse : ServerResponse
	{
		public object Response = "";
		
		override protected bool ParseResponse(object response) {
			//Ensure Response is reset when a new parse happens
			Response = "";
			
			if (Type == ServerResponseType.Login_Failed) {
				Response = "Login credentials are invalid, please try again or create an account";
			} else if (Type == ServerResponseType.Login_Updated) {
				Response = UdonXML.GetNodeValue(response);
			} else if (Type == ServerResponseType.Login_Complete) {
				Response = UdonXML.GetNodeValue(UdonXML.GetChildNode(response, 1)).ToString();
			} else if (Type == ServerResponseType.Added_Item) {
				Response = UdonXML.GetNodeValue(UdonXML.GetChildNode(response, 1));
			} else if (Type == ServerResponseType.Removed_Item) {
				Response = UdonXML.GetNodeValue(UdonXML.GetChildNode(response, 1));
			}
			
			return Response != null;
		}
	}

}