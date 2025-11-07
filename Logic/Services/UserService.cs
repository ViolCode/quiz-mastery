using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Models;


namespace Logic.Services
{
    public class UserService
    {
        private static List<User> _users = new List<User>
        {
            new User { Username = "teacher1", Password = "pass123", Role = "Teacher" },
            new User { Username = "student1", Password = "pass456", Role = "Student" }
        };

        public User Register(string username, string password, string role)
        {
            var user = new User { Username = username, Password = password, Role = role };
            _users.Add(user);
            return user;
        }

        public User Login(string username, string password)
        {
            return _users.FirstOrDefault(u => u.Username == username && u.Password == password);
        }
    }
}
