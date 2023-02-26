const LoginMessage = require("./login-message");
const DatabaseHandler = require("../../../database-handler/database-handler");
var path = require("path");
const UserConnectionData = require("../../../connection-handler/UserConnectionData");
const VRCMessage = require("../../vrc-message");

class LoginHandler {
  /**
   * Handles the first login related message and sets up a user's expecting data
   * @param {UserConnectionData} user - The user this message is for
   * @param {*} res
   * @param {VRCMessage} message - The parsed message from the connection
   */
  static HandleInitialMessage(user, res, message, creatingAccount = false) {
    user.userHashData = new LoginMessage(creatingAccount);
    user.expectingData = LoginHandler.HandleMessage;
    LoginHandler.HandleMessage(user, res, message);
  }

  /**
   * Handles login related messages to build up a user login hash
   * @param {UserConnectionData} user - The user this message is for
   * @param {*} res
   * @param {VRCMessage} message - The parsed message from the connection
   */
  static HandleMessage(user, res, message) {
    let awaitingBits = user.userHashData.AddMessageBytes(message);

    //If expecting data is false, that means it has enough data to complete the hash
    if (user.userHashData.expectingData == false) {
      //Handle finishing up userHashData message
      user.expectingData = null;
      user.userHash = user.userHashData.loginHash;

      if (user.userHashData.creatingAccount)
        return LoginHandler.HandleAccountCreation(user, res);

      return LoginHandler.HandleLogin(user, res);
    } else {
      //Continue listening to data
      user.GenericResponse(`Awaiting ${awaitingBits} bits`, res);
    }
  }

  /**
   * Sends off a request to the database to get a user and responds with the result
   * @param {UserConnectionData} user - The user data that contains all required info
   * @param {*} res - The express response for the user 
   */
  static HandleLogin(user, res) {
    DatabaseHandler.getUserData(user.userHash)
      .then((userData) => {
        user.databaseData = userData;
        user.GenericResponse(userData, res);
      })
      .catch((error) => {
        //Fail due to account not existing or another error
        console.error(error);
        if (error.indexOf("Unable to find user") != -1)
          user.GenericResponse(
            `User does not exist, please create an account first.`,
            res
          );
        user.FailedResponse(res);
      });
  }

  /**
   * Sends off a request to the database to generate a user and responds with the result
   * @param {UserConnectionData} user - The user data that contains all required info
   * @param {*} res - The express response for the user 
   */
  static HandleAccountCreation(user, res) {
    DatabaseHandler.addUserData(user.userHash)
      .then((userData) => {
        user.databaseData = userData;

        user.GenericResponse(userData, res);
      })
      .catch((error) => {
        //Fail due to account not being able to be created
        console.error(error);
        user.FailedResponse(res);
      });
  }
}

module.exports = LoginHandler;
