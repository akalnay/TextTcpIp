////////////////////////////////////////////////////////
// Copyright (c) Alejandro Kalnay                     //
// License: GNU GPLv3                                 //
////////////////////////////////////////////////////////

using System;
using System.Runtime.Serialization;

namespace TextTcpIp
{
    public sealed class TcpIpListenerStartFailedException : TcpIpExceptionBase
    {
        private const string _DEFAULTMESSAGE = "Tcp/Ip Listener start failed.";

        public TcpIpListenerStartFailedException() : this(_DEFAULTMESSAGE) { }

        public TcpIpListenerStartFailedException(string message) : base(message) { }

        public TcpIpListenerStartFailedException(string message, Exception innerException) : base(message, innerException) { }

        private TcpIpListenerStartFailedException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public TcpIpListenerStartFailedException(Exception innerException) : this(_DEFAULTMESSAGE, innerException) { }
    }
}
