using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace PIBcl.Core
{
    public interface IUser
    {
        Guid Id { get; }
        string Name { get; }
        
        string Email { get;  }

         string Telefone { get;  }

        IEnumerable<Claim> Claims { get; }

    }
}
