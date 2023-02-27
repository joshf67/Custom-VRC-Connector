const ResponseData = require("./response-data");
const UserConnectionData = require("../connection-handler/user-connection-data");

/**
 * Class that handles responding to a user
 */
module.exports = class ResponseHandler {
  /**
   * Handles sending off a response to a user
   * @param {UserConnectionData} user - The user to send the response to
   * @param {ResponseData} response - The response content
   * @param {*} res - The express response
   */
  static HandleResponse(user, res, response) {
    user.lastMessage = (res) => {
      res.send(JSON.stringify(response)).end();
    };
    user.lastMessage(res);
  }

  /**
   * Responds to a user's request with a static succeed value
   * @param {UserConnectionData} user - The user to send the response to
   * @param {*} res
   */
  static SucceedResponse(user, res) {
    ResponseHandler.HandleResponse(
      user,
      new ResponseData("succeeded", null),
      res
    );
  }

  /**
   * Responds to a user's request with a static failed value
   * @param {UserConnectionData} user - The user to send the response to
   * @param {*} res
   */
  static FailResponse(user, res) {
    ResponseHandler.HandleResponse(user, new ResponseData("failed", null), res);
  }
};
