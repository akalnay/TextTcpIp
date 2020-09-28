////////////////////////////////////////////////////////
// Copyright (c) Alejandro Kalnay                     //
// License: GNU GPLv3                                 //
////////////////////////////////////////////////////////

using System;
using System.Runtime.Serialization;

namespace TextTcpIp
{
    public abstract class GenericValueExceptionBase<T> : Exception
    {
        protected GenericValueExceptionBase() { }

        protected GenericValueExceptionBase(string message) : base(message) { }

        protected GenericValueExceptionBase(string message, Exception innerException) : base(message, innerException) { }

        protected GenericValueExceptionBase(SerializationInfo info, StreamingContext context) : base(info, context) { }

        protected GenericValueExceptionBase(T val, string message) : this(message) => Val = val;

        protected T Val { get; set; }
    }
}
