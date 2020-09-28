////////////////////////////////////////////////////////
// Copyright (c) Alejandro Kalnay                     //
// License: GNU GPLv3                                 //
////////////////////////////////////////////////////////

using System;
using System.Runtime.Serialization;

namespace TextTcpIp
{
    public sealed class TcpIpListenerAcceptTcpClientFailedException : TcpIpExceptionBase
    {
        private const string _DEFAULTMESSAGE = "Tcp/Ip Listener accept tcp/client failed.";

        public TcpIpListenerAcceptTcpClientFailedException() : this(_DEFAULTMESSAGE) { }

        public TcpIpListenerAcceptTcpClientFailedException(string message) : base(message) { }

        public TcpIpListenerAcceptTcpClientFailedException(string message, Exception innerException) : base(message, innerException) { }

        private TcpIpListenerAcceptTcpClientFailedException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public TcpIpListenerAcceptTcpClientFailedException(Exception innerException) : this(_DEFAULTMESSAGE, innerException) { }
    }
}
