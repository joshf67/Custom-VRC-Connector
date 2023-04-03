const logger = require("../logger");
const URLMessage = require("./url-message");

/**
 * Unpack a hex URL into the binary equivalent
 * @param {number} message - The binary message encoded into a number
 * @returns {URLMessage} - The parsed message
 */
module.exports.UnpackHexMessage = function UnpackHexMessage(message) {
  messageBitArray = parseInt(message, 16).toString(2).split("");

  //Pack message to be the same length as MESSAGE_BITS_LENGTH to combat 0s being removed from end
  messageBitArray = Array(
    process.env.MESSAGE_BITS_LENGTH - messageBitArray.length
  )
    .fill("0")
    .concat(messageBitArray);

  //The first bit is used to determine if the type was sent along with the message
  if (messageBitArray[0] == "1") {
    return new URLMessage(
      messageBitArray.slice(process.env.MESSAGE_TYPE_BITS + 1),
      parseInt(
        messageBitArray.slice(1, process.env.MESSAGE_TYPE_BITS + 1).join(""),
        2
      )
    );
  } else {
    return new URLMessage(messageBitArray.slice(1));
  }
};
