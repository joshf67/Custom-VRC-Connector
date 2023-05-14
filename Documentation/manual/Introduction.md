# VRC Server Connector Introduction

[Skip to the API explanation?]

## What?

VRC Server Connector is an API that allows your to send and recieve arbitrary messages to/from external servers

## Why?

VRC does not allow arbitrary amounts of data out of the VRC client easily, this has affected some of the functionality of worlds by limiting them to only being capable of downloading data from a server (using the [Video Player](https://docs.vrchat.com/docs/video-players) or [String](https://docs.vrchat.com/docs/string-loading)/[Image](https://docs.vrchat.com/docs/image-loading) loading) through [static URLs](https://udonsharp.docs.vrchat.com/vrchat-api/#vrcurl) or [user entered URLs](https://udonsharp.docs.vrchat.com/vrchat-api/#vrcurlinputfield).

The community has already come up with methods to bypass these limitations locally, however, these all come with their own downsides that a creator has to account for ([this API also has downsides](#negatives)):

[AvatarImageReader (depricated)](https://github.com/Miner28/AvatarImageReader)

## How?

An external NodeJS server is set up to listen to requests from [String](https://docs.vrchat.com/docs/string-loading)/[Image](https://docs.vrchat.com/docs/image-loading) loading and converts the URL into the binary equivilent. This is then parsed based on the logic implemented on the server, the server then responds to the VRC loader request with an appropriate response that is then parsed on the VRC client side using UdonJSON and handled.

## Positives

- Can send arbitrary data out of VRC to an externally hosted server, which means that users are not able to tamper with the data directly. 

- Data can be compressed and arbirary length of bits can be used to decrease the required messages being sent.

- The server is not coupled to the world, so you can connect to a single external server from multiple VRC worlds.
  
- Uses VRC features under the hood, so should be inherently cross-compatible.

## Negatives

- Due to relying on the [String](https://docs.vrchat.com/docs/string-loading)/[Image](https://docs.vrchat.com/docs/image-loading) under the hood, the upload rate is very slow. Using the default message length of 21 bits, each loader used has an effective rate of ~4 bits/s (initial overheads means longer messages are more efficient), as you can see this is very limited on what you can send so you need to be clever with your requests.

- Small messages are not optimized due to the [String](https://docs.vrchat.com/docs/string-loading) loader being required for the first and last message to read the response.

- Each bit of data that you want to send needs to be accounted for as a VRCUrl. This means that larger message sizes require exponentially more VRCUrls. This increases world build times and will increase the world filesize, From some initial tests each VRCUrl takes [0.02ms and 1.15 bytes to build](VRCUrlTesting.md)

- Handling individual bits and trying to communicate with limited bandwidth requires some creative thinking and is not a drag-and-drop solution.

## Future Development?

- Currently the [Video Player](https://docs.vrchat.com/docs/video-players) is unused, if this was implemented the upload speed could be increased to ~63 bits per 5s (~12 bits/s) or even ~84 bits if both [Video Player](https://docs.vrchat.com/docs/video-players) could be combined.

- Currently only the [String](https://docs.vrchat.com/docs/string-loading) loader is used to handle responses, this means the first and last requests require it, so you have to wait 5s for them before starting a new message, and other slowdowns due to this.

- Converting Objects aren't supported the best, this may cause some issues with custom data types that cannot fit inside DataTokens.