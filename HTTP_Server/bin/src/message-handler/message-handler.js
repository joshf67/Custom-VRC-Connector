const logger = require("../logger");
const { UnpackHexMessage } = require("./message-unpacker");
const UserConnectionData = require("../connection-handler/user-connection-data");
const { MessageTypes } = require("./message-types");

const ResponseHandler = require("../response-handler/response-handler");
const ResponseData = require("../response-handler/response-data");
const { ResponseTypes } = require("../response-handler/response-types");

//Message handlers
const LoginHandler = require("./message-handlers/login-handler/login-handler");


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

      //TODO check if the response includes a type, if so ignore the last request and continue
      if (user.expectingData != null)
        return user.expectingData(
          user,
          res,
          UnpackHexMessage(req.params["0"]).Message
        );

      let unpackedMessage = UnpackHexMessage(req.params["0"], true);

      //Switch statement that handles all different types of messages
      switch (unpackedMessage.Type) {
        case MessageTypes.Login:
          LoginHandler.HandleInitialMessage(user, res, unpackedMessage.Message);
          break;
        case MessageTypes.AccountCreation:
          LoginHandler.HandleInitialMessage(
            user,
            res,
            unpackedMessage.Message,
            true
          );
          break;
        case MessageTypes.AcknowledgeMessage:
          user.lastMessageTime = Date.now();
          break;
        default:
          console.error(
            "No valid option has been setup for message type: " +
              unpackedMessage.Type
          );
          ResponseHandler.HandleResponse(user, res, new ResponseData(ResponseTypes.Type_Fail));
          break;
      }

    } catch (e) {
      logger.error(e, false);
      ResponseHandler.ResendResponse(user, res);
    }
  }
}

module.exports = MessageHandler;
