# Custom-VRC-Connector

## THIS IS STILL A WIP, IT SHOULD WORK CURRENTLY BUT THERE ARE NO GUARANTEES ANY OF THE IMPLEMENTATIONS ARE GOOD TO USE
 
HTTP Server - Base NPM project to respond to any connection 

Unity - All code and demo scene (Hopefully shouldn't be broken as it just uses VRCSDK stuff and U#, everything else should be included)


---
### Thanks to all of these people for helping along the way:

Gorialis - UdonHashLib used for hashing login data

Hax - For the MessagePacker script, general help and suggestions on methods to use

@Merlin - Creating UdonSharp and for general help with bugs/issues

@GlitchyDev, @Miner28 and @BocuD - AvatarImageReader code to read in data from a render texture (old video player method, no longer used, but thanks is still deserved)

# Modify the url to send data
```
    Inside app.js on line 44:
    app.use('/sendMessage=*', ServerConnector.HandleConnection.bind(ServerConnector));
    change '/sendMessage=*' to whatever you want your messages to be recieved on
```

# Setup Server .env file
```
PORT=(string) - The port you want to host the server on

MESSAGE_TYPE_BITS=(int) - The amount of bits that will be used for determining the message type
MESSAGE_BITS_LENGTH=(int) - The amount of bits that will be sent per message (including the bits used for the type)

DATABASE_URL=(string) - The url of the Database, for example: "mongodb+srv://-----.-----.mongodb.net/-----"
DATABASE_NAME=(string) - The name of the Database
DATABASE_CONNECTION_TIMEOUT=(int) - The amount of time to wait until failing a connection to the Database in MS
DATABASE_USER_CERT=(string) - The location of the mongodb login certificate relative to HTTP_SERVER folder

LOGIN_HASH_CHARACTERS=(int) - The amount of characters your login hash will have

PRUNE_INACTIVE_TIME_MINUTES=(int) - Unused at the moment but will be used to remove inactive players from the user connection

DEVELOPMENT_MODE=(boolean) - Enables logging throughout the server
```

# Setup URLS:

## VRC URL Tool
Supply a starting URL, a number of URLs and the "Generate URLs" will appear

![URL TOOL OBJECT](/README/VRCURLs.png)

![URL TOOL](/README/VRC%20URL%20Tool.png)

## Generate VRCUrl Tool
Supply a gameObject object to hold URLs, starting URL, a number of URLs and the "Generate URLs" button will appear

![URL GENERATOR TOOL](/README/Generate%20VRCUrls.png)
