const logger = require("../logger");
const VRCMessage = require("./vrc-message")

/**
 *
 * @param {number} message - The binary message encoded into a number
 * @param {boolean} includesType - Determines if this message has the type included
 * @returns {VRCMessage} - The parsed message
 */
module.exports.UnpackHexMessage = function UnpackHexMessage(
  message,
  includesType = false
) {
  messageBitArray = parseInt(message, 16).toString(2).split("");

  //Pack message to be the same length as MESSAGE_BITS_LENGTH to combat 0s being removed from end
  messageBitArray = Array(
    process.env.MESSAGE_BITS_LENGTH - messageBitArray.length
  )
    .fill("0")
    .concat(messageBitArray);

  if (includesType) {
    return new VRCMessage(
      messageBitArray.slice(process.env.MESSAGE_TYPE_BITS),
      parseInt(
        messageBitArray.slice(0, process.env.MESSAGE_TYPE_BITS).join(""),
        2
      )
    );
  } else {
    return new VRCMessage(messageBitArray);
  }
};
