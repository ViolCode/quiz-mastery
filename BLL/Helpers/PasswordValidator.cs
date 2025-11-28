using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BLL.Helpers
{
    public static class PasswordValidator
    {
        public static bool IsValidPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return false;
            var regex = new Regex(@"^(?=.*[A-Z])(?=.*[0-9])[A-Za-z0-9]{8,}$");
            return regex.IsMatch(password);
        }
    }
}
