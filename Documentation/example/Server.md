# Configuring Server Settings

## Server Environment File

The node server requires a .env file inside the base folder with the following configuration
<table>
    <tbody>
        <th> Setting </th>
        <th> Value Type </th>
        <th> Description </th>
        <tr>
            <td> PORT </td>
            <td> [String](xref:System.String) </td>
            <td> This is the port that the NodeJS server will listen to requests on </td>
        </tr>
        <tr>
            <td> MESSAGE_TYPE_BITS </td>
            <td> [Int](xref:System.Int) </td>
            <td> This is the bit length used for determining a message's type </td>
        </tr>
        <tr>
            <td> MESSAGE_BITS_LENGTH </td>
            <td> [Int](xref:System.Int) </td>
            <td> This is the bit length of the entire message (including MESSAGE_TYPE_BITS) </td>
        </tr>
        <tr>
            <td> DATABASE_URL </td>
            <td> [String](xref:System.String) </td>
            <td> The URL of the database, for example "mongodb+srv://**.**.mongodb.net/**" </td>
        </tr>
        <tr>
            <td> DATABASE_NAME </td>
            <td> [String](xref:System.String) </td>
            <td> The name of the MongoDB collection to be used for the Database </td>
        </tr>
        <tr>
            <td> DATABASE_CONNECTION_TIMEOUT </td>
            <td> [Int](xref:System.Int) </td>
            <td> The total time in MS until a stalled database connection fails </td>
        </tr>
        <tr>
            <td> DATABASE_USER_CERT </td>
            <td> [String](xref:System.String) </td>
            <td> The location (relative to initial folder) of the MongoDB login certificate </td>
        </tr>
        <tr>
            <td> LOGIN_HASH_CHARACTERS </td>
            <td> [Int](xref:System.Int) </td>
            <td> This is the length required for a VRC user's login to the database </td>
        </tr>
        <tr>
            <td> PRUNE_INACTIVE_TIME_MINUTES </td>
            <td> [String](xref:System.Int) </td>
            <td> Controls how long a VRC user has to not message for them to be pruned(removed) from the connect user storage </td>
        </tr>
        <tr>
            <td> DEVELOPMENT_MODE </td>
            <td> [String](xref:System.Boolean) </td>
            <td> Controls if logging should be enable on the server </td>
        </tr>
    </tbody>
</table>

for example:
```javascript
PORT=33646

MESSAGE_TYPE_BITS=4
MESSAGE_BITS_LENGTH=21

DATABASE_URL="mongodb+srv://xxxx.xxxx.mongodb.net/RPGDatabase"
DATABASE_NAME="RPGDatabase"
DATABASE_CONNECTION_TIMEOUT=2000
DATABASE_USER_CERT="Mongodb/mongo.pem"

LOGIN_HASH_CHARACTERS=9

PRUNE_INACTIVE_TIME_MINUTES=30

DEVELOPMENT_MODE=true
```

<br>

## Modifying The Server's Listening URL

By default the server will listen for any messages that start with "/sendMessage=", however, this can be changed by modifying line 52 inside the App.js file:

<span style="color:red"> Remember to keep the * wildcard at the end otherwise the server will only listen to the exact URL "/sendMessage=" </span>

```javascript
//ignore favicon
app.use("/favicon.ico", function() {});

//Bind all messages to custom server logic
app.use('/sendMessage=*', ServerConnector.HandleConnection); // <----- Change this line here

// catch 404 and forward to error handler
app.use(function(req, res, next) {
  next(createError(404));
});
```

<span style="color:red"> Don't forget to update the [VRC URL settings](VRC.md)</span>