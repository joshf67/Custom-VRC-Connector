using UdonSharp;
using VRC.Udon;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.StringLoading;
using VRC.SDK3.Image;

using Joshf67.ServerConnector.Development;
using Joshf67.ServerConnector.Downloader;
using VRC.SDK3.Data;

namespace Joshf67.ServerConnector
{

    /// <summary>
    /// Abstract base class to enable easy connections to for servers
    /// </summary>
	[RequireComponent(typeof(ImageDownloaderListener), typeof(StringDownloaderListener))]
	public abstract class Connector : UdonSharpBehaviour
    {
        /// <summary>
        /// Used to translate messages into URLs
        /// </summary>
	    [SerializeField]
	    private ConnectorUrlToolManager URLLookup;

        /// <summary>
        /// Stores the current message to enable re-sending if necessary
        /// </summary>
	    private VRCUrl currentlySelectedURL;

        /// <summary>
        /// Used to listen to onSuccess/onError of Stirng downloaders due to old Image downloader thread errors
        /// </summary>
        [SerializeField]
	    private StringDownloaderListener stringDownloaderListener;

        /// <summary>
        /// Used to listen to onSuccess/onError due to old Image downloader thread errors
        /// </summary>
        [SerializeField]
	    private ImageDownloaderListener imageDownloaderListener;

        /// <summary>
        /// Controls how many bits are packed into each message
        /// </summary>
	    [SerializeField]
        protected byte packingMessageBitSize = 21;

        /// <summary>
        /// Controls how many bits are used for specifying the message type
        /// </summary>
        [SerializeField]
        protected byte messageTypeSize = 4;

        /// <summary>
        /// Used to make sure only 1 message is used by each type every 5 seconds
        /// </summary>
        private readonly float CONNECTION_TIMEOUT_RATE = 5;

        /// <summary>
        /// Used to retry a message if the server doesn't respond
        /// </summary>
        private readonly float CONNECTION_RETRY_RATE = 15;

        /// <summary>
        /// Stores the current timeout (due to VRC) on the String downloader
        /// </summary>
        [SerializeField]
        private float stringRequestTimeout = 0;

        /// <summary>
        /// Stores if the String Downloader is currently in use and awaiting a response
        /// </summary>
        [SerializeField]
        private bool sendingStringMessage = false;

        /// <summary>
        /// Stores the current timeout (due to VRC) on the Image downloader
        /// </summary>
        [SerializeField]
        private float imageRequestTimeout = 0;

        /// <summary>
        /// Stores if the String Downloader is currently in use and awaiting a response
        /// </summary>
        [SerializeField]
        private bool sendingImageMessage = false;
        
        /// <summary>
        /// Stores the image result from an Image Downloader to be disposed of after a request
        /// </summary>
        private VRCImageDownloader imageDownloader = new VRCImageDownloader();

        /// <summary>
        /// Holds all of the messages to send in DataList buffers
        /// </summary>
        [SerializeField]
        private DataList messageBuffer = new DataList();

        /// <summary>
        /// The current message being sent
        /// </summary>
        [SerializeField]
        private int currentMessageIndex = 0;

        /// <summary>
        /// Setup all the required downloader variables when initiated
        /// </summary>
        public void Start()
	    {
		    stringDownloaderListener = GetComponent<StringDownloaderListener>();
		    imageDownloaderListener = GetComponent<ImageDownloaderListener>();
		    
            stringRequestTimeout = CONNECTION_TIMEOUT_RATE;
		    imageRequestTimeout = CONNECTION_TIMEOUT_RATE;
            
            imageDownloader = new VRCImageDownloader();
            ManagerStart();
        }

        /// <summary>
        /// Update the String/Image downloader variables and set up next requests
        /// </summary>
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

        /// <summary>
        /// Check if the a response from the server has been returned and update the string downloader variable
        /// </summary>
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
		    
		    //Check if a String Downloader has recieved unexpected message response from the message
		    if (stringDownloaderListener.DownloaderStatus == DownloaderMessageStatus.Unexpected_Request) {
		    	
		    	//TODO Figure out what this error means
			    stringDownloaderListener.DownloaderStatus = DownloaderMessageStatus.Awaiting_Request;
			    sendingStringMessage = false;
			    currentMessageIndex = 0;
		    }
		    
		    //Check if a String Downloader has recieved user not logged in response from the message
		    if (stringDownloaderListener.DownloaderStatus == DownloaderMessageStatus.User_Not_Logged_In) {
		    	
			    //TODO: Initiate requsting user to log in
			    stringDownloaderListener.DownloaderStatus = DownloaderMessageStatus.Awaiting_Request;
			    sendingStringMessage = false;
			    currentMessageIndex = 0;
		    }
		    
		    if (stringDownloaderListener.DownloaderStatus == DownloaderMessageStatus.Server_Error) {
		    	
			    //TODO Figure out what to do when this error occurs
			    Debug.Log("An error has occured on the server side...");
		    }
	    }

        /// <summary>
        /// Check if the a response from the server has been returned and update the image downloader variable
        /// </summary>
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

        /// <summary>
        /// Make sure that everything is ready to send off a message before sending off a message
        /// </summary>
        /// <returns> If the message is ready to be sent </returns>
        private bool ReadyToSendMessage()
        {
            if (messageBuffer.Count == 0) return false;

            //A message hasn't been accepted/rejected so await for it to go through
            if (sendingImageMessage || sendingStringMessage) return false;

            //True means String Downloader is being used so wait for it to be ready
            //False means Video Downloader is being used so wait for it to be ready
            return SelectMessengerType() ? stringRequestTimeout < 0 : imageRequestTimeout < 0;
        }

        /// <summary>
        /// Handle sending a message to a server if available
        /// </summary>
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
                    if (DevelopmentManager.IsConnectorEnabled(DevelopmentMode.Basic))
                        Debug.LogError($"Invalid String URL for message {message}, please ensure this is set up correctly");

                    //Implement an error handler here for when your URLs are setup incorrectly
                    return;
                }

                if (DevelopmentManager.IsConnectorEnabled(DevelopmentMode.Basic))
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
                    if (DevelopmentManager.IsConnectorEnabled(DevelopmentMode.Basic))
                        Debug.LogError($"Invalid Image URL for message {message}, please ensure this is set up correctly");

                    //Implement an error handler here for when your URLs are setup incorrectly
                    return;
                }

	            if (DevelopmentManager.IsConnectorEnabled(DevelopmentMode.Basic))
                    Debug.Log($"Sending Image Downloader message of {message} to {currentlySelectedURL}");

                //Send off a request to the Image Downloader
                imageRequestTimeout = CONNECTION_TIMEOUT_RATE;
                sendingImageMessage = true;
                if (imageDownloader != null) imageDownloader.Dispose();
                imageDownloader = new VRCImageDownloader();
	            imageDownloader.DownloadImage(currentlySelectedURL, null, (UdonBehaviour)(Component)imageDownloaderListener, null);
            }
        }

        /// <summary>
        /// <para> 
        /// Select which type of downloader can be used for this part of the message.
        /// </para>
        /// <para>
        /// StringDownloader will always be used on the first and last message to ensure the message was recieved
        /// This will mean that messages may be delayed by up to 5 seconds on the first message if a previous message was sent within that time
        /// A message that consists of two requests will not benefit from the Image Downloader optimization and may take up to, above delay + 5s, max 10s
        /// Anything above two requests will have: (potential delay from above) + request sent out every 2.5s
        /// </para>
        /// </summary>
        /// <returns> The type of Downloader to use, True means String Downloader, False means Video Downloader</returns>
        private bool SelectMessengerType()
        {
            //If this message is the last one then use the String Downloader
            if (currentMessageIndex == 0 || currentMessageIndex == messageBuffer[0].DataList.Count - 1) return true;

            //Alternate between String Downloader and Image Downloader
            //Starting with the Image Downloader on index 1
            return currentMessageIndex % 2 == 0;
        }

        /// <summary>
        /// Select the next available message in the buffer if available
        /// </summary>
        /// <returns> The message/url to send </returns>
        private int SelectMessage()
        {
            if (messageBuffer.Count == 0 || currentMessageIndex >= messageBuffer[0].DataList.Count) return -1;

            DataToken message;
            if (messageBuffer[0].DataList.TryGetValue(currentMessageIndex, out message))
            {
                return message.Int;
            }
            return -1;
        }

        /// <summary>
        /// Increment the current message index and run any checks on the message buffer
        /// </summary>
        private void IncrementMessage()
        {
            //Message buffer shouldn't ever be empty but just in case
            if (messageBuffer.Count == 0) return;
            currentMessageIndex++;

            //Check if the next message is not the end of this message buffer
            if (currentMessageIndex < messageBuffer[0].DataList.Count) return;

            //reset the current message index and remove the first message buffer from the current message buffer
            messageBuffer.RemoveAt(0);
            currentMessageIndex = 0;
        }

        /// <summary>
        /// Virtual function for any children to add onto the Start function
        /// </summary>
        protected virtual void ManagerStart() {}

        /// <summary>
        /// Virtual function for any children to add onto the Update function
        /// </summary>
        protected virtual void ManagerUpdate() {}

        /// <summary>
        /// Required function for any children to implement to recieve messages
        /// </summary>
        /// <param name="response"> The server XML response </param>
        public abstract void HandleMessage(string response);

        /// <summary>
        /// Adds a single message to the buffer to be sent
        /// </summary>
        /// <param name="message"> The message to add </param>
        public void AddMessageToBuffer(int message)
        {
            DataList newMessage = new DataList();
            newMessage.Add(message);
            messageBuffer.Add(newMessage);
        }

        /// <summary>
        /// Adds multiple messages to the buffer to be sent (usually batched with a single response type on the first message)
        /// </summary>
        /// <param name="messages"> The messages to add </param>
        public void AddMessagesToBuffer(DataList messages)
        {
            //Ensure all messages are of type int
            for (int messageIndex = 0; messageIndex < messages.Count; messageIndex++) 
            {
                if (messages[messageIndex].TokenType == TokenType.Int)
                    continue;

                if (DevelopmentManager.IsConnectorEnabled(DevelopmentMode.Basic))
                    Debug.Log("Message being added to Connector's message buffer is in an invalid format");

                return;
            }

            if (DevelopmentManager.IsConnectorEnabled(DevelopmentMode.Basic))
            {
                Debug.Log("Messages have been added to Connector");
                for (int messageIndex = 0; messageIndex < messages.Count; messageIndex++)
                {
                    Debug.Log(messages[messageIndex].Int);
                }
            }
            messageBuffer.Add(messages);
        }
    }

}