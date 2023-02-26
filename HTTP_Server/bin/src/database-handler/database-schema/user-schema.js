const mongoose = require("mongoose");

const UserSchema = new mongoose.Schema({
  loginHash: {
    type: String,
    unique: true,
  },
});
module.exports = UserSchema;
