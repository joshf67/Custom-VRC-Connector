var UserConnectionData = require("./UserConnectionData")

class ConnectionHandler {

    static _users = new Map();
    static get Users() {
        return ConnectionHandler._users;
    }

    static ContinueInitializingConnection(ipHash) {
        if (!ConnectionHandler.Users.has(ipHash)) 
        {
            console.error("Failed to continue initializing user as they have not been set up");
            return;
        }

        var currentUser = ConnectionHandler.Users.get(ipHash);
        //currentUser.
    }

    static AddUser(ipHash) {
        if (ConnectionHandler.Users.has(ipHash)) {
            return ConnectionHandler.Users.get(ipHash);
        }

        let newUserData = new UserConnectionData(ipHash);
        ConnectionHandler.Users.set(ipHash, newUserData);
        return newUserData;
    }

    static GetUser(ipHash) {
        return ConnectionHandler.Users.get(ipHash);
    }

    static RemoveUser(ipHash) {
        return ConnectionHandler.Users.delete(ipHash);
    }

    static GetConnectedUsers() {
        return ConnectionHandler.Users;
    }

    static GetConnectedUsersCount() {
        return ConnectionHandler.Users.size;
    }

    static PruneInactiveUsers() {
        let currentSearchTime = Date.now().getTime();
        let pruneIfAfterMS = process.env.PRUNE_INACTIVE_TIME_MINUTES * 60000;
        for(value of ConnectionHandler.Users) {
            if (currentSearchTime - value.getTime() > pruneIfAfterMS)
                this.RemoveUser(value.ipHash);
        }
    }

}

module.exports = ConnectionHandler;