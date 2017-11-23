﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{
    public class ServiceCertValidator : X509CertificateValidator
    {
        public override void Validate(X509Certificate2 certificate)
        {
            //TODO "syslog"
            X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, "syslog");

            if (!certificate.Issuer.Equals(srvCert.Issuer))
            {
                throw new Exception("Certificate is not from the valid issuer.");
            }
        }
    }
}
