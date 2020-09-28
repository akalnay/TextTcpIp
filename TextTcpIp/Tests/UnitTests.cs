////////////////////////////////////////////////////////
// Copyright (c) Alejandro Kalnay                     //
// License: GNU GPLv3                                 //
////////////////////////////////////////////////////////

using NSubstitute;
using NSubstitute.Core;
using NSubstitute.Extensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace TextTcpIp
{
    internal abstract class TcpIp12Client_TestsBase
    {
        protected static class TestCases
        {
            private static readonly IPEndPoint _IPENDPOINT = new IPEndPoint(IPAddress.Loopback, 54321);

            public static IEnumerable<TestCaseData> SingleValue(bool testReturnsAValue)
            {
                string valueToSend = "abc";
                TestCaseData testCaseData = new TestCaseData(_IPENDPOINT, valueToSend);
                if (testReturnsAValue)
                    testCaseData.Returns(valueToSend);
                yield return testCaseData;
            }

            public static IEnumerable<TestCaseData> MultipleValues()
            {
                string[] values = { "abc1", "abc2", "abc3", "abc4" };
                yield return new TestCaseData(_IPENDPOINT, (object)values).Returns(values);
            }

            public static IEnumerable<TestCaseData> ClientConnect()
            {
                yield return new TestCaseData(_IPENDPOINT).Returns(true);
            }

            public static IEnumerable<TestCaseData> MultipleValues2()
            {
                static IEnumerable<string> GetMultipleValues2()
                {
                    for (int i = 32; i < 127; i++)
                        yield return new string((char)i, 1000);
                }

                string[] values = GetMultipleValues2().ToArray();
                yield return new TestCaseData(_IPENDPOINT, (object)values).Returns(values);
            }

            public static IEnumerable<TestCaseData> ServerNotStarted()
            {
                yield return new TestCaseData(_IPENDPOINT);
            }

            public static IEnumerable<TestCaseData> ServerNotStarted2()
            {
                yield return new TestCaseData(_IPENDPOINT, typeof(ArgumentOutOfRangeException));
            }

            public static IEnumerable<TestCaseData> SingleValue2()
            {
                string valueToSend = "abc";
                yield return new TestCaseData(_IPENDPOINT, valueToSend, typeof(ArgumentOutOfRangeException));
            }

            public static IEnumerable<TestCaseData> JustAnEndPoint()
            {
                yield return new TestCaseData(_IPENDPOINT);
            }

            public static IEnumerable<TestCaseData> JustAnEndPoint2()
            {
                yield return new TestCaseData(_IPENDPOINT, typeof(DivideByZeroException));
            }
        }

        protected static async Task<string> GetValueSentByClient3Async(IPEndPoint ipEndPoint, string dataToSend)
        {
            TextTcpIpClient client = default;
            string result = default;

            void ServerReceivedADataItem(object sender, int identifier, SynchronizationContext synchronizationContext, string value)
            {
                result = value;
                client.Disconnect();
                TcpReceiver tcpReceiver = (TcpReceiver)sender;
                ITcpIpNode tcpIpNode = tcpReceiver.TcpIpNode;
                TextTcpIpServer tcpIpServer12 = (TextTcpIpServer)tcpIpNode;
                tcpIpServer12.Stop();
            }

            void ClientIsConnected(object sender, ITcpClient tcpClient)
            {
                client = (TextTcpIpClient)sender;
                client.Send(dataToSend);
            }

            async void ServerIsListening(object sender, ITcpListener tcpListener)
            {
                await TextTcpIpClient.StartAsync(ipEndPoint, new TcpIpClientOptions { OnConnected = ClientIsConnected }).ConfigureAwait(false);
            }

            await TextTcpIpServer.StartAsync(ipEndPoint,
                new TcpIpServerOptions { OnListeningStarted = ServerIsListening, OnDataItemReceived = ServerReceivedADataItem }).ConfigureAwait(false);
            return result;
        }

        protected static async Task<IEnumerable<string>> GetValuesSentByClient2Async(IPEndPoint ipEndPoint, string[] dataToSend)
        {
            ICollection<string> dataReceived = new Collection<string>();
            TextTcpIpClient client = default;

            void ServerReceivedADataItem(object sender, int streamIdentifier, SynchronizationContext synchronizationContext, string value)
            {
                dataReceived.Add(value);
                if (dataReceived.Count == dataToSend.Length)
                {
                    client.Disconnect();
                    TcpReceiver tcpReceiver = (TcpReceiver)sender;
                    ITcpIpNode tcpIpNode = tcpReceiver.TcpIpNode;
                    TextTcpIpServer tcpIpServer = (TextTcpIpServer)tcpIpNode;
                    tcpIpServer.Stop();
                }
            }

            void ClientIsConnected(object sender, ITcpClient tcpClient)
            {
                client = (TextTcpIpClient)sender;
                foreach (string data in dataToSend)
                    client.Send(data);
            }

            async void ServerIsListening(object sender, ITcpListener tcpListener)
            {
                await TextTcpIpClient.StartAsync(ipEndPoint, new TcpIpClientOptions { OnConnected = ClientIsConnected }).ConfigureAwait(false);
            }

            await TextTcpIpServer.StartAsync(ipEndPoint,
                new TcpIpServerOptions { OnListeningStarted = ServerIsListening, OnDataItemReceived = ServerReceivedADataItem }).ConfigureAwait(false);
            return dataReceived;
        }

        protected static async Task<bool> DoConnected2Async(IPEndPoint ipEndPoint)
        {
            bool result = false;
            TextTcpIpServer server = default;

            void ClientIsConnected(object sender, ITcpClient tcpClient)
            {
                result = tcpClient.Connected;
                ((TextTcpIpClient)sender).Disconnect();
                server.Stop();
            }

            async void ServerIsListening(object sender, ITcpListener tcpListener)
            {
                server = (TextTcpIpServer)sender;
                await TextTcpIpClient.StartAsync(ipEndPoint, new TcpIpClientOptions { OnConnected = ClientIsConnected }).ConfigureAwait(false);
            }

            await TextTcpIpServer.StartAsync(ipEndPoint, new TcpIpServerOptions { OnListeningStarted = ServerIsListening }).ConfigureAwait(false);
            return result;
        }

        protected static async Task<bool> DoConnecting2Async(IPEndPoint ipEndPoint)
        {
            bool result = false;
            TextTcpIpServer server = default;

            void ClientIsConnecting(object sender, ITcpClient tcpClient)
            {
                result = true;
            }

            void ClientIsConnected(object sender, ITcpClient tcpClient)
            {
                ((TextTcpIpClient)sender).Disconnect();
                server.Stop();
            }

            async void ServerIsListening(object sender, ITcpListener tcpListener)
            {
                server = (TextTcpIpServer)sender;
                await TextTcpIpClient.StartAsync(ipEndPoint,
                    new TcpIpClientOptions { OnConnecting = ClientIsConnecting, OnConnected = ClientIsConnected }).ConfigureAwait(false);
            }

            await TextTcpIpServer.StartAsync(ipEndPoint, new TcpIpServerOptions { OnListeningStarted = ServerIsListening }).ConfigureAwait(false);
            return result;
        }

        protected static async Task<string> DoServerSendsValueToClientAsync(IPEndPoint ipEndPoint, string dataToSend)
        {
            string result = default;
            TextTcpIpServer server = default;

            void ClientReceivedADataItem(object sender, int streamIdentifier, SynchronizationContext synchronizationContext, string value)
            {
                result = value;
                TcpReceiver tcpReceiver = (TcpReceiver)sender;
                ITcpIpNode tcpIpNode = tcpReceiver.TcpIpNode;
                TextTcpIpClient tcpIpClient = (TextTcpIpClient)tcpIpNode;
                tcpIpClient.Disconnect();
                server.Stop();
            }

            void ClientAdded(object sender, string clientIdentifier, ITcpClient tcpClient)
            {
                server.Send(dataToSend, clientIdentifier);
            }

            async void ServerIsListening(object sender, ITcpListener tcpListener)
            {
                server = (TextTcpIpServer)sender;
                await TextTcpIpClient.StartAsync(ipEndPoint, new TcpIpClientOptions { OnDataItemReceived = ClientReceivedADataItem }).ConfigureAwait(false);
            }

            await TextTcpIpServer.StartAsync(ipEndPoint,
                new TcpIpServerOptions { OnListeningStarted = ServerIsListening, OnClientAdded = ClientAdded }).ConfigureAwait(false);
            return result;
        }

        protected static async Task<IEnumerable<string>> DoServerSendsValuesToClientAsync(IPEndPoint ipEndPoint, string[] dataToSend)
        {
            ICollection<string> dataReceived = new Collection<string>();
            TextTcpIpServer server = default;

            void ClientReceivedADataItem(object sender, int streamIdentifier, SynchronizationContext synchronizationContext, string value)
            {
                dataReceived.Add(value);
                if (dataReceived.Count == dataToSend.Length)
                {
                    TcpReceiver tcpReceiver = (TcpReceiver)sender;
                    ITcpIpNode tcpIpNode = tcpReceiver.TcpIpNode;
                    TextTcpIpClient tcpIpClient = (TextTcpIpClient)tcpIpNode;
                    tcpIpClient.Disconnect();
                    server.Stop();
                }
            }

            void ClientAdded(object sender, string clientIdentifier, ITcpClient tcpClient)
            {
                foreach (string data in dataToSend)
                    server.Send(data, clientIdentifier);
            }

            async void ServerStartedListening(object sender, ITcpListener tcpListener)
            {
                server = (TextTcpIpServer)sender;
                await TextTcpIpClient.StartAsync(ipEndPoint, new TcpIpClientOptions { OnDataItemReceived = ClientReceivedADataItem }).ConfigureAwait(false);
            }

            await TextTcpIpServer.StartAsync(ipEndPoint,
                new TcpIpServerOptions { OnListeningStarted = ServerStartedListening, OnClientAdded = ClientAdded }).ConfigureAwait(false);
            return dataReceived;
        }

        protected static async Task StartClientWithNoServer2Async(IPEndPoint ipEndPoint)
        {
            await TextTcpIpClient.StartAsync(ipEndPoint).ConfigureAwait(false);
        }

        protected static async Task DoTcpClientConnectAsyncThrowsTcpIpClientConnectFailedException(IPEndPoint ipEndPoint, Type typeOfExceptionToThrow)
        {
            ITcpClient GetTcpClient()
            {
                void ThrowException(CallInfo callInfo)
                {
                    throw (Exception)Activator.CreateInstance(typeOfExceptionToThrow);
                }

                ITcpClient tcpClient = Substitute.For<ITcpClient>();
                tcpClient.When(tcpClnt => { tcpClnt.ConnectAsync(Arg.Any<IPAddress>(), Arg.Any<int>()); })
                         .Do(ThrowException);
                return tcpClient;
            }

            await TextTcpIpClient.StartAsync(ipEndPoint, new TcpIpClientOptions { GetTcpClient = GetTcpClient }).ConfigureAwait(false);
        }

        protected static async Task DoNetworkStreamReadAsyncThrowsTcpIpClientConnectFailedException(IPEndPoint ipEndPoint, string dataToSend,
                                                                                                Type typeOfExceptionToThrow)
        {
            TextTcpIpServer server = default;
            ExceptionDispatchInfo exceptionDispatchInfo = default;

            ITcpClient GetTcpClient()
            {
                void ThrowException(CallInfo callInfo)
                {
                    throw (Exception)Activator.CreateInstance(typeOfExceptionToThrow);
                }

                ITcpClient tcpClient = Substitute.For<ITcpClient>();
                INetworkStream networkStream = Substitute.For<INetworkStream>();
                tcpClient.GetStream().Returns(networkStream);
                networkStream.When(networkStrm => { networkStrm.ReadAsync(Arg.Any<byte[]>(), Arg.Any<int>(), Arg.Any<int>()); })
                             .Do(ThrowException);
                return tcpClient;
            }

            void ClientAdded(object sender, string clientIdentifier, ITcpClient tcpClient)
            {
                server.Send(dataToSend, clientIdentifier);
            }

            async void ServerIsListening(object sender, ITcpListener tcpListener)
            {
                server = (TextTcpIpServer)sender;
                try
                {
                    await TextTcpIpClient.StartAsync(ipEndPoint, new TcpIpClientOptions { GetTcpClient = GetTcpClient }).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                    server.Stop();
                }
            }

            await TextTcpIpServer.StartAsync(ipEndPoint, new TcpIpServerOptions { OnListeningStarted = ServerIsListening, OnClientAdded = ClientAdded }).ConfigureAwait(false);
            exceptionDispatchInfo?.Throw();
        }

        protected static async Task DoAcceptTcpClientAsyncThrowsTcpIpListenerStartListeningAsyncException(IPEndPoint ipEndPoint, Type typeOfExceptionToThrow)
        {
            ITcpListener GetTcpListener(IPAddress ipAddress, int port)
            {
                void ThrowException(CallInfo callInfo)
                {
                    throw (Exception)Activator.CreateInstance(typeOfExceptionToThrow);
                }

                ITcpListener tcpListener = Substitute.For<ITcpListener>();
                tcpListener.When(tcpLstnr => { tcpLstnr.AcceptTcpClientAsync(); })
                           .Do(ThrowException);
                return tcpListener;
            }

            await TextTcpIpServer.StartAsync(ipEndPoint, new TcpIpServerOptions { GetTcpListener = GetTcpListener }).ConfigureAwait(false);
        }

        protected static async Task TcpReceiverThrowsAnExceptionWhileReading(IPEndPoint ipEndPoint)
        {
            async void ServerIsListening(object sender, ITcpListener tcpListener)
            {
                try
                {
                    await TextTcpIpClient.StartAsync(ipEndPoint).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    // Event though at this point the Tcp Server has connected and started listening, it is possible that by the time the TcpIpClient
                    // tries to start reading the server will have already closed due to the exception thrown in the ThrowException() method of the 
                    // mocked ITcpReceiver. We are ignoring exceptions thrown by the TcpIpClient as we are only interested in testing the exception
                    // thrown by the TcpIpServer.
                }
            }

            static ITcpReceiver GetTcpReceiver(ITcpIpNode tcpIpNode, int identifier, int bufferLength, char endOfLine, SynchronizationContext synchronizationContext,
                                            Action<object, int, SynchronizationContext, string> onDataItemReceived,
                                            Action<object, byte[], int> onDataReceived)
            {
                static int ThrowException(CallInfo callInfo)
                {
                    throw new InvalidOperationException("In ReadAsync");
                }

                ITcpReceiver tcpReceiver = Substitute.ForPartsOf<TcpReceiver>(tcpIpNode, identifier, bufferLength, endOfLine, synchronizationContext,
                                                                                onDataItemReceived, onDataReceived);
                tcpReceiver.Configure().ReadAsync(Arg.Any<INetworkStream>(), Arg.Any<byte[]>())
                                       .Returns(ThrowException);
                return tcpReceiver;
            }

            await TextTcpIpServer.StartAsync(ipEndPoint,
                new TcpIpServerOptions { GetTcpReceiver = GetTcpReceiver, OnListeningStarted = ServerIsListening }).ConfigureAwait(false);
        }

        protected static async Task DoClientSendsToAServerThatIsNotListeningAsync(IPEndPoint ipEndPoint, string valueToSend)
        {
            TextTcpIpServer server                      = default;
            TextTcpIpClient client                      = default;
            ExceptionDispatchInfo exceptionDispatchInfo = default;

            void ClientIsConnected(object sender, ITcpClient tcpClient)
            {
                client = (TextTcpIpClient)sender;
                server.Stop();
            }

            async void ServerIsListening(object sender, ITcpListener tcpListener)
            {
                server = (TextTcpIpServer)sender;
                try
                {
                    await TextTcpIpClient.StartAsync(ipEndPoint, new TcpIpClientOptions { OnConnected = ClientIsConnected }).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    // There is a race condition between the task that asynchronously reads data (TcpReceiver.ReadAsync()) and the task
                    // that connects to the server and starts reading messsages from the server.
                    // Once the client starts it automatically connects to the server and it starts reading messages from the server.
                    // It is possible that by the time the TcpIpClient tries to start reading messages the server will have already closed
                    // due to the "server.Stop()" method invoked once the client connects.  We are ignoring exceptions here as we are only
                    // interested in exceptions ocurring when a client tries to send a message to a server that already closed.
                }
            }

            void ClientSend()
            {
                try
                {
                    client.Send(valueToSend);
                }
                catch (Exception exception)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                    client.Disconnect();
                }
            }

            await TextTcpIpServer.StartAsync(ipEndPoint, new TcpIpServerOptions { OnListeningStarted = ServerIsListening }).ConfigureAwait(false);
            ClientSend();
            exceptionDispatchInfo?.Throw();
        }
    }

    [TestFixture]
    [Category("TextTcpIp Tests")]
    internal sealed class TcpIp12Client_Tests : TcpIp12Client_TestsBase
    {
        #region Happy Path tests

        // Tcp/Ip client sends text to the Tcp/Ip server.  This test verifies that the server received the text sent by the client.
        [TestCaseSource(typeof(TestCases), nameof(TestCases.SingleValue), new object[] { true })]
        public async Task<string> WhenATcpClientSendsADataValueToTheTcpServer_ThenTheTcpServerReceivesTheDataValueSentAsync(IPEndPoint ipEndPoint, string dataToSend)
        {
            return await GetValueSentByClient3Async(ipEndPoint, dataToSend).ConfigureAwait(false);
        }

        // Tcp/Ip client sends multiple text values to the Tcp/Ip server.  This test verifies that the server received the text values sent by the client.
        [TestCaseSource(typeof(TestCases), nameof(TestCases.MultipleValues))]
        public async Task<IEnumerable<string>> WhenATcpClientSendsMultipleDataValuesToTheTcpServer_ThenTheTcpServerReceivesTheDataValuesSent(IPEndPoint ipEndPoint, string[] dataToSend)
        {
            return await GetValuesSentByClient2Async(ipEndPoint, dataToSend).ConfigureAwait(false);
        }

        // This test verifies that when the Tcp/Ip client connects to the Tcp/Ip server then the client's Connected event is raised.
        [TestCaseSource(typeof(TestCases), nameof(TestCases.ClientConnect))]
        public async Task<bool> WhenTheTcpClientConnectsToTheTcpServer_ThenTheConnectedEventIsRaisedAsync(IPEndPoint ipEndPoint)
        {
            return await DoConnected2Async(ipEndPoint).ConfigureAwait(false);
        }

        // This test verifies that when the Tcp/Ip client is about to connect to the Tcp/Ip server then the client's Connecting event is raised.
        [TestCaseSource(typeof(TestCases), nameof(TestCases.ClientConnect))]
        public async Task<bool> WhenTheTcpClientIsAboutToConnectToTheTcpServer_ThenTheConnectingEventIsRaisedAsync(IPEndPoint ipEndPoint)
        {
            return await DoConnecting2Async(ipEndPoint).ConfigureAwait(false);
        }

        // Tcp/Ip server sends text to a Tcp/Ip client.  This test verifies that the client received the text sent by the server.
        [TestCaseSource(typeof(TestCases), nameof(TestCases.SingleValue), new object[] { true })]
        public async Task<string> WhenATcpServerSendsADataValueToATcpClient_ThenTheTcpClientReceivesTheDataValueSent(IPEndPoint ipEndPoint, string dataToSend)
        {
            return await DoServerSendsValueToClientAsync(ipEndPoint, dataToSend).ConfigureAwait(false);
        }

        // Tcp/Ip server sends multiple text values to a Tcp/Ip client.  This test verifies that the client received the text values sent by the server.
        [TestCaseSource(typeof(TestCases), nameof(TestCases.MultipleValues2))]
        public async Task<IEnumerable<string>> WhenATcpServerSendsMultipleDataValuesToATcpClient_ThenTheTcpClientReceivesTheDataValuesSent(IPEndPoint ipEndPoint, string[] dataToSend)
        {
            return await DoServerSendsValuesToClientAsync(ipEndPoint, dataToSend).ConfigureAwait(false);
        }

        #endregion Happy Path tests

        #region Unhappy Path tests

        // This test verifies that if the Tcp/Ip client tries to connect to a Tcp/Ip server that has not started then a TcpIpClientConnectFailedException
        [TestCaseSource(typeof(TestCases), nameof(TestCases.ServerNotStarted))]
        public void WhenTheTcpServerHasNotStarted_ThenATcpIpClientConnectFailedExceptionIsThrown(IPEndPoint ipEndPoint)
        {
            // Github issue # 37150:  For .NET Core 2.2, the exception thrown is a System.Net.Internals.SocketExceptionFactory+ExtendedSocketException
            // instead of the documented System.Net.Sockets.SocketException.
            // System.Net.Internals.SocketExceptionFactory+ExtendedSocketException descends from System.Net.Sockets.SocketException.
            // https://github.com/dotnet/runtime/issues/37150
            TcpIpClientConnectFailedException exception =
                Assert.ThrowsAsync<TcpIpClientConnectFailedException>(async () => await StartClientWithNoServer2Async(ipEndPoint).ConfigureAwait(false));
            Assert.That(exception.InnerException, Is.AssignableTo<SocketException>());
        }

        // Test for an exception when the client tries to connect 
        [TestCaseSource(typeof(TestCases), nameof(TestCases.ServerNotStarted2))]
        public void WhenTcpClientConnectAsyncFails_ThenATcpIpClientConnectFailedExceptionIsThrown(IPEndPoint ipEndPoint, Type typeOfExceptionToThrow)
        {
            TcpIpClientConnectFailedException exception =
                Assert.ThrowsAsync<TcpIpClientConnectFailedException>(async () => await DoTcpClientConnectAsyncThrowsTcpIpClientConnectFailedException(ipEndPoint, typeOfExceptionToThrow).ConfigureAwait(false));
            Assert.That(exception.InnerException, Is.TypeOf(typeOfExceptionToThrow));
        }

        // Test for Client receiving failure
        [TestCaseSource(typeof(TestCases), nameof(TestCases.SingleValue2))]
        public void WhenNetworkStreamReadAsyncFails_ThenATcpIpClientConnectFailedExceptionIsThrown(IPEndPoint ipEndPoint, string dataToSend,
                                                                                                    Type typeOfExceptionToThrow)
        {
            TcpIpClientConnectFailedException exception =
                 Assert.ThrowsAsync<TcpIpClientConnectFailedException>(
                     async () => await DoNetworkStreamReadAsyncThrowsTcpIpClientConnectFailedException(ipEndPoint, dataToSend, typeOfExceptionToThrow).ConfigureAwait(false));
            Assert.That(exception.InnerException, Is.TypeOf<TcpIpReceiveFailedException>());
            Assert.That(exception.InnerException.InnerException, Is.TypeOf<TcpIpInnerReceiveFailedException>());
            Assert.That(exception.InnerException.InnerException.InnerException, Is.TypeOf(typeOfExceptionToThrow));
        }

        // Test for Server accepting client failure
        [TestCaseSource(typeof(TestCases), nameof(TestCases.JustAnEndPoint2))]
        public void WhenAcceptTcpClientAsyncFails_ThenATcpIpListenerStartListeningAsyncExceptionIsThrown(IPEndPoint ipEndPoint, Type typeOfExceptionToThrow)
        {
            TcpIpListenerStartListeningAsyncException exception =
                Assert.ThrowsAsync<TcpIpListenerStartListeningAsyncException>(async () => await DoAcceptTcpClientAsyncThrowsTcpIpListenerStartListeningAsyncException(ipEndPoint, typeOfExceptionToThrow).ConfigureAwait(false));
            Assert.That(exception.InnerException, Is.TypeOf<TcpIpListenerAcceptTcpClientFailedException>());
            Assert.That(exception.InnerException.InnerException, Is.TypeOf(typeOfExceptionToThrow));
        }

        // Test for an exception being thrown when the client tries to send a value to a server that has stopped listening
        [TestCaseSource(typeof(TestCases), nameof(TestCases.SingleValue), new object[] { false })]
        public void WhenAClientTriesToSendToAServerThatHasStopped_ThenATcpIpSendFailedExceptionIsThrown(IPEndPoint ipEndPoint, string valueToSend)
        {
            TcpIpSendFailedException exception = Assert.ThrowsAsync<TcpIpSendFailedException>(async () => await DoClientSendsToAServerThatIsNotListeningAsync(ipEndPoint, valueToSend).ConfigureAwait(false));
        }

        // Test for an exception being thrown when the TcpReceiver member reads data via its ReadAsync() method
        [TestCaseSource(typeof(TestCases), nameof(TestCases.JustAnEndPoint))]
        public void WhenTcpReceiverReadAsyncFails_ThenThatAsync_ThenAnExceptionIsThrown(IPEndPoint ipEndPoint)
        {
            TcpIpListenerStartListeningAsyncException exception = Assert.ThrowsAsync<TcpIpListenerStartListeningAsyncException>(async () => await TcpReceiverThrowsAnExceptionWhileReading(ipEndPoint).ConfigureAwait(false));
            Assert.That(exception.InnerException, Is.TypeOf<TcpIpListenerAcceptTcpClientFailedException>());
            Assert.That(exception.InnerException.InnerException, Is.TypeOf<ObjectDisposedException>());
        }

        #endregion Unhappy Path tests
    }
}