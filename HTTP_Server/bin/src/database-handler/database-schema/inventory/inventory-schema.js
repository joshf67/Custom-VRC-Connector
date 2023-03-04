const mongoose = require("mongoose");

const InventorySchema = new mongoose.Schema({
  Currency: {
    type: Number,
    unique: false,
    default: 0,
  },
  Items: {
    type: Array,
    unique: false,
    default: [],
  },
});
module.exports.InventorySchema = InventorySchema;

module.exports.InventorySchemaJS = class InventorySchemaJS {
  Currency = 0;
  Items = [];

  constructor(currency, items) {
    this.Currency = currency ?? 0;
    this.Items = items ?? [];
  }
};
