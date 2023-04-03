function log(message) {
    if (!process.env.DEVELOPMENT_MODE) return;
    console.log(message);
};

function warn(message) {
    if (!process.env.DEVELOPMENT_MODE) return;
    console.warn(message);
};

function error(message, throwable) {
    if (throwable == true) {
        throw new error(message);
    } else {
        console.error(message);
    }
}

module.exports = { log, error };