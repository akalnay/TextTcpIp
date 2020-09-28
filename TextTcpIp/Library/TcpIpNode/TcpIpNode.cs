////////////////////////////////////////////////////////
// Copyright (c) Alejandro Kalnay                     //
// License: GNU GPLv3                                 //
////////////////////////////////////////////////////////

using System;
using System.Text;
using System.Threading;

namespace TextTcpIp
{
    public abstract class TcpIpNode : ITcpIpNode
    {
        private const int _DEFAULTBUFFERLENGTH = 256;
        private const char _DEFAULTENDOFLINE = '\n';
        private static readonly Func<ITcpIpNode, int, int, char, SynchronizationContext, Action<object, int, SynchronizationContext, string>,
                                        Action<object, byte[], int>, ITcpReceiver> _DEFAULTGETTCPRECEIVER =
            (tcpIpNode, identifier, bufferLength, endOfLine, synchronizationContext, onDataItemReceived, onDataReceived) =>
            new TcpReceiver(tcpIpNode, identifier, bufferLength, endOfLine, synchronizationContext, onDataItemReceived, onDataReceived);
        private readonly Func<ITcpIpNode, int, int, char, SynchronizationContext, Action<object, int, SynchronizationContext, string>,
                                Action<object, byte[], int>, ITcpReceiver> _getTcpReceiver;
        private readonly Action<object, int, SynchronizationContext, string> _onDataItemReceived;
        private readonly Action<object, byte[], int> _onDataReceived;
        private ITcpReceiver _tcpReceiver;
        private bool _disconnecting;

        protected TcpIpNode(int identifier, int bufferLength, char endOfLine, SynchronizationContext synchronizationContext,
                                Func<ITcpIpNode, int, int, char, SynchronizationContext, Action<object, int, SynchronizationContext, string>,
                                    Action<object, byte[], int>, ITcpReceiver> getTcpReceiver,
                                Action<object, int, SynchronizationContext, string> onDataItemReceived,
                                Action<object, byte[], int> onDataReceived)
        {
            ThrowIf.ArgumentIsOutOfRange(bufferLength, bLength => bLength < 1, nameof(bufferLength));
            Identifier = identifier;
            BufferLength = bufferLength;
            EndOfLine = endOfLine;
            SynchronizationContext = synchronizationContext;
            _getTcpReceiver = getTcpReceiver ?? _DEFAULTGETTCPRECEIVER;
            _onDataItemReceived = onDataItemReceived;
            _onDataReceived = onDataReceived;
        }

        protected bool Disconnecting
        {
            get { return _disconnecting; }
            set
            {
                _disconnecting = value;
                TcpReceiver.Disconnecting = _disconnecting;
            }
        }

        private ITcpReceiver GetTcpReceiver(int identifier, int bufferLength, char endOfLine, SynchronizationContext synchronizationContext,
                    Action<object, int, SynchronizationContext, string> onDataItemReceived, Action<object, byte[], int> onDataReceived)
        {
            ITcpReceiver tcpReceiver = default;
            try
            {
                tcpReceiver = _getTcpReceiver(this, identifier, bufferLength, endOfLine, synchronizationContext, onDataItemReceived, onDataReceived);
            }
            catch (Exception exception)
            {
                if (!Disconnecting)
                    throw new TcpIpReceiverDelegateFailedException(exception);
            }
            return tcpReceiver;
        }

        public ITcpReceiver TcpReceiver
        {
            get
            {
                if (_tcpReceiver == null)
                    _tcpReceiver = GetTcpReceiver(Identifier, BufferLength, EndOfLine, SynchronizationContext, _onDataItemReceived, _onDataReceived);
                return _tcpReceiver;
            }
        }

        public int Identifier { get; }
        public int BufferLength { get; }
        public char EndOfLine { get; }
        public SynchronizationContext SynchronizationContext { get; }

        public static int DefaultBufferLength { get; } = _DEFAULTBUFFERLENGTH;
        public static char DefaultEndOfLine { get; } = _DEFAULTENDOFLINE;

        protected void Send(string value, ITcpClient tcpClient)
        {
            value += EndOfLine;
            INetworkStream networkStream = default;
            byte[] bytes = default;
            try
            {
                ThrowIf.IsNull(tcpClient, "Error in Send().  tcpClient can't be null!");
                try
                {
                    bytes = Encoding.ASCII.GetBytes(value);
                }
                catch (Exception exception)
                {
                    if (!Disconnecting)
                        throw new TcpIpGetBytesFailedException(exception);
                }
                try
                {
                    networkStream = tcpClient.GetStream();
                }
                catch (Exception exception)
                {
                    if (!Disconnecting)
                        throw new TcpIpGetStreamFailedException(exception);
                }
                if (networkStream.CanWrite)
                {
                    try
                    {
                        networkStream.Write(bytes, 0, bytes.Length);
                    }
                    catch (Exception exception)
                    {
                        if (!Disconnecting)
                            throw new TcpIpNetworkStreamWriteFailedException(exception);
                    }
                }
                else
                {
                    if (!Disconnecting)
                        throw new TcpIpNetworkStreamIsNotWriteableException();
                }
            }
            catch (Exception exception)
            {
                if (!Disconnecting)
                    throw new TcpIpSendFailedException(exception);
            }
        }
    }
}
