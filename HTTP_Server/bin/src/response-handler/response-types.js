const ResponseTypes = Object.freeze({
    None: -100,
    Failed_To_Parse: -2,
    Failed: -1,
    Succeeded: 0,
    Login_Updated: 1,
    Login_Complete: 2,
    Account_Creation_Complete: 3,
});
module.exports.ResponseTypes = ResponseTypes;