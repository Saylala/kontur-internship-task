using System;

namespace Kontur.GameStats.Server.Exceptions
{
    public class InvalidRequestException : Exception
    {
        public InvalidRequestException(string message) : base(message)
        {
        }
    }
}
