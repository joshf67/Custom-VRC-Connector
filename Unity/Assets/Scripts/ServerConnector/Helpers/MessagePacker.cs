using System;
using UnityEngine;
using VRC.SDK3.Data;
using Joshf67.ServerConnector.Development;
using UdonSharp;

namespace Joshf67.ServerConnector.Packing
{

    /// <summary>
    /// Static helper class that handles packing any messages together to compress the required bits of data
    /// </summary>
    public static class MessagePacker
    {

        //There are some weird things that happen if you try to bitshift with a full 32 bit value or negative value
        //(I don't understand it but that's what I've been told) therefore do not use this function unless you know what you're doing
        //or keep your bits to less than 32??
        /// <summary>
        /// Adds messages and parts of messages onto each other to compress the message
        /// </summary>
        /// <param name="packedChunks"> The final DataList that all URLs are added to </param>
        /// <param name="bits"> The current bits being packed </param>
        /// <param name="bitCount"> The amount of bits being packed </param>
        /// <param name="chunkSize"> The total size of every chunk </param>
        /// <param name="currentChunk"> The current chunk with messages packed into it </param>
        /// <param name="chunkRemainingBits"> The amount of bits left before this chunk is full </param>
        private static void AddMessageBits(ref DataList packedChunks, int bits, int bitCount,
                                            byte chunkSize, ref int currentChunk,
                                            ref int chunkRemainingBits)
        {
            if (chunkRemainingBits >= bitCount)
            {
                if (DevelopmentManager.IsMessagePackerEnabled(DevelopmentMode.Advanced))
                    Debug.Log("Adding message content: " + bits + ", With bit count of: " + bitCount +
                        ", to the current working packed chunk which is: " + currentChunk +
                        ", With a remaining size of: " + chunkRemainingBits + " bits");

                chunkRemainingBits -= bitCount;
                currentChunk |= bits << chunkRemainingBits;

                if (DevelopmentManager.IsMessagePackerEnabled(DevelopmentMode.Advanced))
                    Debug.Log("Added message content: " + bits +
                        ", to the packed message which results in the chunk: " + currentChunk +
                        ", With a remaining size of: " + chunkRemainingBits + " bits");
            }
            else
            {
                int remainder = bitCount - chunkRemainingBits;
                currentChunk |= bits >> remainder;
                bits &= ~(-1 << remainder);

                //Add the working chunk to the list and then reset it
                if (DevelopmentManager.IsMessagePackerEnabled(DevelopmentMode.Advanced))
                    Debug.Log("Adding message bits resulted in a new chunk, adding current working chunk: " + currentChunk +
                        ", Remaining bits: " + bits + ", are being packed into a new chunk taking up: " + remainder + " bits");

                packedChunks.Add(currentChunk);
                currentChunk = 0;

                //Add bit to indicate type is not included at the start this message by subtracting 1 from the chunkSize
                //Due to 0s being treated as nothing (as you would expect) the method is to check if the int being recieved on the server is less than:
                //2 ^ (packedMessageBitSize - 1), for example a 21 bit number can only be 1048575 or below due to the last bit being the type bit
                chunkRemainingBits = chunkSize - 1;

                AddMessageBits(ref packedChunks, bits, remainder, chunkSize, ref currentChunk, ref chunkRemainingBits);
            }
        }

        /// <summary>
        /// Tries to pack an object message into a useable url
        /// </summary>
        /// <param name="message"> The object to pack </param>
        /// <param name="messageType"> The type of message that is being sent </param>
        /// <param name="packedMessageBitSize"> The total size of a packed chunk/message </param>
        /// <param name="messageTypeSize"> The total bits count to be used for the message type </param>
        /// <returns> A DataList containing all the packed/converted URLs </returns>
        public static DataList PackMessageBytesToURL(object message, ConnectorMessageType messageType, byte packedMessageBitSize, byte messageTypeSize)
        {
            DataList dataList = new DataList();

            //If the object being passed is an array of other objects, try to structure the data and then call packing
            if (message.GetType() == typeof(object[]))
            {
                for (int i = 0; i < ((object[])message).Length; i++)
                {
                    //If the message object is a compressed message then add it directly to messages
                    if (IsCompressedMessage(((object[])message)[i]))
                    {
                        dataList.Add((DataDictionary)((object[])message)[i]);
                    }
                    else
                    {
                        //Otherwise compress it and add it
                        dataList.Add(CompressMessage(((object[])message)[i]));
                    }
                }

                if (DevelopmentManager.IsMessagePackerEnabled(DevelopmentMode.Advanced))
                    Debug.Log("Trying to pack message to URL with object array: " + dataList.Count);

                return PackMessageBytesToURL(dataList, messageType, packedMessageBitSize, messageTypeSize);
            }

            if (DevelopmentManager.IsMessagePackerEnabled(DevelopmentMode.Advanced))
                Debug.Log("Trying to pack message to URL with object: " + message);

            //If the message is not an array, wrap the message based on type
            if (message.GetType() == typeof(DataDictionary))
            {
                if (IsCompressedMessage((DataDictionary)message))
                {
                    dataList.Add((DataDictionary)message);
                }
                else
                {
                    dataList.Add(new DataToken((DataDictionary)message));
                }
            }  
            else if (message.GetType() == typeof(DataList))
            {
                dataList = (DataList)message;
            }
            else
            {
                dataList.Add(new DataToken(message));
            }

            //Pack a single message object
            return PackMessageBytesToURL(dataList, messageType, packedMessageBitSize, messageTypeSize);
        }

        /// <summary>
        /// Pack a single DataToken message into a useable url
        /// </summary>
        /// <param name="message"> The DataToken to pack </param>
        /// <param name="messageType"> The type of message that is being sent </param>
        /// <param name="packedMessageBitSize"> The total size of a packed chunk/message </param>
        /// <param name="messageTypeSize"> The total bits count to be used for the message type </param>
        /// <returns> A DataList containing all the packed/converted URLs </returns>
        public static DataList PackMessageBytesToURL(DataToken message, ConnectorMessageType messageType, byte packedMessageBitSize, byte messageTypeSize)
        {
            DataList wrappedMessage = new DataList();
            wrappedMessage.Add(message);
            return PackMessageBytesToURL(wrappedMessage, messageType, packedMessageBitSize, messageTypeSize);
        }

        /// <summary>
        /// Pack message bytes into a useable url
        /// </summary>
        /// <param name="messages"> The messages to pack </param>
        /// <param name="messageType"> The type of message that is being sent </param>
        /// <param name="packedMessageBitSize"> The total size of a packed chunk/message </param>
        /// <param name="messageTypeSize"> The total bits count to be used for the message type </param>
        /// <returns> A DataList containing all the packed/converted URLs </returns>
        public static DataList PackMessageBytesToURL(DataList messages, ConnectorMessageType messageType, byte packedMessageBitSize, byte messageTypeSize)
        {
            //Check that there are enough bits in the packed message to hold a message
            if (packedMessageBitSize <= messageTypeSize + 1 || packedMessageBitSize > 31) return null;

            int currentChunk = 0;
            int chunkRemainingBits = packedMessageBitSize;
            DataList packedChunks = new DataList();
            DataList messagesToUse = messages;

            //Ensure the message provided is in the correct format
            if (messages.Count == 0)
            {
                if (DevelopmentManager.IsMessagePackerEnabled(DevelopmentMode.Warning))
                    Debug.LogWarning("MessagePacker trying to pack empty message to URL");

                return packedChunks;
            } 
            else
            {
                //If the provided messages is actually a single compressed message then wrap it
                if (IsCompressedMessage(messages))
                {
                    if (DevelopmentManager.IsMessagePackerEnabled(DevelopmentMode.Advanced))
                        Debug.Log("MessagePacker trying to pack message to URL using a single compressed message, wrapping it");

                    messagesToUse = new DataList();
                    messagesToUse.Add(messages);
                }
            }

            //Add bit to indicate type is included at the start this message
            AddMessageBits(ref packedChunks, 1, 1, packedMessageBitSize, ref currentChunk, ref chunkRemainingBits);

            //Add type bits to be read on the server
            AddMessageBits(ref packedChunks, (int)messageType, messageTypeSize, packedMessageBitSize, ref currentChunk, ref chunkRemainingBits);

            //Loop through every message and pack it
            for (int messageIndex = 0; messageIndex < messagesToUse.Count; messageIndex++)
            //foreach (DataList message in messages)
            {
                DataToken messageAtIndex;
                DataDictionary validMessage = null;

                if (messagesToUse.TryGetValue(messageIndex, TokenType.DataDictionary, out messageAtIndex))
                    validMessage = messageAtIndex.DataDictionary;

                //Make sure the message is in the right format
                if (messageAtIndex.TokenType != TokenType.DataDictionary ||
                    !IsCompressedMessage(messageAtIndex.DataDictionary, true))
                {
                    validMessage = CompressMessage(messages[messageIndex]);
                }

                //Ensure valid message was setup properly
                if (validMessage == null || validMessage.Count == 0)
                    continue;

                int messageBits = validMessage["bits_to_use"].Int;
                DataList messageBytes = validMessage["message_bytes"].DataList;
                PackingType messagePackingType = (PackingType)validMessage["packing_type"].Int;

                int compressedMessageByteSize = messageBits % 8;

                if (DevelopmentManager.IsMessagePackerEnabled(DevelopmentMode.Advanced))
                    Debug.Log("Messages being packed with total bit count of: " + messageBits);

                //Loop through all the bits required to be added and pack them
                for (int messageByteIndex = 0; messageByteIndex < messageBytes.Count; messageByteIndex++)
                {
                    if (DevelopmentManager.IsMessagePackerEnabled(DevelopmentMode.Advanced))
                        Debug.Log("Message byte being packed: " + messageBytes[messageByteIndex].Byte);

                    //Check if the message uses compression and is at one of the indcies that need to be checked
                    //Big Endian stores the biggest byte first, so the first message is important
                    //Little Endian stores the smallest byte first, so the last message is important
                    //Sequential wants to remove from the end, so the last message is important
                    if (compressedMessageByteSize == 0 ||
                        (messagePackingType == PackingType.BigEndian && messageByteIndex != 0) ||
                        (messagePackingType != PackingType.BigEndian && messageByteIndex != messageBytes.Count - 1))
                    {
                        //No compression is required so pack the byte as normal
                        AddMessageBits(ref packedChunks, messageBytes[messageByteIndex].Byte,
                            8, packedMessageBitSize, ref currentChunk, ref chunkRemainingBits);
                        continue;
                    }

                    byte compressedMessageByte;

                    //Sequential packing type requires shifting right to remove any trailing bits
                    if (messagePackingType == PackingType.Sequential)
                    {
                        //shift the final bits out of the byte
                        compressedMessageByte = (byte)((messageBytes[messageByteIndex].Byte >> 8 - compressedMessageByteSize) & 0xFF);
                    }
                    //Little/Big Endian packing types requires masking out parts of the first/final byte
                    else
                    {
                        //Mask the largest bits of the byte to remove any bits that should be ignored
                        compressedMessageByte = (byte)(messageBytes[messageByteIndex].Byte & (0xFF >> 8 - compressedMessageByteSize));
                    }

                    if (DevelopmentManager.IsMessagePackerEnabled(DevelopmentMode.Advanced))
                        Debug.Log("Message byte with packing type: " + messagePackingType +
                            " being packed is compressed, packing the byte with an offset of: " + (8 - compressedMessageByteSize)
                            + " Resulting in: " + compressedMessageByte);

                    //pack the offset byte
                    AddMessageBits(ref packedChunks, compressedMessageByte, compressedMessageByteSize,
                        packedMessageBitSize, ref currentChunk, ref chunkRemainingBits);
                }
            }

            //The last message will always need to be added to the packedChunks due to the way that the compression works, so add it at the final stage.
            packedChunks.Add(currentChunk);

            if (DevelopmentManager.IsMessagePackerEnabled(DevelopmentMode.Basic))
            {
                Debug.Log("Final packed chunks: ");
                //Debug all the final packed chunks
                for (int i = 0; i < packedChunks.Count; i++)
                {
                    Debug.Log(packedChunks[i]);
                }
            }

            return packedChunks;
        }

        /// <summary>
        /// Compares if an object parameter is in the format of a compressed message
        /// </summary>
        /// <param name="message"> The message to check </param>
        /// <param name="logAsWarning"> Determines if the message requires warning mode and will be logged as such </param>
        /// <returns> Boolean determining the structure type </returns>
        public static bool IsCompressedMessage(object message, bool logAsWarning = false)
        {
            Type _messageType = message.GetType();
            if (_messageType == typeof(DataDictionary))
            {
                return IsCompressedMessage((DataDictionary)message, logAsWarning);
            }
            else if (_messageType == typeof(DataToken) && ((DataToken)message).TokenType == TokenType.DataDictionary)
            {
                return IsCompressedMessage(((DataToken)message).DataDictionary, logAsWarning);
            }

            return false;
        }

        /// <summary>
        /// Compares if a DataList parameter is in the format of a compressed message
        /// </summary>
        /// <param name="message"> The message to check </param>
        /// <param name="logAsWarning"> Determines if the message requires warning mode and will be logged as such </param>
        /// <returns> Boolean determining the structure type </returns>
        public static bool IsCompressedMessage(DataDictionary message, bool logAsWarning = false)
        {
            //Make sure the message is in the right format
            if (message == null || !message.ContainsKey("original_message") ||
                !message.ContainsKey("bits_to_use") || !message.ContainsKey("message_bytes") ||
                !message.ContainsKey("packing_type"))
            {

                //Log this data if required
                if ((logAsWarning && DevelopmentManager.IsMessagePackerEnabled(DevelopmentMode.Warning)) ||
                    (!logAsWarning && DevelopmentManager.IsMessagePackerEnabled(DevelopmentMode.Advanced)))
                {
                    if (message == null)
                    {
                        if (logAsWarning)
                        {
                            Debug.LogWarning("MessagePacker testing isCompressedMessage, false due to message being null");
                        } 
                        else
                        {
                            Debug.Log("MessagePacker testing isCompressedMessage, false due to message being null");
                        } 
                    }
                    else
                    {
                        //Test all of the variables exist
                        DataToken originalMessage;
                        message.TryGetValue("original_message", out originalMessage);

                        DataToken bitsToUse;
                        message.TryGetValue("bits_to_use", TokenType.Int, out bitsToUse);

                        DataToken messageBytes;
                        message.TryGetValue("message_bytes", TokenType.DataList, out messageBytes);

                        DataToken packingType;
                        message.TryGetValue("packing_type", TokenType.Int, out packingType);

                        //Build the error message based on the result of above
                        string errorMessage = "";
                        if (originalMessage.Error != DataError.None)
                            errorMessage += "Original message was not supplied in the correct format: " + originalMessage.Error + "\n";

                        if (bitsToUse.Error != DataError.None)
                            errorMessage += "Original message was not supplied in the correct format: " + bitsToUse.Error + "\n";

                        if (messageBytes.Error != DataError.None)
                            errorMessage += "Original message was not supplied in the correct format: " + messageBytes.Error + "\n";

                        if (packingType.Error != DataError.None)
                            errorMessage += "Original message was not supplied in the correct format: " + packingType.Error + "\n";

                        if (logAsWarning)
                        {
                            Debug.LogWarning("MessagePacker testing isCompressedMessage, false due to:" + errorMessage);
                        }
                        else
                        {
                            Debug.Log("MessagePacker testing isCompressedMessage, false due to:" + errorMessage);
                        }
                    }
                }

                return false;
            }

            return true;
        }

        /// <summary>
        /// Compress any built-in/unity types into a standard structure
        /// <para> Calling this function directly will alter the _messageBytes DataList to a compressed state </para>
        /// </summary>
        /// <param name="message"> The message to compress </param>
        /// <param name="messageBytes"> The DataList Bytes for the message </param>
        /// <param name="bitsToUse"> The bits required from this message, will be calculated if not provided </param>
        /// <param name="packingType"> Determines the method used for packing bits </param>
        /// <returns> An DataDictionary containing 4 keys:
        /// original_message == The original message,
        /// bits_to_use == The bits required for the compress message,
        /// message_bytes == The DataList of the bytes of the compressed message
        /// packing_type == The PackingType of the compressed message
        /// </returns>
        private static DataDictionary CompressMessage(DataToken message, DataList messageBytes, int bitsToUse = -1, int packingType = 0)
        {
            if (messageBytes.Count == 0 && DevelopmentManager.IsMessagePackerEnabled(DevelopmentMode.Basic))
                Debug.Log("CompressMessage is being given a message with no Bytes: " + message);

            DataDictionary _message = new DataDictionary();
            _message.Add("original_message", message); //The original object message
            _message.Add("bits_to_use", bitsToUse); //The bit offset to be used with 
            _message.Add("message_bytes", messageBytes); //The bytes for the original object message

            //Determines the packing method used with the message
            PackingType messagePackingType = ByteConverter.GetPackingType(message);

            //This is needed due to a bug not allowing default Enums in parameters
            PackingType _packingType = (PackingType)packingType;

            //Set up the actual packing value for the message packer to use
            //This will use the default type for the object if a packingType is not provided
            if (_packingType == PackingType.None)
                _packingType = messagePackingType;
            _message.Add("packing_type", Convert.ToInt32(_packingType));

            //If the packing type is the opposite of the converted message's packed type then flip the bytes
            if (messagePackingType == PackingType.BigEndian && _packingType == PackingType.LittleEndian ||
                messagePackingType == PackingType.LittleEndian && _packingType == PackingType.BigEndian)
            {
                messageBytes.Reverse();
            }

            if (_message["message_bytes"].DataList != null)
            {
                if (bitsToUse == -1)
                {
                    _message["bits_to_use"] = new DataToken(_message["message_bytes"].DataList.Count * 8);
                }
                else
                {
                    if (DevelopmentManager.IsMessagePackerEnabled(DevelopmentMode.Advanced))
                        Debug.Log("CompressMessage is using custom bit length: " + bitsToUse
                            + ", PackingType: " + messagePackingType);

                    //If the bits to use end up being less than the message, we need to remove bytes
                    if ((messageBytes.Count * 8 > bitsToUse) && messageBytes.Count > 0)
                    {
                        while ((messageBytes.Count * 8) - 8 >= bitsToUse)
                        {
                            //Big endian is the only packing type that needs to remove from the front
                            messageBytes.RemoveAt(_packingType == PackingType.BigEndian ? 0 : messageBytes.Count - 1);
                        }

                        if (DevelopmentManager.IsMessagePackerEnabled(DevelopmentMode.Advanced))
                            Debug.Log("CompressMessage is using custom bit length smaller than the total bits which resulted in bytes: " + messageBytes.Count);
                    }
                    //If the bits to use end up being more than the message, we need to add padding bytes to the end
                    else if (messageBytes.Count * 8 < bitsToUse)
                    {
                        //Big endian is the only packing type that needs to add to the front
                        if (_packingType == PackingType.BigEndian)
                            messageBytes.Reverse();

                        while (messageBytes.Count * 8 < bitsToUse)
                        {
                            messageBytes.Add((byte)0);
                        }

                        //Big endian means we have to flip the DataList to add to it, now we need to un-flip it
                        if (_packingType == PackingType.BigEndian)
                            messageBytes.Reverse();

                        if (DevelopmentManager.IsMessagePackerEnabled(DevelopmentMode.Advanced))
                            Debug.Log("CompressMessage is using custom bit length larger than the total bits which resulted in bytes: " + messageBytes.Count);
                    }
                }

                return _message;
            }

            Debug.LogError($"Cannot compress message, cannot accept type of object: {message.GetType()}, please pre-convert them using the ByteConverter");
            return null;
        }

        /// <summary>
        /// Compress any built-in/unity types into a standard structure
        /// </summary>
        /// <param name="message"> The message to compress </param>
        /// <param name="bitsToUse"> The bits required from this message, will be calculated if not provided </param>
        /// <param name="packingType"> Determines the method used for packing bits </param>
        /// <returns> An DataDictionary containing 4 keys:
        /// original_message == The original message,
        /// bits_to_use == The bits required for the compress message,
        /// message_bytes == The DataList of the bytes of the compressed message
        /// packing_type == The PackingType of the compressed message
        /// </returns>
        public static DataDictionary CompressMessage(DataList message, int bitsToUse = -1, int packingType = 0)
        {
            if (DevelopmentManager.IsMessagePackerEnabled(DevelopmentMode.Advanced))
                Debug.Log("MessagePacker is compressing a message with a DataList parameter: " + message);

            //Check if the message needs to be converted or not, this is a simple test and assumes all the types are the same
            if (message.Count == 0 || message[0].TokenType == TokenType.Byte)
                return CompressMessage(message, message, bitsToUse);

            return CompressMessage(message, ByteConverter.ConvertDataList(message), bitsToUse, packingType);
        }

        /// <summary>
        /// Compress any built-in/unity types into a standard structure
        /// </summary>
        /// <param name="message"> The message to compress </param>
        /// <param name="bitsToUse"> The bits required from this message, will be calculated if not provided </param>
        /// <param name="packingType"> Determines the method used for packing bits </param>
        /// <returns> An DataDictionary containing 4 keys:
        /// original_message == The original message,
        /// bits_to_use == The bits required for the compress message,
        /// message_bytes == The DataList of the bytes of the compressed message
        /// packing_type == The PackingType of the compressed message
        /// </returns>
        public static DataDictionary CompressMessage(object message, int bitsToUse = -1, int packingType = 0)
        {
            if (DevelopmentManager.IsMessagePackerEnabled(DevelopmentMode.Advanced))
                Debug.Log("MessagePacker is compressing a message with an Object parameter: " + message);

            return CompressMessage(new DataToken(message),
                ByteConverter.ConvertObject(message),
                bitsToUse, packingType);
        }

        /// <summary>
        /// Compress any DataToken types into a standard structure
        /// </summary>
        /// <param name="message"> The DataToken to compress </param>
        /// <param name="bitsToUse"> The bits required from this message, will be calculated if not provided </param>
        /// <param name="packingType"> Determines the method used for packing bits </param>
        /// <returns> An DataDictionary containing 4 keys:
        /// original_message == The original message,
        /// bits_to_use == The bits required for the compress message,
        /// message_bytes == The DataList of the bytes of the compressed message
        /// packing_type == The PackingType of the compressed message
        /// </returns>
        public static DataDictionary CompressMessage(DataToken message, int bitsToUse = -1, int packingType = 0)
        {
            if (DevelopmentManager.IsMessagePackerEnabled(DevelopmentMode.Advanced))
                Debug.Log("MessagePacker is compressing a message with a DataToken parameter: " + message);

            return CompressMessage(message, ByteConverter.ConvertDataToken(message), bitsToUse, packingType);
        }

    }

}