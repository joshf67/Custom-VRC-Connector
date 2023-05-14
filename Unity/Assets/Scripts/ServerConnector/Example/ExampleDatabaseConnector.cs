using System;
using UnityEngine;
using UnityEngine.UI;

using Joshf67.ServerConnector.Server;
using VRC.SDK3.Data;
using Joshf67.ServerConnector.Development;
using Joshf67.ServerConnector.Packing;

namespace Joshf67.ServerConnector.Example
{

    /// <summary>
    /// Example connector to show how to connect to a databse and read/update values
    /// </summary>
    public class ExampleDatabaseConnector : Connector
    {
        /// <summary>
        /// The field used to accept login inputs for account messages
        /// </summary>
        public InputField usernameField;

        /// <summary>
        /// The field used to accept password inputs for account messages
        /// </summary>
        public InputField passwordField;

        /// <summary>
        /// The field used to display the server's responses to the user
        /// </summary>
        public Text responseField;

        /// <summary>
        /// The server response handler to be used for this example
        /// </summary>
        public ExampleServerResponse ExampleResponseParser;

        private double currency = 0;
        private DataList items = new DataList();

        /// <summary>
        /// The field used to display all the user's items
        /// </summary>
        public Text itemField;

        /// <summary>
        /// The field used to accept inputs for item addition messages
        /// </summary>
        public InputField AddItemField;

        /// <summary>
        /// The field used to accept inputs for item removal messages
        /// </summary>
        public InputField RemoveItemField;

        /// <summary>
        /// Used to hash any messages for simple encryption
        /// </summary>
        [SerializeField]
        protected UdonHashLib hasher;

        /// <summary>
        /// Queue up login messages to send to the server and attempt to log in
        /// </summary>
        public void Login()
        {
            AddMessagesToBuffer(MessagePacker.PackMessageBytesToURL(GenerateLoginHashBytes(), ConnectorMessageType.Login, packingMessageBitSize, messageTypeSize));
        }

        /// <summary>
        /// Queue up a create account message to the server and attempt to create an account
        /// </summary>
        public void CreateAccount()
        {
            AddMessagesToBuffer(MessagePacker.PackMessageBytesToURL(GenerateLoginHashBytes(), ConnectorMessageType.AccountCreation, packingMessageBitSize, messageTypeSize));
        }

        /// <summary>
        /// Queue up an add item message to the server and attempt to add it
        /// </summary>
        public void AddItem()
        {
            if (AddItemField.text == "") return;

            //Only allow 8 bit indices for the item to add
            byte itemId = (byte)(int.Parse(AddItemField.text) & 0xFF);
            if (itemId < 0 || itemId > 255) return;

            //Set up the options to send to the server's add item function
            //1st bit = adding, next 7 bits = The amount of items to add, next 8 bits... = itemId
            AddMessagesToBuffer(
                MessagePacker.PackMessageBytesToURL(
                    new object[] {
                    MessagePacker.CompressMessage(true, 1),
                    MessagePacker.CompressMessage((byte)1, 7),
                    MessagePacker.CompressMessage(itemId)
                    }, ConnectorMessageType.ModifyItem, packingMessageBitSize, messageTypeSize
                )
            );
        }

        /// <summary>
        /// Queue up a remove item message to the server and attempt to remove it
        /// </summary>
        public void RemoveItem()
        {
            if (RemoveItemField.text == "") return;

            //Only allow 7 bit indices for removing
            byte itemIndex = (byte)(int.Parse(RemoveItemField.text) & 0xFF);
            if (itemIndex < 0 || itemIndex > 255) return;

            DataList packingParams = new DataList();
            packingParams.Add(MessagePacker.CompressMessage(false, 1));
            packingParams.Add(MessagePacker.CompressMessage((byte)1, 7));
            packingParams.Add(MessagePacker.CompressMessage(itemIndex));

            //Set up the options to send to the server's add item function
            //1st bit = removing, next 7 bits = The amount of indices to remove, next 8 bits... = index to remove
            AddMessagesToBuffer(
                MessagePacker.PackMessageBytesToURL(
                    packingParams, ConnectorMessageType.ModifyItem,
                    packingMessageBitSize, messageTypeSize
                )
            );
        }

        /// <summary>
        /// Generate a login hash based on a set of pre-determined parameters
        /// </summary>
        /// <returns> Returns the compressed login hash </returns>
        private DataDictionary GenerateLoginHashBytes()
        {
            string usernameHash = ConvertSHA256ToMessage(ConvertTextToHash(usernameField.text + passwordField.text));

            if (DevelopmentManager.IsConnectorEnabled(DevelopmentMode.Advanced))
            {
                Debug.Log(usernameHash);
                Debug.Log("Login hash converted size: " + ByteConverter.ConvertUTF8String(usernameHash).Count);
            }

            return MessagePacker.CompressMessage(ByteConverter.ConvertUTF8String(usernameHash));
        }

        /// <summary>
        /// Converts a text to SHA512 using UdonHashLib
        /// </summary>
        /// <param name="text"> The text to convert </param>
        /// <returns> A SHA512 hash of the text</returns>
        public string ConvertTextToHash(string text)
        {
            return hasher.SHA512_UTF8(text);
        }

        /// <summary>
        /// Convert a SHA256 hash into a shorter 9 character hash to decrease login message time,
        /// This decreases the security and increase the chance of collision but due to 2.5s rate limiting this is required to make it reasonable
        /// </summary>
        /// <param name="hash"> The string to hash </param>
        /// <returns> A 9 character string message of the hashed value </returns>
        public string ConvertSHA256ToMessage(string hash)
        {
            if (hash.Length != 128) return "";
            return new string(new char[] { hash[0], hash[15], hash[31], hash[47], hash[63], hash[79], hash[95], hash[111], hash[127] });
        }

        /// <summary>
        /// Take a UserSchema and setup the display for the data
        /// </summary>
        /// <param name="userData"> The user data to use with the setup </param>
        private void SetupUserData(DataToken userData)
        {
            if (!UserSchema.IsUserSchema(userData, out DataDictionary userSchema))
            {
                if (DevelopmentManager.IsConnectorEnabled(DevelopmentMode.Basic))
                    Debug.Log("Trying to setup user data with invalid data, is not UserSchema");

                return;
            }

            currency = InvetorySchema.GetCurrency(userData);
            items.Clear();
            items = InvetorySchema.GetInventoryItems(userData);

            RenderInventoryText();
        }

        /// <summary>
        /// Renders out the inventory to the display
        /// </summary>
        private void RenderInventoryText()
        {
            itemField.text = $"Currency: {currency}\n";
            for (int i = 0; i < items.Count; i++)
            {
                itemField.text += $"Item {i}: {ItemSchema.GetItemID(items[i])}\n";
            }
        }

        /// <summary>
        /// Accept any responses from the server and handle them
        /// </summary>
        /// <param name="response"> The response from the server </param>
        public override void HandleMessage(string response)
        {

            if (DevelopmentManager.IsConnectorEnabled(DevelopmentMode.Basic))
                Debug.Log("Server message has been recieved and is being handled by example connector");

            ExampleResponseParser.Parse(response, out DataToken HandlerResult);
            switch (ExampleResponseParser.Type)
            {
                case ServerResponseType.Login_Updated:
                    responseField.text = $"Recieved a login update message with the response: {HandlerResult.String}";
                    break;

                case ServerResponseType.Login_Complete:
                    SetupUserData(ExampleResponseParser.Content);
                    responseField.text = $"Recieved a login complete message with the response: {UserSchema.GetLoginHash(HandlerResult.DataDictionary)}";
                    break;

                case ServerResponseType.Login_Failed:
                    responseField.text = $"Recieved a login failed message with the response: {HandlerResult.String}";
                    break;

                case ServerResponseType.Account_Creation_Complete:
                    responseField.text = $"Recieved a account creation succeeded message with the response: {UserSchema.GetLoginHash(HandlerResult.DataDictionary)}";
                    break;

                case ServerResponseType.Added_Item:
                    if (ExampleResponseParser.Content.TokenType == TokenType.DataList)
                    {
                        for (int i = 0; i < ExampleResponseParser.Content.DataList.Count; i++)
                        {
                            items.Add(ExampleResponseParser.Content.DataList[i].DataDictionary);
                        }
                    }
                    else
                    {
                        if (DevelopmentManager.IsConnectorEnabled(DevelopmentMode.Basic))
                            Debug.Log("Server response has returned with an add item type but the message was in a incorrect format");
                    }
                    responseField.text = $"Recieved a add item message adding items: {HandlerResult.DataList.Count}";
                    RenderInventoryText();
                    break;

                case ServerResponseType.Removed_Item:
                    if (ExampleResponseParser.Content.TokenType == TokenType.DataList)
                    {
                        for (int i = 0; i < ExampleResponseParser.Content.DataList.Count; i++)
                        {
                            if (Convert.ToInt32(ExampleResponseParser.Content.DataList[i].Double) < items.Count)
                                items.RemoveAt(Convert.ToInt32(ExampleResponseParser.Content.DataList[i].Double));
                        }
                    }
                    else
                    {
                        if (DevelopmentManager.IsConnectorEnabled(DevelopmentMode.Basic))
                            Debug.Log("Server response has returned with an remove item type but the message was in a incorrect format");
                    }
                    responseField.text = $"Recieved a remove item message removing items: {HandlerResult.DataList.Count}";
                    RenderInventoryText();
                    break;
                default:
                    if (DevelopmentManager.IsConnectorEnabled(DevelopmentMode.Basic))
                        Debug.Log("Server response has returned with an unhandle type: " + ExampleResponseParser.Type);
                    responseField.text = "Recieved an invalid message response type";
                    break;
            }
        }
    }

}