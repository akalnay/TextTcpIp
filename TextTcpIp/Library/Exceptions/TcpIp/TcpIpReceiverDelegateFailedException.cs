////////////////////////////////////////////////////////
// Copyright (c) Alejandro Kalnay                     //
// License: GNU GPLv3                                 //
////////////////////////////////////////////////////////

using System;
using System.Runtime.Serialization;

namespace TextTcpIp
{
    public sealed class TcpIpReceiverDelegateFailedException : TcpIpExceptionBase
    {
        private const string _DEFAULTMESSAGE = "Tcp/Ip Receiver delegate failed.";

        public TcpIpReceiverDelegateFailedException() : this(_DEFAULTMESSAGE) { }

        public TcpIpReceiverDelegateFailedException(string message) : base(message) { }

        public TcpIpReceiverDelegateFailedException(string message, Exception innerException) : base(message, innerException) { }

        private TcpIpReceiverDelegateFailedException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public TcpIpReceiverDelegateFailedException(Exception innerException) : this(_DEFAULTMESSAGE, innerException) { }
    }
}
