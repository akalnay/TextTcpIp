////////////////////////////////////////////////////////
// Copyright (c) Alejandro Kalnay                     //
// License: GNU GPLv3                                 //
////////////////////////////////////////////////////////

using System;

namespace TextTcpIp
{
    internal static class ThrowIf
    {
        #region ArgumentIsNull()

        public static void ArgumentIsNull<T>(T value, string paramName)
        {
            if (value == null)
                throw new ArgumentNullException(paramName);
        }

        #endregion ArgumentIsNull()

        #region ArgumentDoesNotPassValiedation

        public static void ArgumentDoesNotPassValidation<T>(this T value, Func<T, bool> isValid, string paramName)
        {
            if (!isValid(value))
                throw new ArgumentValueIsInvalidException<T>(value, paramName);
        }

        public static void ArgumentDoesNotPassValidation<T>(this T value, Func<T, bool> isValid, string paramName, string message)
        {
            if (!isValid(value))
                throw new ArgumentValueIsInvalidException<T>(value, paramName, message);
        }

        #endregion ArgumentDoesNotPassValiedation

        #region IsNull()

        public static void IsNull<T>(T value)
        {
            if (value == null)
                throw ValueIsNullException.Create(value);
        }

        public static void IsNull<T>(T value, string message)
        {
            if (value == null)
                throw ValueIsNullException.Create(value, message);
        }

        #endregion IsNull()

        #region ArgumentIsOutOfRange()

        public static void ArgumentIsOutOfRange<T>(this T value, Func<T, bool> isOutOfRange, string paramName)
        {
            ThrowIf.ArgumentIsOutOfRange(value, isOutOfRange, paramName, ArgumentOutOfRangeExceptionMessage);
        }

        public static void ArgumentIsOutOfRange<T>(this T value, Func<T, bool> isOutOfRange, string paramName, string message)
        {
            if (isOutOfRange(value))
                throw new ArgumentOutOfRangeException(paramName, value, message);
        }


        #region ArgumentOutOfRangeExceptionMessage

        private const string _ARGUMENTOUTOFRANGEEXCEPTIONMESSAGEDEFAULT = "Specified argument was out of the range of valid values.";   // This value should come from a resource.
                                                                                                                                        // Use ExceptionExt.GetMessage<TException>()
                                                                                                                                        // to get the .Net Framework's original
                                                                                                                                        // exception message
        private static string _ARGUMENTOUTOFRANGEEXCEPTIONMESSAGE;

        private static string ArgumentOutOfRangeExceptionMessage
        {
            get
            {
                if (_ARGUMENTOUTOFRANGEEXCEPTIONMESSAGE == null)
                    _ARGUMENTOUTOFRANGEEXCEPTIONMESSAGE = _ARGUMENTOUTOFRANGEEXCEPTIONMESSAGEDEFAULT;
                return _ARGUMENTOUTOFRANGEEXCEPTIONMESSAGE;
            }
        }

        #endregion ArgumentOutOfRangeExceptionMessage

        #endregion ArgumentIsOutOfRange()
    }
}
