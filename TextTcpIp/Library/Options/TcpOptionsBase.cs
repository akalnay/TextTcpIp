////////////////////////////////////////////////////////
// Copyright (c) Alejandro Kalnay                     //
// License: GNU GPLv3                                 //
////////////////////////////////////////////////////////

using System;
using System.Threading;

namespace TextTcpIp
{
    public abstract class TcpOptionsBase
    {
        private const int _DEFAULTBUFFERLENGTH = 256;
        private const char _DEFAULTENDOFLINE = '\n';
        private static readonly Func<ITcpIpNode, int, int, char, SynchronizationContext, Action<object, int, SynchronizationContext, string>,
                                        Action<object, byte[], int>, ITcpReceiver> _DEFAULTGETTCPRECEIVER =
            (tcpIpNode, identifier, bufferLength, endOfLine, synchronizationContext, onDataItemReceived, onDataReceived) =>
            new TcpReceiver(tcpIpNode, identifier, bufferLength, endOfLine, synchronizationContext, onDataItemReceived, onDataReceived);

        public abstract int Identifier { get; set; }
        public int BufferLength { get; set; } = _DEFAULTBUFFERLENGTH;
        public char EndOfLine { get; set; } = _DEFAULTENDOFLINE;
        public SynchronizationContext SynchronizationContext { get; set; }
        public Func<ITcpIpNode, int, int, char, SynchronizationContext,
            Action<object, int, SynchronizationContext, string>, Action<object, byte[], int>, ITcpReceiver> GetTcpReceiver
        { get; set; } = _DEFAULTGETTCPRECEIVER;

        public Action<object, int, SynchronizationContext, string> OnDataItemReceived { get; set; }
        public Action<object, byte[], int> OnDataReceived { get; set; }
    }
}
