using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ServerConnector
{

    public static class MessagePacker
    {

        const byte MESSAGE_TYPE_SIZE = 4;

        //There are some weird things that happen if you try to bitshift with a full 32 bit value or negative value
        //(I don't understand it but that's what I've been told) therefore do not use this function unless you know what you're doing
        //or keep your bits to less than 32??
        private static void AddMessageBits(int[] packedChunks, int bits, int bitCount,
                                            ref byte currentMessage, byte chunkSize,
                                            ref int currentChunk, ref int chunkRemainingBits)
        {
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
                chunkRemainingBits = chunkSize;
                //TODO add message type included bit at the start of every message
                AddMessageBits(packedChunks, bits, remainder, ref currentMessage, chunkSize, ref currentChunk, ref chunkRemainingBits);
            }
        }

        public static int[] PackMessageBytes(byte[] message, ConnectorMessageType messageType, byte packedMessageBitSize)
        {
            if (packedMessageBitSize <= MESSAGE_TYPE_SIZE || packedMessageBitSize > 31) return null;
            byte currentMessage = 0;
            int currentChunk = 0;
            int chunkRemainingBits = packedMessageBitSize;

            int[] packedChunks = new int[Mathf.CeilToInt((message.Length * 8f / packedMessageBitSize) + ((MESSAGE_TYPE_SIZE * 2) / packedMessageBitSize))];
            if (packedChunks.Length > 255) return null;

            //TODO add message type included bit at the start of every message
            AddMessageBits(packedChunks, (int)messageType, MESSAGE_TYPE_SIZE, ref currentMessage, packedMessageBitSize, ref currentChunk, ref chunkRemainingBits);

            foreach (byte messageByte in message)
            {
                AddMessageBits(packedChunks, messageByte, 8, ref currentMessage, packedMessageBitSize, ref currentChunk, ref chunkRemainingBits);
            }

            //This was used to naively pack messages to a full length message
	        //AddMessageBits(packedChunks, (int)ConnectorMessageType.MessageFinished, MESSAGE_TYPE_SIZE, ref currentMessage, packedMessageBitSize, ref currentChunk, ref chunkRemainingBits);
            
            packedChunks[packedChunks.Length - 1] = currentChunk;
            return packedChunks;
        }

    }

}