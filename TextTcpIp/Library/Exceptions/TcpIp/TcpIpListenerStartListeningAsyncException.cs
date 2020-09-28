////////////////////////////////////////////////////////
// Copyright (c) Alejandro Kalnay                     //
// License: GNU GPLv3                                 //
////////////////////////////////////////////////////////

using System;
using System.Runtime.Serialization;

namespace TextTcpIp
{
    public sealed class TcpIpListenerStartListeningAsyncException : TcpIpExceptionBase
    {
        private const string _DEFAULTMESSAGE = "Tcp/Ip Listener StartListeningAsync failed.";

        public TcpIpListenerStartListeningAsyncException() : this(_DEFAULTMESSAGE) { }

        public TcpIpListenerStartListeningAsyncException(string message) : base(message) { }

        public TcpIpListenerStartListeningAsyncException(string message, Exception innerException) : base(message, innerException) { }

        private TcpIpListenerStartListeningAsyncException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public TcpIpListenerStartListeningAsyncException(Exception innerException) : this(_DEFAULTMESSAGE, innerException) { }
    }
}
