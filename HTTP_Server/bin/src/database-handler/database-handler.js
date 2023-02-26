const { MongoClient } = require("mongodb");
const mongoose = require("mongoose");
const fs = require("fs");
const path = require("path");
const logger = require("../logger");
const UserSchema = require("./database-schema/user-schema");
const UserModel = require("./database-schema/user-model");

class DatabaseHandler {
  client;

  constructor() {
    Connect(
      process.env.DATABASE_URL,
      path.join(__dirname, "../../../", process.env.DATABASE_USER_CERT)
    ).then(
      function (client) {
        this.client = client;
      }.bind(this)
    );

    // If the Node process ends, close the Mongoose connection
    process.on("SIGINT", function () {
      mongoose.connection.close(function () {
        logger.log("Mongoose disconnected on app termination");
        process.exit(0);
      });
    });
  }

  /**
   * Request user from database if the hash exists, if not create it
   * @param {string} userLoginHash - The user's login
   * @returns {Promise} The user's database entry
   */
  async addUserData(userLoginHash) {
    return new Promise((resolve, reject) => {
      //Try to get the data before adding it
      this.getUserData(userLoginHash)
        .then((user) => {
          resolve(user);
        })
        .catch((error) => {
          //Generate new user data and save it to the database
          let newUser = new UserModel({ loginHash: userLoginHash });
          newUser.save().finally(() => {
            //Get that saved data from the database
            //Can probably be replaced with a blank response
            this.getUserData(userLoginHash)
              .then((user) => {
                resolve(user);
              })
              .catch((error) => {
                //If the data doesn't exist, something went wrong
                logger.error(error);
                reject(error);
              });
          });
        });
    });
  }

  /**
   * Request user from database if the hash exists
   * @param {string} userLoginHash - The user's login
   * @returns {Promise} The user's database entry
   */
  async getUserData(userLoginHash) {
    return new Promise((resolve, reject) => {
      UserModel.find({ loginHash: userLoginHash })
        .then((value) => {
          if (value.length == 0) {
            reject(`Unable to find user: ${userLoginHash}`);
          } else {
            resolve(value);
          }
        })
        .catch((error) => {
          reject("Unable to find user: " + error);
        });
    });
  }
}

module.exports = new DatabaseHandler();

/**
 * Handles connecting to database
 * @param {string} uri - The location of the database
 * @param {string} cert - The certification to connect to the database
 * @returns {Promise} - Returns a promise for the connection
 */
function Connect(uri, cert) {
  return new Promise((resolve) => {
    mongoose
      .connect(uri, {
        ssl: true,
        sslValidate: false,
        sslKey: cert,
        sslCert: cert,
        authMechanism: "MONGODB-X509",
      })
      .then((client) => {
        resolve(client);
      })
      .catch((error) => {
        logger.error(error);
      });
  });
}
