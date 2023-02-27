const logger = require("../logger");
const { UnpackHexMessage } = require("./message-unpacker");
const UserConnectionData = require("../connection-handler/user-connection-data");
const { MessageTypes } = require("./message-types");

//Message handlers
const LoginHandler = require("./message-handlers/login-handler/login-handler");

/**
 * Class tha handles the entry point for a message from VRC to the Server
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
        res.send("No handler has been set up for current message type").end();
        break;
    }
  }
}

module.exports = MessageHandler;
