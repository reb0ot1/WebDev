using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using SIS.HTTP.Enums;

namespace SIS.HTTP.Exceptions
{
    public class BadRequestException : Exception
    {
        private const string DefaultMessage = "The Request was malformed or contains unsupported elements.";

        public const HttpResponseStatusCode HttpResponseCode = HttpResponseStatusCode.BadRequest;

        public BadRequestException() : base(DefaultMessage)
        {
        }
    }
}
