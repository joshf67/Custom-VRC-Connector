const LoginMessage = require("./login-message");
const DatabaseHandler = require("../../../database-handler/database-handler");
var path = require("path");
const UserConnectionData = require("../../../connection-handler/user-connection-data");
const VRCMessage = require("../../vrc-message");
const ResponseHandler = require("../../../response-handler/response-handler");
const ResponseData = require("../../../response-handler/response-data");

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
      ResponseHandler.HandleResponse(
        user,
        res,
        new ResponseData(
          "Login Hash Part Recieved",
          `Awaiting ${awaitingBits} bits`
        )
      );
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
        ResponseHandler.HandleResponse(
          user,
          res,
          new ResponseData("Login Complete", userData)
        );
      })
      .catch((error) => {
        //Fail due to account not existing or another error
        console.error(error);
        if (error.indexOf("Unable to find user") != -1)
          ResponseHandler.HandleResponse(
            user,
            res,
            new ResponseData("Login Complete", "User does not exist")
          );
        ResponseHandler.FailResponse(user, res);
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

        ResponseHandler.HandleResponse(
          user,
          res,
          new ResponseData("Account Creation Complete", userData)
        );
      })
      .catch((error) => {
        //Fail due to account not being able to be created
        console.error(error);
        ResponseHandler.FailResponse(user, res);
      });
  }
}

module.exports = LoginHandler;
