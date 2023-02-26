/**
 * Stores any user related data
 */
module.exports = class UserConnectionData {
  ipHash = null;
  userHash = null;
  userHashData = null;
  lastMessageTime = null;
  lastMessage = null;
  expectingData = null;
  databaseData = null;

  /**
   * Constructs a class to store all user connection data
   * @param {string} ipHash - The IP of the user hashed to anonymize it 
   */
  constructor(ipHash) {
    this.ipHash = ipHash;
    this.lastMessageTime = Date.now();
  }

  SetupLastMessage(lastFunction, res) {
    this.lastMessageTime = Date.now();
    this.lastMessage = lastFunction;
    this.lastMessage(res);
  }

  GenericResponse(responseValue, res) {
    this.SetupLastMessage((res) => {
        res.send(responseValue).end();
      }, res);
  }

  SucceedResponse(res) {
    this.GenericResponse("succeed", res);
  }

  FailedResponse(res) {
    this.GenericResponse("failed", res);
    this.expectingData = null;
  }
};
