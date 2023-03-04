const ResponseTypes = Object.freeze({
    None: -100,
    Failed_To_Parse: -2,
    Failed: -1,
    Succeeded: 0,
    Login_Updated: 1,
    Login_Failed: 2,
    Login_Complete: 3,
    Account_Creation_Complete: 4,
});
module.exports.ResponseTypes = ResponseTypes;