using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using ModelLayer.Model;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace RepositoryLayer.TokenGenerate
{
    public class Generate
    {
        public static string Token(string key,string issu, string aud, string id,LoginModel model)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim("Id", id),
                new Claim("Email", model.Email),   
            };

            var Sectoken = new JwtSecurityToken(issu, aud, claims,
              expires: DateTime.Now.AddMinutes(15),
              signingCredentials: credential);

            var token = new JwtSecurityTokenHandler().WriteToken(Sectoken);            

            return token;
        }

        public static string ResetPassToken(string mail, string key, string issu, string aud)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {               
                new Claim("Email", mail),
            };

            var Sectoken = new JwtSecurityToken(issu, aud, claims,
                 expires: DateTime.Now.AddMinutes(20),
              signingCredentials: credential);
             
            var token = new JwtSecurityTokenHandler().WriteToken(Sectoken);

            return token;
        }
    }
}
