# VRC Server Connector API Introduction

[All of the API code is documented here](../api/Joshf67.ServerConnector.html), however, here is a brief overview of the most common parts of the API that you will interact with:

## ServerConnector

This is the class that you will inherit from if you want to build upon helper functions that allow messages to and from the external server by overriding the [HandleMessage](xref:Joshf67.ServerConnector.Connector.HandleMessage(System.String)) method.

## ServerResponse

This is the class that you will inherit from if you want to handle server responses by overriding the [HandleResponse](xref:Joshf67.ServerConnector.Example.ExampleServerResponse.HandleResponse(DataDictionary,DataToken@)) method.

## MessagePacker

This static class is probably what you will interact with when converting your data into compressed messages that the URL packer will use.

To pack a set of data into a URL message buffer, you need to provide the method [PackMessageBytesToURL](xref:Joshf67.ServerConnector.Packing.MessagePacker.PackMessageBytesToURL(DataList,Joshf67.ServerConnector.ConnectorMessageType,System.Byte,System.Byte)) with a [DataList](xref:DataList) of preferably pre-compressed messages, however, this function will try to convert inputs into a compressed format (this might not be optimized for your use case).

To compress messages you will want to call the static function [CompressMessage](xref:Joshf67.ServerConnector.Packing.MessagePacker.CompressMessage(DataToken,System.Int32,System.Int32)) with a [DataToken](xref:DataToken), (optional) the amount of bits you want from the message and finally (optional) the [type of packing](xref:Joshf67.ServerConnector.Packing.PackingType) you want to use for the message.

## ByteConverter

If you are interacting with this manually then it's probably because you want manually convert values into their bytes, the [API reference](xref:Joshf67.ServerConnector.Packing.ByteConverter) will give you all the methods available.
