using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Calendar.Data;

namespace Calendar.Services
{
    public interface iJWTAuthentication
    {
        string ValidateAndCreateJWT(tbUser reqModel);
    }
}
