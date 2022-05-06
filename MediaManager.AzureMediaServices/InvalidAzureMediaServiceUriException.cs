namespace MediaManager.AzureMediaServices
{
    public class InvalidAzureMediaServiceUriException : Exception
    {
        public InvalidAzureMediaServiceUriException() : base()
        {
        }

        public InvalidAzureMediaServiceUriException(string message) : base(message)
        {
        }

        public InvalidAzureMediaServiceUriException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
