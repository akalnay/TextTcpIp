////////////////////////////////////////////////////////
// Copyright (c) Alejandro Kalnay                     //
// License: GNU GPLv3                                 //
////////////////////////////////////////////////////////

using System;
using System.Runtime.Serialization;

namespace TextTcpIp
{
    public sealed class TcpIpNetworkStreamWriteFailedException : TcpIpExceptionBase
    {
        private const string _DEFAULTMESSAGE = "Tcp/Ip Networkstream write failed.";

        public TcpIpNetworkStreamWriteFailedException() : this(_DEFAULTMESSAGE) { }

        public TcpIpNetworkStreamWriteFailedException(string message) : base(message) { }

        public TcpIpNetworkStreamWriteFailedException(string message, Exception innerException) : base(message, innerException) { }

        private TcpIpNetworkStreamWriteFailedException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public TcpIpNetworkStreamWriteFailedException(Exception innerException) : this(_DEFAULTMESSAGE, innerException) { }
    }
}
