const mongoose = require("mongoose");
const UserSchema = require("./user-schema")

const UserModel = mongoose.model("users", UserSchema, "userCollection");
module.exports = UserModel;