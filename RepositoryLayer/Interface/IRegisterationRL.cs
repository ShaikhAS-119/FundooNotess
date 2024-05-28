using Microsoft.Data.SqlClient;
using ModelLayer.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Interface
{
    public interface IRegisterationRL
    {
        public int Register(RegistrationModel model);

        public string Login(LoginModel model);
        public bool ForgetPasswordAsync(ForgetPasswordModel model);

        public int ResetPassword(string token, string hashpass);
    }
}
