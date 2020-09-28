////////////////////////////////////////////////////////
// Copyright (c) Alejandro Kalnay                     //
// License: GNU GPLv3                                 //
////////////////////////////////////////////////////////

using System.Threading.Tasks;

namespace TextTcpIp
{
    public interface INetworkStream
    {
        Task<int> ReadAsync(byte[] buffer, int offset, int count);
        bool CanWrite { get; }
        void Write(byte[] buffer, int offset, int size);
    }
}
