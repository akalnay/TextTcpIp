////////////////////////////////////////////////////////
// Copyright (c) Alejandro Kalnay                     //
// License: GNU GPLv3                                 //
////////////////////////////////////////////////////////

using System;
using System.Runtime.Serialization;

namespace TextTcpIp
{
    public sealed class TcpIpListenerInstantiationFailedException : TcpIpExceptionBase
    {
        private const string _DEFAULTMESSAGE = "Tcp/Ip Server instantiation failed.";

        public TcpIpListenerInstantiationFailedException() : this(_DEFAULTMESSAGE) { }

        public TcpIpListenerInstantiationFailedException(string message) : base(message) { }

        public TcpIpListenerInstantiationFailedException(string message, Exception innerException) : base(message, innerException) { }

        private TcpIpListenerInstantiationFailedException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public TcpIpListenerInstantiationFailedException(Exception innerException) : this(_DEFAULTMESSAGE, innerException) { }
    }
}
