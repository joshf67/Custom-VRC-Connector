const { createHash } = require("crypto");
const logger = require("./logger");
const ConnectionManager = require("./connection-handler/connection-handler");
const MessageHandler = require("./message-handler/message-handler");

/**
 * Class that handles the entry point for a connection from VRC to the Server
 */
class ServerConnector {

  /**
   * Convert the incomming ip to a hash and pass that
   *  onto the message handler instead of storing the IP
   * @param {Object} req - The request
   * @param {Object} res - The result
   * @returns Fails to continue if the request is malformed
   */
  static HandleConnection(req, res) {
    let ipHashed = createHash("sha3-256")
      .update(req.socket.remoteAddress)
      .update(process.env.HASH_SALT)
      .digest("hex");

    //Get/Add the user from the hashed ip
    let userConnection = ConnectionManager.GetUser(ipHashed);
    if (userConnection == null) {
      userConnection = ConnectionManager.AddUser(ipHashed);
      logger.log(`New user has connected with hashed ip: ${ipHashed}`);
    }

    //Ensure url has only 1 message
    if (Object.keys(req.params).length != 1) return;

    MessageHandler.HandleMessage(userConnection, req, res);
  }
}

module.exports = ServerConnector;
