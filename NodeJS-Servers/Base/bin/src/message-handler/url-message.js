/**
 * Class to contain any parsed messages from VRC connection URL
 */
class URLMessage {
  /**
   * @summary Store the parsed URL message's content
   * @type { String[] }
   */
  _message = null;

  /**
   * @summary Public getter for the parsed URL message's content
   * @type { String[] }
   */
  get Message() {
    return this._message;
  }

  /**
   * Store the parsed type if it exists
   */
  _type = null;

  /**
   * Public getter for the parsed type if it exists
   */
  get Type() {
    return this._type;
  }

  /**
   *
   * @param {string[]} message - The binary message encoded into a string
   * @param {number} type - The binary message type encoded into a number
   */
  constructor(message, type = null) {
    this._message = message;
    this._type = type;
  }
}

module.exports = URLMessage;