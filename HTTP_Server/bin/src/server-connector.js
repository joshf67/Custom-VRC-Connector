const { createHash } = require("crypto");
const logger = require("./logger");
const ConnectionManager = require("./connection-handler/ConnectionManager");
const MessageHandler = require("./message-handler/message-handler")

/**
 * Class tha handles the entry point for a connection from VRC to the Server
 */
class ServerConnector {

    constructor(app) {
        this.app = app;
    }

    //Convert the incomming ip to a hash and pass that onto the server logic instead of storing the IP
    HandleConnection(req, res) {
        res.setHeader("Content-Type", "text/plain");
        let ipHashed = createHash("sha3-256").update(req.socket.remoteAddress).update(process.env.HASH_SALT).digest("hex");

        //Get/Add the user from the hashed ip
        let userConnection = ConnectionManager.GetUser(ipHashed);
        if (userConnection == null) {
          userConnection = ConnectionManager.AddUser(ipHashed);
          logger.log(`New user has connected with hashed ip: ${ipHashed}`);
        }

        //Ensure url has only 1 message
        if (Object.keys(req.params).length != 1) return;
        MessageHandler.HandleMessage(userConnection, req, res)
    }

}

module.exports = ServerConnector