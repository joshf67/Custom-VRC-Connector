const mongoose = require("mongoose");
const {
  InventorySchema,
  InventorySchemaJS,
} = require("../inventory/inventory-schema");

/**
 * The MongoDB schema for a user inside the database
 */
const UserSchema = new mongoose.Schema({

  /**
   * Stores the user's login hash to be used with the database
   */
  loginHash: {
    type: String,
    unique: true,
  },

  /**
   * Stores any inventory related data for the user
   */
  Inventory: {
    type: InventorySchema,
    unique: false,
    default: new InventorySchemaJS(),
  },
});

module.exports.UserSchema = UserSchema;

/**
 * The JS creator for the user's MongoDB Schema
 */
class UserSchemaJS {
  loginHash = "";
  Inventory = null;

  constructor(loginHash, invenotry) {
    this.loginHash = loginHash ?? "";
    this.invenotry = invenotry ?? new InventorySchemaJS();
  }
}

module.exports.UserSchemaJS = UserSchemaJS;
