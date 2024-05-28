using Microsoft.Data.SqlClient;
using ModelLayer.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryLayer.Interface
{
    public interface IRegisterationBL
    {
        public RegistrationResponse Register(RegistrationModel model);
        public string Login(LoginModel model);
        public bool ForgetPassword(ForgetPasswordModel model);
        public int ResetPassword(string token, ResetPasswordModel model);
    }
}
