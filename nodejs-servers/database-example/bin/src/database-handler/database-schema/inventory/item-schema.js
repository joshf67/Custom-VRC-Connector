const mongoose = require("mongoose");

/**
 * The MongoDB schema for an item inside the database
 */
const ItemSchema = new mongoose.Schema({
  /**
   * Contains the itemID for this item
   */
  itemID: {
    type: Number,
    unique: false,
    default: -1,
  },
});

module.exports.ItemSchema = ItemSchema;

/**
 * The JS creator for a item's MongoDB Schema
 */
class ItemSchemaJS {
  itemID = -1;

  constructor(itemID) {
    this.itemID = itemID ?? -1;
  }
};

module.exports.ItemSchemaJS = ItemSchemaJS;