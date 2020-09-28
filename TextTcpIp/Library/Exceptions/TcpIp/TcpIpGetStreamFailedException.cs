////////////////////////////////////////////////////////
// Copyright (c) Alejandro Kalnay                     //
// License: GNU GPLv3                                 //
////////////////////////////////////////////////////////

using System;
using System.Runtime.Serialization;

namespace TextTcpIp
{
    public sealed class TcpIpGetStreamFailedException : TcpIpExceptionBase
    {
        private const string _DEFAULTMESSAGE = "Tcp/Ip GetStream failed.";

        public TcpIpGetStreamFailedException() : this(_DEFAULTMESSAGE) { }

        public TcpIpGetStreamFailedException(string message) : base(message) { }

        public TcpIpGetStreamFailedException(string message, Exception innerException) : base(message, innerException) { }

        private TcpIpGetStreamFailedException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public TcpIpGetStreamFailedException(Exception innerException) : this(_DEFAULTMESSAGE, innerException) { }
    }
}
