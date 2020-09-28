# TextTcpIP
TextTcpIP is a [.NET Core](https://docs.microsoft.com/en-us/dotnet/core/introduction) library containing implementations of a [TCP/IP](https://en.wikipedia.org/wiki/Transmission_Control_Protocol) client and a `TC/IP Server`.  The protocol used for transmitting data between the client and the server is [ASCII](https://en.wikipedia.org/wiki/Transmission_Control_Protocol) text, with each message sent between the client and the server is defined as a single line of text.  Lines are terminated with the New Line character (`\n`).

A major goal when developing the library was to be able to test it via unit tests.  The initial intent was only to develop a TCP/IP Client; the TCP/IP Server was developed out of necessity as it became a required ingredient for testing the client.  While the Server component provides the necessary functionality for testing the Client, it shouldn't be considered a "production-ready" TCP/IP Server.

It can be argued that using a TCP/IP Server to test a TCP/IP Client crosses the line between unit testing and integration testing.  There's a bit of truth to that, however given that all the tests run in the client machine no interactions with an external network are required.  Furthermore, there was no other way to test the behavior of the client for "unhappy path" scenarios, such as what happens if the client tries to connect to a server that has not started, or what happens if the server disconnects while the client is receiving a message from the server.

Internally, the client and server use .NET's [TCPClient](https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.tcpclient?view=netcore-3.1), [TCPListener](https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.tcpclient?view=netcore-3.1), and [NetworkStream](https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.networkstream?view=netcore-3.1) classes.  For testing purposes, it became necessary to have the option of substituting actual instances of those classes with testing doubles (mocks).  This necessitated creating proxies for those classes with the intent of using dependency injection to remove the client's dependency on them and allowing the creation of mocks from an interface instead.

An additional complication in creating the library was the asynchronous nature of the client and server interaction.  Both the client and server run independently of one another, and within each one of those there are asynchronous processes that do the following:

**For the Client:**
- Connecting to the Server
- Receiving messages from the Server

**For the Server:**
- Acceping connections from a Client
- Receving messages from a Client

While the receiving of messages is asynchronous for both the client and the server, message sending is synchronous.  The reason for this is two-fold:
- The messages that the client will be sending to the server are sequential in nature.  It's important that a client doesn't send a message to the server until the server acknoledges the reception of a message sent.
- The messages that the client sends are very short, just a few bytes long.

## The Tests

The tests can be divided into *happy* and *unhappy* path tests.  As those terms imply, the "Happy Path Tests" verify the expected behavior when both the client and the server are working as expected; the "Unhappy Path Tests" do just the opposite, they verify what happens when not everything goes according to plan.

### Happy Path Tests

- A client sends a single message to the server.  Verify that what the server receives is exactly what the client sent.
- A client sends multiple messages to the server.  Verify that the server receives all the messages sent by the client.
- The client connects to the server.  Verify that the corresponding event is raised.
- The client is about to connect to the server.  Verify that the corresponding event is raised.
- The server sends a message to the client.  Verify that the client receives exactly what the server sent.
- The server sends multiple messages to the client.  Verify that the client receives all the messages sent by the server.

### Unhappy Path Tests

- A client tries to connect to a server that has not started.  Verify that the corresponding exception is thrown.
- The client's code throws an exception while the client is trying to connect to the server.  Verify that the corresponding exception is thrown.
- The client's code throws an exception while the client is receiving a message from the server.  Verify that the corresponding exception is thrown.  Note that in this case the exception that indicates that the receiving failed is an inner exception to an exception that indicates that the connection also failed.
- The server's code throws an exception while accepting a client.  Verify that the corresponding exception is thrown.  Note that in this case the exception that indicates that the server accepting a client failed is an inner exception to an exception that indicates that the server listening for connections failed.
- The client's code throws an exception when it tries to send a message to a server that has stopped listening.  Verify that the corresponding exception is thrown.
- An exception is thrown while the server is reading a message from the client.  Verify that the corresponding exception is thrown.  Note that for the server, receiving exceptions are thrown within the context of the server acception a client which itself is within the context of the server starting to listen for connections.

There are probably a lot more tests that could be done for both the client and the server, but for now these will have to do. 