/**
 * Stores data for a message's generation
 */
class MessageBuilder {
  updateCallback = null;
  finishCallback = null;
  messageBits = [];
  messageLength = 0;
  options = null;

  /**
   * @param {Function} updateCallback - The callback to use when the message has been added to
   * @param {Function} finishCallback - The callback to use when the message has been built
   * @param {int} messageLength - Waits until messages add up to this many bits before finishing
   * @param {object} options - An object containing any optional variables
   */
  constructor(updateCallback, finishCallback, messageLength, options) {
    this.updateCallback = updateCallback;
    this.finishCallback = finishCallback;
    this.messageLength = messageLength;
    this.options = options;
  }

  /**
   * Handles building up the message
   * @param {byte[]} message - the character bytes to add to the message
   */
  AddMessageBytes(user, res, message) {
    user.expectingDataState.messageBits = user.expectingDataState.messageBits.concat(message);
    if (user.expectingDataState.messageBits.length >= user.expectingDataState.messageLength) {
      return user.expectingDataState?.finishCallback?.(user, res, user.expectingDataState.messageBits, user.expectingDataState.options);
    }

    return user.expectingDataState?.updateCallback?.(
      user,
      res,
      user.expectingDataState.messageLength - user.expectingDataState.messageBits.length
    );
  }
}

module.exports = MessageBuilder;
