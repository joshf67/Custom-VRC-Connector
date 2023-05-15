const mongoose = require("mongoose");
const { UserSchema } = require("./user-schema");

/**
 * Stores the mongoose model for the MongoDB's user data
 */
const UserModel = mongoose.model("users", UserSchema, "userCollection");

module.exports = UserModel;