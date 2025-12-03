using Contracts.Enums;
using Contracts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Mappers
{
    public class RoleMapper : IRoleMapper
    {
        public Role FromId(int id) => id switch
        {
            1 => Role.Student,
            2 => Role.Teacher,
            _ => throw new InvalidOperationException("Unknown role")
        };

        public int ToId(Role role) => role switch
        {
            Role.Student => 1,
            Role.Teacher => 2,
            _ => throw new InvalidOperationException("Unknown role")
        };
    }
}
