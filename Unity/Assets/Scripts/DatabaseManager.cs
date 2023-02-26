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

    public class DatabaseManager : DatabaseBase
    {

        //public override void HandleUntrustedError()
        //{
        //    return;
        //}

        public override void HandleMessage(string response)
        {
            return;
        }

    }

}