using Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Interfaces
{
    public interface IRoleMapper
    {
        Role FromId(int id);
        int ToId(Role role);
    }
}

