////////////////////////////////////////////////////////
// Copyright (c) Alejandro Kalnay                     //
// License: GNU GPLv3                                 //
////////////////////////////////////////////////////////

using System.Threading;

namespace TextTcpIp
{
    public interface ITcpIpNode
    {
        int BufferLength { get; }
        char EndOfLine { get; }
        int Identifier { get; }
        SynchronizationContext SynchronizationContext { get; }
        ITcpReceiver TcpReceiver { get; }
    }
}
