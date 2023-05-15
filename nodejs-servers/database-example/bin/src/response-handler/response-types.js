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
    Succeeded: 0,

    /**
     * This indicates that the server has recieved and update to the login updated
     */
    Login_Updated: 1,

    /**
     * This indicates that the server recieved enough information for a login but it failed
     */
    Login_Failed: 2,

    /**
     * This indicates that the server recieved enough information for a login and it succeeded
     */
    Login_Complete: 3,

    /**
     * This indicates that the server recieved enough information
     * for a login and sucessfully created an account with it
     */
    Account_Creation_Complete: 4,

    /**
     * This indicates that the server recieved a message for updating a user's items
     */
    Item_Updated: 5,

    /**
     * This indicates that the server recieved a message for adding a user's items
     */
    Added_item: 6,

    /**
     * This indicates that the server recieved a message for removing a user's items
     */
    Removed_item: 7
});

module.exports.ResponseTypes = ResponseTypes;