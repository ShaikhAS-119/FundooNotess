using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace RepositoryLayer.Service
{
    internal class VerifyPass
    {
        //get password from hash
        public static bool GetPass(string newPass, string storedHashedPassword)        
        {
            byte[] hashBytes = Convert.FromBase64String(storedHashedPassword);
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(newPass, salt, 10000);
            byte[] enteredHash = pbkdf2.GetBytes(20);

            for (int i = 0; i < 20; i++)
            {
                if (hashBytes[i + 16] != enteredHash[i])
                {
                    return false;
                }
            }

            return true;                               
        }
    }
}
