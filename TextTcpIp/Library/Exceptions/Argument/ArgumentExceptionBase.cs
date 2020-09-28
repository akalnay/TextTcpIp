////////////////////////////////////////////////////////
// Copyright (c) Alejandro Kalnay                     //
// License: GNU GPLv3                                 //
////////////////////////////////////////////////////////

using System;
using System.Runtime.Serialization;

namespace TextTcpIp
{
    public abstract class ArgumentExceptionBase<T> : ArgumentException
    {
        protected ArgumentExceptionBase() { }

        protected ArgumentExceptionBase(string message) : base(message) { }

        protected ArgumentExceptionBase(string message, Exception innerException) : base(message, innerException) { }

        protected ArgumentExceptionBase(string message, string paramName) : base(message, paramName) { }

        protected ArgumentExceptionBase(string message, string paramName, Exception innerException) : base(message, paramName, innerException) { }

        protected ArgumentExceptionBase(SerializationInfo info, StreamingContext context) : base(info, context) { }

        protected ArgumentExceptionBase(T val, string message, string paramName) : base(message, paramName) => Val = val;

        protected T Val { get; set; }
    }
}
