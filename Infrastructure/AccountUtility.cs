using JobScheduler.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JobScheduler.Infrastructure
{
    public static class AccountUtility
    {
        public static async Task<TokenModel> GenerateToken(LoginViewModel model, UserManager<User> _userManager, IConfiguration _configuration)
        {
            TokenModel tokenModel = null;
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                return null;
            }

            if (await _userManager.CheckPasswordAsync(user, model.Password))
            {
                try
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles != null && roles.Count > 0)
                    {
                        var tokenHandler = new JwtSecurityTokenHandler();
                        var key = Encoding.ASCII.GetBytes(_configuration["Tokens:Key"]);

                        //meta informazioni da inserire dei token jwt 
                        var tokenDescriptor = new SecurityTokenDescriptor
                        {
                            Subject = new ClaimsIdentity(new Claim[]
                            {
                                        new Claim(ClaimTypes.Name, user.Id.ToString()),
                                        new Claim(ClaimTypes.Role, roles?.FirstOrDefault())
                            }),

                            //se voglio che i token non scadano, possono mettere un valore molto alto
                            Expires = DateTime.UtcNow.AddDays(7),
                            //algoritmo di cifratura
                            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                        };

                        var token = tokenHandler.CreateToken(tokenDescriptor);

                        tokenModel = new TokenModel
                        {
                            //converto oggetto tokendescri in stringa per passarla al client
                            Token = tokenHandler.WriteToken(token),
                            Expiration = token.ValidTo,
                        };
                    }
                }
                catch (Exception)
                {
                    tokenModel = null;
                }
            }

            return tokenModel;
        }

    }
}

