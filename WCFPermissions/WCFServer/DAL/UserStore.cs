using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WCFServer.DAL
{
    /// <summary>
    /// Mock User Storage for the security demos
    /// </summary>
    public class UserStore
    {
        private static User[] store = new User[]
        {
            new User{ Username = "akeem", FullName="Akeem Valdez", Password = "akeem1", Roles = new string[] {"Admin"}, UserId = 1},
            new User{ Username = "zachery", FullName = "Zachery Fisher", Password = "zachery1", Roles = new string[] {"Accounting"}, UserId = 2},
            new User{ Username = "jordan", FullName = "Jordan Ayers", Password = "jordan1", Roles = new string[] {"HR"}, UserId = 3},
            new User{ Username = "nick", FullName = "Nicholas Daniel", Password = "nick1", Roles = new string[] {"Secretary"}, UserId = 4},
            new User{ Username = "hello", FullName = "Hello World", Password = "world", Roles = new string[] {"API"}, UserId = 5, CertificateThumbprint = "CN=sslstreamtest; 48D5760D449114D08E0297AD9BDB29028326BD96"}
        };

        /// <summary>
        /// Validate a username/password, return true if success
        /// </summary>
        public static bool ValidateUser(string username, string password)
        {
            return store.Any(i => i.Username == username && i.Password == password);
        }

        /// <summary>
        /// Validate certificate thumbprint
        /// </summary>
        public static bool ValidateUser(string certificateThumbprint)
        {
            return store.Any(i => i.CertificateThumbprint == certificateThumbprint);
        }

        /// <summary>
        /// Retrieve a user entity by username
        /// </summary>
        public static User GetUserByUsername(string username)
        {
            return store.FirstOrDefault(i => i.Username == username);
        }

        /// <summary>
        /// Retrieve a user entity by certificate thumbprint
        /// </summary>
        public static User GetUserByCertificate(string certificateThumbprint)
        {
            return store.FirstOrDefault(i => i.CertificateThumbprint == certificateThumbprint);
        }


    }
}
