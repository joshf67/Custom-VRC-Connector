using Joshf67.ServerConnector.Development;
using UnityEngine;
using VRC.SDK3.Data;

namespace Joshf67.ServerConnector.Example
{

    /// <summary>
    /// Allows easy parsing of a User's data through helper functions
    /// </summary>
    public static class UserSchema
    {
        /// <summary>
        /// Test if a DataDictionary is actually a user schema
        /// </summary>
        /// <param name="data"> The DataDictionary to test </param>
        /// <param name="userSchema"> The DataDictionary restult if this is a UserSchema </param>
        /// <returns> Boolean if the DataDictionary is a user schema </returns>
        public static bool IsUserSchema(DataToken data, out DataDictionary userSchema)
        {
            DataToken discard;
            DataDictionary _data = new DataDictionary();
            userSchema = _data;

            if (data.TokenType != TokenType.DataDictionary) return false;
            _data = data.DataDictionary;

            if (_data.TryGetValue("loginHash", TokenType.String, out discard))
            {
                userSchema = _data;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Test and Get the login hash for a user
        /// </summary>
        /// <param name="data"> The user to get the hash for </param>
        /// <param name="loginHash"> The resulting user's login hash </param>
        /// <returns> Boolean if the user login hash exists </returns>
        public static bool GetLoginHash(DataToken data, out string loginHash)
        {
            if (!IsUserSchema(data, out DataDictionary userSchema))
            {
                if (DevelopmentManager.IsSchemaEnabled(DevelopmentMode.Basic))
                    Debug.Log("Trying to get Login Hash for an invalid User Schema");

                loginHash = "";
                return false;
            }

            loginHash = userSchema["loginHash"].String;
            return true;
        }

        /// <summary>
        /// Get the login hash for a user
        /// </summary>
        /// <param name="data"> The user to get the hash for </param>
        /// <returns> The user's login hash </returns>
        public static string GetLoginHash(DataToken data)
        {
            GetLoginHash(data, out string loginHash);
            return loginHash;
        }

        /// <summary>
        /// Get the inventory for a user
        /// </summary>
        /// <param name="data"> The user to get the inventory for </param>
        /// <param name="inventory"> The resulting user's inventory </param>
        /// <returns> Boolean if the user inventory exists </returns>
        public static bool GetInventory(DataToken data, out DataDictionary inventory)
        {
            if (!IsUserSchema(data, out DataDictionary userSchema))
            {
                if (DevelopmentManager.IsSchemaEnabled(DevelopmentMode.Basic))
                    Debug.Log("Trying to get Inventory for an invalid User Schema");

                inventory = null;
                return false;
            }

            inventory = userSchema["Inventory"].DataDictionary;
            return true;
        }

        /// <summary>
        /// Get the inventory for a user
        /// </summary>
        /// <param name="data"> The user to get the inventory for </param>
        /// <returns> The user's inventory </returns>
        public static DataDictionary GetInventory(DataToken data)
        {
            GetInventory(data, out DataDictionary inventory);
            return inventory;
        }
    }

}