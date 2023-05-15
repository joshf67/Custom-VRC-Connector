/**
 * Enum to allow the use of non-magic numbers for response bit sizes
 */
const MessageLength = Object.freeze({
    /**
     * Indicates that a login request is a byte per character
     */
    Login: process.env.LOGIN_HASH_CHARACTERS * 8,
  
    /**
     * Indicates the a itemId is 8 bits
     */
    Item: 8,
  
    /**
     * Indicates the a item index is 8 bits
     */
    ItemIndex: 8,
  });
  
  module.exports = MessageLength;
  