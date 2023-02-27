
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public abstract class ServerResponse : UdonSharpBehaviour
{
	public string Type;
	
	public void Parse(string response) {
		ParseResposeType(response);
		ParseResponse(response);
	}
	
	private void ParseResposeType(string response) {
		if (response.Split(',').Length == 0)
			Type = "Failed To Parse";
		Type = response.Split(',')[0];
	}
	
	protected abstract void ParseResponse(string response);
}