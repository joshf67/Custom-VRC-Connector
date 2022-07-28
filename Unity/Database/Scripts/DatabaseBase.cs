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
        public VrcUrlToolManager urlLookup;
        public VRCUnityVideoPlayer player;
        public ReadRenderTexture response;

        public UdonHashLib hasher;
        public byte packingMessageBitSize;

        //const float CONNECTION_TIMEOUT_RATE = 5f;
        public float CONNECTION_TIMEOUT_RATE = 5f;
        float sendMessageWait = 0;
        bool sendingMessage = false;


        public int[] messageBuffer = new int[0];

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
                    if (currentlySelectedURL != null) player.PlayURL(currentlySelectedURL);
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

        public void AddMessageToBuffer(int message)
        {
            ArrayUtilities.AddToArray(ref messageBuffer, message);
        }

        public void AddMessagesToBuffer(int[] messages)
        {
            ArrayUtilities.AddRangeToArray(ref messageBuffer, messages);
        }
    }

}