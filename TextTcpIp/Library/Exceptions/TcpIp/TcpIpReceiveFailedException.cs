////////////////////////////////////////////////////////
// Copyright (c) Alejandro Kalnay                     //
// License: GNU GPLv3                                 //
////////////////////////////////////////////////////////

using System;
using System.Runtime.Serialization;

namespace TextTcpIp
{
    public sealed class TcpIpReceiveFailedException : TcpIpExceptionBase
    {
        private const string _DEFAULTMESSAGE = "Tcp/Ip Receive failed.";

        public TcpIpReceiveFailedException() : this(_DEFAULTMESSAGE) { }

        public TcpIpReceiveFailedException(string message) : base(message) { }

        public TcpIpReceiveFailedException(string message, Exception innerException) : base(message, innerException) { }

        private TcpIpReceiveFailedException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public TcpIpReceiveFailedException(Exception innerException) : this(_DEFAULTMESSAGE, innerException) { }
    }
}
