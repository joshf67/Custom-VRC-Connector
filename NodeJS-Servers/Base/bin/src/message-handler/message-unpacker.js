const logger = require("../logger");
const URLMessage = require("./url-message");

/**
 * Unpack a hex URL into the binary equivalent
 * @param {number} message - The binary message encoded into a number
 * @returns {URLMessage} - The parsed message
 */
module.exports.UnpackHexMessage = function UnpackHexMessage(message) {
  //Convert the hex URL into the binary representation
  messageBitArray = parseInt(message, 16).toString(2).split("");

  //If the type is included then parse out the message and the type
  //To determine if the message includes the type we take
  // the first bit as it is always provided with any message as
  // either 1 or 0 due to the packing method on VRC side
  if (messageBitArray[0] == "1") {
    return new URLMessage(
      messageBitArray.slice(parseInt(process.env.MESSAGE_TYPE_BITS) + 1),
      parseInt(
        messageBitArray
          .slice(1, parseInt(process.env.MESSAGE_TYPE_BITS) + 1)
          .join(""),
        2
      )
    );
  } else {
    //Pack message to be the same length as MESSAGE_BITS_LENGTH to
    // combat 0s being removed from end and remove 1 bit for no type included
    return new URLMessage(
      Array(
        parseInt(process.env.MESSAGE_BITS_LENGTH) - messageBitArray.length - 1
      ).fill("0").concat(messageBitArray)
    );
  }
};