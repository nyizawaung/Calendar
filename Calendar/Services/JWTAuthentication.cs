using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Calendar.Data;
using Microsoft.IdentityModel.Tokens;

namespace Calendar.Services
{
    public class JWTAuthentication : iJWTAuthentication
    {
        private readonly string TokenKey;
        public JWTAuthentication(string tokenKey)
        {
            TokenKey = tokenKey;
        }
        public string ValidateAndCreateJWT(tbUser reqModel)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(tokenHandler.CreateToken(
                new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, reqModel.Name),
                        new Claim(ClaimTypes.Email,reqModel.Email)
                    }),
                    Expires = DateTime.UtcNow.AddDays(1),
                    SigningCredentials =
                    new SigningCredentials(
                        new SymmetricSecurityKey(Encoding.ASCII.GetBytes(TokenKey)),
                        SecurityAlgorithms.HmacSha256Signature)
                }));


        }
    }
}
