const MessageLength = Object.freeze({
    Login: process.env.LOGIN_HASH_CHARACTERS * 8,
    Item: 8,
    ItemIndex: 8
});
module.exports.MessageLength = MessageLength;