namespace Joshf67.ServerConnector
{
    
	/// <summary>
	/// Used so the server can understand what is supposed to happen for a response
	/// </summary>
	public enum ConnectorMessageType
	{
		/// <summary>
		/// The default value, will fail to send
		/// </summary>
		Invalid = -1,

		/// <summary>
		/// Any messages relating to logging in
		/// </summary>
        Login = 0,

		/// <summary>
		/// Any messages relating to account creation
		/// </summary>
	    AccountCreation = 1,

		/// <summary>
		/// Any general messages
		/// </summary>
		GeneralMessage = 2,

		/// <summary>
		/// Any item related messages
		/// </summary>
		ModifyItem = 3,

		/// <summary>
		/// A message reserved for acknowledging that the user still exists
		/// </summary>
	    AcknowledgeMessage = 14,

		/// <summary>
		/// A message reserved for any variable length data that can't be calculated
		/// </summary>
        MessageFinished = 15
    }

}