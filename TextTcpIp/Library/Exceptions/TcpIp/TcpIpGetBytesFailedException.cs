////////////////////////////////////////////////////////
// Copyright (c) Alejandro Kalnay                     //
// License: GNU GPLv3                                 //
////////////////////////////////////////////////////////

using System;
using System.Runtime.Serialization;

namespace TextTcpIp
{
    public sealed class TcpIpGetBytesFailedException : TcpIpExceptionBase
    {
        private const string _DEFAULTMESSAGE = "Tcp/Ip GetBytes failed.";

        public TcpIpGetBytesFailedException() : this(_DEFAULTMESSAGE) { }

        public TcpIpGetBytesFailedException(string message) : base(message) { }

        public TcpIpGetBytesFailedException(string message, Exception innerException) : base(message, innerException) { }

        private TcpIpGetBytesFailedException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public TcpIpGetBytesFailedException(Exception innerException) : this(_DEFAULTMESSAGE, innerException) { }
    }
}
