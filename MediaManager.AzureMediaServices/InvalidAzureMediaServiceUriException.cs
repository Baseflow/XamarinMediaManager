using System;
namespace MediaManager.AzureMediaServices
{
    class InvalidAzureMediaServiceUriException : Exception
    {
        public InvalidAzureMediaServiceUriException(string message) : base(message)
        {
        }
    }
}
