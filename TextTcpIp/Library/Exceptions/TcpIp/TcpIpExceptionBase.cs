////////////////////////////////////////////////////////
// Copyright (c) Alejandro Kalnay                     //
// License: GNU GPLv3                                 //
////////////////////////////////////////////////////////

using System;
using System.Runtime.Serialization;

namespace TextTcpIp
{
    public abstract class TcpIpExceptionBase : Exception
    {
        public TcpIpExceptionBase() { }

        public TcpIpExceptionBase(string message) : base(message) { }

        public TcpIpExceptionBase(string message, Exception innerException) : base(message, innerException) { }

        protected TcpIpExceptionBase(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
