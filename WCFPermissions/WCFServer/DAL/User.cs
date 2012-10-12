using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WCFServer.DAL
{
    /// <summary>
    /// Sample User entity for the mock UserStore engine.
    /// </summary>
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string CertificateThumbprint { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string[] Roles { get; set; }
    }
}
