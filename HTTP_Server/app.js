var createError = require('http-errors');
var express = require('express');
var path = require('path');
var cookieParser = require('cookie-parser');
var logger = require('morgan');
var nocache = require('nocache');

var indexRouter = require('./routes/index');
var usersRouter = require('./routes/users');

const ServerConnector = require('./bin/src/server-connector');
const ConnectionHandler = require("./bin/src/connection-handler/connection-handler");
const { randomBytes, createHash } = require("crypto");

var app = express();

//Set up a timer to prune all inactive users after a certain time, checks every minute
setInterval(() => {
  //Put inside a try catch to ensure the server doesn't randomly crash if something breaks
  try {
    ConnectionHandler.PruneInactiveUsers()
  } catch (e) {
    console.error(e);
  }
}, 60000);

//Setup current server instance salt
process.env.HASH_SALT = createHash("sha3-256").update(randomBytes(16).toString("hex")).digest("hex");

// view engine setup
app.set('views', path.join(__dirname, 'views'));
app.set('view engine', 'jade');

app.use(logger('dev'));
app.use(express.json());
app.use(express.urlencoded({ extended: false }));
app.use(cookieParser());
app.use(express.static(path.join(__dirname, 'public')));
app.use('/static', express.static(path.join(__dirname, 'public')))

app.set('etag', false);
app.use(nocache());

app.use('/', indexRouter);

//ignore favicon
app.use("/favicon.ico", function() {
  
});

//Bind all messages to custom server logic
app.use('/sendMessage=*', ServerConnector.HandleConnection);

// catch 404 and forward to error handler
app.use(function(req, res, next) {
  next(createError(404));
});

// error handler
app.use(function(err, req, res, next) {
  // set locals, only providing error in development
  res.locals.message = err.message;
  res.locals.error = req.app.get('env') === 'development' ? err : {};

  // render the error page
  res.status(err.status || 500);
  res.render('error');
});

module.exports = app;
