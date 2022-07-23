var ConnectionManager = require('./ConnectionManager')
var path = require('path');

class ServerLogic {

    app;
    messagesRecieved = [];
    pageResponse = [];
    connManager = new ConnectionManager();
    videoToSend = 0;

    constructor(app) {
        this.app = app;
    }

    HandleMessage(req, res) {
        let url = req.baseUrl.substring(req.url.indexOf("/") + 1);
        this.pageResponse.push('You have connected with the URL: ' + url + "\n");

        this.messagesRecieved.push(req);

        let userConnection = this.connManager.GetUser(req.socket.remoteAddress);
        if (userConnection == null) {
            this.connManager.AddUser(req.socket.remoteAddress);
            console.log(req.socket.remoteAddress);
        } else {
            if (Date.now() - userConnection.lastMessageTime < 500) {
                console.log("User tried to connect to fast");
                return;
            }
            userConnection.lastMessageTime = Date.now();
        }

        if (url.length >= 2 && url.length <= 8) {

            var messageType = parseInt(url.substr(0, 2), 16);
            var messageBytes = this.HexToBytes(url.substr(2, 6));

            if (this.videoToSend++ % 2 == 0) {
                console.log("Sent Failed");
                res.sendFile(path.join('/public', 'failed.mp4'), {root: './'});
            } else {
                console.log("Sent Succeeded");
                res.sendFile(path.join('/public', 'succeed.mp4'), {root: './'});
            }

        }
        
    }

    // Convert a hex string to a byte array
    HexToBytes(hex) {
        for (var bytes = [], c = 0; c < hex.length; c += 2)
        {
            bytes.push(parseInt(hex.substr(c, 2), 16));
        }
        return bytes;
    }

}

module.exports = ServerLogic;