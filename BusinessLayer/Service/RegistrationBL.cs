using BusinessLayer.Service;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ModelLayer.Model;
using RepositoryLayer.Interface;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;

namespace RepositoryLayer.Service
{
    public class RegistrationBL : IRegisterationBL
    {
        private readonly IRegisterationRL _registerationRL;

        
        
        public RegistrationBL(IRegisterationRL _registeration) 
        {
            _registerationRL = _registeration;
           
        }

        public RegistrationResponse Register(RegistrationModel model)
        {
            var hash = HashPass.GetHash(model.Password);
            model.Password = hash;

            var data = _registerationRL.Register(model);
            var newModelResponse = new RegistrationResponse();

            if (data > 0)
            {
                newModelResponse.Email = model.Email;
                newModelResponse.FirstName = model.FirstName;
                newModelResponse.LastName = model.LastName;

                return newModelResponse;
            }

            return null;

        }

        public string Login(LoginModel model)
        {
           
           var data =_registerationRL.Login(model);

            if (data != null)
            {                                
                return data;
            }
            return null;
        }


        public bool ForgetPassword(ForgetPasswordModel model)
        {
            var data = _registerationRL.ForgetPasswordAsync(model);
            return data;
        }
        
        public int ResetPassword(string token, ResetPasswordModel model)
        {
            var hashpass = HashPass.GetHash(model.Password);
            var data = _registerationRL.ResetPassword(token, hashpass);
            return data;
        }


    }
}
