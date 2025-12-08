using BLL.Helpers;
using Contracts.DTOs;
using Contracts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class UserService : IUserService //implements
    {
        private readonly IUserAccess _userAccess; //uses

        public UserService(IUserAccess userAccess)
        {
            _userAccess = userAccess;
        }

        public void Register(UserDto user)
        {
            var existing = _userAccess.GetUserByEmail(user.Email);
            if (existing != null)
            {
                throw new InvalidOperationException("An account with this email already exists.");
            }

            user.PasswordHash = PasswordHasher.Hash(user.PasswordHash);
            _userAccess.CreateUser(user);
        }

        public void UpdateProfile(UserDto user)
        {
            if (!string.IsNullOrEmpty(user.PasswordHash))
            {
                user.PasswordHash = PasswordHasher.Hash(user.PasswordHash);
            }
            _userAccess.UpdateUser(user);
        }

        public void DeleteAccount(int id)
        {
            _userAccess.DeleteUser(id);
        }

        public UserDto? Login(string email, string password)
        {
            var user = _userAccess.GetUserByEmail(email);
            if (user == null) return null;

            return PasswordHasher.Verify(password, user.PasswordHash) ? user : null;
        }

        public List<(int Id, string Name)> GetAllRoles()
        {
            return _userAccess.GetAllRoles();
        }

        public UserDto? FindByEmail(string email)
        {
            return _userAccess.GetUserByEmail(email);
        }
    }
}
