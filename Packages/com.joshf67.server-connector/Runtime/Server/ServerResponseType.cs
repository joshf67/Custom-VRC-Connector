namespace Joshf67.ServerConnector.Server
{
    
	/// <summary>
	/// Used to determine the response type from a server
	/// </summary>
	public enum ServerResponseType
	{
		/// <summary>
		/// The server did not respond with a type
		/// </summary>
		None = -100,

		/// <summary>
		/// The server was not expecting a response
		/// </summary>
		Unexpected_Request = -99,

		/// <summary>
		/// The server was expecting a message with a different type
		/// </summary>
		Type_Fail = -98,

		/// <summary>
		/// The server was unable to parse the message
		/// </summary>
		Failed_To_Parse = -97,
		
		/// <summary>
		/// The server encountered an error with the request
		/// </summary>
		Server_Error = -40,
		
		/// <summary>
		/// The user is not currently logged in when making a request
		/// </summary>
		User_Not_Logged_In = -2,

		/// <summary>
		/// The server failed to recieve and handle the message for some reason
		/// </summary>
		Failed = -1,

		/// <summary>
		/// The server succeeded in recieving and handling the message
		/// </summary>
		Succeeded = 0,
		
		/// <summary>
		/// The login hash was updated
		/// </summary>
		Login_Updated = 1,

		/// <summary>
		/// The server failed to find a user for the built login hash
		/// </summary>
		Login_Failed = 2,

		/// <summary>
		/// The server managed to find a user for the built login hash
		/// </summary>
		Login_Complete = 3,
		
		/// <summary>
		/// The server was able to create a user for the built login hash
		/// </summary>
		Account_Creation_Complete = 4,
		
		/// <summary>
		/// The server has recieved an update to the item related message
		/// </summary>
		Item_Updated = 5,

        /// <summary>
        /// The server has updated a user's inventory items to add the new items
        /// </summary>
        Added_Item = 6,

		/// <summary>
		/// The server has updated a user's inventory items to remove the requested items
		/// </summary>
		Removed_Item = 7,
	}

}