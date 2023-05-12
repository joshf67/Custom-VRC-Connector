const logger = require("../../../logger");
const DatabaseHandler = require("../../../database-handler/database-handler");
var path = require("path");
const UserConnectionData = require("../../../connection-handler/user-connection-data");
const URLMessage = require("../../url-message");
const ResponseHandler = require("../../../response-handler/response-handler");
const ResponseData = require("../../../response-handler/response-data");
const { ResponseTypes } = require("../../../response-handler/response-types");
const MessageBuilder = require("../../message-builder");
const { MessageLength } = require("../../message-length");

class LoginHandler {
  /**
   * Handles the first login related message and sets up a user's expecting data
   * @param {UserConnectionData} user - The user this message is for
   * @param {*} res
   * @param {URLMessage} message - The parsed message from the connection
   * @param {bool} creatingAccount - Controls if the login hash will be used to create a new user
   */
  static HandleInitialMessage(user, res, message, creatingAccount = false) {
    
    //Build the message listener data
    user.expectingDataState = new MessageBuilder(
      LoginHandler.HandleMessageUpdate,
      LoginHandler.HandleMessageFinish,
      MessageLength.Login,
      { creatingAccount: creatingAccount }
    );
    
    //Setup the message listener and then send the remaining message bits into it
    user.expectingDataCallback = LoginHandler.HandleMessage;
    user.expectingDataCallback(user, res, message);
  }

  /**
   * Handles login related messages to build up a user login hash
   * @param {UserConnectionData} user - The user this message is for
   * @param {*} res
   * @param {String[]} message - The parsed message from the connection
   */
  static HandleMessage(user, res, message) {
    //If the user is already logged in, return the correct log in
    if (user.loginHash != null) {
      return LoginHandler.HandleLogin(user, res);
    }

    //Handle the current message
    return user.expectingDataState.AddMessageBytes(user, res, message);
  }

  /**
   * Handles login related messages that add to the login hash
   * @param {UserConnectionData} user - The user this message is for
   * @param {*} res
   * @param {*} bitsRemaining - The bits remaining until the message is complete
   */
  static HandleMessageUpdate(user, res, bitsRemaining) {
    //Continue listening to data
    ResponseHandler.HandleResponse(
      user,
      res,
      new ResponseData(
        ResponseTypes.Login_Updated,
        `Login Hash Part Recieved, Awaiting ${bitsRemaining} bits`
      )
    );
  }

  /**
   * Handles finishing up login related messages to build up a user login hash
   * @param {UserConnectionData} user - The user this message is for
   * @param {*} res
   * @param {string[]} fullMessageBits - The message in a string binary array
   * @param {*} options - The options passed to the message builder on creation
   */
  static HandleMessageFinish(user, res, fullMessageBits, options) {
    let loginBytes = [];
    for (let i = 0; i < MessageLength.Login; i += 8) {
      loginBytes.push(parseInt(fullMessageBits.slice(i, i + 8).join(""), 2));
    }

    const loginHash = new TextDecoder().decode(new Uint8Array(loginBytes));

    logger.log(loginHash);

    //Handle finishing up userHashData message
    user.expectingDataCallback = null;
    user.expectingDataState = null;
    user.userHash = loginHash;

    if (options?.creatingAccount)
      return LoginHandler.HandleAccountCreation(user, res);

    return LoginHandler.HandleLogin(user, res);
  }

  /**
   * Sends off a request to the database to get a user and responds with the result
   * @param {UserConnectionData} user - The user data that contains all required info
   * @param {*} res - The express response for the user
   */
  static HandleLogin(user, res) {
    DatabaseHandler.getUserData(user.userHash)
      .then((userData) => {
        user.databaseData = userData[0];
        return ResponseHandler.HandleResponse(
          user,
          res,
          new ResponseData(ResponseTypes.Login_Complete, userData[0])
        );
      })
      .catch((error) => {
        //Fail due to account not existing or another error
        console.error(error);
        if (error.indexOf("Unable to find user") != -1)
          return ResponseHandler.HandleResponse(
            user,
            res,
            new ResponseData(ResponseTypes.Login_Failed, "User does not exist")
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
        user.databaseData = userData[0];

        ResponseHandler.HandleResponse(
          user,
          res,
          new ResponseData(ResponseTypes.Account_Creation_Complete, userData[0])
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
