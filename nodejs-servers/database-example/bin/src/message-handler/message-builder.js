/**
 * Allows for requests to be read and build up a message over time
 */
class MessageBuilder {

  /**
   * Used to store the callback to call everytime the builder recieves a request
   */
  updateCallback = null;

  /**
   * Used to store the final callback that will be called when the request is built
   */
  finishCallback = null;
  
  /**
   * Used to store the request's bits that have been read
   */
  messageBits = [];
  
  /**
   * Used to store how many bits this builder should read before finishing
   */
  messageLength = 0;
  
  /**
   * Used to store any options to be passed to the final finishCallback
   */
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
    //Add the additional message onto the end of the user's message data state
    user.expectingDataState.messageBits =
      user.expectingDataState.messageBits.concat(message);

    //Check if the message is the size that is expected
    if (
      user.expectingDataState.messageBits.length >=
      user.expectingDataState.messageLength
    ) {
      //Finish up the message as no more data is required
      return user.expectingDataState?.finishCallback?.(
        user,
        res,
        user.expectingDataState.messageBits,
        user.expectingDataState.options
      );
    }

    //If the message is still expecting data then update and await further data
    return user.expectingDataState?.updateCallback?.(
      user,
      res,
      user.expectingDataState.messageLength -
        user.expectingDataState.messageBits.length
    );
  }
}

module.exports = MessageBuilder;