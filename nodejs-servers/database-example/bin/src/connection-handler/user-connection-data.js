const { Document } = require("mongoose");

/**
 * Stores any user related data
 */
class UserConnectionData {
  /**
   * Stores a hashed version of the user's IP that randomizes every server restart
   */
  ipHash = null;

  /**
   * Stores the last time that a server recieved a message from a user
   */
  lastMessageTime = null;

  /**
   * Store the last sent response from the user incase it needs to be resent
   */
  lastMessage = null;

  /**
   * Store a function that allows the user to continue a response from multiple messages
   */
  expectingDataCallback = null;

  /**
   * Stores a function that allows the user to cleanup any expecting data functions if required
   */
  clearExpectingDataCallback = null;

  /**
   * Stores the current state of the user when the server is expecting data
   */
  expectingDataState = null;

  /**
   * Handle resetting any user connection data
   */
  ResetExpectingDataCallback() {
    if (this.clearExpectingDataCallback != null)
      this.clearExpectingDataCallback();
    this.expectingDataCallback = null;
    this.clearExpectingDataCallback = null;
    this.expectingDataCallback = null;
  }

  /**
   * Stores the user's database login details
   */
  userHash = null;

  /**
   * Stores the user's mongoose database data
   * @type {Document}
   */
  databaseData = null;

  /**
   * Constructs a class to store all user connection data
   * @param {string} ipHash - The IP of the user hashed to anonymize it
   */
  constructor(ipHash) {
    this.ipHash = ipHash;
    this.lastMessageTime = Date.now();
  }
}

module.exports = UserConnectionData;
