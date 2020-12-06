using System;
using System.Net;

namespace Wool.DevChallenge.Api.Exceptions
{
    [Serializable]
    public class TrolleyCalculationRemoteException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        public TrolleyCalculationRemoteException(HttpStatusCode statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}