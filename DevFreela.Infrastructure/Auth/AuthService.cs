using DevFreela.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DevFreela.Infrastructure.Auth
{
    public class AuthService : IAuthService
    {

        //para conseguir gerar um token utilizar-se varios dados de configuralção 
        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string ComputedSha256Hash(string password)
        {
            //usa uma instancia do SHA256 para dotnet, vai ter um objeto capaz de obter o password e criar um has dele
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Compute - retorna byte array
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                // converte by array para string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2")); // x2 faz com que seja convertido em representação hexadecimal
                }

                return builder.ToString();
            }
        }

        public string GenerateJwtToken(string email, string role)
        {
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var key = _configuration["Jwt:Key"];
            /* chave utilizada junto com algoritimo de assinatura de hash /Enconding para obter os bytes, depois inicializa a chave simetrica de segurança  */
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)); 
            // credencias utilizadas para assinarem o token, com todas as informações, com algoritmos e dados do token JWT
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //claims -> informação sobre o usuario, no qual aquele token pertence
            var claims = new List<Claim>
            {
                new Claim("userName", email),
                new Claim(ClaimTypes.Role, role)
            };

            var token = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    expires: DateTime.Now.AddHours(8),
                    signingCredentials: credentials,
                    claims: claims
                );

            var tokenHandler = new JwtSecurityTokenHandler();

            var stringToken = tokenHandler.WriteToken(token);

            return stringToken;
        }
    }
}
