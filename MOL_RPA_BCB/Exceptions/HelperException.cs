﻿namespace Exceptions
{
    public class HelperException : AppException
    {
        public HelperException() : base() { }

        public HelperException(string message) : base(message) { }

        public HelperException(string message, Exception innerException) : base(message, innerException) { }
    }
}