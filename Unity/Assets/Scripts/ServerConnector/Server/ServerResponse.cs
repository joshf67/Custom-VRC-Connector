
using Joshf67.ServerConnector.Development;
using Joshf67.ServerConnector.Packing;
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;

namespace Joshf67.ServerConnector.Server
{

    /// <summary>
    /// Base abstract class to allow generic server response reading
    /// </summary>
    public abstract class ServerResponse : UdonSharpBehaviour
    {
        /// <summary>
        /// Store the parsed type of this response into an easily accessible place to not have to cast in the future
        /// </summary>
        private ServerResponseType _type = ServerResponseType.None;

        /// <summary>
        /// Public getter for the type of response
        /// </summary>
        public ServerResponseType Type
        {
            get { return _type; }
        }

        /// <summary>
        /// Stores the parsed content of the response message into an easily accessible place
        /// </summary>
        private DataToken _content;

        /// <summary>
        /// Public getter for the content of the response
        /// </summary>
        public DataToken Content
        {
            get { return _content; }
        }

        /// <summary>
        /// Stores the full message recieved from a server into an easily accessible place
        /// </summary>
        private DataToken _response;

        /// <summary>
        /// Public getter for the full message recieved from a server
        /// </summary>
        public DataToken Response
        {
            get { return _response; }
        }

        /// <summary>
        /// Parses a JSON string into the JSON DataDictionary equivilent
        /// </summary>
        /// <param name="message"> The JSON String to parse </param>
        /// <param name="response"> The resulting JSON DataDictionary </param>
        /// <returns> If the message was parsed correctly </returns>
        private static bool ParseInitialResponse(string message, out DataDictionary response)
        {
            DataToken serverResponse;
            if (VRCJson.TryDeserializeFromJson(message, out serverResponse))
            {
                if (DevelopmentManager.IsServerResponseEnabled(DevelopmentMode.Advanced))
                {
                    LogServerResponse(serverResponse);
                }

                DataToken discard;
                if (serverResponse.TokenType != TokenType.DataDictionary ||
                    !serverResponse.DataDictionary.TryGetValue("type", TokenType.Double, out discard) ||
                    !serverResponse.DataDictionary.TryGetValue("response", out discard))
                {
                    if (DevelopmentManager.IsServerResponseEnabled(DevelopmentMode.Basic))
                        Debug.Log("Parsed Server Response but it was in an incorrect format: " +
                        serverResponse.TokenType);

                    response = new DataDictionary();
                    return false;
                }

                response = serverResponse.DataDictionary;

                return true;
            }

            if (DevelopmentManager.IsServerResponseEnabled(DevelopmentMode.Basic))
            {
                Debug.Log("Server Response failed to parse into JSON: " + serverResponse);
            }

            response = new DataDictionary();
            return false;
        }

        /// <summary>
        /// Parses a JSON String and finds the Response type
        /// </summary>
        /// <param name="response"> The JSON String to parse </param>
		/// <returns> The response type of the message </returns>
        public static ServerResponseType GetMessageType(string response)
        {
            DataDictionary responseParsed;
            if (ParseInitialResponse(response, out responseParsed))
                return GetMessageType(responseParsed);

            return ServerResponseType.None;
        }


        /// <summary>
        /// Parses a JSON DataDictionary and finds the Response type
        /// </summary>
        /// <param name="response"> The JSON DataDictionary to parse </param>
		/// <returns> The response type of the message </returns>
        public static ServerResponseType GetMessageType(DataDictionary response)
        {
            DataToken responseType;

            if (response.TryGetValue("type", TokenType.Double, out responseType))
            {
                if (DevelopmentManager.IsServerResponseEnabled(DevelopmentMode.Basic))
                {
                    Debug.Log("Parsed Server Response and getting type: " + responseType.Double);
                }

                return (ServerResponseType)Convert.ToInt32(responseType.Double);
            }

            if (DevelopmentManager.IsServerResponseEnabled(DevelopmentMode.Basic))
                Debug.Log("Failed to parse Server Response type");

            return ServerResponseType.None;
        }

        /// <summary>
        /// Parses a JSON String and finds the Response content
        /// </summary>
        /// <param name="response"> The JSON String to parse </param>
        /// <returns> The content of a response or an error if invalid </returns>
        public static DataToken GetMessageContent(string response)
        {
            DataDictionary responseParsed;
            if (ParseInitialResponse(response, out responseParsed))
                return GetMessageContent(responseParsed);

            return new DataToken(DataError.UnableToParse);
        }

        /// <summary>
        /// Parses a JSON DataDictionary and finds the Response content
        /// </summary>
        /// <param name="response"> The JSON DataDictionary to parse </param>
        /// <returns> The content of a response or an error if invalid </returns>
        public static DataToken GetMessageContent(DataDictionary response)
        {
            DataToken result;
            if (response.TryGetValue("response", out result))
            {
                if (DevelopmentManager.IsServerResponseEnabled(DevelopmentMode.Basic))
                {
                    Debug.Log("Parsed Server Response and getting content: " + result.TokenType);
                }

                return result;
            }

            if (DevelopmentManager.IsServerResponseEnabled(DevelopmentMode.Basic))
                Debug.Log("Failed to parse Server Response content");

            return new DataToken(DataError.UnableToParse);
        }

        /// <summary>
        /// Parses a JSON String and handle the message
        /// </summary>
        /// <param name="message"> The JSON String to parse </param>
        /// <param name="HandleResult"> The result of the ServerRespons handler </param>
        /// <returns> Boolean if the response was handled or not </returns>
        public bool Parse(string message, out DataToken HandleResult)
        {
            DataDictionary serverResponse;
            if (ParseInitialResponse(message, out serverResponse))
            {
                _response = serverResponse;
                _type = GetMessageType(serverResponse);
                _content = GetMessageContent(serverResponse);
                return HandleResponse(serverResponse, out HandleResult);
            }

            _response = new DataToken(DataError.UnableToParse);
            HandleResult = new DataToken(DataError.UnableToParse);
            return false;
        }

        /// <summary>
        /// Parses a JSON String and handle the response but ignore the result
        /// </summary>
        /// <param name="message"> The JSON String to parse </param>
        /// <returns> Boolean if the response was handled or not </returns>
        public bool ParseWithoutResult(string message)
        {
            DataToken ignoreHandleResult;
            DataDictionary serverResponse;
            if (ParseInitialResponse(message, out serverResponse))
            {
                _response = serverResponse;
                _type = GetMessageType(serverResponse);
                _content = GetMessageContent(serverResponse);
                return HandleResponse(serverResponse, out ignoreHandleResult);
            }

            _response = new DataToken(DataError.UnableToParse);
            return false;
        }

        /// <summary>
        /// Parses a JSON String and does not try to handle it, useful for saving logic for later
        /// </summary>
        /// <param name="message"> The JSON String to parse </param>
        /// <returns> Boolean if the response was handled or not </returns>
        public bool ParseWithoutHandling(string message)
        {
            DataDictionary serverResponse;
            if (ParseInitialResponse(message, out serverResponse))
            {
                _response = serverResponse;
                _type = GetMessageType(serverResponse);
                _content = GetMessageContent(serverResponse);
            }

            _response = new DataToken(DataError.UnableToParse);
            return false;
        }

        /// <summary>
        /// Handles the parsed message
        /// </summary>
        /// <param name="response"> The JSON DataDictionary to be handled </param>
        /// <param name="HandleResult"> The result of the ServerRespons handler </param>
        /// <returns> Boolean if the response was parsed properly </returns>
        protected abstract bool HandleResponse(DataDictionary response, out DataToken HandleResult);

        /// <summary>
        /// Logs a server response and all of it's variables
        /// </summary>
        /// <param name="response"> The response to parse </param>
        /// <param name="parsedLevel"> The current level of recursion in the parse </param>
        /// <param name="recusrionStack"> Recursion causes issues with variables, so implement own stack </param>
        protected static void LogServerResponse(DataToken response)
        {
            Debug.Log("Logging server response to console:");
            DataList recursionStack = new DataList();
            recursionStack.Add(response);
            LogServerResponse(0, ref recursionStack);
        }

        /// <summary>
        /// Recursively logs a server response
        /// </summary>
        /// <param name="parsedLevel"> The current level of recursion in the parse </param>
        /// <param name="recusrionStack"> Recursion causes issues with variables, so implement own stack </param>
        private static void LogServerResponse(int indentLevel, ref DataList recusrionStack)
        {
            //Indent has to be manually calculated
            string indentLevelString = "";
            for (int i = 0; i < indentLevel; i++)
            {
                indentLevelString += "  ";
            }

            //Due to recursion variable options, implent a stack and grab the variable from that
            switch (recusrionStack[recusrionStack.Count - 1].TokenType)
            {
                case TokenType.DataList:
                    for (int i = 0; i < recusrionStack[recusrionStack.Count - 1].DataList.Count; i++)
                    {
                        if (recusrionStack[recusrionStack.Count - 1].DataList[i].TokenType == TokenType.DataList ||
                            recusrionStack[recusrionStack.Count - 1].DataList[i].TokenType == TokenType.DataDictionary)
                        {
                            //Add the current loop variables to the stack
                            recusrionStack.Add(i);
                            recusrionStack.Add(recusrionStack[recusrionStack.Count - 2].DataList[i]);
                            LogServerResponse(indentLevel + 1, ref recusrionStack);

                            //Resetup the loop variables after recursion
                            i = recusrionStack[recusrionStack.Count - 1].Int;
                            recusrionStack.RemoveAt(recusrionStack.Count - 1);
                        }
                        else
                        {
                            Debug.Log(indentLevelString +
                                ByteConverter.ReturnDataTokenValueAsObject(recusrionStack[recusrionStack.Count - 1]));
                        }
                    }
                    break;

                case TokenType.DataDictionary:
                    DataList keys = recusrionStack[recusrionStack.Count - 1].DataDictionary.GetKeys();
                    DataList values = recusrionStack[recusrionStack.Count - 1].DataDictionary.GetValues();
                    for (int i = 0; i < keys.Count; i++)
                    {
                        Debug.Log(indentLevelString + "Key: " +
                            ByteConverter.ReturnDataTokenValueAsObject(keys[i]) + ", " + keys[i].TokenType + ", Value: " +
                            ByteConverter.ReturnDataTokenValueAsObject(values[i]) + ", " + values[i].TokenType);

                        //If the field is another List/Dictionary then recurse into it
                        if (values[i].TokenType == TokenType.DataList ||
                            values[i].TokenType == TokenType.DataDictionary)
                        {
                            //Add the current loop variables to the stack
                            recusrionStack.Add(i);
                            recusrionStack.Add(values[i]);
                            LogServerResponse(indentLevel + 1, ref recusrionStack);

                            //Resetup the loop variables after recursion
                            i = recusrionStack[recusrionStack.Count - 1].Int;
                            recusrionStack.RemoveAt(recusrionStack.Count - 1);
                            keys = recusrionStack[recusrionStack.Count - 1].DataDictionary.GetKeys();
                            values = recusrionStack[recusrionStack.Count - 1].DataDictionary.GetValues();
                        }
                    }
                    break;

                default:
                    Debug.Log(indentLevelString +
                        ByteConverter.ReturnDataTokenValueAsObject(recusrionStack[recusrionStack.Count - 1]));
                    break;
            }

            //Remove the variable from the stack once used
            recusrionStack.RemoveAt(recusrionStack.Count - 1);
        }
    }

}