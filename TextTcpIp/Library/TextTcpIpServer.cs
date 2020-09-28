////////////////////////////////////////////////////////
// Copyright (c) Alejandro Kalnay                     //
// License: GNU GPLv3                                 //
////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace TextTcpIp
{
    public class TextTcpIpServer : TcpIpNode, IDisposable
    {
        private static readonly Func<IPAddress, int, ITcpListener> _DEFAULTGETTCPLISTENER = (ipAddress, port) => new TcpListenerProxy(ipAddress, port);
        private readonly Func<IPAddress, int, ITcpListener> _getTcpListener;
        private ITcpListener _tcpListener;
        private readonly IDictionary<string, ITcpClient> _tcpClients = new Dictionary<string, ITcpClient>();

        private TextTcpIpServer(int streamIdentifier, int bufferLength, char endOfLine, SynchronizationContext synchronizationContext,
                                Func<IPAddress, int, ITcpListener> getTcpListener,
                                Func<ITcpIpNode, int, int, char, SynchronizationContext,
                                        Action<object, int, SynchronizationContext, string>, Action<object, byte[], int>, ITcpReceiver> getTcpReceiver,
                                Action<object, int, SynchronizationContext, string> onDataItemReceived,
                                Action<object, byte[], int> onDataReceived
            ) : base(streamIdentifier, bufferLength, endOfLine, synchronizationContext, getTcpReceiver, onDataItemReceived, onDataReceived)
        {
            _getTcpListener = getTcpListener ?? _DEFAULTGETTCPLISTENER;
        }

        public event EventHandler<TcpIpListenerAddedClient12EventArgs> ClientAdded;
        public event EventHandler<TcpIpListenerListen12EventArgs> ListeningStarting;
        public event EventHandler<TcpIpListenerListen12EventArgs> ListeningStarted;

        public async Task StartListeningAsync(IPAddress ipAddress, int port)
        {
            ThrowIf.ArgumentIsNull(ipAddress, nameof(ipAddress));
            Disconnecting = false;
            try
            {
                _tcpListener = GetTcpListener(ipAddress, port);
                if (_tcpListener != null)
                {
                    StartTcpListener(_tcpListener);
                    await AcceptTcpClientsAsync(_tcpListener).ConfigureAwait(false);
                }
            }
            catch (Exception exception)
            {
                throw new TcpIpListenerStartListeningAsyncException(exception);
            }
        }

        public void Stop()
        {
            Disconnecting = true;
            Dispose();
        }

        private void OnListeningStarting(ITcpListener tcpListener) => ListeningStarting?.Invoke(this, new TcpIpListenerListen12EventArgs(tcpListener));

        private void OnListeningStarted(ITcpListener tcpListener) => ListeningStarted?.Invoke(this, new TcpIpListenerListen12EventArgs(tcpListener));

        private void OnClientAdded(string clientKey, ITcpClient client) => ClientAdded?.Invoke(this, new TcpIpListenerAddedClient12EventArgs(clientKey, client));

        private ITcpListener GetTcpListener(IPAddress ipAddress, int port)
        {
            ITcpListener tcpListener = default;
            try
            {
                tcpListener = _getTcpListener(ipAddress, port);
            }
            catch (Exception exception)
            {
                if (!Disconnecting)
                    throw new TcpIpListenerInstantiationFailedException(exception);
            }
            return tcpListener;
        }

        private void StartTcpListener(ITcpListener tcpListener)
        {
            try
            {
                OnListeningStarting(tcpListener);
                tcpListener.Start();
                OnListeningStarted(tcpListener);
            }
            catch (Exception exception)
            {
                if (!Disconnecting)
                    throw new TcpIpListenerStartFailedException(exception);
            }
        }

        private async Task AcceptTcpClientsAsync(ITcpListener tcpListener)
        {
            ExceptionDispatchInfo exceptionDispatchInfo = default;
            bool errorOcurred = false;
            while (!Disconnecting && !errorOcurred)
            {
                ITcpClient tcpClient = await DoAcceptTcpClientAsync(tcpListener).ConfigureAwait(false);
                AddClient(_tcpClients, tcpClient);

                async Task ClientReceiveAsync()
                {
                    try
                    {
                        await TcpReceiver.ReceiveAsync(tcpClient).ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        errorOcurred = true;
                        exceptionDispatchInfo = ExceptionDispatchInfo.Capture(e);
                        tcpListener.Stop();
                    }
                }

                Task task = Task.Run(ClientReceiveAsync);
            }
            if (exceptionDispatchInfo != null)
                throw new TcpIpAcceptClientsFailedException(exceptionDispatchInfo.SourceException);
        }

        private async Task<ITcpClient> DoAcceptTcpClientAsync(ITcpListener tcpListener)
        {
            ITcpClient tcpClient = default;
            try
            {
                tcpClient = await tcpListener.AcceptTcpClientAsync().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                if (!Disconnecting)
                    throw new TcpIpListenerAcceptTcpClientFailedException(exception);
            }
            return tcpClient;
        }

        private void AddClient(IDictionary<string, ITcpClient> tcpClients, ITcpClient client)
        {
            string tcpClientKey = Guid.NewGuid().ToString();
            tcpClients.Add(tcpClientKey, client);
            OnClientAdded(tcpClientKey, client);
        }

        public void Send(string value, string tcpClientKey)
        {
            Send(value, _tcpClients[tcpClientKey]);
        }

        #region Static Start() Methods

        public static async Task StartAsync(IPAddress ipAddress, int port)
        {
            await StartAsync(ipAddress, port, null).ConfigureAwait(false);
        }

        public static async Task StartAsync(IPEndPoint ipEndPoint)
        {
            await StartAsync(ipEndPoint, null).ConfigureAwait(false);
        }

        public static async Task StartAsync(IPEndPoint ipEndPoint, TcpIpServerOptions tcpIpServerOptions)
        {
            ThrowIf.ArgumentIsNull(ipEndPoint, nameof(ipEndPoint));
            await StartAsync(ipEndPoint.Address, ipEndPoint.Port, tcpIpServerOptions).ConfigureAwait(false);
        }

        public static async Task StartAsync(IPAddress ipAddress, int port, TcpIpServerOptions tcpIpServerOptions)
        {
            if (tcpIpServerOptions == null)
                tcpIpServerOptions = new TcpIpServerOptions();
            using (TextTcpIpServer tcpIpServer = new TextTcpIpServer(tcpIpServerOptions.Identifier, tcpIpServerOptions.BufferLength,
                    tcpIpServerOptions.EndOfLine, tcpIpServerOptions.SynchronizationContext, tcpIpServerOptions.GetTcpListener,
                    tcpIpServerOptions.GetTcpReceiver, tcpIpServerOptions.OnDataItemReceived, tcpIpServerOptions.OnDataReceived))
            {
                if (tcpIpServerOptions.OnListeningStarting != null)
                    tcpIpServer.ListeningStarting += (o, e) => tcpIpServerOptions.OnListeningStarting(o, e.TcpListener);
                if (tcpIpServerOptions.OnListeningStarted != null)
                    tcpIpServer.ListeningStarted += (o, e) => tcpIpServerOptions.OnListeningStarted(o, e.TcpListener);
                if (tcpIpServerOptions.OnClientAdded != null)
                    tcpIpServer.ClientAdded += (o, e) => tcpIpServerOptions.OnClientAdded(o, e.TcpClientKey, e.TcpClient);
                await tcpIpServer.StartListeningAsync(ipAddress, port).ConfigureAwait(false);
            }
        }

        #endregion Static Start() Methods

        #region IDisposable Support

        private bool _disposed = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            void DisposeOfManagedObjects()
            {
                static void StopTcpListener(ITcpListener tcpListener)
                {
                    try
                    {
                        tcpListener.Stop();
                    }
                    catch (Exception exception)
                    {
                        throw new TcpIpListenerStopFailedException(exception);
                    }
                }

                void CloseClients()
                {
                    foreach (ITcpClient tcpClient in _tcpClients.Values)
                    {
                        tcpClient?.Close();
                    }
                }

                CloseClients();
                StopTcpListener(_tcpListener);
            }

            if (!_disposed)
            {
                if (disposing)
                    DisposeOfManagedObjects();
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}
