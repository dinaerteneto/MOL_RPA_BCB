using Exceptions;

namespace Helpers.Exceptions
{
    public class HelperException : CotacaoException
    {
        public HelperException() : base() { }

        public HelperException(string message) : base(message) { }

        public HelperException(string message, Exception innerException) : base(message, innerException) { }
    }
}