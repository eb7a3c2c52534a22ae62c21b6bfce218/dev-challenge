using System;

namespace Wool.DevChallenge.Api.Exceptions
{
    [Serializable]
    public class ApplicationSettingsInvalidException : Exception
    {
        public ApplicationSettingsInvalidException(string message) : base (message)
        { }
    }
}