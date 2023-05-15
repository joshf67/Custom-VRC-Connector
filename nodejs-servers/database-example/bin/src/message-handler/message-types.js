/**
 * Enum to be used when parsing a VRC request
 */
const MessageTypes = Object.freeze({
    /**
     * Indicates the request is invalid
     */
    Invalid: -1,
  
    /**
     * Indicates the request is a login message
     */
    Login: 0,
  
    /**
     * Indicates the request is an account creation message
     */
    AccountCreation: 1,
  
    /**
     * Indicates the request is a general message type
     */
    GeneralMessage: 2,
  
    /**
     * Indicates the request is a item modification message
     */
    ModifyItem: 3,
  
    /**
     * Indicates the request is an acknowledgement message
     */
    AcknowledgeMessage: 14,
  
    /**
     * Indicates the request is the end of a message
     */
    MessageFinished: 15,
  });
  
  module.exports.MessageTypes = MessageTypes;
  