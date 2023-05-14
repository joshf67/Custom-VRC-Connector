const mongoose = require("mongoose");

const ItemSchema = new mongoose.Schema({
  itemID: {
    type: Number,
    unique: false,
    default: -1,
  },
});
module.exports.ItemSchema = ItemSchema;

module.exports.ItemSchemaJS = class ItemSchemaJS {
  itemID = -1;

  constructor(itemID) {
    this.itemID = itemID ?? -1;
  }
};
