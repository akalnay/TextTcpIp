////////////////////////////////////////////////////////
// Copyright (c) Alejandro Kalnay                     //
// License: GNU GPLv3                                 //
////////////////////////////////////////////////////////

using System;
using System.Runtime.Serialization;

namespace TextTcpIp
{
    public sealed class TcpIpNetworkStreamIsNotWriteableException : TcpIpExceptionBase
    {
        private const string _DEFAULTMESSAGE = "Tcp/Ip Networkstream is not writeable.";

        public TcpIpNetworkStreamIsNotWriteableException() : this(_DEFAULTMESSAGE) { }

        public TcpIpNetworkStreamIsNotWriteableException(string message) : base(message) { }

        public TcpIpNetworkStreamIsNotWriteableException(string message, Exception innerException) : base(message, innerException) { }

        private TcpIpNetworkStreamIsNotWriteableException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public TcpIpNetworkStreamIsNotWriteableException(Exception innerException) : this(_DEFAULTMESSAGE, innerException) { }
    }
}
