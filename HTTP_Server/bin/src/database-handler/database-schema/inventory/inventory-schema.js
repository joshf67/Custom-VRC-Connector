const mongoose = require("mongoose");

const InventorySchema = new mongoose.Schema({
  Currency: {
    type: Number,
    unique: true,
    default: 0,
  },
  Items: {
    type: Array,
    unique: true,
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
