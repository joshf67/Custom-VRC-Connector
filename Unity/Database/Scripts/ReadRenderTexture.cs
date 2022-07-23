using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.Events;
using VRC.SDKBase;
using VRC.Udon;

namespace VRCDatabase
{

    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ReadRenderTexture : UdonSharpBehaviour
    {
        /* This class is ripped and morphed from https://github.com/Miner28/AvatarImageReader
         * Thanks to BocuD, GlitchyDev and Miner28 for making this
         */

        [Header("Render references")]
        public GameObject renderQuad;

        public Camera renderCamera;
        public CustomRenderTexture renderTexture;
        public Texture2D donorInput;
        public DatabaseBase manager;

        private Color32[] _colors;
        public Color32[] Colors
        {
            get => _colors;
            private set => _colors = value;
        }

        public void StartReading()
        {
            renderCamera.enabled = true;
        }

        public void StopReading()
        {
            renderCamera.enabled = false;
            renderQuad.SetActive(false);
        }

        public void OnPreCull()
        {
            renderQuad.SetActive(true);
        }

        public void OnPostRender()
        {

            renderQuad.SetActive(false);
            donorInput.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0, false);
            Colors = donorInput.GetPixels32();

            //If in unity editor enable visual debuging of frame
            #if UNITY_EDITOR
                donorInput.Apply();
            #endif

            manager.HandleMessage(Colors);
            StopReading();

        }

    }

}