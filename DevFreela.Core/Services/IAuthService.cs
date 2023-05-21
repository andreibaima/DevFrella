using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevFreela.Core.Services
{
    public interface IAuthService
    {
        //role -> papel para o usuario
        string GenerateJwtToken(string email, string role);
        string ComputedSha256Hash(string password);
    }
}
