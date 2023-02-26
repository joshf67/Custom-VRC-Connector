const logger = require("../../../logger");

/**
 * Stores data for loginHash generation
 */
class LoginMessage {
  expectingData;
  creatingAccount = false;

  loginHashBits = [];
  loginHash = "";

  loginHash;

  /**
   * @param {boolean} creatingAccount - Determines if this loginHash is being used to create a new account
   */
  constructor(creatingAccount) {
    this.loginHash = "";
    this.creatingAccount = creatingAccount;
  }

  /**
   * Handles building up the loginHash
   * @param {byte[]} message - the character bytes to add to login hash
   */
  AddMessageBytes(message) {
    if (this.expectingData == false) return false;

    this.loginHashBits = this.loginHashBits.concat(message);
    if (this.loginHashBits.length >= process.env.LOGIN_HASH_CHARACTERS * 8) {
      this.expectingData = false;

      for (let i = 0; i < process.env.LOGIN_HASH_CHARACTERS * 8; i += 8) {
        this.loginHash += String.fromCharCode(
          parseInt(this.loginHashBits.slice(i, i + 8).join(""), 2)
        );
      }

      logger.log(this.loginHash);
      return 0;
    }

    return (process.env.LOGIN_HASH_CHARACTERS * 8) - this.loginHashBits.length
  }
}

module.exports = LoginMessage;
