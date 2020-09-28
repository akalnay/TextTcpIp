////////////////////////////////////////////////////////
// Copyright (c) Alejandro Kalnay                     //
// License: GNU GPLv3                                 //
////////////////////////////////////////////////////////

using System;
using System.Runtime.Serialization;

namespace TextTcpIp
{
    public sealed class TcpIpInnerReceiveFailedException : TcpIpExceptionBase
    {
        private const string _DEFAULTMESSAGE = "Tcp/Ip Inner Receive failed.";

        public TcpIpInnerReceiveFailedException() : this(_DEFAULTMESSAGE) { }

        public TcpIpInnerReceiveFailedException(string message) : base(message) { }

        public TcpIpInnerReceiveFailedException(string message, Exception innerException) : base(message, innerException) { }

        private TcpIpInnerReceiveFailedException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public TcpIpInnerReceiveFailedException(Exception innerException) : this(_DEFAULTMESSAGE, innerException) { }
    }
}
