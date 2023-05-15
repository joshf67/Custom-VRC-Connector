/** Output a log to the console if development mode is enabled
 * @param {any} message - The message to output
 */
function log(message) {
    if (!process.env.DEVELOPMENT_MODE) return;
    console.log(message);
  }
  
  /**
   * Output a warning to the console if development mode is enabled
   * @param {any} message - The message to output
   */
  function warn(message) {
    if (!process.env.DEVELOPMENT_MODE) return;
    console.warn(message);
  }
  
  /**
   * Output an error message, either as a throw or to the console
   * if development mode is enabled
   * @param {any} message - The message to output
   * @param {boolean} throwable - Controls if error should throw or console.error
   */
  function error(message, throwable) {
    if (throwable == true) {
      throw new error(message);
    } else {
      console.error(message);
    }
  }
  
  module.exports = { log, warn, error };