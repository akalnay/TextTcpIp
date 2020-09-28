////////////////////////////////////////////////////////
// Copyright (c) Alejandro Kalnay                     //
// License: GNU GPLv3                                 //
////////////////////////////////////////////////////////

using System;
using System.Runtime.Serialization;

namespace TextTcpIp
{
    public sealed class TcpIpSendFailedException : TcpIpExceptionBase
    {
        private const string _DEFAULTMESSAGE = "Tcp/Ip Send failed.";

        public TcpIpSendFailedException() : this(_DEFAULTMESSAGE) { }

        public TcpIpSendFailedException(string message) : base(message) { }

        public TcpIpSendFailedException(string message, Exception innerException) : base(message, innerException) { }

        private TcpIpSendFailedException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public TcpIpSendFailedException(Exception innerException) : this(_DEFAULTMESSAGE, innerException) { }
    }
}
