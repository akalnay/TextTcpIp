////////////////////////////////////////////////////////
// Copyright (c) Alejandro Kalnay                     //
// License: GNU GPLv3                                 //
////////////////////////////////////////////////////////

using System.Net.Sockets;
using System.Threading.Tasks;

namespace TextTcpIp
{
    public sealed class NetworkStreamProxy : INetworkStream
    {
        private readonly NetworkStream _networkStream;

        public NetworkStreamProxy(NetworkStream networkStream)
        {
            ThrowIf.ArgumentIsNull(networkStream, nameof(networkStream));
            _networkStream = networkStream;
        }

        public bool CanWrite => _networkStream.CanWrite;

        public Task<int> ReadAsync(byte[] buffer, int offset, int count) => _networkStream.ReadAsync(buffer, offset, count);

        public void Write(byte[] buffer, int offset, int size) => _networkStream.Write(buffer, offset, size);
    }
}
