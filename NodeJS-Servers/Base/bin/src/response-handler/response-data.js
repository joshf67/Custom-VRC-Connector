/**
 * Stores any response related data
 */
class ResponseData {

  /**
   * Stores the type of this response
   */
  _responseType = null;

  /**
   * Public getter for the type of this response
   */
  get Type() {
    return this._responseType;
  }

  /**
   * Stores the value of this response
   */
  _responseValue = null;

  /**
   * Public getter for the value of this response
   */
  get Response() {
    return this._responseValue;
  }

  /**
   * Constructs a class to store response data
   * @param {String} responseType - The type of response being sent back
   * @param {Object} response - The response to send back
   */
  constructor(responseType, responseValue) {
    this._responseType = responseType;
    this._responseValue = responseValue;
  }

  /**
   * Override toJSON function to format the response
   * @returns A JSON parsed result in specific format
   */
  toJSON() {
    return {
      type: this.Type,
      response: this.Response,
    };
  }
}

module.exports = ResponseData;
