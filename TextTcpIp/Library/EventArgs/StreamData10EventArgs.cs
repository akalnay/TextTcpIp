////////////////////////////////////////////////////////
// Copyright (c) Alejandro Kalnay                     //
// License: GNU GPLv3                                 //
////////////////////////////////////////////////////////

using System;
using System.Threading;

namespace TextTcpIp
{
    public sealed class StreamData10EventArgs : EventArgs
    {
        public StreamData10EventArgs(int identifier, SynchronizationContext synchronizationContext, string data)
        {
            Data                   = data;
            SynchronizationContext = synchronizationContext;
            Identifier             = identifier;
        }

        public int Identifier { get; }

        public SynchronizationContext SynchronizationContext { get; }

        public string Data { get; }
    }
}
