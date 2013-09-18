using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace RadicalGeek.Common.Security
{
    public sealed class MultipleCertificatesException : CertificateException
    {
        public X509Certificate[] Certificates { get; set; }

        public MultipleCertificatesException(CertificateInfo certificateInfo, X509Certificate[] certificates)
            : base(string.Format("Multiple certificates were found with the supplied CertificateInfo parameters:\r\n{0}\r\nCertificates:\r\n{1}", GetCertificateInfo(certificateInfo),GetCertificateSubjects(certificates)))
        {
            Certificates = certificates;
            CertificateInfo = certificateInfo;
        }

        private static object GetCertificateSubjects(IEnumerable<X509Certificate> certificates)
        {
            return string.Join(Environment.NewLine, certificates.Select(c => string.Format("\t{0}", c.Subject)));
        }
    }
}