
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ServerConnector.Response
{
	
	public class ServerLoginResponse : ServerResponse
	{
		public object Response = "";
		
		override protected bool ParseResponse(object response) {
			//Ensure Response is reset when a new parse happens
			Response = "";
			
			if (Type == ServerResponseType.Login_Updated) {
				Response = UdonXML.GetNodeValue(response);
			} else if (Type == ServerResponseType.Login_Complete) {
				Response = UdonXML.GetNodeValue(UdonXML.GetChildNode(response, 1)).ToString();
			}
			
			return Response != null;
		}
	}

}