////////////////////////////////////////////////////////
// Copyright (c) Alejandro Kalnay                     //
// License: GNU GPLv3                                 //
////////////////////////////////////////////////////////

using System;
using System.Runtime.Serialization;

namespace TextTcpIp
{
    public sealed class TcpIpListenerStopFailedException : TcpIpExceptionBase
    {
        private const string _DEFAULTMESSAGE = "Tcp/Ip Listener Stop failed.";

        public TcpIpListenerStopFailedException() : this(_DEFAULTMESSAGE) { }

        public TcpIpListenerStopFailedException(string message) : base(message) { }

        public TcpIpListenerStopFailedException(string message, Exception innerException) : base(message, innerException) { }

        private TcpIpListenerStopFailedException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public TcpIpListenerStopFailedException(Exception innerException) : this(_DEFAULTMESSAGE, innerException) { }
    }
}
