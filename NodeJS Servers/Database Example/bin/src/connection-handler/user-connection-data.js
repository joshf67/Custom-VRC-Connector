const { Document } = require("mongoose");

/**
 * Stores any user related data
 */
module.exports = class UserConnectionData {
  //Stores a hashed version of the user's IP that randomizes every server restart
  ipHash = null;

  lastMessageTime = null;
  
  //Store the last sent response from the user incase it needs to be resent
  lastMessage = null;

  //Store a function that allows the user to continue a response from multiple messages
  expectingDataCallback = null;
  clearExpectingDataCallback = null;
  expectingDataState = null;

  ResetExpectingDataCallback() {
    if (this.clearExpectingDataCallback != null) this.clearExpectingDataCallback();
    this.expectingDataCallback = null;
    this.clearExpectingDataCallback = null;
    this.expectingDataCallback = null;
  }

  //Stores the user's database login details 
  userHash = null;
  userHashData = null;

  //Stores the user's mongoose database data
  /**
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
};
