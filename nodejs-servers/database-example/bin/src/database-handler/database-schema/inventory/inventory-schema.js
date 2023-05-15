const mongoose = require("mongoose");

/**
 * The MongoDB schema for a inventory inside the database
 */
const InventorySchema = new mongoose.Schema({
  /**
   * Stores the current currency that a user has
   */
  Currency: {
    type: Number,
    unique: false,
    default: 0,
  },

  /**
   * Stores the current items a user has
   */
  Items: {
    type: Array,
    unique: false,
    default: [],
  },
});
module.exports.InventorySchema = InventorySchema;

/**
 * The JS creator for a user's inventory MongoDB Schema
 */
class InventorySchemaJS {
  Currency = 0;
  Items = [];

  constructor(currency, items) {
    this.Currency = currency ?? 0;
    this.Items = items ?? [];
  }
};

module.exports.InventorySchemaJS = InventorySchemaJS;