////////////////////////////////////////////////////////
// Copyright (c) Alejandro Kalnay                     //
// License: GNU GPLv3                                 //
////////////////////////////////////////////////////////

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace TextTcpIp
{
    public sealed class TcpClientProxy : ITcpClient, IDisposable
    {
        private readonly TcpClient _tcpClient;

        public TcpClientProxy() : this(new TcpClient())
        {
        }

        public TcpClientProxy(TcpClient tcpClient)
        {
            _tcpClient = tcpClient;
        }

        public bool Connected => _tcpClient.Connected;

        public Socket Client
        {
            get => _tcpClient.Client;
            set => _tcpClient.Client = value;
        }

        public void Close() => _tcpClient.Close();

        public Task ConnectAsync(IPAddress address, int port) => _tcpClient.ConnectAsync(address, port);

        public INetworkStream GetStream() => new NetworkStreamProxy(_tcpClient.GetStream());

        #region IDisposable Support

        private bool _disposed = false;

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                    Close();
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
