/**
 * Stores any response related data
 */
module.exports = class ResponseData {
  _responseType = null;
  get Type() {
    return this._responseType;
  }

  _responseValue = null;
  get Response() {
    return this._responseValue;
  }

  /**
   * Constructs a class to store response data
   * @param {String} responseType - The type of response being sent back
   * @param {*} response - The response to send back
   */
  constructor(responseType, responseValue) {
    this._responseType = responseType;
    this._responseValue = responseValue;
  }

  toJSON() {
    return {
      type: this.Type,
      response: this.Response,
    };
  }
};
