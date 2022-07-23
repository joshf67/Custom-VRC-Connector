var UserConnectionData = require("./UserConnectionData")

class ConnectionManager {

    #users = new Map();

    ContinueInitializingConnection(ip) {
        if (!this.#users.has(ip)) 
        {
            console.error("Failed to continue initializing user as they have not been set up");
            return;
        }

        var currentUser = this.#users.get(ip);
        //currentUser.
    }

    AddUser(ip) {
        if (this.#users.has(ip)) {
            return this.#users.get(ip);
        }

        let newUserData = new UserConnectionData(ip, Math.round(Math.random() * 100));
        this.#users.set(ip, newUserData);
        return newUserData;
    }

    GetUser(ip) {
        return this.#users.get(ip);
    }

    RemoveUser(ip) {
        return this.#users.delete(ip);
    }

    GetConnectedUsers() {
        return this.#users;
    }

    GetConnectedUsersCount() {
        return this.#users.size;
    }

}

module.exports = ConnectionManager;