////////////////////////////////////////////////////////
// Copyright (c) Alejandro Kalnay                     //
// License: GNU GPLv3                                 //
////////////////////////////////////////////////////////

using System;
using System.Runtime.Serialization;

namespace TextTcpIp
{
    public class ArgumentValueIsInvalidException<T> : ArgumentExceptionBase<T>
    {
        public ArgumentValueIsInvalidException()
        {
        }

        public ArgumentValueIsInvalidException(string message) : base(message)
        {
        }

        public ArgumentValueIsInvalidException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ArgumentValueIsInvalidException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
        {
        }

        public ArgumentValueIsInvalidException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ArgumentValueIsInvalidException(string message, string paramName) : base(message, paramName)
        {
        }

        public ArgumentValueIsInvalidException(T val, string paramName) : this(val, "The argument's value does not pass validation.", paramName)
        {
        }

        public ArgumentValueIsInvalidException(T val, string message, string paramName) : base(val, message, paramName)
        {
        }
    }
}
