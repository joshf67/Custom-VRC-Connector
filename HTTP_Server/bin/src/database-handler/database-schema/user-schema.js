const mongoose = require("mongoose");

const UserSchema = new mongoose.Schema({
  loginHash: {
    type: String,
    unique: true,
  },
}, {
  // toObject: {
  //   transform: function (doc, ret) {
  //     delete ret._id;
  //     delete ret.__v;
  //   }
  // }
});
module.exports = UserSchema;
