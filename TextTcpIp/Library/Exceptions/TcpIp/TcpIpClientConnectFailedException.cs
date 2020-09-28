////////////////////////////////////////////////////////
// Copyright (c) Alejandro Kalnay                     //
// License: GNU GPLv3                                 //
////////////////////////////////////////////////////////

using System;
using System.Runtime.Serialization;

namespace TextTcpIp
{
    public sealed class TcpIpClientConnectFailedException : TcpIpExceptionBase
    {
        private const string _DEFAULTMESSAGE = "Tcp/Ip Client Connect failed.";

        public TcpIpClientConnectFailedException() : this(_DEFAULTMESSAGE) { }

        public TcpIpClientConnectFailedException(string message) : base(message) { }

        public TcpIpClientConnectFailedException(string message, Exception innerException) : base(message, innerException) { }

        private TcpIpClientConnectFailedException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public TcpIpClientConnectFailedException(Exception innerException) : this(_DEFAULTMESSAGE, innerException) { }
    }
}
