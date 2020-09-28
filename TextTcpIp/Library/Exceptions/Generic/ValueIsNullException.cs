////////////////////////////////////////////////////////
// Copyright (c) Alejandro Kalnay                     //
// License: GNU GPLv3                                 //
////////////////////////////////////////////////////////

using System;
using System.Runtime.Serialization;

namespace TextTcpIp
{
    public sealed class ValueIsNullException<T> : GenericValueExceptionBase<T>
    {
        public ValueIsNullException() { }

        public ValueIsNullException(string message) : base(message) { }

        public ValueIsNullException(string message, Exception innerException) : base(message, innerException) { }

        private ValueIsNullException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public ValueIsNullException(T value) : this(value, $"The value can't be null.") { }

        public ValueIsNullException(T value, string message) : base(value, message) { }

        public T Value
        {
            get => Val;
            set => Val = value;
        }
    }

    public static class ValueIsNullException
    {
        public static ValueIsNullException<T> Create<T>(T value)
        {
            return new ValueIsNullException<T>(value);
        }

        public static ValueIsNullException<T> Create<T>(T value, string message)
        {
            return new ValueIsNullException<T>(value, message);
        }
    }
}
