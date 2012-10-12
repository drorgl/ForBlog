using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using WCFServer.DAL;

namespace WCFServer.Security
{
    /// <summary>
    /// Custom Certificate Validator
    /// <para>Will check if there's a user with that certificate</para>
    /// </summary>
    public class CustomCertificateValidator : X509CertificateValidator
    {
        public override void Validate(System.Security.Cryptography.X509Certificates.X509Certificate2 certificate)
        {
            if (!UserStore.ValidateUser(certificate.Subject + "; " + certificate.Thumbprint))
                throw new SecurityTokenValidationException("Unauthorized");
        }
    }
}
