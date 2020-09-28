////////////////////////////////////////////////////////
// Copyright (c) Alejandro Kalnay                     //
// License: GNU GPLv3                                 //
////////////////////////////////////////////////////////

using System;
using System.Threading;
using System.Threading.Tasks;

namespace TextTcpIp
{
    public interface ITcpReceiver
    {
        int BufferLength { get; }
        bool Disconnecting { get; set; }
        char EndOfLine { get; }
        int Identifier { get; }
        SynchronizationContext SynchronizationContext { get; }

        event EventHandler<StreamData10EventArgs> DataItemReceived;
        event EventHandler<BufferedDataReceivedEventArgs> DataReceived;

        Task ReceiveAsync(ITcpClient client);

        Task<int> ReadAsync(INetworkStream networkStream, byte[] buffer);
    }
}
