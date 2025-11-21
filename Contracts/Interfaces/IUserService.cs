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
        UserDto? Login(string email, string password); 
        void Register(UserDto user);                         
        void UpdateProfile(UserDto user);                    
        void DeleteAccount(int id);
    }
}
