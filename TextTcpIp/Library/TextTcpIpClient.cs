////////////////////////////////////////////////////////
// Copyright (c) Alejandro Kalnay                     //
// License: GNU GPLv3                                 //
////////////////////////////////////////////////////////

using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace TextTcpIp
{
    public class TextTcpIpClient : TcpIpNode, IDisposable
    {
        private static readonly Func<ITcpClient> _DEFAULTGETTCPCLIENT = () => new TcpClientProxy();
        private readonly Func<ITcpClient> _getTcpClient;
        private ITcpClient _tcpClient;

        private TextTcpIpClient(int identifier, int bufferLength, char endOfLine, SynchronizationContext synchronizationContext,
                                Func<ITcpClient> getTcpClient,
                                Func<ITcpIpNode, int, int, char, SynchronizationContext,
                                    Action<object, int, SynchronizationContext, string>, Action<object, byte[], int>, ITcpReceiver> getTcpReceiver,
                                Action<object, int, SynchronizationContext, string> onDataItemReceived,
                                Action<object, byte[], int> onDataReceived
                                ) : base(identifier, bufferLength, endOfLine, synchronizationContext, getTcpReceiver, onDataItemReceived, onDataReceived)
        {
            _getTcpClient = getTcpClient ?? _DEFAULTGETTCPCLIENT;
        }

        public event EventHandler<TcpIpClientConnect12EventArgs> Connecting;

        public event EventHandler<TcpIpClientConnect12EventArgs> Connected;

        public void Disconnect()
        {
            Disconnecting = true;
            Dispose();
        }

        private void OnConnecting(ITcpClient tcpClient) => Connecting?.Invoke(this, new TcpIpClientConnect12EventArgs(tcpClient));

        private void OnConnected(ITcpClient tcpClient) => Connected?.Invoke(this, new TcpIpClientConnect12EventArgs(tcpClient));

        public async Task ConnectAsync(IPAddress ipAddress, int port)
        {
            ThrowIf.ArgumentIsNull(ipAddress, nameof(ipAddress));
            await ConnectAsyncImpl(ipAddress, port).ConfigureAwait(false);
        }

        private async Task ConnectAsyncImpl(IPAddress ipAddress, int port)
        {
            try
            {
                Disconnecting = false;
                _tcpClient = _getTcpClient();
                OnConnecting(_tcpClient);
                await _tcpClient.ConnectAsync(ipAddress, port).ConfigureAwait(false);
                OnConnected(_tcpClient);
                await TcpReceiver.ReceiveAsync(_tcpClient).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                throw new TcpIpClientConnectFailedException(exception);
            }
        }

        public void Send(string value)
        {
            Send(value, _tcpClient);
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

        public static async Task StartAsync(IPEndPoint ipEndPoint, TcpIpClientOptions tcpIpClientOptions)
        {
            ThrowIf.ArgumentIsNull(ipEndPoint, nameof(ipEndPoint));
            await StartAsync(ipEndPoint.Address, ipEndPoint.Port, tcpIpClientOptions).ConfigureAwait(false);
        }

        public static async Task StartAsync(IPAddress ipAddress, int port, TcpIpClientOptions tcpIpClientOptions)
        {
            if (tcpIpClientOptions == null)
                tcpIpClientOptions = new TcpIpClientOptions();
            using (TextTcpIpClient tcpIpClient = new TextTcpIpClient(tcpIpClientOptions.Identifier, tcpIpClientOptions.BufferLength,
                    tcpIpClientOptions.EndOfLine, tcpIpClientOptions.SynchronizationContext, tcpIpClientOptions.GetTcpClient,
                    tcpIpClientOptions.GetTcpReceiver, tcpIpClientOptions.OnDataItemReceived, tcpIpClientOptions.OnDataReceived))
            {
                if (tcpIpClientOptions.OnConnecting != null)
                    tcpIpClient.Connecting += (o, e) => tcpIpClientOptions.OnConnecting(o, e.TcpClient);
                if (tcpIpClientOptions.OnConnected != null)
                    tcpIpClient.Connected += (o, e) => tcpIpClientOptions.OnConnected(o, e.TcpClient);
                await tcpIpClient.ConnectAsync(ipAddress, port).ConfigureAwait(false);
            }
        }

        #endregion Static Start() Methods

        #region IDisposable Support

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            void DisposeOfManagedObjects()
            {
                _tcpClient?.Close();
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
