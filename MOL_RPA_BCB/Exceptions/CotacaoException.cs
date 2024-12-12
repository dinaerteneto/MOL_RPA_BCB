namespace Exceptions
{
    public class CotacaoException : Exception
    {
        public CotacaoException() : base() { }

        public CotacaoException(string message) : base(message) { }

        public CotacaoException(string message, Exception innerException) : base(message, innerException) { }
    }
}