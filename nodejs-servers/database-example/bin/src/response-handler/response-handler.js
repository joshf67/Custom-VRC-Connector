const ResponseData = require("./response-data");
const UserConnectionData = require("../connection-handler/user-connection-data");
const { ResponseTypes } = require("./response-types");

/**
 * The default response to make whenever something goes wrong
 */
const resendData = new ResponseData(ResponseTypes.Failed_To_Parse, null);

/**
 * Class that handles responding to a user
 */
class ResponseHandler {
  /**
   * Handles sending off a response to a user
   * @param {UserConnectionData} user - The user to send the response to
   * @param {ResponseData} response - The response content
   * @param {Object} res - The express response
   */
  static HandleResponse(user, res, response) {
    //Add try catch to reset the current user if it fails

    user.lastMessage = (res) => {
      res.setHeader("Content-Type", "text/plain");

      try {
        //Convert to XML until VRC release JSON parsing, converted to JSON then to JS then to XML to stop XML parsing issues
        res.send(JSON.stringify(response)).end();
      } catch (e) {
        //If an error occurs send back a message requesting a resend of the current batch of data
        console.error(e);
        res.send(JSON.stringify(resendData)).end();
      }
    };
    user.lastMessage(res);
  }

  /**
   * Responds to a user's request with a static succeed value
   * @param {UserConnectionData} user - The user to send the response to
   * @param {Object} res
   */
  static SucceedResponse(user, res) {
    ResponseHandler.HandleResponse(
      user,
      res,
      new ResponseData(ResponseTypes.Succeeded, null)
    );
  }

  /**
   * Responds to a user's request with a static failed value
   * @param {UserConnectionData} user - The user to send the response to
   * @param {Object} res
   */
  static FailResponse(user, res) {
    ResponseHandler.HandleResponse(
      user,
      res,
      new ResponseData(ResponseTypes.Failed, null)
    );
  }

  /**
   * Responds to a user's request with a static resend value
   * @param {UserConnectionData} user - The user to send the response to
   * @param {Object} res
   */
  static ResendResponse(user, res) {
    ResponseHandler.HandleResponse(
      user,
      res,
      resendData
    );
  }
};

module.exports = ResponseHandler;