using BLL.Helpers;
using Contracts.DTOs;
using Contracts.Interfaces;
using Contracts.Models;
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
        private readonly IUserRole _roleRepo;

        public UserService(IUserAccess userAccess, IUserRole roleRepo)
        {
            _userAccess = userAccess;
            _roleRepo = roleRepo;
        }

        public List<UserRole> GetAvailableRoles() => _roleRepo.GetAllRoles();

        public void Register(UserDto user)
        {
            var role = _roleRepo.GetRoleById(user.RoleId);
            if (role == null) throw new Exception("Invalid role");

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
    }
}
