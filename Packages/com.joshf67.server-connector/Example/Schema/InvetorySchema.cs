using Joshf67.ServerConnector.Development;
using UnityEngine;
using VRC.SDK3.Data;

namespace Joshf67.ServerConnector.Example
{

    /// <summary>
    /// Allows easy parsing of a User's Inventory data through helper functions
    /// </summary>
    public static class InvetorySchema
    {

        /// <summary>
        /// Test if a DataDictionary is actually an Inventory schema
        /// </summary>
        /// <param name="data"> The DataDictionary to test </param>
        /// <param name="inventorySchema"> The DataDictionary restult if this is a InventorySchema </param>
        /// <returns> Boolean if the DataDictionary is a Inventory schema </returns>
        public static bool IsInventorySchema(DataToken data, out DataDictionary inventorySchema)
        {
            DataDictionary _inventoryData = new DataDictionary();
            inventorySchema = _inventoryData;

            if (data.TokenType != TokenType.DataDictionary) return false;
            _inventoryData = data.DataDictionary;

            //Check if the data has the keys for the inventory
            if (_inventoryData.TryGetValue("loginHash", TokenType.String, out DataToken discard))
            {
                inventorySchema = _inventoryData;
                return true;
            }

            //If it doesn't test if the data is actually a UserSchema
            if (UserSchema.IsUserSchema(data, out DataDictionary userSchema))
            {
                inventorySchema = userSchema["Inventory"].DataDictionary;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Get the currency for a user/inventory
        /// </summary>
        /// <param name="data"> The user/inventory to get the currency for </param>
        /// <param name="currency"> The resulting user's currency </param>
        /// <returns> Boolean if the user's currency exists </returns>
        public static bool GetCurrency(DataToken data, out double currency)
        {
            if (!IsInventorySchema(data, out DataDictionary userSchema))
            {
                if (DevelopmentManager.IsSchemaEnabled(DevelopmentMode.Basic))
                    Debug.Log("Trying to get Currnecy for an invalid User Schema");

                currency = 0;
                return false;
            }

            currency = userSchema["Inventory"].DataDictionary["Currency"].Double;
            return true;
        }

        /// <summary>
        /// Get the currency for a user
        /// </summary>
        /// <param name="data"> The user to get the currency for </param>
        /// <returns> The user's currency </returns>
        public static double GetCurrency(DataToken data)
        {
            GetCurrency(data, out double currency);
            return currency;
        }

        /// <summary>
        /// Get the items for a user/inventory
        /// </summary>
        /// <param name="data"> The user/inventory to get the inventory items for </param>
        /// <param name="items"> The resulting user's inventory items </param>
        /// <returns> Boolean if the user's inventory items exists </returns>
        public static bool GetInventoryItems(DataToken data, out DataList items)
        {
            if (!IsInventorySchema(data, out DataDictionary userSchema))
            {
                if (DevelopmentManager.IsSchemaEnabled(DevelopmentMode.Basic))
                    Debug.Log("Trying to get Currnecy for an invalid User Schema");

                items = new DataList();
                return false;
            }

            items = userSchema["Inventory"].DataDictionary["Items"].DataList;
            return true;
        }

        /// <summary>
        /// Get the items for a user/inventory
        /// </summary>
        /// <param name="data"> The user/inventory to get the inventory items for </param>
        /// <returns> The user's inventory items </returns>
        public static DataList GetInventoryItems(DataToken data)
        {
            GetInventoryItems(data, out DataList items);
            return items;
        }
    }

}