using Contracts.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Interfaces
{
    public interface IUserService
    {
        void Register(UserDto user);                         
        void UpdateProfile(UserDto user);                    
        void DeleteAccount(int id);
        UserDto? Login(string email, string password);
        List<(int Id, string Name)> GetAllRoles();
        //UserDto? FindByEmail(string email);
    }
}
