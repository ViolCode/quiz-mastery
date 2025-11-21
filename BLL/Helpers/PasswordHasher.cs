using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Helpers
{
    public static class PasswordHasher
    {
        public static string Hash(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new Exception("Password is null or empty");
            }

            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);

            return Convert.ToBase64String(hash);
        }

        public static bool Verify(string password, string hash)
        {
            return Hash(password) == hash;
        }
    }
}
