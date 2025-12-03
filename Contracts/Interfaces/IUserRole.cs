using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Interfaces
{
    public interface IUserRoleRepository
    {
        List<UserRole> GetAllRoles();
        UserRole? GetRoleById(int id);
    }
}
