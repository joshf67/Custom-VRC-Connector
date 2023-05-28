/**
 * Enum for the response types to send back to the VRC client
 */
const ResponseTypes = Object.freeze({
    /**
     * This indicates that no type was provided when responding to the VRC client
     */
    None: -100,

    /**
     * This indicates that the server was not expecting a request from the VRC client
     */
    Unexpected_Request: -99,

    /**
     * This indicates that the server was expecting a message with a
     * different type than the one it recieved from the VRC client
     */
    Type_Fail: -98,

    /**
     * This indicates that the server failed to understand the VRC client's request
     */
    Failed_To_Parse: -97,

    /**
     * This indicates that the server has had an internal error
     */
    Server_Error: -40,

    /**
     * This indicates that the current user requesting data is not logged in
     */
    User_Not_Logged_In: -2,

    /**
     * This indicates that the request has failed for some reason
     */
    Failed: -1,

    /**
     * This inidcates that the request has succeeded
     */
    Succeeded: 0
});

module.exports = ResponseTypes;