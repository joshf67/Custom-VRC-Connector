var UserConnectionData = require("./user-connection-data");

/**
 * Class that handles all of the current active users on the server
 */
class ConnectionHandler {

  /**
   * Store all of the users in a single static location
   */
  static _users = new Map();
  
  /**
   * Public static getter for the current users connected
   */
  static get Users() {
    return ConnectionHandler._users;
  }

  /**
   * Adds a user to the current users list
   * @param {string} ipHash - The hashed version of the user's IP
   * @returns the newly created UserConnectionData
   */
  static AddUser(ipHash) {
    if (ConnectionHandler.Users.has(ipHash)) {
      return ConnectionHandler.Users.get(ipHash);
    }

    let newUserData = new UserConnectionData(ipHash);
    ConnectionHandler.Users.set(ipHash, newUserData);
    return newUserData;
  }

  /**
   * Gets a user from the current users lists
   * @param {string} ipHash - The hashed version of the user's IP
   * @returns the UserConnectionData associated with the user
   */
  static GetUser(ipHash) {
    return ConnectionHandler.Users.get(ipHash);
  }

  /**
   * Removes a user from the users list
   * @param {string} ipHash - The hashed version of the user's IP
   * @returns If the user was removed
   */
  static RemoveUser(ipHash) {
    return ConnectionHandler.Users.delete(ipHash);
  }

  /**
   * Returns all of the currently connected users
   * @returns All the connected users's UserConnectionData
   */
  static GetConnectedUsers() {
    return ConnectionHandler.Users;
  }

  /**
   * Returns a count of all of the currently connected users
   * @returns Count of all the connected users's UserConnectionData
   */
  static GetConnectedUsersCount() {
    return ConnectionHandler.Users.size;
  }

  /**
   * Removes any users for the user list if they have been inactive for too long
   */
  static PruneInactiveUsers() {
    let currentSearchTime = Date.now();
    let pruneIfAfterMS = process.env.PRUNE_INACTIVE_TIME_MINUTES * 60000;
    for (var user of ConnectionHandler.Users.values()) {
      if (currentSearchTime - user.lastMessageTime > pruneIfAfterMS)
        this.RemoveUser(user.ipHash);
    }
  }
}

module.exports = ConnectionHandler;
