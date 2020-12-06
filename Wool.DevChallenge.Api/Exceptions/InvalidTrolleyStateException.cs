using System;

namespace Wool.DevChallenge.Api.Exceptions
{
    [Serializable]
    public class InvalidTrolleyStateException : Exception
    {
        public InvalidTrolleyStateException(string message) : base(message)
        { }
    }
}