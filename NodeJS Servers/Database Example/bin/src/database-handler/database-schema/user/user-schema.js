const mongoose = require("mongoose");
const {
  InventorySchema,
  InventorySchemaJS,
} = require("../inventory/inventory-schema");

const UserSchema = new mongoose.Schema({
  loginHash: {
    type: String,
    unique: true,
  },
  Inventory: {
    type: InventorySchema,
    unique: false,
    default: new InventorySchemaJS(),
  },
});
module.exports.UserSchema = UserSchema;

module.exports.UserSchemaJS = class UserSchemaJS {
  loginHash = "";
  Inventory = null;

  constructor(loginHash, invenotry) {
    this.loginHash = loginHash ?? "";
    this.invenotry = invenotry ?? new InventorySchemaJS();
  }
};
