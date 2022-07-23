using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.SDK3.Video.Components;
using VRC.SDK3.Video.Components.Base;
using VRC.SDK3.Components.Video;
using System.Runtime.InteropServices;
using System;

namespace VRCDatabase
{

    public abstract class DatabaseBase : UdonSharpBehaviour
    {
        public VRCUrl currentlySelectedURL;
        public VRCUrlTool urlLookup;
        public VRCUnityVideoPlayer player;
        public ReadRenderTexture response;

        public UdonHashLib hasher;

        //const float CONNECTION_TIMEOUT_RATE = 5f;
        public float CONNECTION_TIMEOUT_RATE = 5f;
        float sendMessageWait = 0;
        bool sendingMessage = false;

        
        public ushort[] messageBuffer = new ushort[0];

        public void Start()
        {
            response.manager = this;
            ManagerStart();
        }

        public void Update()
        {
            sendMessageWait -= Time.deltaTime;
            if (!sendingMessage)
            {
                if (sendMessageWait <= 0 && messageBuffer.Length != 0)
                {
                    sendingMessage = true;
                    currentlySelectedURL = urlLookup.ConvertMessageToVRCUrl(messageBuffer[0]);
                    player.PlayURL(currentlySelectedURL);
                }
            }

            ManagerUpdate();
        }

        protected void HandleConnectionAttempt(bool connectionSuccess)
        {
            if (connectionSuccess)
            {
                ArrayUtilities.RemoveValueFromArrayAtIndex(ref messageBuffer, 0);
                sendingMessage = false;
            }
            else
            {
                sendingMessage = false;
            }
        }

        public override void OnVideoStart()
        {
            sendMessageWait = CONNECTION_TIMEOUT_RATE;
            response.StartReading();
        }

        public override void OnVideoEnd()
        {
            response.StopReading();
            HandleConnectionAttempt(true);
        }

        public override void OnVideoError(VideoError videoError)
        {
            HandleConnectionAttempt(false);

            if (videoError == VideoError.RateLimited)
            {
                sendMessageWait = CONNECTION_TIMEOUT_RATE;
            }

            if (videoError == VideoError.AccessDenied)
            {
                HandleUntrustedError();
            }
        }

        protected virtual void ManagerStart()
        {

        }

        protected virtual void ManagerUpdate()
        {

        }

        public abstract void HandleUntrustedError();

        public abstract void HandleMessage(Color32[] pixles);

        public string ConvertTextToHash(string text)
        {
            return hasher.SHA512_UTF8(text);
        }

        public string ConvertSHA256ToMessage(string hash)
        {
            if (hash.Length != 128) return "";
            return new string(new char[] { hash[0], hash[15], hash[31], hash[47], hash[63], hash[79], hash[95], hash[111], hash[127] });
        }

        public ushort[] PackMessageBytes(byte[] message, DatabaseMessageTypes messageType)
        {
            if (message.Length == 0) return null;

            //Calculate the total amount of ushorts needed for both the message and messageType
            float ushortNeededForMessage = message.Length / 2f;
            float ushortNeededForMessageTypes = ushortNeededForMessage / 4f;
            int ushortNeededForTotal = Mathf.CeilToInt(ushortNeededForMessage + ushortNeededForMessageTypes);

            ushort[] ret = new ushort[ushortNeededForTotal];

            const int MESSAGE_TYPE_OFFSET = 4;
            int MESSAGE_TYPE = ((int)messageType);

            int currentMessageBitsPacked = 0;
            int currentMessageBytesPacked = 0;
            int currentMessage;

            for (int currentMessagesPacked = 0; currentMessagesPacked < ushortNeededForTotal; currentMessagesPacked++)
            {
                //setup message and pack type into the first 4 bits
                currentMessage = MESSAGE_TYPE;

                //switch based on if current pack is half finished
                if (currentMessageBitsPacked % 8 == 4)
                {
                    //lop off first 4 bits then left-shift by 4 to apply message offset
                    currentMessage |= message[currentMessageBytesPacked] >> 4 << MESSAGE_TYPE_OFFSET;
                    currentMessageBytesPacked++;

                    //left-shift by 8 to apply message offset and previous bit pack offset
                    currentMessage |= message[currentMessageBytesPacked] << 8;
                    currentMessageBytesPacked++;
                }
                else
                {
                    //left-shift by 4 to apply message offset
                    currentMessage |= message[currentMessageBytesPacked] << MESSAGE_TYPE_OFFSET;
                    currentMessageBytesPacked += 1;

                    //lop off last 4 bits of next byte by masking the last 4 bits
                    //left-shift by 4 to apply message offset
                    //then left-shift by 4 to offset to just after previous message
                    currentMessage |= (message[currentMessageBytesPacked] & 0b11110000) << MESSAGE_TYPE_OFFSET << 4;
                }


                currentMessageBitsPacked += 12;
                ret[currentMessagesPacked] = (ushort)currentMessage;
            }

            return ret;
        }

        public void AddMessageToBuffer(ushort message)
        {
            ArrayUtilities.AddToArray(ref messageBuffer, message);
        }

        public void AddMessagesToBuffer(ushort[] messages)
        {
            ArrayUtilities.AddRangeToArray(ref messageBuffer, messages);
        }
    }

}