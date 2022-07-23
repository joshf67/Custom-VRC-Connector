using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

namespace VRCDatabase
{

    public class DatabaseDebugger : DatabaseBase
    {
        public InputField usernameField;
        public InputField passwordField;
        public Slider timeoutSlider;
        public int read = 0;

        protected override void ManagerUpdate()
        {
            CONNECTION_TIMEOUT_RATE = timeoutSlider.value;
        }

        public void Login()
        {
            string usernameHash = ConvertSHA256ToMessage(ConvertTextToHash(usernameField.text));
            byte[] usernameBytes = new byte[usernameHash.Length];

            for (int i = 0; i < usernameHash.Length; i++)
            {
                usernameBytes[i] = (byte)usernameHash[i];
            }

            AddMessagesToBuffer(PackMessageBytes(usernameBytes, DatabaseMessageTypes.LoginUsername));
        }

        public override void HandleMessage(Color32[] pixles)
        {
            read++;
            Debug.Log(read);
            if (pixles[0].g > 128)
            {
                Debug.Log("Succeeded");
            }
            else if (pixles[0].r > 128)
            {
                Debug.Log("Failed");
            }

        }

        public override void HandleUntrustedError()
        {
            Debug.LogError("Rate limited, please increase delay");
            return;
        }
    }

}