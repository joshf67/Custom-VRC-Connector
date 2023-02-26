/**
 * Class to contain any parsed messages from VRC
 */
module.exports = class VRCMessage {
  _message = null;
  get Message() {
    return this._message;
  }

  _type = null;
  get Type() {
    return this._type;
  }

  /**
   * 
   * @param {number} message - The binary message encoded into a number
   * @param {number} type - The binary message type encoded into a number
   */
  constructor(message, type = null) {
    this._message = message;
    this._type = type;
  }
};