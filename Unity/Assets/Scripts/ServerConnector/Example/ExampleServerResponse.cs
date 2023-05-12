using Joshf67.ServerConnector.Development;
using UnityEngine;
using VRC.SDK3.Data;
using Joshf67.ServerConnector.Server;

namespace Joshf67.ServerConnector.Example
{

    /// <summary>
    /// Example response to show how to handle server responses
    /// </summary>
    public class ExampleServerResponse : ServerResponse
    {

        /// <summary>
        /// Handles the parsed message
        /// </summary>
        /// <param name="response"> The JSON DataDictionary to be handled </param>
        /// <param name="HandleResult"> The result of the ServerRespons handler </param>
        /// <returns> Boolean if the response was parsed properly </returns>
        override protected bool HandleResponse(DataDictionary response, out DataToken HandleResult)
        {
            HandleResult = new DataToken(DataError.None);
            bool messageWasParsedSucessfully = false;

            if (Type == ServerResponseType.Login_Failed)
            {
                if (DevelopmentManager.IsServerResponseEnabled(DevelopmentMode.Basic))
                    Debug.Log("ServerExampleResponse is handling a login failed type");

                //The login has failed for some reason, so we need to display this to the user
                HandleResult = new DataToken("Login credentials are invalid, please try again or create an account");
            }
            else if (Type == ServerResponseType.Login_Updated)
            {
                if (DevelopmentManager.IsServerResponseEnabled(DevelopmentMode.Basic))
                    Debug.Log("ServerExampleResponse is handling a login update type");

                //Server sends message with a string response so this will display the bits remaining until login completion
                messageWasParsedSucessfully = response.TryGetValue("response", TokenType.String, out HandleResult);
            }
            else if (Type == ServerResponseType.Login_Complete)
            {
                if (DevelopmentManager.IsServerResponseEnabled(DevelopmentMode.Basic))
                    Debug.Log("ServerExampleResponse is handling a login complete type");

                messageWasParsedSucessfully = response.TryGetValue("response", TokenType.DataDictionary, out HandleResult);
            }
            else if (Type == ServerResponseType.Account_Creation_Complete)
            {
                if (DevelopmentManager.IsServerResponseEnabled(DevelopmentMode.Basic))
                    Debug.Log("ServerExampleResponse is handling a account creation complete type");

                messageWasParsedSucessfully = response.TryGetValue("response", TokenType.DataDictionary, out HandleResult);
            }
            else if (Type == ServerResponseType.Item_Updated)
            {
                if (DevelopmentManager.IsServerResponseEnabled(DevelopmentMode.Basic))
                    Debug.Log("ServerExampleResponse is handling a item update type");

                //Server sends message with a string response so this will display the bits remaining until item adding/remove message completion
                messageWasParsedSucessfully = response.TryGetValue("response", TokenType.String, out HandleResult);
            }
            else if (Type == ServerResponseType.Added_Item)
            {
                if (DevelopmentManager.IsServerResponseEnabled(DevelopmentMode.Basic))
                    Debug.Log("ServerExampleResponse is handling a Added item type");

                messageWasParsedSucessfully = response.TryGetValue("response", TokenType.DataList, out HandleResult);
            }
            else if (Type == ServerResponseType.Removed_Item)
            {
                if (DevelopmentManager.IsServerResponseEnabled(DevelopmentMode.Basic))
                    Debug.Log("ServerExampleResponse is handling a Removed item type");

                messageWasParsedSucessfully = response.TryGetValue("response", TokenType.DataList, out HandleResult);
            }

            return !messageWasParsedSucessfully;
        }
    }

}