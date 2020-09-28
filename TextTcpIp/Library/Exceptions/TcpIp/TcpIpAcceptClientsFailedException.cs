////////////////////////////////////////////////////////
// Copyright (c) Alejandro Kalnay                     //
// License: GNU GPLv3                                 //
////////////////////////////////////////////////////////

using System;
using System.Runtime.Serialization;

namespace TextTcpIp
{
    public sealed class TcpIpAcceptClientsFailedException : TcpIpExceptionBase
    {
        private const string _DEFAULTMESSAGE = "Tcp/Ip Accept Clients failed.";

        public TcpIpAcceptClientsFailedException() : this(_DEFAULTMESSAGE) { }

        public TcpIpAcceptClientsFailedException(string message) : base(message) { }

        public TcpIpAcceptClientsFailedException(string message, Exception innerException) : base(message, innerException) { }

        private TcpIpAcceptClientsFailedException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public TcpIpAcceptClientsFailedException(Exception innerException) : this(_DEFAULTMESSAGE, innerException) { }
    }
}
