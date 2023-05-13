/**
 * Class to contain any parsed messages from VRC connection URL
 */
module.exports = class URLMessage {
  /**
   * @type { String[] } - Binary string
   */
  _message = null;
  /**
   * @type { String[] } - Binary string
   */
  get Message() {
    return this._message;
  }

  _type = null;
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
};