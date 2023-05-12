using Joshf67.ServerConnector.Development;
using UnityEngine;
using VRC.SDK3.Data;

namespace Joshf67.ServerConnector.Example
{

    /// <summary>
    /// Allows easy parsing of a User's item data through helper functions
    /// </summary>
    public static class ItemSchema
    {
        /// <summary>
        /// Test if a DataDictionary is actually a item schema
        /// </summary>
        /// <param name="data"> The DataDictionary to test </param>
        /// <param name="itemSchema"> The DataDictionary restult if this is a ItemSchema </param>
        /// <returns> Boolean if the DataDictionary is a item schema </returns>
        public static bool IsItemSchema(DataToken data, out DataDictionary itemSchema)
        {
            DataToken discard;
            DataDictionary _data = new DataDictionary();
            itemSchema = _data;

            if (data.TokenType != TokenType.DataDictionary) return false;
            _data = data.DataDictionary;

            if (_data.TryGetValue("itemID", TokenType.Double, out discard))
            {
                itemSchema = _data;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Get the ItemID for an item
        /// </summary>
        /// <param name="data"> The item to get the ItemID for </param>
        /// <param name="itemID"> The resulting ItemID </param>
        /// <returns> Boolean if the ItemID exists </returns>
        public static bool GetItemID(DataToken data, out double itemID)
        {
            if (!IsItemSchema(data, out DataDictionary itemSchema))
            {
                if (DevelopmentManager.IsSchemaEnabled(DevelopmentMode.Basic))
                    Debug.Log("Trying to get Login Hash for an invalid User Schema");

                itemID = -1;
                return false;
            }

            itemID = itemSchema["itemID"].Double;
            return true;
        }

        /// <summary>
        /// Get the ItemID for an item
        /// </summary>
        /// <param name="data"> The item to get the ItemID for </param>
        /// <returns> The Item's ItemID </returns>
        public static double GetItemID(DataToken data)
        {
            GetItemID(data, out double itemID);
            return itemID;
        }
    }

}