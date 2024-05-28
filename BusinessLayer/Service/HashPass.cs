using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace BusinessLayer.Service
{
    internal class HashPass
    {        
        public static string GetHash(string pass)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(pass, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            byte[] hashBytes = new byte[16 + 20];
            //copy method (source, sourceStartIndex, destination, destinationStatIndex, lenghtToCopy)
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            string hashedPassword = Convert.ToBase64String(hashBytes);

            return hashedPassword;
        }
    }
}
