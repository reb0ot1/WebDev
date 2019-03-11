using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace MvcFramework.Services
{
    public class HashService : IHashService
    {
        public HashService()
        {

        }

        public string Hash(string stringToHash)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(stringToHash));

                var hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();

                return hash;
            }
        }
    }
}
