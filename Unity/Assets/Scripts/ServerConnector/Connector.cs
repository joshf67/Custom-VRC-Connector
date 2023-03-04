using UdonSharp;
using VRC.Udon;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.SDK3.StringLoading;
using VRC.SDK3.Image;
using VRC.Udon.Common.Interfaces;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using System;

using ServerConnector.Downloader;

namespace ServerConnector
{

	[RequireComponent(typeof(ImageDownloaderListener), typeof(StringDownloaderListener))]
	public abstract class Connector : UdonSharpBehaviour
    {
	    //Used to translate messages into URLs
	    [SerializeField]
	    private ConnectorUrlToolManager URLLookup;
	    private VRCUrl currentlySelectedURL;
	    
	    //Used to listen to onSuccess/onError due to thread errors
	    [SerializeField]
	    private StringDownloaderListener stringDownloaderListener;
	    [SerializeField]
	    private ImageDownloaderListener imageDownloaderListener;

	    //Used to hash any messages for simple encryption
	    [SerializeField]
	    protected UdonHashLib hasher;
	    [SerializeField]
        protected byte packingMessageBitSize;

        //Used to make sure only 1 message is used by each type every 5 seconds
	    private readonly float CONNECTION_TIMEOUT_RATE = 5;
        
	    //Used to retry a message if the server doesn't respond
	    private readonly float CONNECTION_RETRY_RATE = 15;

        //Variables to handle StringDownloader requests
        [SerializeField]
        private float stringRequestTimeout = 0;
        [SerializeField]
        private bool sendingStringMessage = false;

        //Variables to handle ImageDownloader requests
        [SerializeField]
        private float imageRequestTimeout = 0;
        [SerializeField]
        private bool sendingImageMessage = false;
        private VRCImageDownloader imageDownloader = new VRCImageDownloader();

        //Variables to hold all messaging data
        [SerializeField]
        private int messageToSend = -1;
        public int[] messageBuffer = new int[0];
        [SerializeField]
        private int currentMessageIndex = 0;
        [SerializeField]
        private int currentMessageEndIndex = -1;

        //Toggling this will allow debug outputs
	    private bool developmentMode = true;
	    public bool DevelopmentMode {
		    get { return developmentMode;}
	    }
	    
        //Toggling this will force the system to use the String Downloader resulting in double the message time but allow debugging the result message
        private bool forceStringLoader = false;

        public void Start()
	    {
		    stringDownloaderListener = GetComponent<StringDownloaderListener>();
		    imageDownloaderListener = GetComponent<ImageDownloaderListener>();
		    
            stringRequestTimeout = CONNECTION_TIMEOUT_RATE;
		    imageRequestTimeout = CONNECTION_TIMEOUT_RATE;
            
            imageDownloader = new VRCImageDownloader();
            ManagerStart();
        }

        private void Update()
        {
	        UpdateStringDownloader();
	        UpdateImageDownloader();
            
            if (ReadyToSendMessage())
            {
                SendMessage();
            }

            ManagerUpdate();
        }
        
	    //Check if the a response from the server has been returned and update the image downloader variable
	    private void UpdateStringDownloader() {
	    	
	    	//Update the String Downloaders timeout
		    if (stringRequestTimeout > -CONNECTION_RETRY_RATE) {
			    stringRequestTimeout -= Time.deltaTime;
		    } else if (sendingStringMessage) {
			    sendingStringMessage = false;
		    }
	        
		    //Check if a String Downloader has been returned recieved a response from the message
		    if (stringDownloaderListener.DownloaderStatus == DownloaderMessageStatus.Message_Sent) {
			    stringDownloaderListener.DownloaderStatus = DownloaderMessageStatus.Awaiting_Request;
			    sendingStringMessage = false;
			    HandleMessage(stringDownloaderListener.RequestResult.Result);
			    IncrementMessage();
		    }
	        
		    //Check if a String Downloader has recieved failed response from the message
		    if (stringDownloaderListener.DownloaderStatus == DownloaderMessageStatus.Failed_To_Send) {
			    stringDownloaderListener.DownloaderStatus = DownloaderMessageStatus.Awaiting_Request;
			    sendingStringMessage = false;
			    currentMessageIndex = 0;
		    }
	        
		    //Check if a String Downloader has recieved type doesn't exist response from the message
		    if (stringDownloaderListener.DownloaderStatus == DownloaderMessageStatus.Type_Fail) {
		    	
		    	//TODO Potentially break as world is not up to date.
			    stringDownloaderListener.DownloaderStatus = DownloaderMessageStatus.Awaiting_Request;
			    sendingStringMessage = false;
			    currentMessageIndex = 0;
		    }
		    
		    //Check if a String Downloader has recieved type doesn't exist response from the message
		    if (stringDownloaderListener.DownloaderStatus == DownloaderMessageStatus.Unexpected_Request) {
		    	
		    	//TODO Figure out what this error means
			    stringDownloaderListener.DownloaderStatus = DownloaderMessageStatus.Awaiting_Request;
			    sendingStringMessage = false;
			    currentMessageIndex = 0;
		    }
	    }
	    
	    //Check if the a response from the server has been returned and update the image downloader variable
	    private void UpdateImageDownloader() {
	    	
	    	//Update the Image Downloaders timeout
		    if (imageRequestTimeout > -CONNECTION_RETRY_RATE) {
			    imageRequestTimeout -= Time.deltaTime;
		    } else if (sendingImageMessage) {
			    sendingImageMessage = false;
		    }
	        
		    //Check if a Image Downloader has recieved a response from the message
		    if (imageDownloaderListener.DownloaderStatus == DownloaderMessageStatus.Request_Error) {
			    imageDownloaderListener.DownloaderStatus = DownloaderMessageStatus.Awaiting_Request;
			    sendingImageMessage = false;
			    IncrementMessage();
		    }
	    }

        //Make sure that everything is ready to send off a message before sending off a message
        private bool ReadyToSendMessage()
        {
            if (messageBuffer.Length == 0) return false;

            //A message hasn't been accepted/rejected so await for it to go through
            if (sendingImageMessage || sendingStringMessage) return false;

            //True means String Downloader is being used so wait for it to be ready
            //False means Video Downloader is being used so wait for it to be ready
            return SelectMessengerType() ? stringRequestTimeout < 0 : imageRequestTimeout < 0;
        }

        //Handle sending a message to a server if available
        private void SendMessage()
        {
            int message = SelectMessage();

            //If the current message to be sent is not valid then break
	        if (message == -1) return;
	        
	        //Convert the message buffer to a URL for String/Image Downloader
	        currentlySelectedURL = URLLookup.ConvertMessageToVRCUrl(message);

            if (SelectMessengerType())
            {
                if (currentlySelectedURL == null)
                {
                    if (DevelopmentMode)
                        Debug.LogError($"Invalid String URL for message {message}, please ensure this is set up correctly");
                    return;
                }

                if (DevelopmentMode)
                    Debug.Log($"Sending String Downloader message of {message} to {currentlySelectedURL}");

                //Send off a request to the String Downloader
                stringRequestTimeout = CONNECTION_TIMEOUT_RATE;
                sendingStringMessage = true;
                VRCStringDownloader.LoadUrl(currentlySelectedURL, (UdonBehaviour)(Component)stringDownloaderListener);
            }
            else
            {
                if (currentlySelectedURL == null)
                {
                    if (DevelopmentMode)
                        Debug.LogError($"Invalid Image URL for message {message}, please ensure this is set up correctly");
                    return;
                }

	            if (DevelopmentMode)
                    Debug.Log($"Sending Image Downloader message of {message} to {currentlySelectedURL}");

                //Send off a request to the Image Downloader
                imageRequestTimeout = CONNECTION_TIMEOUT_RATE;
                sendingImageMessage = true;
                if (imageDownloader != null) imageDownloader.Dispose();
                imageDownloader = new VRCImageDownloader();
	            imageDownloader.DownloadImage(currentlySelectedURL, null, (UdonBehaviour)(Component)imageDownloaderListener, null);
            }
        }

	    //Select which type of downloader can be used for this part, 
	    //StringDownloader will always be used on the first and last message to ensure the message was recieved
	    //This will mean that messages may be delayed by up to 5 seconds on the first message if a previous message was sent within that time
	    //A message that consists of two requests will not benefit from the Image Downloader optimization and may take up to, above delay + 5s, max 10s
	    //Anything above two requests will have: (potential delay from above) + request sent out every 2.5s
        //True means String Downloader
        //False means Video Downloader
        private bool SelectMessengerType()
        {
            if (forceStringLoader) return true;

            //If this message is the last one then use the String Downloader
            if (currentMessageIndex == currentMessageEndIndex) return true;

            //Alternate between String Downloader and Image Downloader
            //Starting with the Image Downloader to ensure no wait time for String Downloader on the final message
            return (currentMessageEndIndex - currentMessageIndex) % 2 != 0;
        }

        //Select the next available message in the buffer if available
        private int SelectMessage()
        {
            if (messageBuffer.Length == 0 || currentMessageIndex >= messageBuffer.Length) return -1;
            return messageBuffer[currentMessageIndex];
        }

	    //Increment the current message index and run any checks on the message buffer
	    private void IncrementMessage()
        {
            //Message buffer shouldn't ever be empty but just in case
            if (messageBuffer.Length == 0) return;
            currentMessageIndex++;

            //Check if the next message is not the end of this message buffer
            if (currentMessageIndex < currentMessageEndIndex) return;

            //reset the current message index and remove the first message buffer from the current message buffer
            if (currentMessageEndIndex + 1 == messageBuffer.Length)
            {
                messageBuffer = new int[0];
            }
            else
            {
                ArrayUtilities.RemoveRangeFromArray(ref messageBuffer, 0, currentMessageEndIndex + 1);
            }
            currentMessageIndex = 0;
            currentMessageEndIndex = -1;

            if (messageBuffer.Length == 0) return;

            //find the next currentMessageEndIndex if there are more messages
            currentMessageEndIndex = ArrayUtilities.ReturnFirstIndexOfValueFromArray(messageBuffer, -1);
        }

	    //Virtual function for any children to add onto the Start function
        protected virtual void ManagerStart()
        {

        }

	    //Virtual function for any children to add onto the Update function
        protected virtual void ManagerUpdate()
        {

        }

	    //Required function for any children to implement to recieve messages
        public abstract void HandleMessage(string response);

	    //Converts a text to SHA512 using UdonHashLib
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

	    //Adds a single message to the buffer to be sent
        public void AddMessageToBuffer(int message)
        {
            ArrayUtilities.AddToArray(ref messageBuffer, message);
            ArrayUtilities.AddToArray(ref messageBuffer, -1);
            if (currentMessageEndIndex == -1) currentMessageEndIndex = messageBuffer.Length - 1;
        }

	    //Adds multiple messages to the buffer to be sent (usually batched with a single response type on the first message)
        public void AddMessagesToBuffer(int[] messages)
        {
            ArrayUtilities.AddRangeToArray(ref messageBuffer, messages);
            ArrayUtilities.AddToArray(ref messageBuffer, -1);
            if (currentMessageEndIndex == -1) currentMessageEndIndex = messageBuffer.Length - 1;
        }
    }

}