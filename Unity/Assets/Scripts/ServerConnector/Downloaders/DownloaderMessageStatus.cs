namespace Joshf67.ServerConnector.Downloader
{

    /// <summary>
    /// Used to determine what response was recieved from 
    /// </summary>
	public enum DownloaderMessageStatus
	{
		/// <summary>
		/// The downloader failed to connect to the server
		/// </summary>
		Server_Error = -99,

        /// <summary>
        /// 
        /// </summary>
        Unexpected_Request = -1,
		
		/// <summary>
		/// The downloader is awaiting a response from the server
		/// </summary>
		Awaiting_Request = 0,

        /// <summary>
        /// The downloader successfully sent a message
        /// </summary>
        Message_Sent = 1,

        /// <summary>
        /// The downloader failed to connect to the server
        /// </summary>
        Failed_To_Send = 2,

        /// <summary>
        /// The server responded with an invalid type response
        /// </summary>
        Type_Fail = 3,

        /// <summary>
        /// The server responded with a user not logged in response
        /// </summary>
        User_Not_Logged_In = 4,

        /// <summary>
        /// The server responded with an error
        /// </summary>
        Request_Error = 5
	}

}