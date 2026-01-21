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
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userAccess)
        {
            _userRepository = userAccess;
        }

        public void Register(UserDto user)
        {
            var existing = _userRepository.GetUserByEmail(user.Email);
            if (existing != null)
            {
                throw new InvalidOperationException("An account with this email already exists.");
            }

            user.PasswordHash = PasswordHasher.Hash(user.PasswordHash);
            _userRepository.CreateUser(user);
        }

        public void UpdateProfile(UserDto user)
        {
            if (!string.IsNullOrEmpty(user.PasswordHash))
            {
                user.PasswordHash = PasswordHasher.Hash(user.PasswordHash);
            }
            _userRepository.UpdateUser(user);
        }

        public void DeleteAccount(int id)
        {
            _userRepository.DeleteUser(id);
        }

        public UserDto? Login(string email, string password)
        {
            var user = _userRepository.GetUserByEmail(email);
            if (user == null) return null;

            return PasswordHasher.Verify(password, user.PasswordHash) ? user : null;
        }

        public List<(int Id, string Name)> GetAllRoles()
        {
            return _userRepository.GetAllRoles();
        }

        public List<UserDto> GetAllStudents()
        {
            return _userRepository.GetAllStudents();
        }
    }
}
