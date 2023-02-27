using UdonSharp;
using VRC.Udon;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.SDK3.StringLoading;
using VRC.Udon.Common.Interfaces;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using System;

namespace VRCDatabase
{

    public abstract class DatabaseBase : UdonSharpBehaviour
    {
        public VRCUrl currentlySelectedURL;
        public VrcUrlToolManager urlLookup;

        public UdonHashLib hasher;
        public byte packingMessageBitSize;

	    const float CONNECTION_TIMEOUT_RATE = 5.1f;
	    float sendMessageWait = 0;
	    bool sendingMessage = false;

	    public int[] messageBuffer = new int[0];
	    private int[] sentMessageBuffer = new int[0];

        public void Start()
	    {
		    sendMessageWait = CONNECTION_TIMEOUT_RATE;
            ManagerStart();
        }

        public void Update()
        {
            sendMessageWait -= Time.deltaTime;
            if (!sendingMessage)
            {
                if (sendMessageWait <= 0 && messageBuffer.Length != 0)
                {
	                sendingMessage = true;
	                currentlySelectedURL = urlLookup.ConvertMessageToVRCUrl(messageBuffer[0]);
	                if (currentlySelectedURL != null) {
	                	Debug.Log($"Sending message of {messageBuffer[0]}");
		                sendMessageWait = CONNECTION_TIMEOUT_RATE;
	                	VRCStringDownloader.LoadUrl(currentlySelectedURL, (UdonBehaviour)(object)this);
	                }
                }
            }

            ManagerUpdate();
        }

        protected void HandleConnectionAttempt(bool connectionSuccess)
        {
            if (connectionSuccess)
            {
	            ArrayUtilities.AddToArray(ref sentMessageBuffer, messageBuffer[0]);
                ArrayUtilities.RemoveValueFromArrayAtIndex(ref messageBuffer, 0);
                sendingMessage = false;
            }
            else
            {
                sendingMessage = false;
            }
        }
        
	    public override void OnStringLoadSuccess(IVRCStringDownload result)
	    {
	    	base.OnStringLoadSuccess(result);
		    HandleConnectionAttempt(true);
	    	HandleMessage(result.Result);
	    	Debug.Log(result.Result);
	    }
	    
	    public override void OnStringLoadError(IVRCStringDownload result)
	    {
	    	base.OnStringLoadError(result);
		    HandleConnectionAttempt(false);
	    	Debug.Log($"Error result: ${result.Error} with error code: {result.ErrorCode}");
	    }
        
        protected virtual void ManagerStart()
        {

        }

        protected virtual void ManagerUpdate()
        {

        }

        //public abstract void HandleUntrustedError();

	    public abstract void HandleMessage(string response);

        public string ConvertTextToHash(string text)
        {
            return hasher.SHA512_UTF8(text);
        }

	    // Convert a SHA256 hash into a shorter 9 character hash to decrease login message time
	    // This decreases the security and increase the chance of collision but due to 5s rate limiting this is required to make it reasonable
        public string ConvertSHA256ToMessage(string hash)
        {
	        if (hash.Length != 128) return "";
            return new string(new char[] { hash[0], hash[15], hash[31], hash[47], hash[63], hash[79], hash[95], hash[111], hash[127] });
        }

        public void AddMessageToBuffer(int message)
        {
            ArrayUtilities.AddToArray(ref messageBuffer, message);
        }

        public void AddMessagesToBuffer(int[] messages)
        {
            ArrayUtilities.AddRangeToArray(ref messageBuffer, messages);
        }
    }

}