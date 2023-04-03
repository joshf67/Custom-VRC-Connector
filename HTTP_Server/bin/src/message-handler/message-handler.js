const logger = require("../logger");
const { UnpackHexMessage } = require("./message-unpacker");
const UserConnectionData = require("../connection-handler/user-connection-data");
const { MessageTypes } = require("./message-types");

const ResponseHandler = require("../response-handler/response-handler");
const ResponseData = require("../response-handler/response-data");
const { ResponseTypes } = require("../response-handler/response-types");

//Message handlers
const LoginHandler = require("./message-handlers/login-handler/login-handler");
const ItemHandler = require("./message-handlers/item-handler/item-handler");

/**
 * Class that handles the entry point for a message from VRC to the Server
 */
class MessageHandler {
  /**
   * Entry for server handling message requests
   * @param {UserConnectionData} user - The user data for this connected user
   * @param {*} req - The current express request
   * @param {*} res - The express response
   */
  static HandleMessage(user, req, res) {
    logger.log(`${user.ipHash} has tried to send a message`);

    //Enclose the message handler inside a try catch to always return a valid value even if the message fails
    try {
      //ResponseHandler.HandleResponse(user, res, new ResponseData(ResponseTypes.Unexpected_Request));

      let unpackedMessage = UnpackHexMessage(req.params["0"], true);

      //TODO check if the response includes a type, if so ignore the last request and continue
      if (user.expectingDataCallback != null) {
        if (unpackedMessage.Type == null) {
          return user.expectingDataCallback(
            user,
            res,
            unpackedMessage.Message
          );
        } else {
          //Handle resetting user's expecting data because something has gone wrong
          user.ResetExpectingDataCallback();
        }
      }

      //Check that the user is actually logged in before continuing onto non-login based messages
      if (
        user.userHash == null &&
        unpackedMessage.Type != MessageTypes.Login &&
        unpackedMessage.Type != MessageTypes.AccountCreation
      ) {
        return ResponseHandler.HandleResponse(
          user,
          res,
          new ResponseData(ResponseTypes.User_Not_Logged_In)
        );
      }

      //Switch statement that handles all different types of messages
      switch (unpackedMessage.Type) {
        case MessageTypes.Login:
          return LoginHandler.HandleInitialMessage(
            user,
            res,
            unpackedMessage.Message
          );
          break;
        case MessageTypes.AccountCreation:
          return LoginHandler.HandleInitialMessage(
            user,
            res,
            unpackedMessage.Message,
            true
          );
          break;
        case MessageTypes.AcknowledgeMessage:
          return (user.lastMessageTime = Date.now());
          break;
        case MessageTypes.ModifyItem:
          return ItemHandler.HandleInitialMessage(
            user,
            res,
            unpackedMessage.Message
          );
          break;
        default:
          console.error(
            "No valid option has been setup for message type: " +
              unpackedMessage.Type
          );
          return ResponseHandler.HandleResponse(
            user,
            res,
            new ResponseData(ResponseTypes.Type_Fail)
          );
          break;
      }
    } catch (e) {
      logger.error(e, false);
      ResponseHandler.ResendResponse(user, res);
    }
  }
}

module.exports = MessageHandler;
