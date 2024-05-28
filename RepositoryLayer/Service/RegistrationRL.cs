using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ModelLayer.Model;
using RepositoryLayer.Interface;
using RepositoryLayer.TokenGenerate;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Service
{
    public class RegistrationRL : IRegisterationRL
    {
        private readonly IConfiguration _configuration;
        public RegistrationRL(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        //registration method
        public int Register(RegistrationModel model)
        {
            int data = 0;

            SqlConnection con = null;
            try
            {
                
                con = new SqlConnection(_configuration["ConnectionStrings:SqlConnection"]);
                string Checkquery = $"select Email,Password from Persons where Email = '{model.Email}';";
                string Registerquery = $"Insert into Persons(FirstName,LastName,Email,Password)values('{model.FirstName}','{model.LastName}','{model.Email}','{model.Password}');";

                //check
                SqlCommand cmd = new SqlCommand(Checkquery, con);
                con.Open();

                SqlDataReader r = cmd.ExecuteReader();

                if (r.HasRows)
                {
                    data = -1;
                    r.Close();
                }
                else
                {
                    r.Close();
                    //register                                        
                    SqlCommand cmd1 = new SqlCommand(Registerquery, con);
                    data = cmd1.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }
            return data;

        }

        //login method
        public string Login(LoginModel model)
        {
            SqlConnection con = null;
            try
            {
                string connection = _configuration["ConnectionStrings:SqlConnection"];
                con = new SqlConnection(connection);                

                string query = $"select Id,Email,Password from Persons where Email = '{model.Email}';";
               
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();

                SqlDataReader row = cmd.ExecuteReader();
                string id =null;

                if (row.HasRows)
                {                                     
                    string hashpass = null;
                    while (row.Read())
                    {
                        hashpass = row["Password"].ToString();
                         id = row["id"].ToString();
                    }
                    var pass = VerifyPass.GetPass(model.Password, hashpass);
                    if (pass)
                    {
                        var key = _configuration["jwt:key"];
                        var issuer = _configuration["jwt:Issuer"];
                        var audience = _configuration["jwt:Audience"];
                      
                        var token = TokenGenerate.Generate.Token(key, issuer, audience,id, model);
                        return token;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }                                 
        }

        //forget password
        public bool ForgetPasswordAsync(ForgetPasswordModel model)
        {
            bool check = false;
            string Email = null;

            SqlConnection con = null;
            try
            {
                con = new SqlConnection(_configuration.GetConnectionString("SqlConnection"));

                string query = $"select Id, Email from Persons where Email = '{model.Email}';";

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();

                SqlDataReader row = cmd.ExecuteReader();

                if (row.HasRows)
                {
                    while (row.Read())
                    {
                        Email = row["Email"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }

            if (Email == model.Email)
            {

                var key = _configuration["Reset:ResetKey"];
                var issuer = _configuration["jwt:Issuer"];
                var audience = _configuration["jwt:Audience"];

                var token = TokenGenerate.Generate.ResetPassToken(model.Email, key, issuer, audience);

                var port = _configuration["MailValues:Port"];
                var host = _configuration["MailValues:Host"];
                var from = _configuration["MailValues:From"];
                var fromPass = _configuration["MailValues:FromPassword"];
                var subject = _configuration["MailValues:Subject"];
                var resetUrl = _configuration["ResetUrl:ResetPassUrl"];
                var body = token.ToString();

                var mail = EmailService.EmailService.ForgetpasswordMail(model, port, host, from, fromPass, subject, body, resetUrl);
                return check = true;
            }

            return check;
        }

        //reset password
        public int ResetPassword(string token, string hashpass)
        {
            int data = 0;
            var handler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["jwt:Issuer"],
                ValidAudience = _configuration["jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Reset:ResetKey"]))
            };

            SecurityToken validatedToken;
            var principal = handler.ValidateToken(token, validationParameters, out validatedToken);

            var emailClaim = principal.FindFirst("Email");
            if (emailClaim == null)
            {
                throw new Exception("Email claim not found in token.");
            }
            var email = emailClaim.Value;

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("SqlConnection")))
            {
                string Registerquery = $"UPDATE Persons SET Password = '{hashpass}' where Email = '{email}';";

                con.Open();
                SqlCommand cmd1 = new SqlCommand(Registerquery, con);
                data = cmd1.ExecuteNonQuery();

                con.Close();

                return data;
            }
        }
    }
}
