using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using MvcFramework.Logger;

namespace MvcFramework.Services
{
    public class HashService : IHashService
    {
        private readonly ILogger logger;

        public HashService(ILogger logger)
        {
            this.logger = logger;
        }

        public string Hash(string stringToHash)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(stringToHash));

                var hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();

                this.logger.Log(hash);

                return hash;
            }
        }
    }
}
