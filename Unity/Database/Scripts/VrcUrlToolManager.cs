
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace VRCDatabase
{

    public class VrcUrlToolManager : UdonSharpBehaviour
    {        
        [HideInInspector]
        public string urlPrefix = "";

        [HideInInspector]
        public int urlCount = 0;

        [HideInInspector]
        public VRCUrlTool[] urlCollections = new VRCUrlTool[0];

        public VRCUrl ConvertMessageToVRCUrl(int input) 
        {
            int inputInt = Mathf.FloorToInt(input / 8192f);
            if (inputInt > urlCollections.Length)
            {
                Debug.LogError("No URL exists for input: " + input);
                return null;
            }

            if (urlCollections[inputInt] == null)
            {
                Debug.LogError("No URL exists for input: " + input);
                return null;
            }

            return urlCollections[inputInt].ConvertMessageToVRCUrl((input - (inputInt * 8192)));
        }
    }

}