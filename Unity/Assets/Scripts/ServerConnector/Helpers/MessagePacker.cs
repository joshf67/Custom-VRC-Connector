using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ServerConnector
{

    public static class MessagePacker
    {

        //There are some weird things that happen if you try to bitshift with a full 32 bit value or negative value
        //(I don't understand it but that's what I've been told) therefore do not use this function unless you know what you're doing
        //or keep your bits to less than 32??
        private static void AddMessageBits(int[] packedChunks, int bits, int bitCount,
                                            ref byte currentMessage, byte chunkSize,
                                            ref int currentChunk, ref int chunkRemainingBits)
        {
            //todo fix compressing issue
            if (chunkRemainingBits >= bitCount)
            {
                chunkRemainingBits -= bitCount;
                currentChunk |= bits << chunkRemainingBits;
            }
            else
            {
                int remainder = bitCount - chunkRemainingBits;
                currentChunk |= bits >> remainder;
                bits &= ~(-1 << remainder);

                packedChunks[currentMessage++] = currentChunk;

                currentChunk = 0;
                chunkRemainingBits = chunkSize - 1;

                //Add bit to indicate type is not included at the start this message
                AddMessageBits(packedChunks, 0, 1, ref currentMessage, chunkSize, ref currentChunk, ref chunkRemainingBits);
                AddMessageBits(packedChunks, bits, remainder, ref currentMessage, chunkSize, ref currentChunk, ref chunkRemainingBits);
            }
        }

        public static int[] PackMessageBytesToURL(object message, ConnectorMessageType messageType, byte packedMessageBitSize, byte messageTypeSize)
        {
            return PackMessageBytesToURL(new object[] { message }, messageType, packedMessageBitSize, messageTypeSize);
        }

        public static int[] PackMessageBytesToURL(object[] messages, ConnectorMessageType messageType, byte packedMessageBitSize, byte messageTypeSize)
        {
            byte actualMessageBitSize = (byte)(packedMessageBitSize + 1);
            if (actualMessageBitSize <= messageTypeSize || actualMessageBitSize > 31) return null;

            byte currentMessage = 0;
            int currentChunk = 0;
            int chunkRemainingBits = actualMessageBitSize;

            int totalBits = 0;

            //Count the total bits required for all the messages
            for (int i = 0; i < messages.Length; i++)
            {
                if (messages[i].GetType() != typeof(object[])
                    || ((object[])messages[i]).Length != 3
                    || ((object[])messages[i])[1].GetType() != typeof(int)
                    || ((object[])messages[i])[2].GetType() != typeof(byte[]))
                {
                    Debug.LogWarning("Trying to pack message to URL with a non-compressed message, attempting simple message compression");
                    messages[i] = CompressMessage(messages[i]);
                }

                totalBits += (int)((object[])messages[i])[1];
            }
            
            int[] packedChunks = new int[Mathf.CeilToInt((totalBits / packedMessageBitSize) + (messageTypeSize / packedMessageBitSize))];

            //Add bit to indicate type is included at the start this message
            AddMessageBits(packedChunks, 1, 1, ref currentMessage, actualMessageBitSize, ref currentChunk, ref chunkRemainingBits);
            AddMessageBits(packedChunks, (int)messageType, messageTypeSize, ref currentMessage, actualMessageBitSize, ref currentChunk, ref chunkRemainingBits);

            foreach (object message in messages)
            {
                int bitsAdded = 0;
                foreach (byte messageByte in (byte[])((object[])message)[2])
                {
                    if (bitsAdded < (int)((object[])message)[1])
                    {
                        Debug.Log("Added message to chunk: " + currentMessage);
                        AddMessageBits(packedChunks, messageByte, 8, ref currentMessage, actualMessageBitSize, ref currentChunk, ref chunkRemainingBits);
                    }
                    else if (bitsAdded < (int)((object[])message)[1] + 8) //Only run on the last required byte regardless of bytes in the message
                    {
                        AddMessageBits(packedChunks, messageByte, (int)((object[])message)[1] - bitsAdded, ref currentMessage, actualMessageBitSize, ref currentChunk, ref chunkRemainingBits);
                    }
                    bitsAdded += 8;
                }
            }

            packedChunks[packedChunks.Length - 1] = currentChunk;
            return packedChunks;
        }

        public static object CompressMessage(object _message, int _bitsToUse = -1)
        {
            object[] message = new object[3];
            message[0] = _message;
            message[1] = _bitsToUse;

            Type messageType = _message.GetType();

            //If the data being sent in is already converted to byte array, do not try to convert it.
            if (messageType == typeof(byte[]))
            {
                message[2] = (byte[])_message;
                if (_bitsToUse == -1) message[1] = ((byte[])_message).Length * 8;
            }
            else
            {
                if (messageType == typeof(Boolean))
                {
                    message[2] = ByteConverter.ConvertBool((Boolean)_message);
                    if (_bitsToUse == -1) message[1] = 1;
                }

                if (messageType == typeof(Byte))
                {
                    message[2] = new byte[1] { (Byte)_message };
                }

                if (messageType == typeof(SByte))
                {
                    message[2] = ByteConverter.ConvertSByte((SByte)_message);
                }

                if (messageType == typeof(Char))
                {
                    message[2] = ByteConverter.ConvertChar((Char)_message);
                }

                if (messageType == typeof(Int16))
                {
                    message[2] = ByteConverter.ConvertInt16((Int16)_message);
                }

                if (messageType == typeof(UInt16))
                {
                    message[2] = ByteConverter.ConvertUInt16((UInt16)_message);
                }

                if (messageType == typeof(Int32))
                {
                    message[2] = ByteConverter.ConvertInt32((Int32)_message);
                }

                if (messageType == typeof(UInt32))
                {
                    message[2] = ByteConverter.ConvertUInt32((UInt32)_message);
                }

                if (messageType == typeof(Int64))
                {
                    message[2] = ByteConverter.ConvertInt64((Int64)_message);
                }

                if (messageType == typeof(UInt64))
                {
                    message[2] = ByteConverter.ConvertUInt64((UInt64)_message);
                }

                if (messageType == typeof(Single))
                {
                    message[2] = ByteConverter.ConvertSingle((Single)_message);
                }

                if (messageType == typeof(Decimal))
                {
                    message[2] = ByteConverter.ConvertDecimal((Decimal)_message);
                }

                if (messageType == typeof(DateTime))
                {
                    message[2] = ByteConverter.ConvertDateTime((DateTime)_message);
                }

                if (messageType == typeof(String))
                {
                    message[2] = ByteConverter.ConvertASCIIString((string)_message);
                }

                if (_bitsToUse == -1) message[1] = ((byte[])_message).Length * 8;
                if (message[2] != null) return message;

                Debug.LogError($"Cannot compress message, cannot accept type of object: {_message.GetType()}, please pre-convert them using the ByteConverter");

            }

            return message;
        }

    }

}