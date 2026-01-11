using Contracts.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Interfaces
{
    public interface IUserAccess
    {
        UserDto? GetUserById(int id);
        UserDto? GetUserByEmail(string email);  
        List<UserDto> GetAllUsers();
        List<(int Id, string Name)> GetAllRoles();
        List<UserDto> GetAllStudents();
        void CreateUser(UserDto user);          
        void UpdateUser(UserDto user);          
        void DeleteUser(int id);                 
    }
}
