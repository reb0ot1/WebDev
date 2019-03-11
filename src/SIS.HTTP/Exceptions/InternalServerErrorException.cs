using System;
using System.Collections.Generic;
using System.Text;
using SIS.HTTP.Enums;

namespace SIS.HTTP.Exceptions
{
    public class InternalServerErrorException : Exception
    {
        const string DefaultMessage = "The Server has encountered an error.";

        public const HttpResponseStatusCode HttpResponseCode = HttpResponseStatusCode.InternalServerErorr;

        public InternalServerErrorException() : base(DefaultMessage)
        {
        }
    }
}
