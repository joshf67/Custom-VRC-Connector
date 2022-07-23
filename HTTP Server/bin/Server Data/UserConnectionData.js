
class UserConnectionData {
    
    currentIP = "000.000.000.000"
    username = -1;
    lastMessageTime = -1;

    constructor(currentIP, username) {
        this.currentIP = currentIP;
        this.username = username;
        this.lastMessageTime = Date.now();
    }

}

module.exports = UserConnectionData;
