namespace DAL.Exceptions
{
    public class DbOperationException : Exception
    {
        public DbOperationException() { }

        public DbOperationException(string message) : base(message) { }

        public DbOperationException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
