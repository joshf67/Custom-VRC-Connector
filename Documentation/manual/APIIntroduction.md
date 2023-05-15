# VRC Client API

[All of the API code is documented here](../api/Joshf67.ServerConnector.html), however, here is a brief overview of the most common parts of the API that you will interact with:

## [ServerConnector](xref:Joshf67.ServerConnector.Connector)

This is the class that you will inherit from if you want to build upon helper functions that allow messages to and from the external server by overriding the [HandleMessage](xref:Joshf67.ServerConnector.Connector.HandleMessage(System.String)) method.

## [ServerResponse](xref:Joshf67.ServerConnector.Server.ServerResponse)

This is the class that you will inherit from if you want to handle server responses by overriding the [HandleResponse](xref:Joshf67.ServerConnector.Example.ExampleServerResponse.HandleResponse(DataDictionary,DataToken@)) method.

## [MessagePacker](xref:Joshf67.ServerConnector.Packing.MessagePacker)

This static class is probably what you will interact with when converting your data into compressed messages that the URL packer will use.

To pack a set of data into a URL message buffer, you need to provide the method [PackMessageBytesToURL](xref:Joshf67.ServerConnector.Packing.MessagePacker.PackMessageBytesToURL(DataList,Joshf67.ServerConnector.ConnectorMessageType,System.Byte,System.Byte)) with a [DataList](xref:VRC.SDK3.Data.DataList) of preferably pre-compressed messages, however, this function will try to convert inputs into a compressed format (this might not be optimized for your use case).

To compress messages you will want to call the static function [CompressMessage](xref:Joshf67.ServerConnector.Packing.MessagePacker.CompressMessage(DataToken,System.Int32,System.Int32)) with a [DataToken](xref:VRC.SDK3.Data.DataToken), (optional) the amount of bits you want from the message and finally (optional) the [type of packing](xref:Joshf67.ServerConnector.Packing.PackingType) you want to use for the message.

## [ByteConverter](xref:Joshf67.ServerConnector.Packing.ByteConverter)

If you are interacting with this manually then it's probably because you want manually convert values into their bytes, the [API reference](xref:Joshf67.ServerConnector.Packing.ByteConverter) will give you all the methods available.

<br>

# NodeJS Server API

[All of the API code is documented here](../base-nodejs-server/index.html), however, here is a brief overview of the most common parts of the API that you will interact with:

## [ResponseHandler](xref:base-nodejs-server.ResponseHandler)

This is the class that you will will use to respond to the VRC client with a correctly formatted response.

## [ResponseTypes](xref:base-nodejs-server.ResponseTypes)

This is the enum that you will modify to add extra responses for the [response handler](xref:base-nodejs-server.ResponseHandler).

## [MessageBuilder](xref:base-nodejs-server.MessageBuilder)

This class provides helper functions to make building a message from a bunch of requests easier.

## [MessageTypes](xref:base-nodejs-server.MessageTypes)

This is the enum that you will modify to allow more types to be handled in the [message handler](xref:base-nodejs-server.MessageHandler).

## [MessageHandler](xref:base-nodejs-server.MessageHandler)

This is the class that you will modify to add a link from [message types](xref:base-nodejs-server.MessageTypes) to the required [custom message handler](../example/Introduction.md#step-4-handling-vrc-client-messages-on-the-nodejs-server).

## [UserConnectionData](xref:base-nodejs-server.UserConnectionData)

This is the class that every VRC client will be provided to store anything related to them.